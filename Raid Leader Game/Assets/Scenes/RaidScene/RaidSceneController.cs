using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RaidSceneController : MonoBehaviour {

    /*public Image singleTargetHealingNeededBar;
    Image singleTargetHealingProvidedBar;
    Text singleTargetHealingText;
    public Image AoEHealingNeededBar;
    Image AoEHealingProvidedBar;
    Text AoEHealingText;
    public Image singleTargetDamageNeededBar;
    Image singleTargetDamageProvidedBar;
    Text singleTargetDamageText;
    public Image AoEDamageNeededBar;
    Image AoEDamageProvidedBar;
    Text AoEDamageText;*/

    public Text raidText;
    public Text encounterText;

    int maxEncounterBarWidth = 800;
    int staticBarHeight = 50;
    int currentMaxEncounterBarNumber = 0;

    int STHealingNeeded = 300;
    int AoEHealingNeeded = 300;
    int STDamageNeeded = 300;
    int AoEDamageNeeded = 300;

    int STHealingProvided = 300;
    int AoEHealingProvided = 300;
    int STDamageProvided = 300;
    int AoEDamageProvided = 300;

    Enums.EncounterSteps currentStep = Enums.EncounterSteps.EncounterStart;
    int currentSubStep = -1;
    BaseEncounter encounter;

    List<Raider> all = new List<Raider>();

	// Use this for initialization
	void Start () {
        CreateTestRaid();
        CreateTestEncounter();
        raidText.text = "Raid:\n";
        //Get the SingleTargetHealing components

       /* singleTargetHealingProvidedBar = singleTargetHealingNeededBar.GetComponentInChildren<Image>();
        singleTargetHealingText = singleTargetHealingProvidedBar.GetComponentInChildren<Text>();

        //Get the AoEHealing components
        AoEHealingProvidedBar = AoEHealingNeededBar.GetComponentInChildren<Image>();
        AoEHealingText = AoEHealingProvidedBar.GetComponentInChildren<Text>();

        //Get the SingleTargetDamage components
        singleTargetDamageProvidedBar = singleTargetDamageNeededBar.GetComponentInChildren<Image>();
        singleTargetDamageText = singleTargetDamageProvidedBar.GetComponentInChildren<Text>();

        //Get the AoEDamage components
        AoEDamageProvidedBar = AoEDamageNeededBar.GetComponentInChildren<Image>();
        AoEDamageText = AoEDamageProvidedBar.GetComponentInChildren<Text>();

        currentMaxEncounterBarNumber = 300;
        //Rrefactor
        CalculateBar(singleTargetHealingNeededBar, singleTargetHealingProvidedBar, singleTargetHealingText, Enums.ThroughputTypes.SingleTargetHealing);
        CalculateBar(singleTargetHealingNeededBar, singleTargetHealingProvidedBar, singleTargetHealingText, Enums.ThroughputTypes.SingleTargetHealing);
        CalculateBar(singleTargetHealingNeededBar, singleTargetHealingProvidedBar, singleTargetHealingText, Enums.ThroughputTypes.SingleTargetHealing);
        CalculateBar(singleTargetHealingNeededBar, singleTargetHealingProvidedBar, singleTargetHealingText, Enums.ThroughputTypes.SingleTargetHealing);*/
	}

	
	// Update is called once per frame
	void Update () {
    }

   void CalculateBar(Image neededBar, Image providedBar, Text text, Enums.ThroughputTypes type)
    {
        /*
        int width = (int)((float)value / (float)currentMaxEncounterBarNumber * maxEncounterBarWidth);
        bar.rectTransform.sizeDelta = new Vector2(width, staticBarHeight);
        text.text = value.ToString();*/
    }

    void GetValuesForThroughputType(ref int needed, ref int provided, Enums.ThroughputTypes type) {
        switch (type)
        {
            case Enums.ThroughputTypes.SingleTargetHealing:
                needed = STHealingNeeded;
                provided = STHealingProvided;
                break;
            case Enums.ThroughputTypes.SingleTargetDPS:
                needed = STDamageNeeded;
                provided = STDamageProvided;
                break;
            case Enums.ThroughputTypes.AoEHealing:
                needed = AoEHealingNeeded;
                provided = AoEHealingProvided;
                break;
            case Enums.ThroughputTypes.AoEDPS:
                needed = AoEDamageNeeded;
                provided = AoEDamageProvided;
                break;
            default:
                break;
        }
    }

    public void AdvanceNextStep() {

        switch (currentStep)
        {
            case Enums.EncounterSteps.EncounterStart:
                currentStep = Enums.EncounterSteps.CalculateRaiderPerformanceForAttempt;
                AdvanceNextStep();
                break;
            case Enums.EncounterSteps.CalculateRaiderPerformanceForAttempt:
                CalculateEncounterSkill();
                break;
            case Enums.EncounterSteps.AssignCountersToEncounterAbilities:
                break;
            case Enums.EncounterSteps.AssignCounterToEncounterCooldowns:
                break;
            case Enums.EncounterSteps.ResolveAbilitiesCounters:
                break;
            case Enums.EncounterSteps.ResolveCooldownCounters:
                break;
            case Enums.EncounterSteps.EvaluateFight:
                break;
            default:
                break;
        }
    }

    void CalculateEncounterSkill() {
        if (currentSubStep >= 0 || currentSubStep <= (all.Count - 1))
        {
            all[currentSubStep].RaiderStats().ComputeSkillThisAttempt();
            raidText.text += all[currentSubStep].GetName() + ": " + all[currentSubStep].RaiderStats().GetSkillThisAttempt().ToString() + " skill this attempt (" +
                GetDifferenceFromSkillLevel(all[currentSubStep].RaiderStats().GetSkillLevel(), all[currentSubStep].RaiderStats().GetSkillThisAttempt()) + ")\n";

            if (all[currentSubStep].RaiderStats().GetRole() == Enums.CharacterRole.Healer)
            {
                int AoEHPS = all[currentSubStep].RaiderStats().ComputeThroughput(Enums.ThroughputTypes.AoEHealing);
                int SingleHPS = all[currentSubStep].RaiderStats().ComputeThroughput(Enums.ThroughputTypes.SingleTargetHealing);
                print(all[currentSubStep].GetName() + " will do: " + SingleHPS + "/" + AoEHPS + " (SingleHPS/AoEHPS)");
            }
            else if (all[currentSubStep].RaiderStats().GetRole() == Enums.CharacterRole.Tank)
            {
                int AoEHPS = all[currentSubStep].RaiderStats().ComputeThroughput(Enums.ThroughputTypes.AoEHealing);
                int SingleHPS = all[currentSubStep].RaiderStats().ComputeThroughput(Enums.ThroughputTypes.SingleTargetHealing);
                int AoEDPS = all[currentSubStep].RaiderStats().ComputeThroughput(Enums.ThroughputTypes.AoEDPS);
                int SingleDPS = all[currentSubStep].RaiderStats().ComputeThroughput(Enums.ThroughputTypes.SingleTargetDPS);
                print(all[currentSubStep].GetName() + " will do: " + SingleDPS + "/" + AoEDPS + " (SingleDPS/AoEDPS) and " + SingleDPS + "/" + AoEDPS + " (SingleDPS/AoEDPS)");
            }
            else
            {
                int AoEDPS = all[currentSubStep].RaiderStats().ComputeThroughput(Enums.ThroughputTypes.AoEDPS);
                int SingleDPS = all[currentSubStep].RaiderStats().ComputeThroughput(Enums.ThroughputTypes.SingleTargetDPS);
                print(all[currentSubStep].GetName() + " will do: " + SingleDPS + "/" + AoEDPS + " (SingleDPS/AoEDPS)");
                
            }

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

    string GetDifferenceFromSkillLevel(int skill, int thisAttempt) {
        int difference = thisAttempt - skill;

        if (difference > 0)
            return difference.ToString() + " more than normal";
        else if(difference < 0 )
            return difference.ToString() + " less than normal";

        return "normal level";
    }

    //DEBUG FUNCTIONS
    void CreateTestRaid()
    {
        all.Add(new Raider("Everett", new RaiderStats( 10, 30, 5, Enums.CharacterRole.Tank, Enums.CharacterClass.Fighter)));
        all.Add(new Raider("Greybone", new RaiderStats(10, 30, 2, Enums.CharacterRole.Tank, Enums.CharacterClass.Paladin)));

        all.Add(new Raider("Mallusof", new RaiderStats(10, 30, 3, Enums.CharacterRole.Healer, Enums.CharacterClass.Paladin)));
        all.Add(new Raider("Amranar", new RaiderStats(20, 20, 6, Enums.CharacterRole.Healer, Enums.CharacterClass.Totemic)));
        all.Add(new Raider("Granjior", new RaiderStats(5, 40, 9, Enums.CharacterRole.Healer, Enums.CharacterClass.Sorcerous)));


        all.Add(new Raider("Morifa", new RaiderStats(35, 15, 6, Enums.CharacterRole.RangedDPS, Enums.CharacterClass.Sorcerous)));
        all.Add(new Raider("Faerand", new RaiderStats(25, 25, 1, Enums.CharacterRole.RangedDPS, Enums.CharacterClass.Shadow)));
        all.Add(new Raider("Farahn", new RaiderStats(20, 30, 8, Enums.CharacterRole.RangedDPS, Enums.CharacterClass.Totemic)));
        all.Add(new Raider("Kaldorath", new RaiderStats(5, 45, 10, Enums.CharacterRole.MeleeDPS, Enums.CharacterClass.Fighter)));
        all.Add(new Raider("Rahran", new RaiderStats(40, 5, 8, Enums.CharacterRole.MeleeDPS, Enums.CharacterClass.Shadow)));

        currentSubStep = all.Count - 1;
    }

    void CreateTestEncounter() {
        List<EncounterAbility> abilities = new List<EncounterAbility>();

        abilities.Add(new EncounterAbility("First Ability", "Generic ability that should be interrupted", Enums.Ability.Interrupt, new EncounterAbilityEffect(10, 10)));
        abilities.Add(new EncounterAbility("Second Ability", "Generic ability that should be dispelled", Enums.Ability.Dispel, new EncounterAbilityEffect(10, 10)));

        List<BaseCooldown> cooldowns = new List<BaseCooldown>();

        //"this needs to Be completely refactored. The encounters should have health instead of HPS and DPS Requirements"
        encounter = new BaseEncounter(2, 300, 1000, 800, 300, Enums.Difficulties.Easy, abilities, cooldowns);
        encounterText.text = "This encounter needs " + encounter.ActualSingleTargetDPSNeeded.ToString() + " Single Target DPS, "  +
            encounter.ActualAoEDPSNeeded.ToString() + " AoE DPS, "  +
            encounter.ActualSingleTargetHealingNeeded.ToString() + " Single Target HPS, "  +
            encounter.ActualAoEHealingNeeded.ToString() + " AoE HPS, and this is not how it should work.";

    }
}
