using UnityEngine;
using System.Collections;

public static class Enums{

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

    public enum Ability
    {
        Interrupt,
        Stun,
        Slow,
        Dispel,
    }

    public enum Cooldowns {
        DPSCooldown,
        HealingCooldown,
        TankCooldown,
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
        FightDone = 8,
        FightWon = 9,
        FightLost = 10,
        GoToMainScreen = 11,

    }

    public enum Difficulties
    {
        Easy,       //60%
        Normal,     //100%
        Hard        //140%
    }

    public enum CharacterStatus { 
        Ready,
        OnVacation,
        InTraining,
        //Add more
    }

    public enum EncounterPhaseType {
        TimeBased,
        DPSBased,
    }

    public enum RaidHealingState
    {
        TankMediumDamage        = 1 << 1,
        TankHeavyDamage         = 1 << 2,
        RaidSingleMediumDamage  = 1 << 4,
        RaidSingleHeavyDamage   = 1 << 5,
        RaidMultiMediumDamage   = 1 << 7,
        RaidMultiHeavyDamage    = 1 << 8,
        LowestHealthPercent     = 1 << 9,
    }

    public enum GearTypes
    {
        Head = 0,
        Chest = 1,
        Leg = 2,
        Weapon = 3,
        Jewel = 4,
        NumGearTypes = 5,
    }

    public enum SkillTypes
    {
        Throughput = 0,
        Positional = 1,
        Mechanical = 2,
        NumSkillTypes = 3,
    }
}
