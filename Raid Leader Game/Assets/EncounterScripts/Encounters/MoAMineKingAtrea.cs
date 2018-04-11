using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoAMineKingAtrea : BaseEncounter
{
    /*
    - Tank attack (Engulf): have the attack deal damage to the raid, stacking in the same way as the "Keeper of the Mine"
    - Trying to kill the MineKing while a limb is alive makes him heal to full.
    - If he reaches 2% health while a limb is alive, he'll heal

    - On Hard, killing his limbs does not take off any of his health.
        - Cave In
            - If MineKing Atrea's body is the only thing present, he will pulse for increasing AoE damage
            - the AoE damage will stop and reset stacks as soon as another "limb" enters the fight
    - On Easy and Normal, killing his limbs will take off enough health to kill him outright.

    - Phase 1: Attack his legs
        - PreMovePositional (Stomp): Left Leg will stomp on you
        - RaidDamage (Kick): Right Leg will kick you.
        - Killing one leg causes the other leg to die and deal the remaining health in damage, split across the raid

    - Phase 2: kill his arms, like Kologarn from Ulduar.
        - There are DPS checks to make Right Arm stop casting raidwide damage - "Diamond throw"
        - Immune (Crushing Grip): If not countered, he will grab a raid member with Left Arm and deal a z damage over y seconds
        - Killing one arm increases cast times and damage by x %

        
    - Phase 3: Kill his body/face
        - Interrupt: Breath attack, if not interrupted, leaves a debuff on x raidmembers, causing them to bleed.
            - every time it is properly interrupted, the cast time halfs.
        - PostMovePositional: Eye Beams: deals damage during the cast, which you can exit.
    */

    enum MineKingPhase {
        Legs,
        Arms,
        Face,
    }

    // Locals
    // Enemies
    EncounterEnemy LeftLeg = null;
    EncounterEnemy RightLeg = null;
    EncounterEnemy LeftArm = null;
    EncounterEnemy RightArm = null;
    EncounterEnemy Visage = null;
    EncounterEnemy MineKing = null;

    // Ability or Attack Names
    string EngulfString = "Engulf";
    string KickString = "Kick";
    string StompString = "Stomp";
    string CrushingGripString = "Crushing Grip";
    string DiamondThrowString = "Diamond Throw";
    string BreathOfDeathString = "Breath of Death";
    string StenchOfDeathString = "Stench of Death";
    string EyeBeamString = "Eye Beam";

    string LeftLegString = "Left Leg";
    string RightLegString = "Right Leg";
    string LeftArmString = "Left Arm";
    string RightArmString = "Right Arm";
    string VisageString = "Visage";

    // Misc
    MineKingPhase m_phase = MineKingPhase.Legs;
    float m_percentageOfMaxHealthToRemoveOnLimbKill = 0.2f;
    int m_healToFullPercent = 5;
    bool m_hasAnArmBeenKilled = false;
    bool m_hasCrushingGripBeenCountered = false;
    int m_diamondThrowDamageMissing = 0;
    bool m_hasDiamondThrowBeenCountered = false;
    int m_breathOfDeathInterruptedInARow = 0;
    bool m_hasBreathOfDeathBeenCountered = false;

    public MoAMineKingAtrea() : base("Mineking Atrea") { m_encounterEnum = Enums.EncounterEnum.MoAMinekingAtrea; }

    public override void SetupEncounter()
    {
        CreateEnemy("Mineking Atrea", Mathf.RoundToInt(60000), Enums.EncounterEnemyType.Boss);
        MineKing = m_enemies.Find(e => e.Name == "Mineking Atrea");
    }

    public override void BeginEncounter()
    {
        CreateEnemy("Left Leg", Mathf.RoundToInt(15000 * GetDifficultyMultiplier()), Enums.EncounterEnemyType.Add);
        CreateEnemy("Right Leg", Mathf.RoundToInt(15000 * GetDifficultyMultiplier()), Enums.EncounterEnemyType.Add);
        LeftLeg = m_enemies.Find(e => e.Name == "Left Leg");
        RightLeg = m_enemies.Find(e => e.Name == "Right Leg");
        
        bool hasHitTank = false;
        for (int i = 0; i < m_raid.Count; i++)
        {
            if (m_raid[i].Raider.RaiderStats.GetRole() == Enums.CharacterRole.Tank && !hasHitTank)
            {
                hasHitTank = true;
                m_currentRaiderTarget = m_raid[i];
            }
        }

        if (!hasHitTank)
        {
            m_currentRaiderTarget = m_raid[0];
        }

        m_rsc.StartCoroutine(DoTankAttack(1.5f, m_currentRaiderTarget));
        m_rsc.StartCoroutine(WaitForStomp(GetStompWaitTime() / 3.0f));
        m_rsc.StartCoroutine(CastKick(GetKickCastTime()));

        if (m_difficulty == Enums.Difficulties.Hard)
            m_rsc.StartCoroutine(CastCaveIn(1.0f, 1));
    }

    public override void SetupLoot()
    {
        GenerateLoot(55, 5);
    }

    public override void SetupDescriptionAndAbilities()
    {
        m_description = "His sheer size means you must confront him one limb at a time: First his Legs, then his Arms and finally his Visage.";

        m_enemyDescription = new List<EncounterEnemyDescription> {
            new EncounterEnemyDescription(Name, Name + " was once a mere mortal man. Through the power of earth and the magic of the council, he is now so much more."),
            new EncounterEnemyDescription(LeftLegString, ""),
            new EncounterEnemyDescription(RightLegString, ""),
            new EncounterEnemyDescription(LeftArmString, ""),
            new EncounterEnemyDescription(RightArmString, ""),
            new EncounterEnemyDescription(VisageString, ""),
        };
        
        m_encounterAbilities = new List<EncounterAbility> {
            new EncounterAbility(StompString,"Left Leg", "The Left Leg of Mineking Atrea will stomp in the ground every " + GetStompWaitTime() + " seconds, dealing " + GetStompDamage() + " damage to any raider caught in it.", GetStompCastTime(), Enums.Ability.PreMovePositional, Enums.AbilityCastType.Cast),
            new EncounterAbility(CrushingGripString,"Left Arm", "The Left Arm grips a random raid member, dealing " + GetCrushingGripTickDamage() + " damage every second for " + GetNumCrushingGripTicks() + " seconds.", GetCrushingGripCastTime(), Enums.Ability.Immune, Enums.AbilityCastType.Cast),
            new EncounterAbility(DiamondThrowString,"Right Arm", "The Right Arm grabs a load of diamonds, preparing to hurl it at all raid members for " + GetCrushingGripTickDamage() + " damage. The Arm will drop the diamons if " + GetDiamondThrowDamageNeeded() + "% of it's health is taken off during the cast", GetDiamondThrowCastTime(), Enums.Ability.Damage, Enums.AbilityCastType.Cast),
            new EncounterAbility(EyeBeamString,"Visage", "The Visage of "+ Name+" shoots a beam of liquid gold from his eyes, dealing " + GetEyeBeamDamage() + " damage every second to every raider still standing in the beam.", GetEyeBeamCastTime(), Enums.Ability.PreMovePositional, Enums.AbilityCastType.Channel),
            new EncounterAbility(BreathOfDeathString,"Visage", "Every " + GetBreathOfDeathWaitTime()+ " seconds, the Visage of "+ Name + " will soak " + GetNumStenchOfDeathTargets() + " raider" +(GetNumStenchOfDeathTargets() != 1 ? "s" : "") + " in the  " + StenchOfDeathString + " unless interrupted. Every successful interrupt reduces time between casts and casttime by " + Utility.GetPercentString(1.0f-GetBreathOfDeathCastTimeReduction()), GetBreathOfDeathCastTime(), Enums.Ability.Interrupt, Enums.AbilityCastType.Cast),
            new EncounterAbility(StenchOfDeathString,"Visage", "The horrible smell causes " + GetStenchOfDeathTickDamage() + " damage every second for to the afflicted target until dispelled.", 0.0f, Enums.Ability.Dispel, Enums.AbilityCastType.Cast),
            new EncounterAbility(EngulfString, Name, "Every " + GetEngulfCastTime() + " seconds, " + Name + " smashes the entire raid for " + GetEngulfBaseDamage() + " damage. Each hit increases the damage by " + Utility.GetPercentString(GetEngulfMultiplier() - 1.0f) + ". The increased damage resets when " + Name + " is taunted.", 0, Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast),
            new EncounterAbility(KickString, RightLegString, "Every " + GetKickCastTime() + " seconds, " + Name + "'s Right Leg kicks the nearest " + GetNumKickTargets() +" raiders for " + GetKickDamage() + " damage.", 0, Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast),
            new EncounterAbility("Together We Fall", LeftLegString, "Killing either the Left or Right Leg will cause the remaining leg to explode, killing it and dealing it's remaining health in damage split between the raid.", 0, Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast),
            new EncounterAbility("Together We Fall", RightLegString, "Killing either the Left or Right Leg will cause the remaining leg to explode, killing it and dealing it's remaining health in damage split between the raid.", 0, Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast),
            new EncounterAbility("Out of my Way", LeftArmString, "Killing either the Left or Right Arm will cause the remaining arm to cast " + Utility.GetPercentString(1.0f - ArmDeadBonusCastTimeMultiplierDisplay()) + " faster and deal "+ Utility.GetPercentString(ArmDeadBonusDamageMultiplierDisplay() - 1.0f) + " more damage.", 0, Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast),
            new EncounterAbility("Out of my Way", RightArmString, "Killing either the Left or Right Arm will cause the remaining arm to cast " + Utility.GetPercentString(1.0f - ArmDeadBonusCastTimeMultiplierDisplay()) + " faster and deal "+ Utility.GetPercentString(ArmDeadBonusDamageMultiplierDisplay() - 1.0f) + " more damage.", 0, Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast),
            new EncounterAbility("Reflection", Name, "Attacking "+ Name+" directly will reflect "+ Utility.GetPercentString(GetDamageReflectionMultiplier()) + " of the damage dealt back to the attacker.", 0, Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast),
        };

        if (m_difficulty == Enums.Difficulties.Hard)
        {
            m_encounterAbilities.Add(new EncounterAbility("It's just a Stone Wound", Name, "On Hard difficulty, killing off any of " + Name + "'s limbs will not deal any damage to him. His legs will enter the fight when he reaches 60% health and his Visage when he reaches 20% health.", 0, Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast));
            m_encounterAbilities.Add(new EncounterAbility("Cave In", Name, "If none of the Mineking's limbs are in the battle, he will deal " + GetCaveInBaseDamage() + " damage every second and gain a stack of Rage. Each Rage stack increases the damage of Cave In by 100%.", 0, Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast));
            m_encounterAbilities.Add(new EncounterAbility("Regeneration", Name, "If any of the Mineking's limbs are alive when he reaches " + m_healToFullPercent + "%, he will fully heal.", 0, Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast));
        }
        else
        {
            m_encounterAbilities.Add(new EncounterAbility("Part of a Whole", Name, "Killing off any of " + Name + "'s limbs will take " + Utility.GetPercentString(m_percentageOfMaxHealthToRemoveOnLimbKill) + " of his health from him.", 0, Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast));
        }
    }

    public override void CurrentAbilityCountered()
    {
        HandleAbilityTypeCountered(m_currentAbility.Ability);
        if (m_currentAbility.Name == CrushingGripString)
            m_hasCrushingGripBeenCountered = true;

        if (m_currentAbility.Name == BreathOfDeathString) {
            m_breathOfDeathInterruptedInARow++;
            m_hasBreathOfDeathBeenCountered = true;
            m_currentAbility = null;
        }
    }

    public override void HandleOnTakeDamageEvent(int damage, RaiderScript attacker)
    {
        EncounterEnemy currentTarget = m_enemies.Find(e => e.IsCurrentTarget());
        if (currentTarget != null && currentTarget == MineKing)
        {
            attacker.TakeDamage(Mathf.RoundToInt(damage * GetDamageReflectionMultiplier()), "Reflection");
        }

        if (!m_hasDiamondThrowBeenCountered && currentTarget != null && currentTarget == RightArm && m_currentAbility != null && m_currentAbility.Name == DiamondThrowString)
        {
            m_diamondThrowDamageMissing -= damage;
            if (m_diamondThrowDamageMissing <= 0)
            {
                m_rsc.EndCastingAbility(m_currentAbility);
                m_hasDiamondThrowBeenCountered = true;
                m_currentAbility = null;
            }
        }

        if (m_enemies.FindAll(e => e.EnemyType == Enums.EncounterEnemyType.Add).Count != 0 && MineKing != null && MineKing.Healthbar.GetHealthPercent() <= m_healToFullPercent)
            MineKing.Healthbar.ModifyHealth(MineKing.Healthbar.MaxHealth);

        switch (m_phase)
        {
            case MineKingPhase.Legs:
                LeftLeg = m_enemies.Find(e => e.Name == "Left Leg");
                RightLeg = m_enemies.Find(e => e.Name == "Right Leg");
                EncounterEnemy ExplodingLeg = null;

                if (LeftLeg == null)
                {
                    ExplodingLeg = RightLeg;
                }
                else if (RightLeg == null)
                {
                    ExplodingLeg = LeftLeg;
                }

                if (ExplodingLeg != null)
                {
                    int numRaidersAlive = 0;
                    foreach (var raider in m_raid)
                    {
                        if (!raider.IsDead())
                            numRaidersAlive++;
                    }

                    int damageDoneToRaiders = ExplodingLeg.Healthbar.CurrentHealth / numRaidersAlive;
                    foreach (var raider in m_raid)
                    {
                        if (!raider.IsDead())
                            raider.TakeDamage(damageDoneToRaiders, "Together We Fall");
                    }

                    ExplodingLeg.Healthbar.ModifyHealth(-ExplodingLeg.Healthbar.MaxHealth);
                    ExplodingLeg.DestroyHealthBar();
                    m_enemies.Remove(ExplodingLeg);


                    //Ensures their locals are set to null
                    LeftLeg = m_enemies.Find(e => e.Name == "Left Leg");
                    RightLeg = m_enemies.Find(e => e.Name == "Right Leg");

                    //Two limbs died here, so...
                    HandleLimbDeath();
                    HandleLimbDeath();
                }

                break;
            case MineKingPhase.Arms:
                EncounterEnemy LeftArmTemp = m_enemies.Find(e => e.Name == "Left Arm");
                EncounterEnemy RightArmTemp = m_enemies.Find(e => e.Name == "Right Arm");

                if (LeftArm != null && LeftArmTemp == null)
                {
                    HandleLimbDeath();
                    m_hasAnArmBeenKilled = true;
                }
                else if (RightArm != null && RightArmTemp == null)
                {
                    HandleLimbDeath();
                    m_hasAnArmBeenKilled = true;
                }

                LeftArm = m_enemies.Find(e => e.Name == "Left Arm");
                RightArm = m_enemies.Find(e => e.Name == "Right Arm");

                break;
            case MineKingPhase.Face:

                EncounterEnemy VisageTemp = m_enemies.Find(e => e.Name == "Visage");

                if (Visage != null && VisageTemp == null)
                {
                    HandleLimbDeath();
                }

                Visage = m_enemies.Find(e => e.Name == "Visage");
                break;
            default:
                break;
        }

        HandleShouldPhase();
    }
    
    void HandleLimbDeath()
    {
        if (m_difficulty != Enums.Difficulties.Hard)
        {
            MineKing.Healthbar.ModifyHealth(-(MineKing.Healthbar.MaxHealth * m_percentageOfMaxHealthToRemoveOnLimbKill));

            if (MineKing.Healthbar.IsDead())
            {
                MineKing.DestroyHealthBar();
                m_enemies.Remove(MineKing);
                MineKing = null;
            }
        }
    }

    void HandleShouldPhase()
    {
        switch (m_phase)
        {
            case MineKingPhase.Legs:
                if (MineKing.Healthbar.GetHealthPercent() < 61) {
                    CreateEnemy("Left Arm", Mathf.RoundToInt(15000 * GetDifficultyMultiplier()), Enums.EncounterEnemyType.Add);
                    CreateEnemy("Right Arm", Mathf.RoundToInt(15000 * GetDifficultyMultiplier()), Enums.EncounterEnemyType.Add);
                    LeftArm = m_enemies.Find(e => e.Name == "Left Arm");
                    RightArm = m_enemies.Find(e => e.Name == "Right Arm");

                    m_rsc.StartCoroutine(WaitForCrushingGrip(GetCrushingGripWaitTime()/3.0f));
                    m_rsc.StartCoroutine(WaitForDiamondThrow(GetDiamondThrowWaitTime() / 5.0f));

                    m_phase = MineKingPhase.Arms;
                }
                break;
            case MineKingPhase.Arms:
                if (MineKing.Healthbar.GetHealthPercent() < 21)
                {
                    CreateEnemy("Visage", Mathf.RoundToInt(30000 * GetDifficultyMultiplier()), Enums.EncounterEnemyType.Add);
                    Visage = m_enemies.Find(e => e.Name == "Visage");
                    m_rsc.StartCoroutine(WaitForEyeBeam(GetEyeBeamWaitTime()/3.0f));
                    m_rsc.StartCoroutine(WaitForBreathOfDeath(GetBreathOfDeathWaitTime() / 4.0f));
                    m_phase = MineKingPhase.Face;
                }
                break;
            case MineKingPhase.Face:
            default:
                break;
        }
    }

    float GetDamageReflectionMultiplier()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 0.5f;
            case Enums.Difficulties.Normal:
            default:
                return 1.0f;
            case Enums.Difficulties.Hard:
                return 0.75f;
        }
    }

    int GetCaveInBaseDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
            case Enums.Difficulties.Normal:
            default:
                return 0; //This should only happen on Hard
            case Enums.Difficulties.Hard:
                return 50;
        }
    }

    IEnumerator CastCaveIn(float waitTime, int caveInCounter)
    {
        yield return new WaitForSeconds(waitTime);

        if (MineKing != null) {
            if (m_enemies.FindAll(e => e.EnemyType == Enums.EncounterEnemyType.Add).Count == 0)
            {
                foreach (var raider in m_raid)
                {
                    raider.TakeDamage(GetCaveInBaseDamage() * caveInCounter, "Cave In");
                }
                caveInCounter++;
            }
            else
                caveInCounter = 1;

            m_rsc.StartCoroutine(CastCaveIn(1.0f, caveInCounter));
        }
    }

    int GetEngulfBaseDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 25;
            case Enums.Difficulties.Normal:
            default:
                return 50;
            case Enums.Difficulties.Hard:
                return 75;
        }
    }

    float GetEngulfCastTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 3.0f;
            case Enums.Difficulties.Normal:
            default:
                return 2.5f;
            case Enums.Difficulties.Hard:
                return 2.0f;
        }
    }

    float GetEngulfMultiplier()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 1.10f;
            case Enums.Difficulties.Normal:
            default:
                return 1.20f;
            case Enums.Difficulties.Hard:
                return 1.30f;
        }
    }

    float GetEngulfDuration()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 10.0f;
            case Enums.Difficulties.Normal:
            default:
                return 12.0f;
            case Enums.Difficulties.Hard:
                return 15.0f;
        }
    }

    IEnumerator DoTankAttack(float castTime, RaiderScript target)
    {
        yield return new WaitForSeconds(castTime);

        if (!m_rsc.IsRaidDead() && !target.IsDead() && !IsDead())
        {
            int numStacks = GetNumStacksForRaider(m_currentRaiderTarget);

            foreach (var raider in m_raid)
            {
                raider.TakeDamage(Mathf.RoundToInt(GetEngulfBaseDamage() * Mathf.Pow(GetEngulfMultiplier(), numStacks)), "Engulf");
            }

            AddStackstoRaider(m_currentRaiderTarget, 1, GetEngulfDuration());
        }
        else if (target.IsDead())
        {
            m_currentRaiderTarget = null;
            List<RaiderScript> otherTanks = m_rsc.GetRaid().FindAll(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.Tank && x.Raider.GetName() != target.Raider.GetName());
            for (int i = 0; i < otherTanks.Count; i++)
            {
                if (!otherTanks[i].IsDead())
                {
                    m_currentRaiderTarget = otherTanks[i];
                    break;
                }
            }

            if (m_currentRaiderTarget == null)
            {
                for (int i = 0; i < m_raid.Count; i++)
                {
                    if (!m_raid[i].IsDead())
                    {
                        m_currentRaiderTarget = m_raid[i];
                        break;
                    }
                }
            }
        }

        if (!m_rsc.IsRaidDead() && !IsDead())
        {
            m_rsc.StartCoroutine(DoTankAttack(Utility.GetFussyCastTime(GetEngulfCastTime()), m_currentRaiderTarget));
        }
    }

    int GetStompDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 100;
            case Enums.Difficulties.Normal:
            default:
                return 250;
            case Enums.Difficulties.Hard:
                return 500;
        }
    }

    float GetStompCastTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 5.0f;
            case Enums.Difficulties.Normal:
            default:
                return 4.5f;
            case Enums.Difficulties.Hard:
                return 4.0f;
        }
    }

    float GetStompWaitTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 15.0f;
            case Enums.Difficulties.Normal:
            default:
                return 12.5f;
            case Enums.Difficulties.Hard:
                return 10.0f;
        }
    }

    IEnumerator WaitForStomp(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (LeftLeg != null && !m_rsc.IsRaidDead())
        {
            if (m_currentAbility == null)
            {
                InitiatePositionalAbility();
                m_currentAbility = m_encounterAbilities.Find(x => x.Name == StompString);
                m_rsc.StartCoroutine(CastStomp(GetStompCastTime()));
                m_rsc.BeginCastingAbility(m_currentAbility);
                m_rsc.StartCoroutine(WaitForStomp(GetStompCastTime() + GetStompWaitTime()));
            }
            else
            {
                m_rsc.StartCoroutine(WaitForStomp(0.5f));
            }
        }

    }

    IEnumerator CastStomp(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (LeftLeg != null && !m_rsc.IsRaidDead() && m_positionalTargets.Count > 0)
        {
            foreach (var raider in m_positionalTargets)
            {
                raider.TakeDamage(GetStompDamage(), StompString);
            }
        }

        m_rsc.EndCastingAbility(m_currentAbility);
        m_currentAbility = null;
    }

    int GetKickDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 30;
            case Enums.Difficulties.Normal:
            default:
                return 60;
            case Enums.Difficulties.Hard:
                return 100;
        }
    }

    int GetNumKickTargets()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 2;
            case Enums.Difficulties.Normal:
            default:
                return 4;
            case Enums.Difficulties.Hard:
                return 6;
        }
    }

    float GetKickCastTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 4.0f;
            case Enums.Difficulties.Normal:
            default:
                return 3.0f;
            case Enums.Difficulties.Hard:
                return 2.0f;
        }
    }
    
    IEnumerator CastKick(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (RightLeg != null && !m_rsc.IsRaidDead())
        {
            List<RaiderScript> targets = GetRandomRaidTargets(GetNumKickTargets());
            foreach (var raider in targets)
            {
                raider.TakeDamage(GetKickDamage(), KickString);
            }

            m_rsc.StartCoroutine(CastKick(GetKickCastTime()));
        }
    }

    float ArmDeadBonusDamageMultiplierDisplay()
    {
        return 1.5f;
    }

    float ArmDeadBonusDamageMultiplier()
    {
        return (m_hasAnArmBeenKilled) ? ArmDeadBonusDamageMultiplierDisplay() : 1.0f;
    }

    float ArmDeadBonusCastTimeMultiplierDisplay()
    {
        return 0.5f;
    }

    float ArmDeadBonusCastTimeMultiplier()
    {
        return (m_hasAnArmBeenKilled) ? ArmDeadBonusCastTimeMultiplierDisplay() : 1.0f;
    }
    
    int GetCrushingGripTickDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return Mathf.RoundToInt(25 * ArmDeadBonusDamageMultiplier());
            case Enums.Difficulties.Normal:
            default:
                return Mathf.RoundToInt(50 * ArmDeadBonusDamageMultiplier());
            case Enums.Difficulties.Hard:
                return Mathf.RoundToInt(100 * ArmDeadBonusDamageMultiplier());
        }
    }

    int GetNumCrushingGripTicks()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 3;
            case Enums.Difficulties.Normal:
            default:
                return 5;
            case Enums.Difficulties.Hard:
                return 10;
        }
    }

    float GetCrushingGripCastTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 5.0f * ArmDeadBonusCastTimeMultiplier();
            case Enums.Difficulties.Normal:
            default:
                return 4.5f * ArmDeadBonusCastTimeMultiplier();
            case Enums.Difficulties.Hard:
                return 4.0f * ArmDeadBonusCastTimeMultiplier();
        }
    }

    float GetCrushingGripWaitTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 15.0f * ArmDeadBonusCastTimeMultiplier();
            case Enums.Difficulties.Normal:
            default:
                return 12.5f * ArmDeadBonusCastTimeMultiplier();
            case Enums.Difficulties.Hard:
                return 10.0f * ArmDeadBonusCastTimeMultiplier();
        }
    }

    IEnumerator WaitForCrushingGrip(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (!m_rsc.IsRaidDead() && !IsDead() && LeftArm != null)
        {
            if (m_currentAbility == null)
            {
                m_hasCrushingGripBeenCountered = false;
                m_encounterAbilities.Find(x => x.Name == CrushingGripString).SetNewCastTime(GetCrushingGripCastTime());
                m_currentAbility = m_encounterAbilities.Find(x => x.Name == CrushingGripString);
                m_rsc.StartCoroutine(CastCrushingGrip(GetCrushingGripCastTime(), null, GetNumCrushingGripTicks()));
                m_rsc.BeginCastingAbility(m_currentAbility);
            }
            else
            {
                m_rsc.StartCoroutine(WaitForCrushingGrip(0.5f));
            }
        }

    }

    IEnumerator CastCrushingGrip(float waitTime, RaiderScript target, int tickCounter)
    {
        yield return new WaitForSeconds(waitTime);

        if (target == null)
        {
            target = GetRandomRaidTargets(1)[0];
        }

        if (LeftArm != null && !m_rsc.IsRaidDead())
        {
            if (!m_hasCrushingGripBeenCountered && !target.IsDead())
            {
                target.TakeDamage(GetCrushingGripTickDamage(), CrushingGripString);
                tickCounter--;
                if (tickCounter != 0)
                    m_rsc.StartCoroutine(CastCrushingGrip(1.0f, target, tickCounter));
            }   
        }

        if (m_currentAbility != null && m_currentAbility.Name == CrushingGripString)
        {
            m_rsc.StartCoroutine(WaitForCrushingGrip(GetCrushingGripWaitTime()));
            m_rsc.EndCastingAbility(m_currentAbility);
            m_currentAbility = null;
        }

    }

    int GetDiamondThrowDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return Mathf.RoundToInt(100 * ArmDeadBonusDamageMultiplier());
            case Enums.Difficulties.Normal:
            default:
                return Mathf.RoundToInt(250 * ArmDeadBonusDamageMultiplier());
            case Enums.Difficulties.Hard:
                return Mathf.RoundToInt(600 * ArmDeadBonusDamageMultiplier());
        }
    }

    int GetDiamondThrowDamageNeeded()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 2;
            case Enums.Difficulties.Normal:
            default:
                return 4;
            case Enums.Difficulties.Hard:
                return 7;
        }
    }

    float GetDiamondThrowCastTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 5.0f * ArmDeadBonusCastTimeMultiplier();
            case Enums.Difficulties.Normal:
            default:
                return 4.5f * ArmDeadBonusCastTimeMultiplier();
            case Enums.Difficulties.Hard:
                return 4.0f * ArmDeadBonusCastTimeMultiplier();
        }
    }

    float GetDiamondThrowWaitTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 15.0f * ArmDeadBonusCastTimeMultiplier();
            case Enums.Difficulties.Normal:
            default:
                return 12.5f * ArmDeadBonusCastTimeMultiplier();
            case Enums.Difficulties.Hard:
                return 10.0f * ArmDeadBonusCastTimeMultiplier();
        }
    }

    IEnumerator WaitForDiamondThrow(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (!m_rsc.IsRaidDead() && !IsDead() && RightArm != null)
        {
            if (m_currentAbility == null)
            {
                m_hasDiamondThrowBeenCountered = false;
                m_diamondThrowDamageMissing = Mathf.RoundToInt(RightArm.Healthbar.MaxHealth * (GetDiamondThrowDamageNeeded()/100.0f));
                m_encounterAbilities.Find(x => x.Name == DiamondThrowString).SetNewCastTime(GetDiamondThrowCastTime());
                m_currentAbility = m_encounterAbilities.Find(x => x.Name == DiamondThrowString);
                m_rsc.StartCoroutine(CastDiamondThrow(GetDiamondThrowCastTime()));
                m_rsc.StartCoroutine(WaitForDiamondThrow(GetDiamondThrowCastTime() + GetDiamondThrowWaitTime()));
                m_rsc.BeginCastingAbility(m_currentAbility);
            }
            else
            {
                m_rsc.StartCoroutine(WaitForDiamondThrow(0.5f));
            }
        }

    }

    IEnumerator CastDiamondThrow(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (RightArm != null && !m_rsc.IsRaidDead() && !m_hasDiamondThrowBeenCountered)
        {
            foreach (var raider in m_raid)
            {
                raider.TakeDamage(GetDiamondThrowDamage(), DiamondThrowString);
            }
        }

        if(m_currentAbility != null && m_currentAbility.Name == DiamondThrowString)
            m_currentAbility = null;
    }

    int GetEyeBeamDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 25;
            case Enums.Difficulties.Normal:
            default:
                return 50;
            case Enums.Difficulties.Hard:
                return 100;
        }
    }

    float GetEyeBeamCastTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 5.0f;
            case Enums.Difficulties.Normal:
            default:
                return 5.0f;
            case Enums.Difficulties.Hard:
                return 5.0f;
        }
    }

    float GetEyeBeamWaitTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 15.0f;
            case Enums.Difficulties.Normal:
            default:
                return 12.5f;
            case Enums.Difficulties.Hard:
                return 10.0f;
        }
    }

    IEnumerator WaitForEyeBeam(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (Visage != null && !m_rsc.IsRaidDead())
        {
            if (m_currentAbility == null)
            {
                InitiatePositionalAbility();
                m_currentAbility = m_encounterAbilities.Find(x => x.Name == EyeBeamString);
                m_rsc.BeginCastingAbility(m_currentAbility);
                m_rsc.StartCoroutine(DealEyeBeamDamage(0.5f));
                m_rsc.StartCoroutine(CastEyeBeam(GetEyeBeamCastTime()));
                m_rsc.StartCoroutine(WaitForEyeBeam(GetEyeBeamCastTime() + GetEyeBeamWaitTime()));
            }
            else
            {
                m_rsc.StartCoroutine(WaitForEyeBeam(0.5f));
            }
        }

    }

    IEnumerator CastEyeBeam(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        m_rsc.EndCastingAbility(m_currentAbility);
        m_currentAbility = null;
    }

    IEnumerator DealEyeBeamDamage(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (Visage != null && !m_rsc.IsRaidDead() && m_positionalTargets.Count > 0 && m_currentAbility != null && m_currentAbility.Name == EyeBeamString)
        {
            foreach (var raider in m_positionalTargets)
            {
                raider.TakeDamage(GetEyeBeamDamage(), EyeBeamString);
            }
            m_rsc.StartCoroutine(DealEyeBeamDamage(1f));
        }
    }

    float GetBreathOfDeathCastTimeReduction()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 0.85f;
            case Enums.Difficulties.Normal:
            default:
                return 0.60f;
            case Enums.Difficulties.Hard:
                return 0.45f;
        }
    }

    float GetBreathOfDeathCastTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 5.0f * Mathf.Pow(GetBreathOfDeathCastTimeReduction(), m_breathOfDeathInterruptedInARow);
            case Enums.Difficulties.Normal:
            default:
                return 4.0f * Mathf.Pow(GetBreathOfDeathCastTimeReduction(), m_breathOfDeathInterruptedInARow);
            case Enums.Difficulties.Hard:
                return 3.0f * Mathf.Pow(GetBreathOfDeathCastTimeReduction(), m_breathOfDeathInterruptedInARow);
        }
    }
    
    float GetBreathOfDeathWaitTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 15.0f * Mathf.Pow(GetBreathOfDeathCastTimeReduction(), m_breathOfDeathInterruptedInARow);
            case Enums.Difficulties.Normal:
            default:
                return 12.5f * Mathf.Pow(GetBreathOfDeathCastTimeReduction(), m_breathOfDeathInterruptedInARow);
            case Enums.Difficulties.Hard:
                return 10.0f * Mathf.Pow(GetBreathOfDeathCastTimeReduction(), m_breathOfDeathInterruptedInARow);
        }
    }

    IEnumerator WaitForBreathOfDeath(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (Visage != null && !m_rsc.IsRaidDead())
        {
            if (m_currentAbility == null)
            {
                m_hasBreathOfDeathBeenCountered = false;
                m_encounterAbilities.Find(x => x.Name == BreathOfDeathString).SetNewCastTime(GetBreathOfDeathCastTime());
                m_currentAbility = m_encounterAbilities.Find(x => x.Name == BreathOfDeathString);
                m_rsc.StartCoroutine(CastBreathOfDeath(GetBreathOfDeathCastTime()));
                m_rsc.BeginCastingAbility(m_currentAbility);
                m_rsc.StartCoroutine(WaitForBreathOfDeath(GetBreathOfDeathCastTime() + GetBreathOfDeathWaitTime()));
            }
            else
            {
                m_rsc.StartCoroutine(WaitForBreathOfDeath(0.5f));
            }
        }

    }

    IEnumerator CastBreathOfDeath(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (Visage != null && !m_rsc.IsRaidDead() && !m_hasBreathOfDeathBeenCountered)
        {
            List<RaiderScript> debuffTargets = GetRandomRaidTargets(GetNumStenchOfDeathTargets());

            for (int i = 0; i < debuffTargets.Count; i++)
            {
                GameObject temp = new GameObject();
                temp.AddComponent<RaiderDebuff>().Initialize(debuffTargets[i], StenchOfDeathString, GetStenchOfDeathTickDamage());
                debuffTargets[i].AddDebuff(temp);
            }
        }

        if (m_currentAbility != null && m_currentAbility.Name == BreathOfDeathString)
        {
            m_rsc.EndCastingAbility(m_currentAbility);
            m_currentAbility = null;
        }
    }
    
    int GetStenchOfDeathTickDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 10;
            case Enums.Difficulties.Normal:
            default:
                return 30;
            case Enums.Difficulties.Hard:
                return 50;
        }
    }
    
    int GetNumStenchOfDeathTargets()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 1;
            case Enums.Difficulties.Normal:
            default:
                return 2;
            case Enums.Difficulties.Hard:
                return 3;
        }
    }

}


