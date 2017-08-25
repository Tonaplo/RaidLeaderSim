using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RaiderStats {

    //base stats
    GearStats m_gear;
    SkillStats m_skill;
    int m_variation = 0;
    int m_averageThroughput = 0;
    Enums.CharacterRole m_charRole;
    Enums.CharacterClass m_charClass;
    Enums.CharacterSpec m_charSpec;
    BaseAbility m_ability;
    BaseCooldown m_cooldown;

    public GearStats Gear { get { return m_gear; } }
    public SkillStats Skills { get { return m_skill; } }
    public BaseAbility Ability { get { return m_ability; } }
    public BaseCooldown Cooldown { get { return m_cooldown; } }

    //per fight stats
    float m_varianceMultiplierThisAttempt = 0.0f;
    int throughput = 0;

    public float GetVarianceMultiplierThisAttempt() { return m_varianceMultiplierThisAttempt; }
    public int GetVariance() { return m_variation; }
    public float GetVarianceMultiplier() { return 1.0f + (UnityEngine.Random.Range(-GetVariance(), GetVariance()))/100.0f; }
    public int GetAverageThroughput() { return m_averageThroughput; }
    public int GetThroughput() { return throughput; }
    public Enums.CharacterRole GetRole() { return m_charRole; }
    public Enums.CharacterClass GetClass() { return m_charClass; }
    public Enums.CharacterSpec GetCurrentSpec() { return m_charSpec; }
    public Enums.CharacterSpec GetOffSpec() { return Utility.GetOtherSpec(GetCurrentSpec()); }
    public Enums.CharacterRole GetOffSpecRole() { return Utility.GetRoleFromSpec(GetOffSpec()); }

    public RaiderStats(int baseLevel)
    {
        int[] starterValues = new int[(int)Enums.SkillTypes.NumSkillTypes];
        for (int i = 0; i < (int)Enums.SkillTypes.NumSkillTypes; i++)
        {
            starterValues[i] = GenerateRandomLevelFromBase(baseLevel);
        }
        m_skill = new SkillStats(starterValues);

        starterValues = new int[(int)Enums.GearTypes.NumGearTypes];
        for (int i = 0; i < (int)Enums.GearTypes.NumGearTypes; i++)
        {
            starterValues[i] = GenerateRandomLevelFromBase(baseLevel);
        }
        m_gear = new GearStats(starterValues);
        
        m_variation = GenerateVariation();
        ComputeAverageThroughput();
    }

    public RaiderStats(int _gearLevel, int _skillLevel, int _variation, Enums.CharacterRole _charRole, Enums.CharacterClass _charClass)
    {
        m_gear = new GearStats(_gearLevel);
        m_skill = new SkillStats(_skillLevel);
        m_variation = _variation;
        m_charRole = _charRole;
        m_charClass = _charClass;
        FinishRaiderStatGeneration();
        ComputeAverageThroughput();
    }

    public void TrainingFinished()
    {
        m_skill.TrainingFinished();
    }

    public void ReCalculateRaiderStats()
    {
        //SetTestValue();
        Gear.CalculateItemlevels();
        Skills.CalculateAverageSkillLevel();
        SetAbilityFromSpec();
        SetCooldownFromSpec();
        ComputeAverageThroughput();
        ComputeSkillThisAttempt();
        ComputeThroughput();
    }
    
    public int ComputeThroughput()
    {
        throughput = Mathf.RoundToInt(ComputeThroughputInternal() * GetVarianceMultiplierThisAttempt());
        return throughput;
    }

    public int ComputeAverageThroughput()
    {
        m_averageThroughput = ComputeThroughputInternal();
        return m_averageThroughput;
    }

    int ComputeThroughputInternal()
    {
        //Cast to float for precision
        float floatAmount = m_gear.TotalItemLevel / 2;
        
        //Multiply with Gearlevel - increases the base by a percentage
        floatAmount *= (0.5f + ((float)m_skill.GetSkillLevel(Enums.SkillTypes.Throughput) / 100.0f));

        //Adjust so we always contribute 'something'
        floatAmount = StaticValues.MinimumThroughput > floatAmount ? StaticValues.MinimumThroughput : floatAmount;

        return (int)floatAmount;
    }

    public void ComputeSkillThisAttempt()
    {
        m_varianceMultiplierThisAttempt = GetVarianceMultiplier();

        ComputeThroughput();
    }
    
    public int GetAttackOrHealAmount() {
        float value = (GetThroughput() * GetVarianceMultiplier());
        return (int)(value > 1.0f ? value : 1);
    }

    public void ChangeSpec()
    {
        Enums.CharacterRole role = GetRole();
        switch (GetClass())
        {
            case Enums.CharacterClass.Fighter:
                if (role == Enums.CharacterRole.Tank)
                    m_charRole = Enums.CharacterRole.MeleeDPS;
                else
                    m_charRole = Enums.CharacterRole.Tank;
                break;

            case Enums.CharacterClass.Shadow:
                if (role == Enums.CharacterRole.RangedDPS)
                    m_charRole = Enums.CharacterRole.MeleeDPS;
                else
                    m_charRole = Enums.CharacterRole.RangedDPS;
                break;

            case Enums.CharacterClass.Totemic:
                if (role == Enums.CharacterRole.Healer)
                    m_charRole = Enums.CharacterRole.RangedDPS;
                else
                    m_charRole = Enums.CharacterRole.Healer;
                break;

            case Enums.CharacterClass.Sorcerer:
                if (role == Enums.CharacterRole.Healer)
                    m_charRole = Enums.CharacterRole.RangedDPS;
                else
                    m_charRole = Enums.CharacterRole.Healer;
                break;

            case Enums.CharacterClass.Paladin:
                if (role == Enums.CharacterRole.Tank)
                    m_charRole = Enums.CharacterRole.Healer;
                else
                    m_charRole = Enums.CharacterRole.Tank;
                break;
            case Enums.CharacterClass.Occultist:
                if (role == Enums.CharacterRole.MeleeDPS)
                    m_charRole = Enums.CharacterRole.RangedDPS;
                else
                    m_charRole = Enums.CharacterRole.MeleeDPS;
                break;

            default:
                break;
        }
        m_charSpec = Utility.GetSpecFromRoleAndClass(m_charClass, m_charRole);
    }

    public void GetBaseAttackScript(out BaseHealOrAttackScript script)
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
        script.Setup();
    }

    public void GetBaseHealingScript(out BaseHealScript script)
    {
        switch (GetCurrentSpec())
        {

            case Enums.CharacterSpec.Cleric:
                script = new ClericHealScript();
                break;
            case Enums.CharacterSpec.Diviner:
                script = new DivinerHealScript();
                break;
            case Enums.CharacterSpec.Naturalist:
                script = new NaturalistHealScript();
                break;
            case Enums.CharacterSpec.Berserker:
            case Enums.CharacterSpec.Guardian:
            case Enums.CharacterSpec.Knight:
            case Enums.CharacterSpec.Assassin:
            case Enums.CharacterSpec.Scourge:
            case Enums.CharacterSpec.Ranger:
            case Enums.CharacterSpec.Wizard:
            case Enums.CharacterSpec.Elementalist:
            case Enums.CharacterSpec.Necromancer:
            default:
                Debug.LogAssertion("Tried to find heal script for non-healer");
                script = null;
                return;
        }

        script.Setup();
    }

    public static RaiderStats GenerateRaiderStatsFromClass(Enums.CharacterClass Class, int baseLevel)
    {
        RaiderStats returnValue = new RaiderStats(baseLevel);

        returnValue.m_charRole = Utility.GenerateRoleFromClass(Class);
        returnValue.m_charClass = Class;

        FinishRaiderStatGeneration(ref returnValue);
        return returnValue;
    }

    public static RaiderStats GenerateRaiderStatsFromRole(Enums.CharacterRole role, int baseLevel)
    {
        RaiderStats returnValue = new RaiderStats(baseLevel);
        returnValue.m_charClass = Utility.GenerateClassFromRole(role);
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
    
    //Internal Functions
    
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
        rs.m_charSpec = Utility.GetSpecFromRoleAndClass(rs.m_charClass, rs.m_charRole);
        rs.Gear.CalculateItemlevels();
        rs.Skills.CalculateAverageSkillLevel();
        rs.SetAbilityFromSpec();
        rs.SetCooldownFromSpec();
        rs.ComputeAverageThroughput();
        rs.ComputeSkillThisAttempt();
        rs.ComputeThroughput();
    }
    
    void FinishRaiderStatGeneration()
    {
        m_charSpec = Utility.GetSpecFromRoleAndClass(m_charClass, m_charRole);
        Gear.CalculateItemlevels();
        Skills.CalculateAverageSkillLevel();
        SetAbilityFromSpec();
        SetCooldownFromSpec();
        ComputeAverageThroughput();
        ComputeSkillThisAttempt();
        ComputeThroughput();
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
                m_ability = new BaseAbility("RangerImmune", "Provides the m_ability to slow", Enums.Ability.Immune);
                break;
            case Enums.CharacterSpec.Wizard:
                m_ability = new BaseAbility("InterruptWizard", "Provides the m_ability to interrupt", Enums.Ability.Interrupt);
                break;
            case Enums.CharacterSpec.Elementalist:
                m_ability = new BaseAbility("ElementalistImmune", "Provides the m_ability to slow", Enums.Ability.Immune);
                break;
            case Enums.CharacterSpec.Necromancer:
                m_ability = new BaseAbility("NecromancerImmune", "Provides the m_ability to slow", Enums.Ability.Immune);
                break;
            case Enums.CharacterSpec.Scourge:
                m_ability = new BaseAbility("ScourgeStun", "Provides the m_ability to stun", Enums.Ability.Stun);
                break;
            default:
                Debug.LogAssertion("Spec Not Found!");
                break;
        }
    }

    void SetCooldownFromSpec()
    {
        BaseCooldown.CooldownEffects effects = new BaseCooldown.CooldownEffects();
        //Code to give them correct Cooldowns
        switch (m_charSpec)
        {
            case Enums.CharacterSpec.Guardian:
                effects.m_damageReductionMultiplier = 0.3f;
                effects.m_targets = Enums.CooldownTargets.Raid;
                m_cooldown = new BaseCooldown("Protection", "The Guardian and the rest of the raid takes <dr> less damage for the next <dur>.", Enums.Cooldowns.TankCooldown, effects, 15.0f);
                break;
            case Enums.CharacterSpec.Knight:
                effects.m_leechMultiplier = 0.1f;
                effects.m_targets = Enums.CooldownTargets.Raid;
                m_cooldown = new BaseCooldown("Light Infusion", "The Knight and the rest of the raid now heals for <leech> of damage dealt for the next <dur>.", Enums.Cooldowns.TankCooldown, effects, 15.0f);
                break;
            case Enums.CharacterSpec.Cleric:
                effects.m_deepHealingMultiplier = 1.5f;
                m_cooldown = new BaseCooldown("Deep Healing", "Targets are healed up to an additional <deep> more for <dur>.", Enums.Cooldowns.HealingCooldown, effects, 15.0f);
                break;
            case Enums.CharacterSpec.Diviner:
                effects.m_castTimeMultiplier = 0.6f;
                m_cooldown = new BaseCooldown("Quickening", "Reduces cast time of Arcane Mending by <ct> for <dur>.", Enums.Cooldowns.HealingCooldown, effects, 15.0f);
                break;
            case Enums.CharacterSpec.Naturalist:
                effects.m_HoTMultiplier = 0.9f;
                m_cooldown = new BaseCooldown("Lingering Touch", "The next <dur>, healed targets are healed again for an additional <hot> of the initial heal", Enums.Cooldowns.HealingCooldown, effects, 15.0f);
                break;
            case Enums.CharacterSpec.Berserker:
                effects.m_leechMultiplier = 0.05f;
                effects.m_damageMultiplier = 1.2f;
                m_cooldown = new BaseCooldown("Berserk", "The Berserker heals for <leech> of damage dealt and increases damage by <damage> for <dur>.", Enums.Cooldowns.DPSCooldown, effects, 15.0f);
                break;
            case Enums.CharacterSpec.Assassin:
                effects.m_damageMultiplier = 1.50f;
                m_cooldown = new BaseCooldown("Deadly Poison", "Increases damage dealt by <damage> for <dur>.", Enums.Cooldowns.DPSCooldown, effects, 8.0f);
                break;
            case Enums.CharacterSpec.Ranger:
                effects.m_castTimeMultiplier = 0.75f;
                m_cooldown = new BaseCooldown("Rapid Fire", "Reduces cast time by <ct> for <dur>.", Enums.Cooldowns.DPSCooldown, effects, 15.0f);
                break;
            case Enums.CharacterSpec.Wizard:
                effects.m_damageMultiplier = 1.20f;
                effects.m_castTimeMultiplier = 0.80f;
                m_cooldown = new BaseCooldown("Arcane Power", "Increases damage dealt by <damage> and reduces cast time by <ct> for <dur>.", Enums.Cooldowns.DPSCooldown, effects, 15.0f);
                break;
            case Enums.CharacterSpec.Elementalist:
                effects.m_critChanceIncrease = 35;
                effects.m_critEffectIncrease = 0.50f;
                m_cooldown = new BaseCooldown("Arcane Power", "Increases crit chance by <critchance> and crit effect by <criteffect> for <dur>.", Enums.Cooldowns.DPSCooldown, effects, 10.0f);
                break;
            case Enums.CharacterSpec.Necromancer:
                effects.m_castTimeMultiplier = 0.9f;
                effects.m_targets = Enums.CooldownTargets.Raid;
                m_cooldown = new BaseCooldown("Unholy Power", "Provides <ct> reduced cast time for the whole raid for <dur>.", Enums.Cooldowns.DPSCooldown, effects, 15.0f); 
                break;
            case Enums.CharacterSpec.Scourge:
                effects.m_damageMultiplier = 1.1f;
                effects.m_targets = Enums.CooldownTargets.Raid;
                m_cooldown = new BaseCooldown("Bloodthirst", "Provides <damage> increased damage for the whole raid for <dur>.", Enums.Cooldowns.DPSCooldown, effects, 15.0f);
                break;
            default:
                Debug.LogAssertion("Spec Not Found!");
                break;
        }
    }

    //DEBUG
    public void SetTestValue()
    {
        m_gear = new GearStats(10);
        m_skill = new SkillStats(10);
        m_variation = 0;

        FinishRaiderStatGeneration();
    }
}