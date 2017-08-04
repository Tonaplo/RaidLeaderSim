using UnityEngine;
using System.Collections;

public static class Enums{

    public enum StaticValues { 
        minimumThroughput       = 5,
        maxSkill                = 100,
        easyMultiplier          = 75,
        hardMultiplier          = 125,
        interruptIncrease       = 30,
        stunIncrease            = 25,
        healingSTCCIncrease     = 20,
        healingAoECDIncrease    = 80,
        damageSTCCIncrease      = 30,
        damageAoECDIncrease     = 40,
        immunityIncrease        = 100,
        baseRaiderHealth        = 100,
    }

    public enum CharacterRole {
        Tank,
        Healer,
        RangedDPS,
        MeleeDPS
    }

    public enum CharacterClass
    {
        Fighter,    // Guardian / Berserker         -- Tank/MDPS
        Shadow,     // Assassin / Ranger            -- RDPS/MDPS
        Totemic,    // Naturalist / Elementalist    -- Heal/RDPS
        Sorcerer,  // Diviner / Wizard         -- Heal/RDPS
        Paladin,    // Knight / Cleric              -- Tank/Heal
        Occultist,  // Scourge, Necromancer         -- MDPS/RDPS
    }

    public enum CharacterSpec
    {
        Guardian,       //Tank - Fighter
        Knight,         //Tank - Paladin
        Cleric,         //Healer - Paladin
        Diviner,        //Healer - Sorcerer
        Naturalist,     //Healer - Totemic
        Berserker,      //Melee DPS - Fighter
        Assassin,       //Melee DPS - Shadow
        Scourge,        //Melee DPS - Occulist
        Ranger,         //Ranged DPS - Shadow
        Wizard,         //Ranged DPS - Sorcerer
        Elementalist,   //Ranged DPS - Totemic
        Necromancer,    //Ranged DPS - Occultist
    }

    public enum CharacterAttack
    {
        TankStrike,
        RangedFireball,
        MeleeStab,
        HealerSmite,
    }

    public enum AttackValueTypes
    {
        CastTime,
        BaseDamageMultiplier,
    }

    public enum Ability
    {
        Interrupt,
        Stun,
        Slow,
        Dispel,
        ThroughputBoost,
    }

    public enum Cooldowns {
        SingleTargetCooldown,
        AoECooldown,
        HealingCooldown,
        TankCooldown,
        Immunity,
    }

    public enum EncounterAdds { 
        MiniBoss,
        SingleAdd,
        Pair,
        Triple,
        Quad,
        Many,
    }

    public enum EncounterSteps { 
        EncounterStart = 0,
        CalculateRaiderPerformanceForAttempt = 1,
        AssignCountersToEncounterAbilities = 2,
        AssignCounterToEncounterCooldowns = 3,
        ResolveAbilitiesCounters = 4,
        ResolveCooldownCounters = 5,
        FightStart = 6,
        FightInProgress = 7,
        FightWon = 8,
        FightLost = 9,
        GoToMainScreen = 10,

    }

    public enum Difficulties
    {
        Easy,       //60%
        Normal,     //100%
        Hard        //140%
    }

    public enum CharacterFlags { 
        CHARACTER_FLAG_IS_MOVING = 1 << 0,
    }

    public enum EncounterPhaseType {
        TimeBased,
        DPSBased,
    }

    public enum RaidHealingState
    {
        TankMediumDamage    = 1 << 1,
        TankHeavyDamage     = 1 << 2,
        RaidSingleMediumDamage = 1 << 4,
        RaidSingleHeavyDamage  = 1 << 5,
        RaidMultiMediumDamage = 1 << 7,
        RaidMultiHeavyDamage = 1 << 8,
        RandomTargets       = 1 << 9,
    }
}
