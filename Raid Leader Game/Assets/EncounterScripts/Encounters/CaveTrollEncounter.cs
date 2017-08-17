using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CaveTrollEncounter : BaseEncounter
{
    /*
     For this encounter, we are expecting the following for Normal Difficulty:

             Tanks: 2
             Healers: 3
             DPS: 7
             Average ItemLevel: 10
     */

        //This encounter is not at all done
    int m_ClubSwingNumHits = 6;
    float m_ClubSwingHitIncrease = 1.1f;
    float m_ClubSwingCastTime = 2.5f;
    
    float m_PebbleThrowCastTime = 1.5f;
    float m_AvalanceCastTime = 2.5f;
    

    IEnumerator m_currentAbilityCoroutine;

    public CaveTrollEncounter() : base("Cave Troll", 40000) { }

    public override void BeginEncounter()
    {
        List<RaiderScript> pebbletargets = GetRandomRaidTargets(GetPebbleThrowTargetCount());
        bool hasHitTank = false;
        for (int i = 0; i < m_raid.Count; i++)
        {
            if(pebbletargets.Contains(m_raid[i]))
                m_rsc.StartCoroutine(DoBasicAttack(Utility.GetFussyCastTime(m_PebbleThrowCastTime), (int)(GetPebbleThrowDamage() * Random.value), m_raid[i]));

            if (m_raid[i].Raider.RaiderStats.GetRole() == Enums.CharacterRole.Tank && !hasHitTank)
            {
                hasHitTank = true;
                m_rsc.StartCoroutine(DoTankAttack(Utility.GetFussyCastTime(m_ClubSwingCastTime), GetClubSwingDamage(), m_ClubSwingNumHits, m_raid[i]));
            }
        }

        if (!hasHitTank)
        {
            m_rsc.StartCoroutine(DoTankAttack(Utility.GetFussyCastTime(m_ClubSwingCastTime), GetClubSwingDamage(), m_ClubSwingNumHits, m_raid[0]));
        }

        m_rsc.StartCoroutine(WaitForAvalance(GetAvalanceWaitTime()));
    }

    public override void SetupLoot()
    {
        int itemLevel = 15;
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                itemLevel = 10;
                break;
            case Enums.Difficulties.Normal:
            default:
                break;
            case Enums.Difficulties.Hard:
                itemLevel = 20;
                break;
        }

        m_loot = new List<CharacterItem> {
            new CharacterItem(itemLevel),
            new CharacterItem(itemLevel),
            new CharacterItem(itemLevel),
            new CharacterItem(itemLevel),
            new CharacterItem(itemLevel)
        };
    }

    public override void SetupDescription()
    {
        m_attacks = new List<EncounterAttackDescription> {
            new EncounterAttackDescription(new List<Enums.CharacterRole>{ Enums.CharacterRole.Tank}, "Club Swing", "The cave troll swings his mighty club at his target, dealing " + GetClubSwingDamage() + " damage " + m_ClubSwingNumHits +" times, each hit increasing in damage by " + Utility.GetPercentIncreaseString(m_ClubSwingHitIncrease) +", then switches target."),
            new EncounterAttackDescription(new List<Enums.CharacterRole>{ Enums.CharacterRole.Tank, Enums.CharacterRole.Healer, Enums.CharacterRole.MeleeDPS, Enums.CharacterRole.RangedDPS}, "Pebble Slide", "Pebbles from the Cave randomly fall down, hitting "+ GetPebbleThrowTargetCount() + " random raid members for up to " + GetPebbleThrowDamage() + " damage."),
        };
    }

    public override void SetupAbilities()
    {
        m_encounterAbilities = new List<EncounterAbility> {
            new EncounterAbility("Avalance", "Every " + GetAvalanceWaitTime() + " seconds, the Troll bashes the wall of the cave, causing an Avalanche, dealing " + GetAvalanceDamage() + " to all raid members.", m_AvalanceCastTime,Enums.Ability.Interrupt, null ),
        };
    }

    public override void CurrentAbilityCountered()
    {
        m_rsc.StopCoroutine(m_currentAbilityCoroutine);
        m_rsc.StartCoroutine(WaitForAvalance(GetAvalanceWaitTime()));
        m_rsc.EndCastingAbility();
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
                return 1;
            case Enums.Difficulties.Normal:
            default:
                return 2;
            case Enums.Difficulties.Hard:
                return 4;
        }
    }

    int GetAvalanceDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 30;
            case Enums.Difficulties.Normal:
            default:
                return 60;
            case Enums.Difficulties.Hard:
                return 2000; // Basically, if you miss an interrupt, your raid dies.
        }
    }

    float GetAvalanceWaitTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 30.0f;
            case Enums.Difficulties.Normal:
            default:
                return 20.0f;
            case Enums.Difficulties.Hard:
                return 10.0f;
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

    IEnumerator DoTankAttack(float castTime, int damage, int counter, RaiderScript target)
    {
        yield return new WaitForSeconds(castTime);

        if (!target.IsDead() && !IsDead())
        {
            target.TakeDamage(damage);
            if (counter > 0)
            {
                counter--;
                m_rsc.StartCoroutine(DoTankAttack(Utility.GetFussyCastTime(m_ClubSwingCastTime), (int)(damage * m_ClubSwingHitIncrease), counter, target));
            }
            else
            {
                RaiderScript otherTank = m_rsc.GetRaid().Find(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.Tank && x.Raider.GetName() != target.Raider.GetName());
                if (otherTank && !otherTank.IsDead())
                    m_rsc.StartCoroutine(DoTankAttack(Utility.GetFussyCastTime(m_ClubSwingCastTime), (int)(GetClubSwingDamage()), m_ClubSwingNumHits, otherTank));
                else
                    m_rsc.StartCoroutine(DoTankAttack(Utility.GetFussyCastTime(m_ClubSwingCastTime), (int)(damage * m_ClubSwingHitIncrease), m_ClubSwingNumHits, target));
            }
        }
        else if (target.IsDead())
        {
            RaiderScript otherTank = m_rsc.GetRaid().Find(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.Tank && x.Raider.GetName() != target.Raider.GetName());
            if (otherTank && !otherTank.IsDead())
                m_rsc.StartCoroutine(DoTankAttack(Utility.GetFussyCastTime(m_ClubSwingCastTime), GetClubSwingDamage(), m_ClubSwingNumHits, otherTank));
            else
            {
                for (int i = 0; i < m_raid.Count; i++)
                {
                    if (!m_raid[i].IsDead())
                    {
                        m_rsc.StartCoroutine(DoTankAttack(Utility.GetFussyCastTime(m_ClubSwingCastTime), GetClubSwingDamage(), m_ClubSwingNumHits, m_raid[i]));
                        break;
                    }
                }
            }
        }
    }

    IEnumerator WaitForAvalance(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (!IsDead())
        {
            m_currentAbility = m_encounterAbilities[0];
            m_rsc.BeginCastingAbility(m_currentAbility);
            m_currentAbilityCoroutine = CastAvalance(m_AvalanceCastTime);
            m_rsc.StartCoroutine(m_currentAbilityCoroutine);
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
            m_rsc.EndCastingAbility();
        }
    }
}
