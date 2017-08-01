using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RaiderStats {

    //base stats
    int m_gearLevel = 0;
    int m_skillLevel = 0;
    int m_variation = 0;
    int m_averageThroughput = 0;
    Enums.CharacterRole m_charRole;
    Enums.CharacterClass m_charClass;
    Enums.CharacterSpec m_charSpec;
    BaseAbility m_ability;
    BaseCooldown m_cooldown;

    //per fight stats
    int skillThisAttempt = 0;
    int throughput = 0;

    public int GetGearLevel() { return m_gearLevel; }
    public int GetSkillLevel() { return m_skillLevel; }
    public int GetSkillThisAttempt() { return skillThisAttempt; }
    public int GetVariance() { return m_variation; }
    public float GetVarianceMultiplier() { return 1.0f + (UnityEngine.Random.Range(-GetVariance(), GetVariance()))/100.0f; }
    public int GetAverageThroughput() { return m_averageThroughput; }
    public int GetThroughput() { return throughput; }
    public Enums.CharacterRole GetRole() { return m_charRole; }
    public Enums.CharacterClass GetClass() { return m_charClass; }
    public Enums.CharacterSpec GetCurrentSpec() { return m_charSpec; }
    public Enums.CharacterSpec GetOffSpec() { return Utility.GetOtherSpec(GetCurrentSpec()); }
    public Enums.CharacterRole GetOffSpecRole() { return Utility.GetRoleFromSpec(GetOffSpec()); }
    public BaseAbility GetAbility() { return m_ability; }
    public BaseCooldown GetCooldown() { return m_cooldown; }

    RaiderStats(int baseLevel)
    {
        //Generate skill and gear level
        m_skillLevel = GenerateRandomLevelFromBase(baseLevel);
        m_gearLevel = GenerateRandomLevelFromBase(baseLevel);
        
        m_variation = GenerateVariation();
        ComputeAverageThroughput();
    }

    public RaiderStats(int _gearLevel, int _skillLevel, int _variation, Enums.CharacterRole _charRole, Enums.CharacterClass _charClass)
    {
        m_gearLevel = _gearLevel;
        m_skillLevel = _skillLevel;
        m_variation = _variation;
        m_charRole = _charRole;
        m_charClass = _charClass;
        FinishRaiderStatGeneration();
        ComputeAverageThroughput();
    }

    public void ModifySkillLevel(int amount)
    {
        m_skillLevel += amount;

        //Clamp the skill level between 1 and 100
        if (m_skillLevel > (int)Enums.StaticValues.maxSkill)
            m_skillLevel = (int)Enums.StaticValues.maxSkill;
        else if (m_skillLevel < 1)
            m_skillLevel = 1;

        ComputeAverageThroughput();
    }

    public void ModifyGearLevel(int amount)
    {
        m_gearLevel += amount;

        //GearLevel can never be below 0
        m_gearLevel = m_gearLevel < 0 ? 0 : m_gearLevel;

        ComputeAverageThroughput();
    }

    public int ComputeThroughput()
    {
        throughput = ComputeThroughputInternal(GetSkillThisAttempt());
        return throughput;
    }

    public int ComputeAverageThroughput()
    {
        m_averageThroughput = ComputeThroughputInternal(GetSkillLevel());
        return m_averageThroughput;
    }

    int ComputeThroughputInternal(int basevalue)
    {
        //Cast to float for precision
        float floatAmount = basevalue;

        //Multiply with Gearlevel - increases the base by a percentage
        floatAmount *= (1.0f + ((float)GetGearLevel() / 30.0f));

        //Adjust so we always contribute 'something'
        floatAmount = (float)Enums.StaticValues.minimumThroughput > floatAmount ? (float)Enums.StaticValues.minimumThroughput : floatAmount;

        return (int)floatAmount;
    }

    public void ComputeSkillThisAttempt()
    {
        skillThisAttempt = (int)(GetSkillLevel() * GetVarianceMultiplier());
        //Note that this enables characters to go over the 100 skill cap
        if (skillThisAttempt <= 0)
            skillThisAttempt = 1;

        ComputeThroughput();
    }
    
    public int GetSpellAmount(float multiplier) {
        float value = multiplier * (GetThroughput() * GetVarianceMultiplier());
        return (int)(value > 1.0f ? value : 1);
    }

    public static RaiderStats GenerateRaiderStatsFromClass(Enums.CharacterClass Class, int baseLevel)
    {
        RaiderStats returnValue = new RaiderStats(baseLevel);

        returnValue.m_charRole = GenerateRoleFromClass(Class);
        returnValue.m_charClass = Class;

        FinishRaiderStatGeneration(ref returnValue);
        return returnValue;
    }

    public static RaiderStats GenerateRaiderStatsFromRole(Enums.CharacterRole role, int baseLevel)
    {
        RaiderStats returnValue = new RaiderStats(baseLevel);
        returnValue.m_charClass = GenerateClassFromRole(role);
        returnValue.m_charRole = role;

        FinishRaiderStatGeneration(ref returnValue);
        
        return returnValue;
    }

    public static RaiderStats GenerateRaiderStatsFromSpec(Enums.CharacterSpec spec, int baseLevel)
    {
        RaiderStats returnValue = new RaiderStats(baseLevel);

        returnValue.m_charRole = Utility.GetRoleFromSpec(spec);
        returnValue.m_charClass = Utility.GetClassFromSpec(spec);

        FinishRaiderStatGeneration(ref returnValue);
        return returnValue;
    }

    public void GetBaseAttackScript(out BaseAttackScript script)
    {
        switch (GetCurrentSpec())
        {
            case Enums.CharacterSpec.Guardian:
                script = new GuardianAttack();
                break;
            case Enums.CharacterSpec.Knight:
                script = new KnightAttack();
                break;
            case Enums.CharacterSpec.Cleric:
                script = new ClericAttack();
                break;
            case Enums.CharacterSpec.Diviner:
                script = new DivinerAttack();
                break;
            case Enums.CharacterSpec.Naturalist:
                script = new NaturalistAttack();
                break;
            case Enums.CharacterSpec.Berserker:
                script = new BerserkerAttack();
                break;
            case Enums.CharacterSpec.Assassin:
                script = new AssasinAttack();
                break;
            case Enums.CharacterSpec.Scourge:
                script = new ScourgeAttack();
                break;
            case Enums.CharacterSpec.Ranger:
                script = new RangerAttack();
                break;
            case Enums.CharacterSpec.Wizard:
                script = new WizardAttack();
                break;
            case Enums.CharacterSpec.Elementalist:
                script = new ElementalistAttack();
                break;
            case Enums.CharacterSpec.Necromancer:
            default:
                script = new NecromancerAttack();
                break;
        }
        script.SetupAttack();
    }

    public IEnumerator DoHeal(float castTime, RaiderScript caster, int index, RaidSceneController rsc, List<RaiderScript> raid)
    {
        yield return new WaitForSeconds(castTime);
        
        if (!rsc.IsBossDead() && !caster.IsDead())
        {
            int type = UnityEngine.Random.Range(0, 2);
            switch (type)
            {
                case 0:
                    {
                        RaiderScript lowest = null;
                        int lowestDiff = 0;
                        for (int i = 0; i < raid.Count; i++)
                        {
                           int thisDiff = raid[i].GetMaxHealth() - raid[i].GetHealth();
                            if (thisDiff > lowestDiff) {
                                lowest = raid[i];
                                lowestDiff = raid[i].GetMaxHealth() - raid[i].GetHealth();
                            }
                        }

                        if (lowest)
                        {
                            int healamount = GetSpellAmount(1.0f);
                            lowest.TakeHealing(healamount);
                            rsc.DoHeal(healamount, caster.Raider.GetName(), "Big Heal", index);
                        }
                    }
                    break;
                case 1:
                    {
                        int healAmount = GetSpellAmount(1.0f) / 3;
                        int randomIndex = 0;
                        for (int i = 0; i < 3; i++)
                        {
                            randomIndex = UnityEngine.Random.Range(0, raid.Count - 1);
                            if (!raid[randomIndex].IsDead())
                            {
                                raid[randomIndex].TakeHealing(healAmount);
                                rsc.DoHeal(healAmount, caster.Raider.GetName(), "Medium Heal", index);
                            }
                        }
                    }
                    break;
                case 2:
                    {
                        int healAmount = GetSpellAmount(1.0f) / (raid.Count/2);
                        int randomIndex = 0;
                        for (int i = 0; i < raid.Count; i++)
                        {
                            randomIndex = UnityEngine.Random.Range(0, raid.Count - 1);
                            if (!raid[randomIndex].IsDead())
                            {
                                raid[randomIndex].TakeHealing(healAmount);
                                rsc.DoHeal(healAmount, caster.Raider.GetName(), "Small Heal", index);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            float baseCastTime = 1.5f;
            caster.StartCoroutine(DoHeal(baseCastTime + UnityEngine.Random.Range(0, baseCastTime / 10.0f), caster, index, rsc, raid));
        }
    }

    //Internal Functions

    static Enums.CharacterClass GenerateClassFromRole(Enums.CharacterRole role)
    {
        int randomValue = 0;
        switch (role)
        {
            case Enums.CharacterRole.Tank:
                randomValue = UnityEngine.Random.Range(0, 2);
                if (randomValue == 0)
                    return Enums.CharacterClass.Fighter;
                else
                    return Enums.CharacterClass.Paladin;
            case Enums.CharacterRole.MeleeDPS:
                randomValue = UnityEngine.Random.Range(0, 3);
                if (randomValue == 0)
                    return Enums.CharacterClass.Fighter;
                else if(randomValue == 1)
                    return Enums.CharacterClass.Shadow;
                else
                    return Enums.CharacterClass.Occultist;
            case Enums.CharacterRole.Healer:

                randomValue = UnityEngine.Random.Range(0, 3);
                if (randomValue == 0)
                    return Enums.CharacterClass.Totemic;
                else if (randomValue == 1)
                    return Enums.CharacterClass.Sorcerer;
                else
                    return Enums.CharacterClass.Paladin;

            default:
            case Enums.CharacterRole.RangedDPS:

                randomValue = UnityEngine.Random.Range(0, 4);
                if (randomValue == 0)
                    return Enums.CharacterClass.Totemic;
                else if (randomValue == 1)
                    return Enums.CharacterClass.Sorcerer;
                else if(randomValue == 2)
                    return Enums.CharacterClass.Shadow;
                else
                    return Enums.CharacterClass.Occultist;
        }
    }

    static Enums.CharacterRole GenerateRoleFromClass(Enums.CharacterClass Class)
    {
        //implement this correctly later
        int randomValue = UnityEngine.Random.Range(0, 2);
        switch (Class)
        {
            case Enums.CharacterClass.Fighter:
                if (randomValue == 0)
                    return Enums.CharacterRole.Tank;
                else
                    return Enums.CharacterRole.MeleeDPS;

            case Enums.CharacterClass.Shadow:
                if (randomValue == 0)
                    return Enums.CharacterRole.RangedDPS;
                else
                    return Enums.CharacterRole.MeleeDPS;
            case Enums.CharacterClass.Totemic:
                if (randomValue == 0)
                    return Enums.CharacterRole.Healer;
                else
                    return Enums.CharacterRole.RangedDPS;
            case Enums.CharacterClass.Sorcerer:
                if (randomValue == 0)
                    return Enums.CharacterRole.Healer;
                else
                    return Enums.CharacterRole.RangedDPS;
            case Enums.CharacterClass.Paladin:
                if (randomValue == 0)
                    return Enums.CharacterRole.Healer;
                else
                    return Enums.CharacterRole.Tank;
            default:
            case Enums.CharacterClass.Occultist:
                if (randomValue == 0)
                    return Enums.CharacterRole.RangedDPS;
                else
                    return Enums.CharacterRole.MeleeDPS;
        }
    }
    
    static int GenerateRandomLevelFromBase(int baseValue)
    {
        int first = (int)UnityEngine.Random.Range(baseValue / 2, baseValue * 1.5f);
        int second = (int)UnityEngine.Random.Range(baseValue / 2, baseValue * 1.5f);
        int third = (int)UnityEngine.Random.Range(baseValue / 2, baseValue * 1.5f);

        return (int)((first + second + third) / 3);
    }

    static int GenerateVariation()
    {
        //We want to vary a maximum of 20% up AND down
        int variation = 0;
        int iterations = 5;
        for (int i = 0; i < iterations; i++)
        {
            variation += UnityEngine.Random.Range(5, 20);
        }

        variation /= iterations;
        return variation;
    }

    static void FinishRaiderStatGeneration(ref RaiderStats rs)
    {
        rs.SetSpecFromRoleAndClass();
        rs.SetAbilityFromSpec();
        rs.SetCooldownFromSpec();
        rs.SetBaseAbility();
        rs.ComputeAverageThroughput();
        rs.ComputeThroughput();
    }

    void ChangeSpec()
    {
        Enums.CharacterRole role = GetRole();
        switch (GetClass())
        {
            case Enums.CharacterClass.Fighter:
                if (role == Enums.CharacterRole.Tank)
                    role = Enums.CharacterRole.MeleeDPS;
                else
                    role = Enums.CharacterRole.Tank;
                break;

            case Enums.CharacterClass.Shadow:
                if (role == Enums.CharacterRole.RangedDPS)
                    role = Enums.CharacterRole.MeleeDPS;
                else
                    role = Enums.CharacterRole.RangedDPS;
                break;

            case Enums.CharacterClass.Totemic:
                if (role == Enums.CharacterRole.Healer)
                    role = Enums.CharacterRole.RangedDPS;
                else
                    role = Enums.CharacterRole.Healer;
                break;

            case Enums.CharacterClass.Sorcerer:
                if (role == Enums.CharacterRole.Healer)
                    role = Enums.CharacterRole.RangedDPS;
                else
                    role = Enums.CharacterRole.Healer;
                break;

            case Enums.CharacterClass.Paladin:
                if (role == Enums.CharacterRole.Tank)
                    role = Enums.CharacterRole.Healer;
                else
                    role = Enums.CharacterRole.Tank;
                break;
            case Enums.CharacterClass.Occultist:
                if (role == Enums.CharacterRole.MeleeDPS)
                    role = Enums.CharacterRole.RangedDPS;
                else
                    role = Enums.CharacterRole.MeleeDPS;
                break;

            default:
                break;
        }
        SetSpecFromRoleAndClass();
    }

    void FinishRaiderStatGeneration()
    {
        SetSpecFromRoleAndClass();
        SetAbilityFromSpec();
        SetCooldownFromSpec();
        SetBaseAbility();
        ComputeAverageThroughput();
        ComputeThroughput();
    }

    void SetSpecFromRoleAndClass()
    {
        Enums.CharacterRole role = GetRole();
        switch (GetClass())
        {
            case Enums.CharacterClass.Fighter:
                if (role == Enums.CharacterRole.Tank)
                    m_charSpec = Enums.CharacterSpec.Guardian;
                else
                    m_charSpec = Enums.CharacterSpec.Berserker;
                break;

            case Enums.CharacterClass.Shadow:
                if (role == Enums.CharacterRole.RangedDPS)
                    m_charSpec = Enums.CharacterSpec.Ranger;
                else
                    m_charSpec = Enums.CharacterSpec.Assassin;
                break;

            case Enums.CharacterClass.Totemic:
                if (role == Enums.CharacterRole.Healer)
                    m_charSpec = Enums.CharacterSpec.Naturalist;
                else
                    m_charSpec = Enums.CharacterSpec.Elementalist;
                break;

            case Enums.CharacterClass.Sorcerer:
                if (role == Enums.CharacterRole.Healer)
                    m_charSpec = Enums.CharacterSpec.Diviner;
                else
                    m_charSpec = Enums.CharacterSpec.Wizard;
                break;

            case Enums.CharacterClass.Paladin:
                if (role == Enums.CharacterRole.Healer)
                    m_charSpec = Enums.CharacterSpec.Cleric;
                else
                    m_charSpec = Enums.CharacterSpec.Knight;
                break;

            case Enums.CharacterClass.Occultist:
                if (role == Enums.CharacterRole.MeleeDPS)
                    m_charSpec = Enums.CharacterSpec.Scourge;
                else
                    m_charSpec = Enums.CharacterSpec.Necromancer;
                break;

            default:
                break;
        }
    }

    void SetAbilityFromSpec()
    {
        //Code to give them correct abilities

        switch (m_charSpec)
        {
            case Enums.CharacterSpec.Guardian:
                m_ability = new BaseAbility("InterruptGuardian", "Provides the m_ability to interrupt", Enums.Ability.Interrupt);
                break;
            case Enums.CharacterSpec.Knight:
                m_ability = new BaseAbility("KnightStun", "Provides the m_ability to stun", Enums.Ability.Stun);
                break;
            case Enums.CharacterSpec.Cleric:
                m_ability = new BaseAbility("ClericDispel", "Provides the m_ability to dispel", Enums.Ability.Dispel);
                break;
            case Enums.CharacterSpec.Diviner:
                m_ability = new BaseAbility("DivinerDispel", "Provides the m_ability to dispel", Enums.Ability.Dispel);
                break;
            case Enums.CharacterSpec.Naturalist:
                m_ability = new BaseAbility("NaturalistDispel", "Provides the m_ability to dispel", Enums.Ability.Dispel);
                break;
            case Enums.CharacterSpec.Berserker:
                m_ability = new BaseAbility("StunBerserker", "Provides the m_ability to stun", Enums.Ability.Stun);
                break;
            case Enums.CharacterSpec.Assassin:
                m_ability = new BaseAbility("InterruptAssassin", "Provides the m_ability to interrupt", Enums.Ability.Interrupt);
                break;
            case Enums.CharacterSpec.Ranger:
                m_ability = new BaseAbility("RangerSlow", "Provides the m_ability to slow", Enums.Ability.Slow);
                break;
            case Enums.CharacterSpec.Wizard:
                m_ability = new BaseAbility("InterruptWizard", "Provides the m_ability to interrupt", Enums.Ability.Interrupt);
                break;
            case Enums.CharacterSpec.Elementalist:
                m_ability = new BaseAbility("ElementalistSlow", "Provides the m_ability to slow", Enums.Ability.Slow);
                break;
            default:
                break;
        }
    }

    void SetCooldownFromSpec()
    {
        //Code to give them correct abilities
        m_cooldown = new BaseCooldown();
        switch (m_charSpec)
        {
            case Enums.CharacterSpec.Guardian:
                m_cooldown.Initialize("Immunity", "Provides the m_ability to immune", Enums.Cooldowns.Immunity);
                break;
            case Enums.CharacterSpec.Knight:
                m_cooldown.Initialize("TankCD", "Provides the m_ability to immune", Enums.Cooldowns.Immunity);
                break;
            case Enums.CharacterSpec.Cleric:
                m_cooldown.Initialize("HealingCooldown", "Provides the m_ability to immune", Enums.Cooldowns.HealingCooldown);
                break;
            case Enums.CharacterSpec.Diviner:
                m_cooldown.Initialize("HealingCooldown", "Provides the m_ability to immune", Enums.Cooldowns.Immunity);
                break;
            case Enums.CharacterSpec.Naturalist:
                m_cooldown.Initialize("TankCooldown", "Provides the m_ability to immune", Enums.Cooldowns.TankCooldown);
                break;
            case Enums.CharacterSpec.Berserker:
                m_cooldown.Initialize("AoECooldown", "Provides the m_ability to immune", Enums.Cooldowns.AoECooldown);
                break;
            case Enums.CharacterSpec.Assassin:
                m_cooldown.Initialize("SingleTargetCooldown", "Provides the m_ability to immune", Enums.Cooldowns.SingleTargetCooldown);
                break;
            case Enums.CharacterSpec.Ranger:
                m_cooldown.Initialize("AoECooldown", "Provides the m_ability to immune", Enums.Cooldowns.AoECooldown);
                break;
            case Enums.CharacterSpec.Wizard:
                m_cooldown.Initialize("SingleTargetCooldown", "Provides the m_ability to immune", Enums.Cooldowns.SingleTargetCooldown);
                break;
            case Enums.CharacterSpec.Elementalist:
                m_cooldown.Initialize("Immunity", "Provides the m_ability to immune", Enums.Cooldowns.Immunity);
                break;
            default:
                break;
        }
    }

    void SetBaseAbility()
    {
        switch (m_charRole)
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
        m_ability = null;
    }

    //DEBUG
    public void SetTestValue()
    {
        m_gearLevel = 10;
        m_skillLevel = 10;
        m_variation = 0;

        FinishRaiderStatGeneration();
    }
}