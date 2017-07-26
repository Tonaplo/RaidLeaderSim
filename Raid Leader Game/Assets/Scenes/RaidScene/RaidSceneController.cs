using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RaidSceneController : MonoBehaviour {

    public GameObject HealthBarPrefab;
    public Text raidText;
    public Text encounterText;
    public Canvas canvas;

    Enums.EncounterSteps currentStep = Enums.EncounterSteps.EncounterStart;
    int currentSubStep = -1;
    BaseEncounter encounter;
    List<Raider> all = new List<Raider>();
    int[] totalDamage = new int[10];
    HealthBarScript m_bossHealthScript;
    List<RaiderScript> m_raiderScripts;

    public List<RaiderScript> GetRaid() { return m_raiderScripts; }

    // Use this for initialization
    void Start () {
        CreateTestRaid();
        CreateRaidHealthBars();
        CreateTestEncounter();
        raidText.text = "";
        encounterText.text = "Current step is " + (int)currentStep;

        GameObject temp = GameObject.Instantiate(HealthBarPrefab);
        temp.transform.SetParent(canvas.transform);
        m_bossHealthScript = temp.GetComponent<HealthBarScript>();
        m_bossHealthScript.SetupHealthBar(350, 375, 100, 600, encounter.BossHealth);

        totalDamage = new int[all.Count];
        for (int i = 0; i < all.Count; i++)
        {
            totalDamage[i] = 0;
        }
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
                    Enums.CharacterAttack attack = all[i].RaiderStats().GetBaseAttack();
                    m_raiderScripts[i].StartFight(i, all[i], this);
                }
                encounter.BeginEncounter();

                currentStep++;
                encounterText.text = "Current step is " + (int)currentStep;
                break;
            case Enums.EncounterSteps.FightInProgress:
                break;
            case Enums.EncounterSteps.FightWon:
            case Enums.EncounterSteps.FightLost:
                raidText.text = "Total Damage Done:\n\n";
                for (int i = 0; i < all.Count; i++)
                {
                    raidText.text += all[i].GetName() + "(" + all[i].RaiderStats().GetRole()  + ", " + all[i].RaiderStats().GetThroughput() + " throughput) : " + totalDamage[i] + " damage.\n";
                }
                break;
            default:
                break;
        }
    }

    public bool IsBossDead()
    {
        return !(m_bossHealthScript.HealthBarSlider.value > 0);
    }

    public void DealDamage(int damage, string attacker, string attack, int index) {
        string newText = attacker + " deals " + damage + " damage with " + attack + "!\n";
        raidText.text = newText + raidText.text;
        raidText.text.Trim();
        totalDamage[index] += damage;
        m_bossHealthScript.ModifyHealth(-damage);
    }

    void CalculateEncounterSkill() {
        if (currentSubStep >= 0 || currentSubStep <= (all.Count - 1))
        {
            all[currentSubStep].RaiderStats().ComputeSkillThisAttempt();
            string newText = all[currentSubStep].GetName() + ": " + all[currentSubStep].RaiderStats().GetSkillThisAttempt().ToString() + " skill this attempt (" +
                GetDifferenceFromSkillLevel(all[currentSubStep].RaiderStats().GetSkillLevel(), all[currentSubStep].RaiderStats().GetSkillThisAttempt()) + ")\n";

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

    string GetDifferenceFromSkillLevel(int skill, int thisAttempt) {
        int difference = thisAttempt - skill;

        if (difference > 0)
            return difference.ToString() + " more than normal";
        else if(difference < 0 )
            return difference.ToString() + " less than normal";

        return "normal level";
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
    void CreateTestRaid()
    {
        all.Add(new Raider("Everett", new RaiderStats(10, 30, 5, Enums.CharacterRole.Tank, Enums.CharacterClass.Fighter)));
        all.Add(new Raider("Greybone", new RaiderStats(14, 30, 2, Enums.CharacterRole.Tank, Enums.CharacterClass.Paladin)));

        all.Add(new Raider("Mallusof", new RaiderStats(10, 10, 3, Enums.CharacterRole.Healer, Enums.CharacterClass.Paladin)));
        all.Add(new Raider("Amranar", new RaiderStats(12, 20, 6, Enums.CharacterRole.Healer, Enums.CharacterClass.Totemic)));
        all.Add(new Raider("Granjior", new RaiderStats(14, 30, 9, Enums.CharacterRole.Healer, Enums.CharacterClass.Sorcerous)));
        all.Add(new Raider("Farahn", new RaiderStats(19, 40, 9, Enums.CharacterRole.Healer, Enums.CharacterClass.Sorcerous)));


        all.Add(new Raider("Morifa", new RaiderStats(10, 25, 1, Enums.CharacterRole.RangedDPS, Enums.CharacterClass.Sorcerous)));
        all.Add(new Raider("Kaldorath", new RaiderStats(10, 25, 1, Enums.CharacterRole.MeleeDPS, Enums.CharacterClass.Fighter)));

        all.Add(new Raider("Faerand", new RaiderStats(15, 25, 1, Enums.CharacterRole.RangedDPS, Enums.CharacterClass.Shadow)));
        all.Add(new Raider("Rahran", new RaiderStats(15, 25, 1, Enums.CharacterRole.MeleeDPS, Enums.CharacterClass.Shadow)));

        all.Add(new Raider("Fimwack", new RaiderStats(20, 25, 1, Enums.CharacterRole.RangedDPS, Enums.CharacterClass.Totemic)));
        all.Add(new Raider("Miriyal", new RaiderStats(20, 25, 1, Enums.CharacterRole.MeleeDPS, Enums.CharacterClass.Shadow)));

        //all.Add(new Raider("Fimwack", new RaiderStats(40, 45, 1, Enums.CharacterRole.RangedDPS, Enums.CharacterClass.Sorcerous)));
        //all.Add(new Raider("Earendil", new RaiderStats(40, 45, 1, Enums.CharacterRole.MeleeDPS, Enums.CharacterClass.Shadow)));

        for (int i = 0; i < all.Count; i++)
        {
            all[i].CalculateMaxHealth();
        }
    }

    void CreateTestEncounter() {
        List<EncounterAbility> abilities = new List<EncounterAbility>();

        abilities.Add(new EncounterAbility("First Ability", "Generic ability that should be interrupted", Enums.Ability.Interrupt, new EncounterAbilityEffect(10, 10)));
        abilities.Add(new EncounterAbility("Second Ability", "Generic ability that should be dispelled", Enums.Ability.Dispel, new EncounterAbilityEffect(10, 10)));

        List<BaseCooldown> cooldowns = new List<BaseCooldown>();
        encounter = new BaseEncounter(10000, Enums.Difficulties.Easy, abilities, cooldowns, m_raiderScripts, this);
    }
}
