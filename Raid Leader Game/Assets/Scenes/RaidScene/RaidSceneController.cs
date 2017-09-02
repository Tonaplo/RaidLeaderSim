using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class RaidSceneController : MonoBehaviour {

    public GameObject HealthBarPrefab;
    public GameObject MeterPrefab;
    public GameObject ConsumablePrefab;
    public Text raidText;
    public Canvas canvas;
    public Text SetupText;
    public Button PositionalButton;
    public Text PositionalButtonText;
    public GameObject ControlPanel;
    public GameObject SetupPanel;
    public BossCastBarScript BossCastScript;
    public RaidSceneControlPanel CooldownController;

    Enums.EncounterSteps currentStep = Enums.EncounterSteps.EncounterStart;
    BaseEncounter encounter;
    List<Raider> all = new List<Raider>();
    List<RaiderScript> m_raiderScripts;
    MeterControllerScript m_damageMcs;
    MeterControllerScript m_healingMcs;
    float m_fightStartTime;
    int m_numDeadRaidMembers = 0;
    List<GameObject> m_consumableButtons = new List<GameObject>();

    public BaseEncounter Encounter {get  { return encounter; } }

    public List<RaiderScript> GetRaid() { return m_raiderScripts; }

    public int GetBossHealthPercent() {
        return encounter.HealthBar.GetHealthPercent();
    }

    // Use this for initialization
    void Start () {
        //Utility.DebugInitalize();
        all = PlayerData.RaidTeam;
        CreateRaidHealthBars();
        CreateEncounter();
        SetupUI();
        ControlPanel.SetActive(false);
        SetupPanel.SetActive(true);
        PositionalButton.gameObject.SetActive(false);
    }

    void SetupUI()
    {
        raidText.text = "";
        SetupText.text = PlayerData.RaidTeamName + " taking on\n" + encounter.Name + "\non\n" + encounter.Difficulty.ToString() + " difficulty.";

        GameObject temp = GameObject.Instantiate(MeterPrefab);
        temp.transform.SetParent(canvas.transform);
        m_damageMcs = temp.GetComponent<MeterControllerScript>();
        List<Raider> dps = all.FindAll(x => x.RaiderStats.GetRole() == Enums.CharacterRole.MeleeDPS || x.RaiderStats.GetRole() == Enums.CharacterRole.RangedDPS);
        int dpsBarsCount = dps.Count > StaticValues.MaxNumDPSMeterBars ? StaticValues.MaxNumDPSMeterBars : dps.Count;
        m_damageMcs.Initialize("Damage:", 485, 120, 40, 130, dpsBarsCount);
        m_damageMcs.CreateEntriesFromRaid(all);

        GameObject temptwo = GameObject.Instantiate(MeterPrefab);
        temptwo.transform.SetParent(canvas.transform);
        m_healingMcs = temptwo.GetComponent<MeterControllerScript>();
        List<Raider> healers = all.FindAll(x => x.RaiderStats.GetRole() == Enums.CharacterRole.Healer|| x.RaiderStats.GetCurrentSpec() == Enums.CharacterSpec.Knight);
        int healerBarsCount = healers.Count > StaticValues.MaxNumHealingMeterBars ? StaticValues.MaxNumHealingMeterBars : healers.Count;
        m_healingMcs.Initialize("Healing:", 625, 120, 40, 130, healerBarsCount);
        m_healingMcs.CreateEntriesFromRaid(all);

        CooldownController.Initialize();
    }

	
	// Update is called once per frame
	void Update () {
        if (currentStep == Enums.EncounterSteps.FightInProgress && (IsBossDead() || (m_numDeadRaidMembers == StaticValues.RaidTeamSize)))
        {
            BossCastScript.StopCasting();

            if(encounter.CurrentAbility != null)
                EndCastingAbility(encounter.CurrentAbility);

            currentStep++;
            AdvanceNextStep();
        }
    }

    public void AdvanceNextStep() {
        
        SetupText.text = "Current step is " + currentStep;
        switch (currentStep)
        {
            case Enums.EncounterSteps.EncounterStart:
                currentStep++;
                break;
            case Enums.EncounterSteps.ApplyConsumables:
                SetupConsumableButtons();
                currentStep++;
                break;
            case Enums.EncounterSteps.CalculateRaiderPerformanceForAttempt:
                KillOffConsumableButtons();
                CalculateEncounterSkill();
                currentStep++;
                break;
            case Enums.EncounterSteps.ReadyToPull:
                ControlPanel.SetActive(true);
                currentStep++;
                break;
            case Enums.EncounterSteps.FightStart:
                SetupPanel.SetActive(false);
                for (int i = 0; i < all.Count; i++)
                {
                    StartCoroutine(m_raiderScripts[i].StartFight(i*0.1f, i, all[i], this));
                }
                encounter.BeginEncounter();
                m_fightStartTime = Time.time;
                raidText.text = "";
                currentStep++;
                break;
            case Enums.EncounterSteps.FightInProgress:
                break;
            case Enums.EncounterSteps.FightDone:
                /*raidText.text = "Total Stats:\n\n";
                for (int i = 0; i < all.Count; i++)
                {
                    raidText.text += all[i].GetName() + " - " + all[i].RaiderStats.GetCurrentSpec().ToString() + " (ThPut: " + all[i].RaiderStats.GetThroughput() + ", skill: " + all[i].RaiderStats.Skills.AverageSkillLevel + ", VTA: " + all[i].RaiderStats.GetVarianceMultiplierThisAttempt() + ", gear: " + all[i].RaiderStats.Gear.AverageItemLevel + ", var: " + all[i].RaiderStats.GetVariance() + " %)\n";
                }*/
                m_damageMcs.FightEnded(Time.time - m_fightStartTime);
                m_healingMcs.FightEnded(Time.time - m_fightStartTime);
                if (encounter.IsDead())
                {
                    GrantSkillIncreasesFromEncounterVictory();
                }
                    currentStep++;
                break;
            case Enums.EncounterSteps.FightWon:
            case Enums.EncounterSteps.FightLost:
            case Enums.EncounterSteps.GoToMainScreen:
                DataController.controller.Save();
                if (encounter.IsDead())
                {
                    SceneManager.LoadScene("EncounterVictoryScene");
                }
                SceneManager.LoadScene("MainGameScene");
                break;
            default:
                break;
        }
    }

    public bool IsBossDead()
    {
        return encounter.IsDead();
    }

    public void DealDamage(int damage, string attacker, string attack, int index) {
        //string newText = attacker + " deals " + damage + " damage with " + attack + "!\n";
        //raidText.text = newText + raidText.text;
        encounter.HealthBar.ModifyHealth(-damage);
        m_damageMcs.AddAmountToEntry(attacker, index, damage);
    }

    public void DoHeal(int healAmount, string healer, string heal, int index)
    {
        m_healingMcs.AddAmountToEntry(healer, index, healAmount);
    }

    public void RaiderDied()
    {
        m_numDeadRaidMembers++;
    }

    public void BeginCastingAbility(EncounterAbility ab)
    {
        raidText.text = encounter.Name + " begins casting " + ab.Name + "!\n" + raidText.text;
        BossCastScript.InitiateCast(ab.CastTime, ab.Name);

        if (ab.Ability == Enums.Ability.Immune || ab.Ability == Enums.Ability.Interrupt)
        {
            for (int i = 0; i < m_raiderScripts.Count; i++)
            {
                if (m_raiderScripts[i].Raider.RaiderStats.Ability.Ability == ab.Ability && !m_raiderScripts[i].IsDead())
                {
                    m_raiderScripts[i].HealthBarButton.interactable = true;
                    m_raiderScripts[i].HealthBarButton.GetComponent<Image>().color = new Color(Color.green.r, Color.green.g, Color.green.b, 0.98f);
                }
                else
                {
                    m_raiderScripts[i].HealthBarButton.GetComponent<Image>().color = new Color(Color.grey.r, Color.grey.g, Color.grey.b, 0.98f);
                }
            }
        }
        else if (ab.Ability == Enums.Ability.PostMovePositional || ab.Ability == Enums.Ability.PreMovePositional)
        {
            PositionalButton.gameObject.SetActive(true);
            PositionalButtonText.text = "Move out of " + ab.Name + ", raiders!";
            encounter.InitiatePreMovePositionalAbility();
        }
    }

    public void EndCastingAbility(EncounterAbility ab)
    {
        if (ab.Ability == Enums.Ability.Immune || ab.Ability == Enums.Ability.Interrupt)
        {
            for (int i = 0; i < m_raiderScripts.Count; i++)
            {
                m_raiderScripts[i].HealthBarButton.interactable = false;
                m_raiderScripts[i].HealthBarButton.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            }
        }
        else if (ab.Ability == Enums.Ability.PostMovePositional || ab.Ability == Enums.Ability.PreMovePositional)
        {
            PositionalButton.gameObject.SetActive(false);
        }
    }

    public void AttemptToCounterCurrentEncounterAbility(Raider counter)
    {
        for (int i = 0; i < m_raiderScripts.Count; i++)
        {
            if (m_raiderScripts[i].Raider == counter)
            {
                m_raiderScripts[i].HealthBarButton.interactable = false;
                m_raiderScripts[i].HealthBarButton.GetComponent<Image>().color = new Color(Color.grey.r, Color.grey.g, Color.grey.b, 0.98f);
                break;
            }
        }

        bool success = encounter.AttemptToCounterCurrentAbility(counter);

        raidText.text = counter.GetName() + " tried to counter "+ encounter.CurrentAbility.Name + " and" + (success ? " succeeded!\n" : " failed!\n") + raidText.text;
        if (success)
        {
            EndCastingAbility(encounter.CurrentAbility);
            BossCastScript.StopCasting();
            encounter.CurrentAbilityCountered();
        }
    }

    public void UseRaiderCooldown(RaiderScript raider)
    {
        if (raider.Raider.RaiderStats.Cooldown.Cooldowneffects.m_targets == Enums.CooldownTargets.Self)
        {
            raider.AddCooldown(raider.Raider.RaiderStats.Cooldown);
        }
        else if (raider.Raider.RaiderStats.Cooldown.Cooldowneffects.m_targets == Enums.CooldownTargets.Raid)
        {
            for (int i = 0; i < m_raiderScripts.Count; i++)
            {
                m_raiderScripts[i].AddCooldown(raider.Raider.RaiderStats.Cooldown);
            }
        }
        else
        {
            Debug.LogAssertion("CooldownTargetType not found!");
        }
    }

    public void RaiderTaunt(RaiderScript taunter)
    {
        encounter.SetCurrentTarget(taunter);
    }

    public void UseConsumable(ConsumableItem item)
    {
        if (PlayerData.UseConsumable(item)) {
            for (int i = 0; i < m_raiderScripts.Count; i++)
            {
                m_raiderScripts[i].AddConsumable(item);
            }
            KillOffConsumableButtons();
            currentStep++;
            AdvanceNextStep();
        }
    }

    public void OnPositionalButtonClick()
    {
        if (encounter.AttemptToMove())
        {
            PositionalButton.gameObject.SetActive(false);
        }
    }

    void GrantSkillIncreasesFromEncounterVictory()
    {
        int maxAmountOfSkillUps = 2;
        float encounterDifficultyInfluence = 2.5f;

        switch (encounter.Difficulty)
        {
            case Enums.Difficulties.Easy:
                break;
            case Enums.Difficulties.Normal:
                maxAmountOfSkillUps = 3;
                encounterDifficultyInfluence = 1.50f;
                break;
            case Enums.Difficulties.Hard:
                maxAmountOfSkillUps = 4;
                encounterDifficultyInfluence = 1.05f;
                break;
            default:
                break;
        }

        List<int> indices = new List<int>();

        m_raiderScripts.Sort(delegate (RaiderScript x, RaiderScript y)
        {
            return Random.Range(-1, 2);
        });

        for (int i = 0; i < m_raiderScripts.Count; i++)
        {
            if (indices.Count >= maxAmountOfSkillUps)
                break;

            int cutOff = (int)(m_raiderScripts[i].Raider.RaiderStats.Skills.AverageSkillLevel * encounterDifficultyInfluence);
            int roll = Random.Range(0, 100);
            if (cutOff < roll)
                indices.Add(i);
        }

        for (int i = 0; i < indices.Count; i++)
        {
            Enums.SkillTypes type = (Enums.SkillTypes)Random.Range(0, (int)Enums.SkillTypes.NumSkillTypes);
            int maxRoll = ((StaticValues.MaxSkill - m_raiderScripts[indices[i]].Raider.RaiderStats.Skills.AverageSkillLevel) % 20) + 1;
            int newSkillLevel = m_raiderScripts[indices[i]].Raider.RaiderStats.Skills.GetSkillLevel(type) + Random.Range(1, maxRoll);
            m_raiderScripts[indices[i]].Raider.RaiderStats.Skills.ModifySkill(newSkillLevel, type);
            raidText.text = m_raiderScripts[indices[i]].Raider.GetName() + " had " + type.ToString() + " skill increased to " + newSkillLevel + "!\n" + raidText.text;
        }

    }

    void CalculateEncounterSkill() {
        for (int i = 0; i < all.Count; i++)
        {
            all[i].RaiderStats.ComputeSkillThisAttempt();
            string newText = all[i].GetName() + ": " + all[i].RaiderStats.GetThroughput().ToString() + " throughput this try ( " + all[i].RaiderStats.GetAverageThroughput().ToString() + " average throughout)\n";

            raidText.text = newText + raidText.text;
        }
    }

    void SetupConsumableButtons() {

        SetupText.text = "Apply Consumable to Raid:";
        List<ConsumableItem> types = new List<ConsumableItem>();
        for (int i = 0; i < PlayerData.Consumables.Count; i++)
        {
            if (types.FindAll(x => x.Name == PlayerData.Consumables[i].Name).Count == 0)
                types.Add(new ConsumableItem( PlayerData.Consumables[i]));
        }

        for (int i = 0; i < types.Count; i++)
        {
            GameObject temp = GameObject.Instantiate(ConsumablePrefab);
            temp.name = types[i].Name;
            temp.SetActive(true);
            temp.transform.SetParent(canvas.transform);
            temp.transform.SetPositionAndRotation(new Vector3(15 + (i % 3)*130, 275 - (i / 3) * 38, 0), Quaternion.identity);
            temp.AddComponent<RaidSceneConsumablePrefab>();
            temp.GetComponent<RaidSceneConsumablePrefab>().Initialize(this, types[i]);
            m_consumableButtons.Add(temp);
        }
    }

    void KillOffConsumableButtons()
    {
        for (int i = 0; i < m_consumableButtons.Count; i++)
        {
            Destroy(m_consumableButtons[i]);
        }
        m_consumableButtons.Clear();
    }

    void CreateRaidHealthBars()
    {
        m_raiderScripts = new List<RaiderScript>();
        for (int i = 0; i < all.Count; i++)
        {
            GameObject tempTwo = GameObject.Instantiate(HealthBarPrefab);
            tempTwo.name = all[i].GetName();
            tempTwo.transform.SetParent(canvas.transform);
            tempTwo.AddComponent<RaiderScript>();
            m_raiderScripts.Add(tempTwo.GetComponent<RaiderScript>());
            m_raiderScripts[i].Initialize(all[i], tempTwo.GetComponent<HealthBarScript>(), canvas, i);
        }
    }
    
    void CreateEncounter() {

        encounter = Utility.CurrentEncounter;

        GameObject temp = GameObject.Instantiate(HealthBarPrefab);
        temp.name = encounter.Name;
        temp.transform.SetParent(canvas.transform);

        encounter.InitializeForRaid(m_raiderScripts, this, temp.GetComponent<HealthBarScript>());
        Utility.SetCurrentEncounter(encounter);
    }
}
