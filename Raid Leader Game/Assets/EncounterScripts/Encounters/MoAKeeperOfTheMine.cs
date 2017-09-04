using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoAKeeperOfTheMine : BaseEncounter
{
    /*
     For this encounter, we are expecting the following for Normal Difficulty:

             Tanks: 2
             Healers: 3
             DPS: 7
             Average ItemLevel: 10
     */

        //This encounter is not at all done
    float m_ClubSwingCastTime = 2.5f;
    
    float m_PebbleThrowCastTime = 1.5f;
    

    IEnumerator m_currentAbilityCoroutine;

    public MoAKeeperOfTheMine() : base("Keeper of the Mine", 30000) { }

    public override void BeginEncounter()
    {
        List<RaiderScript> pebbletargets = GetRandomRaidTargets(GetPebbleThrowTargetCount());
        m_currentTarget = null;
        m_counter = 1;
        for (int i = 0; i < m_raid.Count; i++)
        {
            if(pebbletargets.Contains(m_raid[i]))
                m_rsc.StartCoroutine(DoBasicAttack(Utility.GetFussyCastTime(m_PebbleThrowCastTime), (int)(GetPebbleThrowDamage() * Random.value), m_raid[i]));

            if (m_raid[i].Raider.RaiderStats.GetRole() == Enums.CharacterRole.Tank && m_currentTarget == null)
            {
                m_rsc.StartCoroutine(DoTankAttack(Utility.GetFussyCastTime(m_ClubSwingCastTime), m_raid[i]));
                m_currentTarget = m_raid[i];
            }
        }

        if (m_currentTarget == null)
        {
            m_rsc.StartCoroutine(DoTankAttack(Utility.GetFussyCastTime(m_ClubSwingCastTime), m_raid[0]));
        }

        m_rsc.StartCoroutine(WaitForAvalance(GetAvalanceWaitTime()));
        
        if(m_difficulty == Enums.Difficulties.Hard)
            m_rsc.StartCoroutine(WaitForGroundPound(GetGroundPoundWaitTime() * 1.25f));
    }

    public override void SetupLoot()
    {
        GenerateLoot(15, 5);
    }

    public override void SetupDescription()
    {
        m_attacks = new List<EncounterAttackDescription> {
            new EncounterAttackDescription(new List<Enums.CharacterRole>{ Enums.CharacterRole.Tank}, "Club Swing", "The " + Name + " swings his mighty club at his target, dealing " + GetClubSwingDamage() + ", each hit increasing in damage by " + Utility.GetPercentIncreaseString(GetClubSwingIncrease()) +". Resets on target switch."),
            new EncounterAttackDescription(new List<Enums.CharacterRole>{ Enums.CharacterRole.Tank, Enums.CharacterRole.Healer, Enums.CharacterRole.MeleeDPS, Enums.CharacterRole.RangedDPS}, "Pebble Slide", "Pebbles from the Cave randomly fall down, hitting "+ GetPebbleThrowTargetCount() + " random raid members for up to " + GetPebbleThrowDamage() + " damage."),
        };
    }

    public override void SetupAbilities()
    {
        m_encounterAbilities = new List<EncounterAbility> {
            new EncounterAbility("Avalanche", "Every " + GetAvalanceWaitTime() + " seconds, " + Name + " bashes the wall of the cave, causing an Avalanche, dealing " + GetAvalanceDamage() + " to all raid members.", GetAvalanceCastTime(),Enums.Ability.Interrupt),
        };

        if (m_difficulty == Enums.Difficulties.Hard)
        {
            m_encounterAbilities.Add(new EncounterAbility("Ground Pound", Name + " raises his fists in the air to pound the ground where the raid stands, dealing " + GetGroundPoundDamage() + " to any raid member that does not move in time.", GetGroundPoundCastTime(), Enums.Ability.PreMovePositional));
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
                return 40;
            case Enums.Difficulties.Normal:
            default:
                return 65;
            case Enums.Difficulties.Hard:
                return 90;
        }
    }

    float GetClubSwingIncrease()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 1.05f;
            case Enums.Difficulties.Normal:
            default:
                return 1.15f;
            case Enums.Difficulties.Hard:
                return 1.25f;
        }
    }

    int GetPebbleThrowDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 20;
            case Enums.Difficulties.Normal:
            default:
                return 40;
            case Enums.Difficulties.Hard:
                return 60;
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
                return 40;
            case Enums.Difficulties.Normal:
            default:
                return 70;
            case Enums.Difficulties.Hard:
                return 2000; // Basically, if you miss an interrupt, your raid dies.
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
                return 125; 
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
                target.TakeDamage(damage);

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
            if (m_currentTarget == target)
            {
                int damage = Mathf.RoundToInt(GetClubSwingDamage() * Mathf.Pow(GetClubSwingIncrease(), m_counter));
                target.TakeDamage(damage);
                m_counter++;
            }
            else
            {
                m_counter = 1;
                int damage = Mathf.RoundToInt(GetClubSwingDamage() * Mathf.Pow(GetClubSwingIncrease(), m_counter));
                target.TakeDamage(damage);
            }

        }
        else if (target.IsDead())
        {
            m_currentTarget = null;
            m_counter = 1;
            List<RaiderScript> otherTanks = m_rsc.GetRaid().FindAll(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.Tank && x.Raider.GetName() != target.Raider.GetName());
            for (int i = 0; i < otherTanks.Count; i++)
            {
                if (!otherTanks[i].IsDead())
                {
                    m_currentTarget = otherTanks[i];
                    break;
                }
            }
            
            if(m_currentTarget == null)
            {
                for (int i = 0; i < m_raid.Count; i++)
                {
                    if (!m_raid[i].IsDead())
                    {
                        m_currentTarget = m_raid[i];
                        break;
                    }
                }
            }
        }

        m_rsc.StartCoroutine(DoTankAttack(Utility.GetFussyCastTime(m_ClubSwingCastTime), m_currentTarget));
    }

    IEnumerator WaitForAvalance(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        
        if (!IsDead())
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
                    m_raid[i].TakeDamage(GetAvalanceDamage());
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
                    m_positionalTargets[i].TakeDamage(GetGroundPoundDamage());
            }

            m_rsc.StartCoroutine(WaitForGroundPound(GetGroundPoundWaitTime()));
            m_rsc.EndCastingAbility(m_currentAbility);
            m_currentAbility = null;
        }
    }
}
