using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RaiderStats {

    //base stats
    int gearLevel = 0;
    int skillLevel = 0;
    int variation = 0;
    int averageThroughput = 0;
    Enums.CharacterRole charRole;
    Enums.CharacterClass charClass;
    Enums.CharacterSpec charSpec;
    BaseAbility ability;
    BaseCooldown cooldown;

    //per fight stats
    int skillThisAttempt = 0;
    int throughput = 0;

    public int GetGearLevel() { return gearLevel; }
    public int GetSkillLevel() { return skillLevel; }
    public int GetSkillThisAttempt() { return skillThisAttempt; }
    public int GetVariance() { return variation; }
    public float GetVarianceMultiplier() { return 1.0f + (UnityEngine.Random.Range(-GetVariance(), GetVariance()))/100.0f; }
    public int GetAverageThroughput() { return averageThroughput; }
    public int GetThroughput() { return throughput; }
    public Enums.CharacterRole GetRole() { return charRole; }
    public Enums.CharacterClass GetClass() { return charClass; }
    public Enums.CharacterSpec GetCurrentSpec() { return charSpec; }
    public Enums.CharacterSpec GetOffSpec() { return Utility.GetOtherSpec(GetCurrentSpec()); }
    public Enums.CharacterRole GetOffSpecRole() { return Utility.GetRoleFromSpec(GetOffSpec()); }
    public BaseAbility GetAbility() { return ability; }
    public BaseCooldown GetCooldown() { return cooldown; }

    RaiderStats(int baseLevel)
    {
        //Generate skill and gear level
        skillLevel = GenerateRandomLevelFromBase(baseLevel);
        gearLevel = GenerateRandomLevelFromBase(baseLevel);
        
        variation = GenerateVariation();
        ComputeAverageThroughput();
    }

    public RaiderStats(int _gearLevel, int _skillLevel, int _variation, Enums.CharacterRole _charRole, Enums.CharacterClass _charClass)
    {
        gearLevel = _gearLevel;
        skillLevel = _skillLevel;
        variation = _variation;
        charRole = _charRole;
        charClass = _charClass;
        FinishRaiderStatGeneration();
        ComputeAverageThroughput();
    }

    public void ModifySkillLevel(int amount)
    {
        skillLevel += amount;

        //Clamp the skill level between 1 and 100
        if (skillLevel > (int)Enums.StaticValues.maxSkill)
            skillLevel = (int)Enums.StaticValues.maxSkill;
        else if (skillLevel < 1)
            skillLevel = 1;

        ComputeAverageThroughput();
    }

    public void ModifyGearLevel(int amount)
    {
        gearLevel += amount;

        //GearLevel can never be below 0
        gearLevel = gearLevel < 0 ? 0 : gearLevel;

        ComputeAverageThroughput();
    }

    public int ComputeThroughput()
    {
        throughput = ComputeThroughputInternal(GetSkillThisAttempt());
        return throughput;
    }

    public int ComputeAverageThroughput()
    {
        averageThroughput = ComputeThroughputInternal(GetSkillLevel());
        return averageThroughput;
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

    public Enums.CharacterAttack GetBaseAttack()
    {
        switch (GetRole())
        {
            case Enums.CharacterRole.Tank:
                return Enums.CharacterAttack.TankStrike;
            case Enums.CharacterRole.Healer:
                return Enums.CharacterAttack.HealerSmite;
            case Enums.CharacterRole.RangedDPS:
                return Enums.CharacterAttack.RangedFireball;
            case Enums.CharacterRole.MeleeDPS:
                return Enums.CharacterAttack.MeleeStab;
            default:
                return Enums.CharacterAttack.TankStrike;
        }
    }

    public int GetSpellAmount(float multiplier) {
        float value = multiplier * (GetThroughput() * GetVarianceMultiplier());
        return (int)(value > 1.0f ? value : 1);
    }

    public static RaiderStats GenerateRaiderStatsFromClass(Enums.CharacterClass Class, int baseLevel)
    {
        RaiderStats returnValue = new RaiderStats(baseLevel);

        returnValue.charRole = GenerateRoleFromClass(Class);
        returnValue.charClass = Class;

        FinishRaiderStatGeneration(ref returnValue);
        return returnValue;
    }

    public static RaiderStats GenerateRaiderStatsFromRole(Enums.CharacterRole role, int baseLevel)
    {
        RaiderStats returnValue = new RaiderStats(baseLevel);
        returnValue.charClass = GenerateClassFromRole(role);
        returnValue.charRole = role;

        FinishRaiderStatGeneration(ref returnValue);
        
        return returnValue;
    }

    public static RaiderStats GenerateRaiderStatsFromSpec(Enums.CharacterSpec spec, int baseLevel)
    {
        RaiderStats returnValue = new RaiderStats(baseLevel);

        returnValue.charRole = Utility.GetRoleFromSpec(spec);
        returnValue.charClass = Utility.GetClassFromSpec(spec);

        FinishRaiderStatGeneration(ref returnValue);
        return returnValue;
    }

    public IEnumerator DoAttack(float castTime, int attackDamage, int index, Raider attacker, Enums.CharacterAttack attack, RaidSceneController rsc, RaiderScript rs)
    {
        yield return new WaitForSeconds(castTime);
        if (!rsc.IsBossDead() && !rs.IsDead())
        {
            rsc.DealDamage(attackDamage, attacker.GetName(), Utility.GetAttackName(GetBaseAttack()), index);
            float baseCastTime = Utility.GetAttackBaseValue(attack, Enums.AttackValueTypes.CastTime);
            int newDamage = GetSpellAmount(Utility.GetAttackBaseValue(GetBaseAttack(), Enums.AttackValueTypes.BaseDamageMultiplier));
            rs.StartCoroutine(DoAttack(baseCastTime + UnityEngine.Random.Range(0, baseCastTime / 10.0f), newDamage, index, attacker, attack, rsc, rs));
        }
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
    }

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

            case Enums.CharacterClass.Sorcerer:
                if (role == Enums.CharacterRole.Healer)
                    charSpec = Enums.CharacterSpec.Diviner;
                else
                    charSpec = Enums.CharacterSpec.Wizard;
                break;

            case Enums.CharacterClass.Paladin:
                if (role == Enums.CharacterRole.Healer)
                    charSpec = Enums.CharacterSpec.Cleric;
                else
                    charSpec = Enums.CharacterSpec.Knight;
                break;

            case Enums.CharacterClass.Occultist:
                if (role == Enums.CharacterRole.MeleeDPS)
                    charSpec = Enums.CharacterSpec.Scourge;
                else
                    charSpec = Enums.CharacterSpec.Necromancer;
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
            case Enums.CharacterSpec.Diviner:
                ability = new BaseAbility("DivinerDispel", "Provides the ability to dispel", Enums.Ability.Dispel);
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

    void SetCooldownFromSpec()
    {
        //Code to give them correct abilities
        cooldown = new BaseCooldown();
        switch (charSpec)
        {
            case Enums.CharacterSpec.Guardian:
                cooldown.Initialize("Immunity", "Provides the ability to immune", Enums.Cooldowns.Immunity);
                break;
            case Enums.CharacterSpec.Knight:
                cooldown.Initialize("TankCD", "Provides the ability to immune", Enums.Cooldowns.Immunity);
                break;
            case Enums.CharacterSpec.Cleric:
                cooldown.Initialize("HealingCooldown", "Provides the ability to immune", Enums.Cooldowns.HealingCooldown);
                break;
            case Enums.CharacterSpec.Diviner:
                cooldown.Initialize("HealingCooldown", "Provides the ability to immune", Enums.Cooldowns.Immunity);
                break;
            case Enums.CharacterSpec.Naturalist:
                cooldown.Initialize("TankCooldown", "Provides the ability to immune", Enums.Cooldowns.TankCooldown);
                break;
            case Enums.CharacterSpec.Berserker:
                cooldown.Initialize("AoECooldown", "Provides the ability to immune", Enums.Cooldowns.AoECooldown);
                break;
            case Enums.CharacterSpec.Assassin:
                cooldown.Initialize("SingleTargetCooldown", "Provides the ability to immune", Enums.Cooldowns.SingleTargetCooldown);
                break;
            case Enums.CharacterSpec.Ranger:
                cooldown.Initialize("AoECooldown", "Provides the ability to immune", Enums.Cooldowns.AoECooldown);
                break;
            case Enums.CharacterSpec.Wizard:
                cooldown.Initialize("SingleTargetCooldown", "Provides the ability to immune", Enums.Cooldowns.SingleTargetCooldown);
                break;
            case Enums.CharacterSpec.Elementalist:
                cooldown.Initialize("Immunity", "Provides the ability to immune", Enums.Cooldowns.Immunity);
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