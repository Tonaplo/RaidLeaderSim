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
        Immune,
        Dispel,
        PreMovePositional,
        PostMovePositional,
    }

    public enum Cooldowns {
        DPSCooldown,
        HealingCooldown,
        TankCooldown,
    }

    public enum CooldownTargets {
        Self,
        Raid,
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
        ApplyConsumables = 1,
        CalculateRaiderPerformanceForAttempt = 2,
        ReadyToPull = 3,
        FightStart = 4,
        FightInProgress = 5,
        FightDone = 6,
        FightWon = 7,
        FightLost = 8,
        GoToMainScreen = 9,

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

    public enum ConsumableType
    {
        ThroughputIncrease,
        CastTimeDecrease,
        HealthIncrease,
        NumTypes = 3,
    }

    public enum ConsumableRarity
    {
        Normal,
        Rare,
        Epic,
    }

    public enum SkillTypes
    {
        Throughput = 0,
        Positional = 1,
        Mechanical = 2,
        NumSkillTypes = 3,
    }
}
