using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class RaidSceneController : MonoBehaviour {

    public GameObject HealthBarPrefab;
    public GameObject MeterPrefab;
    public GameObject ConsumablePrefab;
    public GameObject AdvanceStepButton;
    public Text AdvanceStepButtonText;
    public Text raidText;
    public Canvas canvas;
    public Text SetupText;
    public Button PositionalButton;
    public Text PositionalButtonText;
    public GameObject ControlPanel;
    public GameObject SetupPanel;
    public BossCastBarScript BossCastScript;
    public RaidSceneControlPanel ControlPanelController;
    public Button TankCooldownButton;
    public Button HealerCooldownButton;
    public Button DPSCooldownButton;
    public Text TankCooldownButtonText;
    public Text HealerCooldownButtonText;
    public Text DPSCooldownButtonText;

    Enums.EncounterSteps currentStep = Enums.EncounterSteps.EncounterStart;
    BaseEncounter encounter;
    List<Raider> all = new List<Raider>();
    List<RaiderScript> m_raiderScripts;
    MeterControllerScript m_damageMcs;
    MeterControllerScript m_healingMcs;
    float m_fightStartTime;
    int m_numDeadRaidMembers = 0;
    List<GameObject> m_consumableButtons = new List<GameObject>();
    RaiderScript m_cachedCurrentTarget = null;
    List<string> m_eventLogTexts = new List<string>();
    Enums.EventLogType m_currentEventLogType;

    public BaseEncounter Encounter {get  { return encounter; } }

    public List<RaiderScript> GetRaid() { return m_raiderScripts; }

    // Use this for initialization
    void Start () {
        //Utility.DebugInitalize();

        for (int i = 0; i < (int)Enums.EventLogType.NumEventLogTypes; i++)
        {
            m_eventLogTexts.Add("");
        }

        all = PlayerData.RaidTeam;
        CreateRaidHealthBars();
        CreateEncounter();

        

        //This random sequence of arithmetics makes Unity slow down enough to execute the previous functions fully,
        //before it continues. If this is not here, the previous functions will not have setup the things necessary
        //for the below function to actually work - Thanks Obama (I mean "fuck you, Unity").

        int whatever = 9;
        whatever *= 9;
        bool thisIsStupid = whatever == 54;
        thisIsStupid = !thisIsStupid;

        SetupUI();
        ControlPanel.SetActive(false);
        SetupPanel.SetActive(true);
        PositionalButton.gameObject.SetActive(false);
    }

    void SetupUI()
    {
        AdvanceStepButtonText.text = "Prepare for the attempt";
        raidText.text = "";
        SetupText.text = PlayerData.RaidTeamName + " taking on\n" + encounter.Name + "\non\n" + encounter.Difficulty.ToString() + " difficulty.";

        GameObject temp = GameObject.Instantiate(MeterPrefab);
        temp.transform.SetParent(canvas.transform, false);
        m_damageMcs = temp.GetComponent<MeterControllerScript>();
        List<Raider> dps = all.FindAll(x => x.RaiderStats.GetRole() == Enums.CharacterRole.MeleeDPS || x.RaiderStats.GetRole() == Enums.CharacterRole.RangedDPS);
        int dpsBarsCount = dps.Count > StaticValues.MaxNumDPSMeterBars ? StaticValues.MaxNumDPSMeterBars : dps.Count;
        m_damageMcs.Initialize("Damage:", 485, 120, 40, 130, dpsBarsCount);
        m_damageMcs.CreateEntriesFromRaid(all);

        GameObject temptwo = GameObject.Instantiate(MeterPrefab);
        temptwo.transform.SetParent(canvas.transform, false);
        m_healingMcs = temptwo.GetComponent<MeterControllerScript>();
        List<Raider> healers = all.FindAll(x => x.RaiderStats.GetRole() == Enums.CharacterRole.Healer|| x.RaiderStats.GetCurrentSpec() == Enums.CharacterSpec.Knight);
        int healerBarsCount = healers.Count > StaticValues.MaxNumHealingMeterBars ? StaticValues.MaxNumHealingMeterBars : healers.Count;
        m_healingMcs.Initialize("Healing:", 625, 120, 40, 130, healerBarsCount);
        m_healingMcs.CreateEntriesFromRaid(all);

        ControlPanelController.Initialize();

        List<RaiderScript> dpsScripts = m_raiderScripts.FindAll(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.MeleeDPS || x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.RangedDPS);
        DPSCooldownButtonText.text = "DPS\nCooldowns\n(" + dpsScripts.FindAll(x => !x.CooldownUsed).Count + ")";

        List<RaiderScript> healerScripts = m_raiderScripts.FindAll(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.Healer);
        HealerCooldownButtonText.text = "Healer\nCooldowns\n(" + healerScripts.FindAll(x => !x.CooldownUsed).Count + ")";

        List<RaiderScript> tanks = m_raiderScripts.FindAll(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.Tank);
        TankCooldownButtonText.text = "Tank\nCooldowns\n(" + tanks.FindAll(x => !x.CooldownUsed).Count + ")";
        
    }
	
	// Update is called once per frame
	void Update () {
        if (currentStep == Enums.EncounterSteps.FightInProgress && (IsBossDead() || (m_numDeadRaidMembers == StaticValues.RaidTeamSize)))
        {
            
            BossCastScript.StopCasting();
            
            for (int i = 0; i < m_raiderScripts.Count; i++)
            {
                m_raiderScripts[i].HealthBar.FightEnded();
            }

            if (encounter.CurrentAbility != null)
                EndCastingAbility(encounter.CurrentAbility);

            PositionalButton.gameObject.SetActive(false);

            currentStep++;
            AdvanceNextStep();
        }

        if (currentStep == Enums.EncounterSteps.FightInProgress) {
            if (m_cachedCurrentTarget != encounter.CurrentRaiderTarget) {
                if (m_cachedCurrentTarget != null)
                    m_cachedCurrentTarget.HealthBar.SetCurrentTank(false);

                m_cachedCurrentTarget = encounter.CurrentRaiderTarget;
                m_cachedCurrentTarget.HealthBar.SetCurrentTank(true);
            }
        }
    }

    public void AdvanceNextStep() {
        
        switch (currentStep)
        {
            case Enums.EncounterSteps.EncounterStart:
                currentStep++;
                AdvanceNextStep();
                break;
            case Enums.EncounterSteps.ApplyConsumables:
                AdvanceStepButtonText.text = "Continue without using an item";
                SetupConsumableButtons();
                currentStep++;
                break;
            case Enums.EncounterSteps.CalculateRaiderPerformanceForAttempt:
                KillOffConsumableButtons();
                CalculateEncounterSkill();
                currentStep++;
                AdvanceNextStep();
                break;
            case Enums.EncounterSteps.ReadyToPull:
                ControlPanel.SetActive(true);
                SetEventLogType(0);
                SetupPanel.SetActive(false);
                currentStep++;
                AdvanceStepButtonText.text = "Pull " + encounter.Name + "!";
                break;
            case Enums.EncounterSteps.FightStart:
                AdvanceStepButton.SetActive(false);
                for (int i = 0; i < all.Count; i++)
                {
                    m_raiderScripts[i].HealthBar.SetCurrentTank(i == 0); // Set the first tank to be the currencyTarget
                    StartCoroutine(m_raiderScripts[i].StartFight(UnityEngine.Random.Range( 0.1f, 0.5f), i, all[i], this));
                }
                encounter.BeginEncounter();
                m_fightStartTime = Time.time;
                AddTextToEventLog(encounter.Name + " was pulled!", Enums.EventLogType.Boss);

                for (int i = 0; i < encounter.Enemies.Count; i++)
                {
                    encounter.Enemies[i].Healthbar.gameObject.SetActive(true);
                }

                currentStep++;
                break;
            case Enums.EncounterSteps.FightInProgress:
                break;
            case Enums.EncounterSteps.FightDone:
                AdvanceStepButton.SetActive(true);
                m_damageMcs.FightEnded(Time.time - m_fightStartTime);
                m_healingMcs.FightEnded(Time.time - m_fightStartTime);
                if (encounter.IsDead())
                {
                    AdvanceStepButtonText.text = "Victory!";
                    AddTextToEventLog(encounter.Name + " was defeated by " + PlayerData.RaidTeamName + "!", Enums.EventLogType.Boss);
                    GrantSkillIncreasesFromEncounterVictory();
                }
                else
                {
                    AddTextToEventLog(PlayerData.RaidTeamName + " was defeated by " + encounter.Name + "!", Enums.EventLogType.Boss);
                    AdvanceStepButtonText.text = "Defeat!";
                }
                PlayerData.ConsumeAttempt();

                currentStep++;
                break;
            case Enums.EncounterSteps.FightWon:
            case Enums.EncounterSteps.FightLost:
            case Enums.EncounterSteps.GoToMainScreen:
                DataController.controller.Save();
                if (encounter.IsDead())
                {
                    PlayerData.EncounterBeaten(encounter.EncounterEnum, encounter.Difficulty);
                    SceneManager.LoadScene("EncounterVictoryScene");
                }
                else
                {
                    SceneManager.LoadScene("MainGameScene");
                }
                break;
            default:
                break;
        }
    }

    public void SetEventLogType(int intType)
    {
        Enums.EventLogType type = (Enums.EventLogType)intType;
        m_currentEventLogType = type;
        raidText.text = m_eventLogTexts[(int)type];
    }

    public void AddTextToEventLog(string text, Enums.EventLogType type)
    {
        int minutes = Mathf.RoundToInt(Time.time - m_fightStartTime) / 60;
        int seconds = Mathf.RoundToInt(Time.time - m_fightStartTime) % 60;
        string newText = " <color=#ffff00ff>[" + minutes + ":" + (seconds < 10 ? "0" : "") + seconds + "]:</color> " + text + "\n";
        m_eventLogTexts[(int)type] = newText + m_eventLogTexts[(int)type];

        if (m_currentEventLogType == type)
        {
            raidText.text = m_eventLogTexts[(int)type];
        }

        /*
        if (raidText.text.Length > 1800)
        {
            raidText.text = raidText.text.Remove(raidText.text.LastIndexOf("\n"));
        }
        */
    }

    public bool IsBossDead()
    {
        return encounter.IsDead();
    }

    public bool IsRaidDead()
    {
        return m_numDeadRaidMembers == StaticValues.RaidTeamSize;
    }

    public EncounterEnemy DealDamage(int damage, RaiderScript attacker, int index, out int actualDamage) {
        return DealDamageInternal(damage, attacker, index, out actualDamage, null);
    }

    public EncounterEnemy DealDamageToTarget(int damage, RaiderScript attacker, int index, out int actualDamage, EncounterEnemy target)
    {
        return DealDamageInternal(damage, attacker, index, out actualDamage, target);
    }

    EncounterEnemy DealDamageInternal(int damage, RaiderScript attacker, int index, out int actualDamage, EncounterEnemy target)
    {
        //We cant deal damage to any enemies if there if the fight is over.
        //This can happen if we have DoT damage queued up and the raid dies.
        if (target != null && target.Healthbar.IsDead())
        {
            target = null;
            actualDamage = 0;
            return null;
        }

        EncounterEnemy actualTarget = null;
        if (target == null)
            actualTarget = encounter.TakeDamage(damage, attacker, out actualDamage);
        else 
            actualTarget = encounter.TakeDamageSpecific(damage, attacker, out actualDamage, target);

        m_damageMcs.AddAmountToEntry(attacker.Raider.GetName(), index, actualDamage);
        return actualTarget;
    }

    public void DoHeal(int healAmount, string healer, string heal, int index)
    {
        m_healingMcs.AddAmountToEntry(healer, index, healAmount);
    }

    public void RaiderDied(string raiderName, string killedBy, int damageTaken, int overkill)
    {
        AddTextToEventLog(raiderName + " died from <b>" + killedBy + "</b> (" + damageTaken + " damage taken, " + overkill +" overkill).", Enums.EventLogType.Deaths);
        m_numDeadRaidMembers++;
    }

    public void BeginCastingAbility(EncounterAbility ab)
    {
        string castOrChannel = " casting ";

        switch (ab.CastType)
        {
            case Enums.AbilityCastType.Cast:
                break;
            case Enums.AbilityCastType.Channel:
                castOrChannel = " channeling ";
                break;
            default:
                break;
        }
        AddTextToEventLog(ab.Caster + " begins" + castOrChannel + "<b>" + ab.Name + "</b>!", Enums.EventLogType.Boss);

        BossCastScript.InitiateCast(ab);

        if (ab.Ability == Enums.Ability.Immune || ab.Ability == Enums.Ability.Interrupt || ab.Ability == Enums.Ability.Stun)
        {
            List<RaiderScript> dps = m_raiderScripts.FindAll(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.MeleeDPS || x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.RangedDPS);
            for (int i = 0; i < dps.Count; i++)
            {
                if (dps[i].Raider.RaiderStats.Ability.Ability == ab.Ability && !dps[i].IsDead())
                {
                    dps[i].HealthBarButton.interactable = true;
                    dps[i].HealthBarButton.GetComponent<Image>().color = Color.green;
                }
            }
        }
        else if (ab.Ability == Enums.Ability.PreMovePositional)
        {
            PositionalButton.gameObject.SetActive(true);
            PositionalButtonText.text = "Move out of " + ab.Name + ", raiders!";
            encounter.InitiatePositionalAbility();
        }
    }

    public void EndCastingAbility(EncounterAbility ab)
    {
        if (ab.Ability == Enums.Ability.Immune || ab.Ability == Enums.Ability.Interrupt || ab.Ability == Enums.Ability.Stun)
        {
            List<RaiderScript> dps = m_raiderScripts.FindAll(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.MeleeDPS || x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.RangedDPS);
            for (int i = 0; i < dps.Count; i++)
            {
                dps[i].HealthBarButton.interactable = false;
                dps[i].HealthBarButton.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            }
        }
        else if (ab.Ability == Enums.Ability.PostMovePositional || ab.Ability == Enums.Ability.PreMovePositional)
        {
            PositionalButton.gameObject.SetActive(ab.Ability == Enums.Ability.PostMovePositional);

            if (ab.Ability == Enums.Ability.PostMovePositional) {
                PositionalButtonText.text = "Move out of " + ab.Name + ", raiders!";
                encounter.InitiatePositionalAbility();
            }
        }

        if(ab.Ability != Enums.Ability.Immune)
            BossCastScript.StopCasting();
    }

    public void AttemptToCounterCurrentEncounterAbility(Raider counter)
    {
        for (int i = 0; i < m_raiderScripts.Count; i++)
        {
            if (m_raiderScripts[i].Raider == counter)
            {
                m_raiderScripts[i].HealthBarButton.interactable = false;
                m_raiderScripts[i].HealthBarButton.GetComponent<Image>().color = new Color(0,0,0,0);
                break;
            }
        }

        bool success = encounter.AttemptToCounterCurrentAbility(counter);

        AddTextToEventLog(Utility.GetColoredRaiderName(counter) + " tried to counter <b>" + encounter.CurrentAbility.Name + "</b> and" + (success ? " succeeded!" : " failed!"), Enums.EventLogType.AbilityCounter);
        if (success)
        {
            EndCastingAbility(encounter.CurrentAbility);
            encounter.CurrentAbilityCountered();
        }
    }

    public void UseTankCooldown()
    {
        List<RaiderScript> tanks = m_raiderScripts.FindAll(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.Tank);
        for (int i = 0; i < tanks.Count; i++)
        {
            if (!tanks[i].CooldownUsed)
            {
                tanks[i].UseCooldown();
                break;
            }
        }

        if (tanks.FindAll(x => !x.CooldownUsed).Count == 0)
        {
            TankCooldownButton.interactable = false;
            TankCooldownButtonText.text = "None Left!";
            TankCooldownButton.GetComponent<Image>().color = Color.grey;
        }
        else {
            TankCooldownButtonText.text = "Tank\nCooldowns\n(" + tanks.FindAll(x => !x.CooldownUsed).Count + ")";
        }
    }

    public void UseHealerCooldown()
    {
        List<RaiderScript> healers = m_raiderScripts.FindAll(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.Healer);
        for (int i = 0; i < healers.Count; i++)
        {
            if (!healers[i].CooldownUsed) { 
                healers[i].UseCooldown();
                break;
            }
        }

        if (healers.FindAll(x => !x.CooldownUsed).Count == 0)
        {
            HealerCooldownButton.interactable = false;
            HealerCooldownButtonText.text = "None Left!";
            HealerCooldownButton.GetComponent<Image>().color = Color.grey;
        }
        else
        {
            HealerCooldownButtonText.text = "Healer\nCooldowns\n(" + healers.FindAll(x => !x.CooldownUsed).Count + ")";
        }
    }

    public void UseDPSCooldown()
    {
        List<RaiderScript> dps = m_raiderScripts.FindAll(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.MeleeDPS || x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.RangedDPS);
        for (int i = 0; i < dps.Count; i++)
        {
            if (!dps[i].CooldownUsed) { 
                dps[i].UseCooldown();
                break;
            }
        }

        if (dps.FindAll(x => !x.CooldownUsed).Count == 0)
        {
            DPSCooldownButton.interactable = false;
            DPSCooldownButtonText.text = "None Left!";
            DPSCooldownButton.GetComponent<Image>().color = Color.grey;
        }
        else
        {
            DPSCooldownButtonText.text = "DPS\nCooldowns\n(" + dps.FindAll(x => !x.CooldownUsed).Count + ")";
        }
    }

    public void UseRaiderCooldown(RaiderScript raider)
    {
        AddTextToEventLog(Utility.GetColoredRaiderName(raider.Raider) + " used <i>" + raider.Raider.RaiderStats.Cooldown.Name + "</i>!", Enums.EventLogType.Cooldown);
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
        encounter.SetCurrentRaiderTarget(taunter);
        for (int i = 0; i < m_raiderScripts.Count; i++)
        {
            if(m_raiderScripts[i] != taunter)
                m_raiderScripts[i].HealthBar.SetCurrentTank(false);
        }

        taunter.HealthBar.SetCurrentTank(true);
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
            return UnityEngine.Random.Range(-1, 2);
        });

        for (int i = 0; i < m_raiderScripts.Count; i++)
        {
            if (indices.Count >= maxAmountOfSkillUps)
                break;

            int cutOff = (int)(m_raiderScripts[i].Raider.RaiderStats.Skills.AverageSkillLevel * encounterDifficultyInfluence);
            int roll = UnityEngine.Random.Range(0, 100);
            if (cutOff < roll)
                indices.Add(i);
        }

        for (int i = 0; i < indices.Count; i++)
        {
            Enums.SkillTypes type = (Enums.SkillTypes)UnityEngine.Random.Range(0, (int)Enums.SkillTypes.NumSkillTypes);
            int maxRoll = ((StaticValues.MaxSkill - m_raiderScripts[indices[i]].Raider.RaiderStats.Skills.AverageSkillLevel) % 20) + 1;
            int newSkillLevel = m_raiderScripts[indices[i]].Raider.RaiderStats.Skills.GetSkillLevel(type) + UnityEngine.Random.Range(1, maxRoll);
            m_raiderScripts[indices[i]].Raider.RaiderStats.Skills.ModifySkill(newSkillLevel, type);
            AddTextToEventLog(Utility.GetColoredRaiderName(m_raiderScripts[indices[i]].Raider) + " had " + type.ToString() + " skill increased to " + newSkillLevel + "!", Enums.EventLogType.Boss);
        }

    }

    void CalculateEncounterSkill() {
        for (int i = 0; i < all.Count; i++)
        {
            all[i].RaiderStats.ComputeSkillThisAttempt();
            //AddTextToEventLog(all[i].GetName() + ": " + all[i].RaiderStats.GetThroughput().ToString() + " throughput this try ( " + all[i].RaiderStats.GetAverageThroughput().ToString() + " average throughout)");
            
        }
    }

    void SetupConsumableButtons() {

        SetupText.text = "Your raid may use an item to aid them in this encounter:";
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
            temp.transform.SetParent(canvas.transform, false);
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
            tempTwo.transform.SetParent(canvas.transform, false);
            tempTwo.AddComponent<RaiderScript>();
            m_raiderScripts.Add(tempTwo.GetComponent<RaiderScript>());
            m_raiderScripts[i].Initialize(all[i], tempTwo.GetComponent<HealthBarScript>(), canvas, i);
        }
    }
    
    void CreateEncounter() {

        encounter = Utility.CurrentEncounter;

        GameObject temp = GameObject.Instantiate(HealthBarPrefab);
        temp.name = encounter.Name;
        temp.transform.SetParent(canvas.transform, false);

        encounter.InitializeForRaid(m_raiderScripts, this, temp);
        Utility.SetCurrentEncounter(encounter);
    }

    public void DebuffAdded()
    {
        List<RaiderScript> healers = m_raiderScripts.FindAll(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.Healer);
        for (int i = 0; i < healers.Count; i++)
        {
            healers[i].HealthBar.SetDispelState(true);
        }
    }

    public void AttemptToDispel(RaiderScript dispeller)
    {
        bool success = UnityEngine.Random.Range(0, StaticValues.MaxSkill) <= dispeller.Raider.RaiderStats.GetMechanicalSkillForEncounter();
        RaiderScript debuffed = GetRaid().FindAll(x => x.HasDebuffs())[0];
        if (success)
        {
            debuffed.RemoveDebuff();
        }


        AddTextToEventLog(Utility.GetColoredRaiderName(dispeller.Raider) + " tried to dispel <b>" + Utility.GetColoredRaiderName(debuffed.Raider) + (success ? " succeeded!" : " failed!"), Enums.EventLogType.AbilityCounter);

        dispeller.HealthBar.AttemptedToDispell();

        if (GetRaid().FindAll(x => x.HasDebuffs()).Count == 0) {

            List<RaiderScript> healers = m_raiderScripts.FindAll(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.Healer);
            for (int i = 0; i < healers.Count; i++)
            {
                healers[i].HealthBar.SetDispelState(false);
            }
        }
    }
}
