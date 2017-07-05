using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiderStats {

    int gearLevel = 0;
    int skillLevel = 0;
    int variation = 0;
    int skillThisAttempt = 0;
    Enums.CharacterRole charRole;
    Enums.CharacterClass charClass;
    Enums.CharacterSpec charSpec;
    BaseAbility ability;
    BaseCooldown cooldown;

    public int GetGearLevel() { return gearLevel; }
    public int GetSkillLevel() { return skillLevel; }
    public int GetSkillThisAttempt() { return skillThisAttempt; }
    public int GetVariance() { return variation; }
    public Enums.CharacterRole GetRole() { return charRole; }
    public Enums.CharacterClass GetClass() { return charClass; }
    public BaseAbility GetAbility() { return ability; }
    public BaseCooldown GetCooldown() { return cooldown; }

    public RaiderStats(int _gearLevel, int _skillLevel, int _variation, Enums.CharacterRole _charRole, Enums.CharacterClass _charClass)
    {
        gearLevel = _gearLevel;
        skillLevel = _skillLevel;
        variation = _variation;
        charRole = _charRole;
        charClass = _charClass;
        SetSpecFromRoleAndClass();
    }

    public void ModifySkillLevel(int amount)
    {
        skillLevel += amount;

        //Clamp the skill level between 1 and 100
        if (skillLevel > (int)Enums.StaticValues.maxSkill)
            skillLevel = (int)Enums.StaticValues.maxSkill;
        else if (skillLevel < 1)
            skillLevel = 1;
    }

    public void ModifyGearLevel(int amount)
    {
        gearLevel += amount;

        //GearLevel can never be below 0
        gearLevel = gearLevel < 0 ? 0 : gearLevel;
    }

    public int ComputeThroughput(Enums.ThroughputTypes type)
    {
        //Cast to float for precision
        float floatAmount = skillThisAttempt;// (float)type;

        //Multiply with Gearlevel - increases the base by a percentage
        floatAmount *= (1.0f + ((float)GetGearLevel() / 100.0f));

        //Multiply with SkillLevel, WITHOUT the 1 +
        //floatAmount *= (float)(50.0f + skillThisAttempt) / 100.0f;

        //Adjust so we always contribute 'something'
        floatAmount = (float)Enums.StaticValues.minimumThroughput > floatAmount ? (float)Enums.StaticValues.minimumThroughput : floatAmount;

        return (int)floatAmount;
    }

    public void ComputeSkillThisAttempt()
    {
        skillThisAttempt = (int)(GetSkillLevel() + Random.Range(-GetVariance(), GetVariance()));
        //Note that this enables characters to go over the 100 skill cap
        if (skillThisAttempt <= 0)
            skillThisAttempt = 1;
    }

    public void GenerateRandomStats(Enums.CharacterRole role, int baseLevel)
    {
        //Find the Class
        int randomValue = 0;
        switch (role)
        {
            case Enums.CharacterRole.Tank:
                randomValue = Random.Range(0, 2);
                if (randomValue == 0)
                    charClass = Enums.CharacterClass.Fighter;
                else
                    charClass = Enums.CharacterClass.Paladin;
                break;
            case Enums.CharacterRole.MeleeDPS:
                randomValue = Random.Range(0, 2);
                if (randomValue == 0)
                    charClass = Enums.CharacterClass.Fighter;
                else
                    charClass = Enums.CharacterClass.Shadow;
                break;
            case Enums.CharacterRole.Healer:

                randomValue = Random.Range(0, 3);
                if (randomValue == 0)
                    charClass = Enums.CharacterClass.Totemic;
                else if (randomValue == 1)
                    charClass = Enums.CharacterClass.Sorcerous;
                else
                    charClass = Enums.CharacterClass.Paladin;

                break;
            case Enums.CharacterRole.RangedDPS:

                randomValue = Random.Range(0, 3);
                if (randomValue == 0)
                    charClass = Enums.CharacterClass.Totemic;
                else if (randomValue == 1)
                    charClass = Enums.CharacterClass.Sorcerous;
                else
                    charClass = Enums.CharacterClass.Shadow;

                break;
            default:
                break;
        }

        //Compute the variation
        variation = 100 - baseLevel;
        for (int i = 0; i < 3; i++)
        {
            variation = Random.Range(1, variation);
        }

        //Compute the gearlevel and skill from the baselevel
        //Two passes - first is skill, second is gearlevel
        for (int i = 0; i < 2; i++)
        {
            int first = (int)Random.Range(baseLevel / 2, baseLevel * 1.5f);
            int second = (int)Random.Range(baseLevel / 2, baseLevel * 1.5f);
            int third = (int)Random.Range(baseLevel / 2, baseLevel * 1.5f);

            randomValue = (int)((first + second + third) / 3);
            if (i == 0)
                skillLevel = randomValue;
            else
                gearLevel = randomValue;
        }

    }

    //Internal Functions

    void SetSpecFromRoleAndClass()
    {
        Enums.CharacterRole role = GetRole();
        switch (GetClass())
        {
            case Enums.CharacterClass.Fighter:
                if (role == Enums.CharacterRole.Tank)
                    charSpec = Enums.CharacterSpec.Guardian;
                else
                    charSpec = Enums.CharacterSpec.Berserker;
                break;

            case Enums.CharacterClass.Shadow:
                if (role == Enums.CharacterRole.RangedDPS)
                    charSpec = Enums.CharacterSpec.Ranger;
                else
                    charSpec = Enums.CharacterSpec.Assassin;
                break;

            case Enums.CharacterClass.Totemic:
                if (role == Enums.CharacterRole.Healer)
                    charSpec = Enums.CharacterSpec.Naturalist;
                else
                    charSpec = Enums.CharacterSpec.Elementalist;
                break;

            case Enums.CharacterClass.Sorcerous:
                if (role == Enums.CharacterRole.Healer)
                    charSpec = Enums.CharacterSpec.Cleric;
                else
                    charSpec = Enums.CharacterSpec.Knight;
                break;

            case Enums.CharacterClass.Paladin:
                break;

            default:
                break;
        }
    }

    void SetAbilityFromSpec()
    {
        //Code to give them correct abilities

        switch (charSpec)
        {
            case Enums.CharacterSpec.Guardian:
                ability = new BaseAbility("InterruptGuardian", "Provides the ability to interrupt", Enums.Ability.Interrupt);
                break;
            case Enums.CharacterSpec.Knight:
                ability = new BaseAbility("KnightStun", "Provides the ability to stun", Enums.Ability.Stun);
                break;
            case Enums.CharacterSpec.Cleric:
                ability = new BaseAbility("ClericDispel", "Provides the ability to dispel", Enums.Ability.Dispel);
                break;
            case Enums.CharacterSpec.WitchDoctor:
                ability = new BaseAbility("WitchDoctorDispel", "Provides the ability to dispel", Enums.Ability.Dispel);
                break;
            case Enums.CharacterSpec.Naturalist:
                ability = new BaseAbility("NaturalistDispel", "Provides the ability to dispel", Enums.Ability.Dispel);
                break;
            case Enums.CharacterSpec.Berserker:
                ability = new BaseAbility("StunBerserker", "Provides the ability to stun", Enums.Ability.Stun);
                break;
            case Enums.CharacterSpec.Assassin:
                ability = new BaseAbility("InterruptAssassin", "Provides the ability to interrupt", Enums.Ability.Interrupt);
                break;
            case Enums.CharacterSpec.Ranger:
                ability = new BaseAbility("RangerSlow", "Provides the ability to slow", Enums.Ability.Slow);
                break;
            case Enums.CharacterSpec.Wizard:
                ability = new BaseAbility("InterruptWizard", "Provides the ability to interrupt", Enums.Ability.Interrupt);
                break;
            case Enums.CharacterSpec.Elementalist:
                ability = new BaseAbility("ElementalistSlow", "Provides the ability to slow", Enums.Ability.Slow);
                break;
            default:
                break;
        }
    }

    void SetBaseAbility()
    {
        switch (charRole)
        {
            case Enums.CharacterRole.Tank:
                break;
            case Enums.CharacterRole.Healer:
                break;
            case Enums.CharacterRole.RangedDPS:
                break;
            case Enums.CharacterRole.MeleeDPS:
                break;
            default:
                break;
        }
        ability = null;
    }

}
