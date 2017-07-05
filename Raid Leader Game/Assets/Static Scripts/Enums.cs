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

    public enum ThroughputTypes {
        SingleTargetHealing,
        SingleTargetDPS,
        AoEHealing,
        AoEDPS,
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
        Sorcerous,  // WitchDoctor / Wizard         -- Heal/RDPS
        Paladin,    // Knight / Cleric              -- Tank/Heal
        Occultist,  // Scourge, Necromancer         -- MDPS/RDPS
    }

    public enum CharacterSpec
    {
        Guardian,       //Tank - Fighter
        Knight,         //Tank - Paladin
        Cleric,         //Healer - Paladin
        WitchDoctor,    //Healer - Sorcerous
        Naturalist,     //Healer - Totemic
        Berserker,      //Melee DPS - Fighter
        Assassin,       //Melee DPS - Shadow
        Scourge,        //Melee DPS - Occulist
        Ranger,         //Ranged DPS - Shadow
        Wizard,         //Ranged DPS - Sorcerous
        Elementalist,   //Ranged DPS - Totemic
        Necromancer,    //Ranged DPS - Occultist
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
        EvaluateFight = 6,

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
}
