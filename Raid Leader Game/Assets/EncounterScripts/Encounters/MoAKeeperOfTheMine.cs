using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoAKeeperOfTheMine : BaseEncounter
{
    int m_normalHealth = 60000;
    float m_ClubSwingCastTime = 2.5f;
    float m_PebbleThrowCastTime = 1.5f;
    
    IEnumerator m_currentAbilityCoroutine;

    public MoAKeeperOfTheMine() : base("Keeper of the Mine") { m_encounterEnum = Enums.EncounterEnum.MoAKeeperOfTheMine; }

    public override void SetupEncounter()
    {
        CreateEnemy(m_name, Mathf.RoundToInt(m_normalHealth * GetDifficultyMultiplier()), Enums.EncounterEnemyType.Boss);
    }

    public override void BeginEncounter()
    {
        List<RaiderScript> pebbletargets = GetRandomRaidTargets(GetPebbleThrowTargetCount());
        m_currentRaiderTarget = null;
        for (int i = 0; i < m_raid.Count; i++)
        {
            if(pebbletargets.Contains(m_raid[i]))
                m_rsc.StartCoroutine(DoBasicAttack(Utility.GetFussyCastTime(m_PebbleThrowCastTime), (int)(GetPebbleThrowDamage() * Random.value), m_raid[i]));

            if (m_raid[i].Raider.RaiderStats.GetRole() == Enums.CharacterRole.Tank && m_currentRaiderTarget == null)
            {
                m_currentRaiderTarget = m_raid[i];
            }
        }

        if (m_currentRaiderTarget == null)
        {
            m_currentRaiderTarget = m_raid[0];
        }

        m_rsc.StartCoroutine(DoTankAttack(1.5f, m_currentRaiderTarget));
        m_rsc.StartCoroutine(WaitForAvalance(GetAvalanceWaitTime()));
        
        if(m_difficulty == Enums.Difficulties.Hard)
            m_rsc.StartCoroutine(WaitForGroundPound(GetGroundPoundWaitTime() * 1.25f));
    }

    public override void SetupLoot()
    {
        GenerateLoot(50, 5);
    }

    public override void SetupDescriptionAndAbilities()
    {
        m_description = Name + " guards the entrance to the Mine. He will give his life - and collapse the entry to the mine entirely - to ensure your party do not pass him.";

        m_enemyDescription = new List<EncounterEnemyDescription> {
            new EncounterEnemyDescription(Name, "The massive Keeper is the only enemy in this encounter."),
        };

        m_encounterAbilities = new List<EncounterAbility> {
            new EncounterAbility("Avalanche", Name, "Every " + GetAvalanceWaitTime() + " seconds, " + Name + " bashes the wall of the cave, causing an Avalanche, dealing " + GetAvalanceDamage() + " to all raid members.", GetAvalanceCastTime(),Enums.Ability.Interrupt, Enums.AbilityCastType.Cast),
            new EncounterAbility("Club Swing", Name,"The " + Name + " swings his mighty club at his target, dealing " + GetClubSwingDamage() + ", each hit increasing in damage by " + Utility.GetPercentIncreaseString(GetClubSwingIncrease()) +". Resets on target switch.", 0, Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast),
            new EncounterAbility("Pebble Slide",Name, "Pebbles from the Cave randomly fall down, hitting "+ GetPebbleThrowTargetCount() + " random raid members for up to " + GetPebbleThrowDamage() + " damage.", 0, Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast),
        };

        if (m_difficulty == Enums.Difficulties.Hard)
        {
            m_encounterAbilities.Add(new EncounterAbility("Ground Pound", Name, Name + " raises his fists in the air to pound the ground where the raid stands, dealing " + GetGroundPoundDamage() + " to any raid member that does not move in time.", GetGroundPoundCastTime(), Enums.Ability.PreMovePositional, Enums.AbilityCastType.Cast));
        }
    }

    public override void CurrentAbilityCountered()
    {
        m_rsc.StopCoroutine(m_currentAbilityCoroutine);
        m_rsc.StartCoroutine(WaitForAvalance(GetAvalanceWaitTime()));
        HandleAbilityTypeCountered(m_currentAbility.Ability);
        m_currentAbility = null;
    }

    int GetClubSwingDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 150;
            case Enums.Difficulties.Normal:
            default:
                return 200;
            case Enums.Difficulties.Hard:
                return 250;
        }
    }

    float GetClubSwingIncrease()
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

    float GetClubSwingDuration()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 10.0f;
            case Enums.Difficulties.Normal:
            default:
                return 10.0f;
            case Enums.Difficulties.Hard:
                return 10.0f;
        }
    }

    int GetPebbleThrowDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 100;
            case Enums.Difficulties.Normal:
            default:
                return 150;
            case Enums.Difficulties.Hard:
                return 200;
        }
    }

    int GetPebbleThrowTargetCount()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 2;
            case Enums.Difficulties.Normal:
            default:
                return 3;
            case Enums.Difficulties.Hard:
                return 4;
        }
    }

    int GetAvalanceDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 100;
            case Enums.Difficulties.Normal:
            default:
                return 175;
            case Enums.Difficulties.Hard:
                return 20000; // Basically, if you miss an interrupt, your raid dies.
        }
    }

    float GetAvalanceWaitTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 15.0f;
            case Enums.Difficulties.Normal:
            default:
                return 10.0f;
            case Enums.Difficulties.Hard:
                return 5.0f;
        }
    }

    float GetAvalanceCastTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 5.0f;
            case Enums.Difficulties.Normal:
            default:
                return 3.5f;
            case Enums.Difficulties.Hard:
                return 2.5f;
        }
    }

    int GetGroundPoundDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
            case Enums.Difficulties.Normal:
            default:
                return 0; //This shouldnt work on anything other than Hard
            case Enums.Difficulties.Hard:
                return 300; 
        }
    }

    float GetGroundPoundWaitTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
            case Enums.Difficulties.Normal:
            default:
                return 0.0f; //This shouldnt work on anything other than Hard
            case Enums.Difficulties.Hard:
                return 10.0f;
        }
    }

    float GetGroundPoundCastTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
            case Enums.Difficulties.Normal:
            default:
                return 0.0f; //This shouldnt work on anything other than Hard
            case Enums.Difficulties.Hard:
                return GetAvalanceCastTime();
        }
    }

    IEnumerator DoBasicAttack(float castTime, int damage, RaiderScript target)
    {
        yield return new WaitForSeconds(castTime);

        if (!IsDead())
        {
            if (!target.IsDead())
                target.TakeDamage(damage, "Pebble Throw");

            List<RaiderScript> newTarget = GetRandomRaidTargets(1);
            if(newTarget.Count > 0)
                m_rsc.StartCoroutine(DoBasicAttack(Utility.GetFussyCastTime(m_PebbleThrowCastTime), (int)(GetPebbleThrowDamage() * Random.value), newTarget[0]));
        }
    }

    IEnumerator DoTankAttack(float castTime, RaiderScript target)
    {
        yield return new WaitForSeconds(castTime);

        if (!target.IsDead() && !IsDead())
        {
            int numStacks = GetNumStacksForRaider(m_currentRaiderTarget);
            int damage = Mathf.RoundToInt(GetClubSwingDamage() * Mathf.Pow(GetClubSwingIncrease(), numStacks));
            m_currentRaiderTarget.TakeDamage(damage, "Club Swing");
            AddStackstoRaider(m_currentRaiderTarget, 1, GetClubSwingDuration());

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
            
            if(m_currentRaiderTarget == null)
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
            m_rsc.StartCoroutine(DoTankAttack(Utility.GetFussyCastTime(m_ClubSwingCastTime), m_currentRaiderTarget));
        }
    }

    IEnumerator WaitForAvalance(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        
        if (!IsDead() && !m_rsc.IsRaidDead())
        {
            if (m_currentAbility == null)
            {
                m_currentAbility = m_encounterAbilities.Find(x => x.Name == "Avalanche");
                m_rsc.BeginCastingAbility(m_currentAbility);
                m_currentAbilityCoroutine = CastAvalance(GetAvalanceCastTime());
                m_rsc.StartCoroutine(m_currentAbilityCoroutine);
            }
            else
            {
                m_rsc.StartCoroutine(WaitForAvalance(0.5f));
            }
        }
    }

    IEnumerator CastAvalance(float castTime)
    {
        yield return new WaitForSeconds(castTime);

        if (!IsDead())
        {
            for (int i = 0; i < m_raid.Count; i++)
            {
                if (!m_raid[i].IsDead())
                    m_raid[i].TakeDamage(GetAvalanceDamage(), "Avalanche");
            }

            m_rsc.StartCoroutine(WaitForAvalance(GetAvalanceWaitTime()));
            m_rsc.EndCastingAbility(m_currentAbility);
            m_currentAbility = null;
        }
    }

    IEnumerator WaitForGroundPound(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (!IsDead())
        {
            if (m_currentAbility == null)
            {
                m_currentAbility = m_encounterAbilities.Find(x => x.Name == "Ground Pound");
                m_rsc.BeginCastingAbility(m_currentAbility);
                m_rsc.StartCoroutine(CastGroundPound(GetGroundPoundCastTime()));
            }
            else
            {
                m_rsc.StartCoroutine(WaitForGroundPound(0.5f));
            }
        }
    }

    IEnumerator CastGroundPound(float castTime)
    {
        yield return new WaitForSeconds(castTime);

        if (!IsDead())
        {
            for (int i = 0; i < m_positionalTargets.Count; i++)
            {
                if (!m_positionalTargets[i].IsDead())
                    m_positionalTargets[i].TakeDamage(GetGroundPoundDamage(), "Ground Pound");
            }

            m_rsc.StartCoroutine(WaitForGroundPound(GetGroundPoundWaitTime()));
            m_rsc.EndCastingAbility(m_currentAbility);
            m_currentAbility = null;
        }
    }
}
