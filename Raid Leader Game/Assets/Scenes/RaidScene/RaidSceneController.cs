using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class RaidSceneController : MonoBehaviour {

    public GameObject HealthBarPrefab;
    public GameObject MeterPrefab;
    public Text raidText;
    public Text encounterText;
    public Canvas canvas;
    public BossCastBarScript BossCastScript;

    Enums.EncounterSteps currentStep = Enums.EncounterSteps.EncounterStart;
    int currentSubStep = -1;
    BaseEncounter encounter;
    List<Raider> all = new List<Raider>();
    int[] totalDamage = new int[10];
    List<RaiderScript> m_raiderScripts;
    MeterControllerScript m_damageMcs;
    MeterControllerScript m_healingMcs;
    float m_fightStartTime;
    int m_numDeadRaidMembers = 0;

    public List<RaiderScript> GetRaid() { return m_raiderScripts; }

    public int GetBossHealthPercent() {
        return encounter.HealthBar.GetHealthPercent();
    }

    // Use this for initialization
    void Start () {
        all = PlayerData.RaidTeam;
        CreateRaidHealthBars();
        CreateEncounter();
        SetupUI();
        

        totalDamage = new int[all.Count];
        for (int i = 0; i < all.Count; i++)
        {
            totalDamage[i] = 0;
        }
    }

    void SetupUI()
    {
        raidText.text = "";
        encounterText.text = "Current step is " + (int)currentStep;

        GameObject temp = GameObject.Instantiate(MeterPrefab);
        temp.transform.SetParent(canvas.transform);
        m_damageMcs = temp.GetComponent<MeterControllerScript>();
        List<Raider> dps = all.FindAll(x => x.RaiderStats.GetRole() == Enums.CharacterRole.MeleeDPS || x.RaiderStats.GetRole() == Enums.CharacterRole.RangedDPS);
        int dpsBarsCount = dps.Count > StaticValues.MaxNumDPSMeterBars ? StaticValues.MaxNumDPSMeterBars : dps.Count;
        m_damageMcs.Initialize(150, 140, 40, 175, dpsBarsCount);
        m_damageMcs.CreateEntriesFromRaid(all);

        GameObject temptwo = GameObject.Instantiate(MeterPrefab);
        temptwo.transform.SetParent(canvas.transform);
        m_healingMcs = temptwo.GetComponent<MeterControllerScript>();
        List<Raider> healers = all.FindAll(x => x.RaiderStats.GetRole() == Enums.CharacterRole.Healer|| x.RaiderStats.GetCurrentSpec() == Enums.CharacterSpec.Knight);
        int healerBarsCount = healers.Count > StaticValues.MaxNumHealingMeterBars ? StaticValues.MaxNumHealingMeterBars : healers.Count;
        m_healingMcs.Initialize(330, 140, 40, 175, healerBarsCount);
        m_healingMcs.CreateEntriesFromRaid(all);
    }

	
	// Update is called once per frame
	void Update () {
        if ((currentStep == Enums.EncounterSteps.FightInProgress && IsBossDead()) || (m_numDeadRaidMembers == StaticValues.RaidTeamSize))
        {
            currentStep++;
            AdvanceNextStep();
        }
    }

    public void AdvanceNextStep() {
        
        encounterText.text = "Current step is " + (int)currentStep;
        switch (currentStep)
        {
            case Enums.EncounterSteps.EncounterStart:
                currentStep = Enums.EncounterSteps.CalculateRaiderPerformanceForAttempt;
                currentSubStep = all.Count - 1;
                while (currentSubStep >= 0)
                {
                    AdvanceNextStep();
                }
                break;
            case Enums.EncounterSteps.CalculateRaiderPerformanceForAttempt:
                CalculateEncounterSkill();
                break;
            case Enums.EncounterSteps.AssignCountersToEncounterAbilities:
                currentSubStep = encounter.EncounterAbilities.Count-1;
                AssignCountersToEncounterAbilities();
                break;
            case Enums.EncounterSteps.AssignCounterToEncounterCooldowns:
                currentStep++;
                break;
            case Enums.EncounterSteps.ResolveAbilitiesCounters:
                currentStep++;
                break;
            case Enums.EncounterSteps.ResolveCooldownCounters:
                currentStep++;
                break;
            case Enums.EncounterSteps.FightStart:
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
                raidText.text = "Total Stats:\n\n";
                for (int i = 0; i < all.Count; i++)
                {
                    raidText.text += all[i].GetName() + " - " + all[i].RaiderStats.GetCurrentSpec().ToString() + " (ThPut: " + all[i].RaiderStats.GetThroughput() + ", skill: " + all[i].RaiderStats.Skills.AverageSkillLevel + ", VTA: " + all[i].RaiderStats.GetVarianceMultiplierThisAttempt() + ", gear: " + all[i].RaiderStats.Gear.AverageItemLevel + ", var: " + all[i].RaiderStats.GetVariance() + " %)\n";
                }
                m_damageMcs.FightEnded(Time.time - m_fightStartTime);
                m_healingMcs.FightEnded(Time.time - m_fightStartTime);
                currentStep++;
                break;
            case Enums.EncounterSteps.FightWon:
                if(encounter.IsDead())
                    SceneManager.LoadScene("EncounterVictoryScene");
                currentStep++;
                break;
            case Enums.EncounterSteps.FightLost:
            case Enums.EncounterSteps.GoToMainScreen:
                DataController.controller.Save();
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
        totalDamage[index] += damage;
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
        BossCastScript.InitiateCast(ab.CastTime, ab.Name);

        for (int i = 0; i < m_raiderScripts.Count; i++)
        {
            if (m_raiderScripts[i].Raider.RaiderStats.GetAbility().Ability == ab.Ability && !m_raiderScripts[i].IsDead())
            {
                m_raiderScripts[i].HealthBarButton.interactable = true;
                m_raiderScripts[i].HealthBarButton.GetComponent<Image>().color = new Color(Color.green.r, Color.green.g, Color.green.b, 0.98f);
            }
            else {
                m_raiderScripts[i].HealthBarButton.GetComponent<Image>().color = new Color(Color.grey.r , Color.grey.g , Color.grey.b, 0.98f);
            }
        }
    }

    public void EndCastingAbility()
    {
        for (int i = 0; i < m_raiderScripts.Count; i++)
        {
            m_raiderScripts[i].HealthBarButton.interactable = false;
            m_raiderScripts[i].HealthBarButton.GetComponent<Image>().color = new Color(0, 0, 0, 0);
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
            EndCastingAbility();
            BossCastScript.StopCasting();
            encounter.CurrentAbilityCountered();
        }
    }

    void CalculateEncounterSkill() {
        if (currentSubStep >= 0 || currentSubStep <= (all.Count - 1))
        {
            all[currentSubStep].RaiderStats.ComputeSkillThisAttempt();
            string newText = all[currentSubStep].GetName() + ": " + all[currentSubStep].RaiderStats.GetThroughput().ToString() + " throughput this try ( " + all[currentSubStep].RaiderStats.GetAverageThroughput().ToString() + " average throughout)\n";

            raidText.text = newText + raidText.text;

            currentSubStep--;
            if (currentSubStep < 0)
                currentStep = Enums.EncounterSteps.AssignCountersToEncounterAbilities;
        }
        else
        {
            print("Error! Incorrect amount of substeps for characters!");
            return;
        }
    }

    void AssignCountersToEncounterAbilities() {
        if (currentSubStep >= 0)
        {
            //Need to figure out some UI elements here
            //I think using props and instantiating them correctly would be the best here.

            //Skipping this step for now
            currentSubStep = 0;
            currentStep++;
        }
        else
        {
            currentSubStep = 0;
            currentStep++;
        }
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
            m_raiderScripts.Add(tempTwo.AddComponent<RaiderScript>());
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
