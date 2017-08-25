using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoAVampiricus : BaseEncounter
{
    /*
     For this encounter, we are expecting the following for Normal Difficulty:

             Tanks: 2
             Healers: 3
             DPS: 7
             Average ItemLevel: 10

     Abilities:
            - Needs to introduce the concept of adds
            - has an interruptible cast that will heal boss if not dealt with
            - The boss has a phase where he cant be damaged:
                - here is where you need to deal with adds
                - here is where you need to deal with the premove positional skill too
     */

    //This encounter is not at all done
    int m_ClubSwingNumHits = 6;
    float m_ClubSwingHitIncrease = 1.1f;
    float m_ClubSwingCastTime = 2.5f;

    float m_PebbleThrowCastTime = 1.5f;

    IEnumerator m_currentAbilityCoroutine;

    public MoAVampiricus() : base("Vampiricus", 30000) { }

    //Will work more on this encounter as soon as I have the taunting set up.

    /*public override void BeginEncounter()
    {
        List<RaiderScript> pebbletargets = GetRandomRaidTargets(GetPebbleThrowTargetCount());
        bool hasHitTank = false;
        for (int i = 0; i < m_raid.Count; i++)
        {
            if (pebbletargets.Contains(m_raid[i]))
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
        GenerateLoot(15, 5);
    }

    public override void SetupDescription()
    {
        What you wanna do now is the make a taunt menu in the same place where there's a cooldown menu.
        I am also thinking this would be a good place to make a dispel menu there and have that light up or flash when something needs to be dispelled.
        
        m_attacks = new List<EncounterAttackDescription> {
            new EncounterAttackDescription(new List<Enums.CharacterRole>{ Enums.CharacterRole.Tank}, "Bleeding Bite", Name + " sinks his teeth into his target, dealing " + GetBleedingBiteDamage() + " damage and causing the target to bleed for " + GetBleedingBiteDoTDamage() + " every second until a new target is found."),
            new EncounterAttackDescription(new List<Enums.CharacterRole>{ Enums.CharacterRole.Tank, Enums.CharacterRole.Healer, Enums.CharacterRole.MeleeDPS, Enums.CharacterRole.RangedDPS}, "Pebble Slide", "Pebbles from the Cave randomly fall down, hitting "+ GetPebbleThrowTargetCount() + " random raid members for up to " + GetPebbleThrowDamage() + " damage."),
        };
    }

    public override void SetupAbilities()
    {
        m_encounterAbilities = new List<EncounterAbility> {
            new EncounterAbility("Vampiric Bite", "Every " + GetVampiricBiteWaitTime() + " seconds, the " + Name + " rears up for a bite of his current target, dealing " + GetVampiricBiteDamage() + " healing for " + Utility.GetPercentIncreaseString(GetVampiricBiteHealAmount()) +"% of his maximum health if successful.", GetAvalanceCastTime(),Enums.Ability.Interrupt, null ),
        };
    }

    public override void CurrentAbilityCountered()
    {
        m_currentAbility = null;
        m_rsc.StopCoroutine(m_currentAbilityCoroutine);
        m_rsc.StartCoroutine(WaitForAvalance(GetAvalanceWaitTime()));
        HandleAbilityTypeCountered(m_currentAbility.Ability);
    }

    float GetVampiricBiteCastTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 6.0f;
            case Enums.Difficulties.Normal:
            default:
                return 4.0f;
            case Enums.Difficulties.Hard:
                return 2.0f;
        }
    }

    float GetVampiricBiteWaitTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 20.0f;
            case Enums.Difficulties.Normal:
            default:
                return 17.0f;
            case Enums.Difficulties.Hard:
                return 12.0f;
        }
    }

    float GetVampiricBiteHealAmount()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 0.05f;
            case Enums.Difficulties.Normal:
            default:
                return 0.1f;
            case Enums.Difficulties.Hard:
                return 0.25f;
        }
    }

    int GetVampiricBiteDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 100;
            case Enums.Difficulties.Normal:
            default:
                return 200;
            case Enums.Difficulties.Hard:
                return 300;
        }
    }

    int GetBleedingBiteDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 50;
            case Enums.Difficulties.Normal:
            default:
                return 75;
            case Enums.Difficulties.Hard:
                return 100;
        }
    }

    int GetBleedingBiteDoTDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 10;
            case Enums.Difficulties.Normal:
            default:
                return 25;
            case Enums.Difficulties.Hard:
                return 40;
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
            if (newTarget.Count > 0)
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

    IEnumerator WaitForVampiricBite(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (!IsDead())
        {
            if (m_currentAbility == null)
            {
                m_currentAbility = m_encounterAbilities.Find(x => x.Name == "Vampiric Bite");
                m_rsc.BeginCastingAbility(m_currentAbility);
                m_currentAbilityCoroutine = CastVampiricBite(GetVampiricBiteCastTime());
                m_rsc.StartCoroutine(m_currentAbilityCoroutine);
            }
            else
            {
                m_rsc.StartCoroutine(WaitForVampiricBite(0.5f));
            }
        }
    }

    IEnumerator CastVampiricBite(float castTime)
    {
        yield return new WaitForSeconds(castTime);

        if (!IsDead())
        {
            HealthBar.ModifyHealth(HealthBar.HealthBarSlider.maxValue * GetVampiricBiteHealAmount());

            m_currentTarget.TakeDamage(GetVampiricBiteDamage());
            

            m_rsc.StartCoroutine(WaitForVampiricBite(GetVampiricBiteWaitTime()));
            m_rsc.EndCastingAbility();
        }
    }
    */
}
