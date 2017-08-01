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

    Enums.EncounterSteps currentStep = Enums.EncounterSteps.EncounterStart;
    int currentSubStep = -1;
    BaseEncounter encounter;
    List<Raider> all = new List<Raider>();
    int[] totalDamage = new int[10];
    List<RaiderScript> m_raiderScripts;
    MeterControllerScript m_damageMcs;
    MeterControllerScript m_healingMcs;

    public List<RaiderScript> GetRaid() { return m_raiderScripts; }

    public int GetBossHealthPercent() {
        return encounter.HealthBar.GetHealthPercent();
    }

    // Use this for initialization
    void Start () {
        all = PlayerData.GetRoster();
        CreateRaidHealthBars();
        CreateTestEncounter();
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
        List<Raider> dps = all.FindAll(x => x.RaiderStats().GetRole() == Enums.CharacterRole.MeleeDPS || x.RaiderStats().GetRole() == Enums.CharacterRole.RangedDPS);
        m_damageMcs.Initialize(150, 140, 40, 175, dps.Count);
        m_damageMcs.CreateEntriesFromRaid(all);

        GameObject temptwo = GameObject.Instantiate(MeterPrefab);
        temptwo.transform.SetParent(canvas.transform);
        m_healingMcs = temptwo.GetComponent<MeterControllerScript>();
        List<Raider> healers = all.FindAll(x => x.RaiderStats().GetRole() == Enums.CharacterRole.Healer);
        m_healingMcs.Initialize(330, 140, 40, 175, healers.Count);
        m_healingMcs.CreateEntriesFromRaid(all);
    }

	
	// Update is called once per frame
	void Update () {
        if (currentStep == Enums.EncounterSteps.FightInProgress && IsBossDead()) {
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

                currentStep++;
                encounterText.text = "Current step is " + (int)currentStep;
                break;
            case Enums.EncounterSteps.FightInProgress:
                break;
            case Enums.EncounterSteps.FightWon:
            case Enums.EncounterSteps.FightLost:
                raidText.text = "Total Stats:\n\n";
                for (int i = 0; i < all.Count; i++)
                {
                    raidText.text += all[i].GetName() + " - " + all[i].RaiderStats().GetCurrentSpec().ToString()  + " (ThPut: " + all[i].RaiderStats().GetThroughput() + ", skill: " + all[i].RaiderStats().GetSkillLevel() + ", STA: " + all[i].RaiderStats().GetSkillThisAttempt() + ", gear: " + all[i].RaiderStats().GetGearLevel() + ", var: " + all[i].RaiderStats().GetVariance() + " %)\n";
                }
                currentStep++;
                break;
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

    void CalculateEncounterSkill() {
        if (currentSubStep >= 0 || currentSubStep <= (all.Count - 1))
        {
            all[currentSubStep].RaiderStats().ComputeSkillThisAttempt();
            string newText = all[currentSubStep].GetName() + ": " + all[currentSubStep].RaiderStats().GetThroughput().ToString() + " throughput this try ( " + all[currentSubStep].RaiderStats().GetAverageThroughput().ToString() + " average throughout)\n";

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
    }

    void CreateRaidHealthBars()
    {
        m_raiderScripts = new List<RaiderScript>();
        for (int i = 0; i < all.Count; i++)
        {
            GameObject tempTwo = GameObject.Instantiate(HealthBarPrefab);
            tempTwo.transform.SetParent(canvas.transform);
            tempTwo.AddComponent<RaiderScript>();
            m_raiderScripts.Add(tempTwo.AddComponent<RaiderScript>());
            m_raiderScripts[i].Initialize(all[i], tempTwo.GetComponent<HealthBarScript>(), canvas, i);
        }
    }
    
    //DEBUG FUNCTIONS

    void CreateTestEncounter() {
        List<EncounterAbility> abilities = new List<EncounterAbility>();

        abilities.Add(new EncounterAbility("First Ability", "Generic m_ability that should be interrupted", Enums.Ability.Interrupt, new EncounterAbilityEffect(10, 10)));
        abilities.Add(new EncounterAbility("Second Ability", "Generic m_ability that should be dispelled", Enums.Ability.Dispel, new EncounterAbilityEffect(10, 10)));

        List<BaseCooldown> cooldowns = new List<BaseCooldown>();

        GameObject temp = GameObject.Instantiate(HealthBarPrefab);
        temp.transform.SetParent(canvas.transform);

        encounter = new BaseEncounter("Basic Encounter", 10000, Enums.Difficulties.Easy, abilities, cooldowns, m_raiderScripts, this, temp.GetComponent<HealthBarScript>());
    }
}
