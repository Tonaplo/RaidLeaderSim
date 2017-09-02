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

    IEnumerator m_currentAbilityCoroutine;

    public MoAVampiricus() : base("Vampiricus", 30000) { }

    public override void BeginEncounter()
    {
        bool hasHitTank = false;
        for (int i = 0; i < m_raid.Count; i++)
        {
            if (m_raid[i].Raider.RaiderStats.GetRole() == Enums.CharacterRole.Tank && !hasHitTank)
            {
                hasHitTank = true;
                m_currentTarget = m_raid[i];
                m_rsc.StartCoroutine(DoTankAttack(Utility.GetFussyCastTime(GetBleedingBiteCastTime()), m_raid[i]));
            }
        }

        if (!hasHitTank)
        {
            m_currentTarget = m_raid[0];
            m_rsc.StartCoroutine(DoTankAttack(Utility.GetFussyCastTime(GetBleedingBiteCastTime()), m_raid[0]));
        }

        m_rsc.StartCoroutine(WaitForVampiricBite(GetVampiricBiteWaitTime()));
    }

    public override void SetupLoot()
    {
        GenerateLoot(15, 5);
    }

    public override void SetupDescription()
    {
        m_attacks = new List<EncounterAttackDescription> {
            new EncounterAttackDescription(new List<Enums.CharacterRole>{ Enums.CharacterRole.Tank}, "Bleeding Bite", Name + " sinks his teeth into his target, dealing " + GetBleedingBiteDamage() + " damage and causing the target to bleed for " + GetBleedingBiteDoTDamage() + " every second. Every bite on the same target increases the bleed damage."),
        };
    }

    public override void SetupAbilities()
    {
        m_encounterAbilities = new List<EncounterAbility> {
            new EncounterAbility("Vampiric Bite", "Every " + GetVampiricBiteWaitTime() + " seconds, " + Name + " rears up for a bite of his current target, dealing " + GetVampiricBiteDamage() + " and healing for " + Utility.GetPercentString(GetVampiricBiteHealAmount()) +" of his maximum health if successful.", GetVampiricBiteCastTime(),Enums.Ability.Interrupt, null ),
        };
    }

    public override void CurrentAbilityCountered()
    {
        m_rsc.StopCoroutine(m_currentAbilityCoroutine);
        m_rsc.StartCoroutine(WaitForVampiricBite(GetVampiricBiteWaitTime()));
        HandleAbilityTypeCountered(m_currentAbility.Ability);
        m_currentAbility = null;
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
                return 50 * m_counter;
            case Enums.Difficulties.Normal:
            default:
                return 75 * m_counter;
            case Enums.Difficulties.Hard:
                return 100 * m_counter;
        }
    }

    int GetBleedingBiteDoTDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 10 * m_counter;
            case Enums.Difficulties.Normal:
            default:
                return 25 * m_counter;
            case Enums.Difficulties.Hard:
                return 40 * m_counter;
        }
    }

    float GetBleedingBiteCastTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 5.0f;
            case Enums.Difficulties.Normal:
            default:
                return 4.0f;
            case Enums.Difficulties.Hard:
                return 3.0f;
        }
    }

    float GetBleedingBiteTickLength()
    {
        return GetBleedingBiteCastTime() / GetNumBleedingBiteTicks();
    }

    int GetNumBleedingBiteTicks()
    {
        return 5;
    }

    IEnumerator DoTankAttack(float castTime, RaiderScript target)
    {
        yield return new WaitForSeconds(castTime);

        if (!target.IsDead() && !IsDead())
        {
            if (m_currentTarget == target)
            {
                target.TakeDamage(GetBleedingBiteDamage());
                m_counter++;
            }
            else
            {
                m_counter = 1;
                target.TakeDamage(GetBleedingBiteDamage());
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

            if (m_currentTarget == null)
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

        m_rsc.StartCoroutine(DoTankAttack(Utility.GetFussyCastTime(GetBleedingBiteCastTime()), m_currentTarget));
        m_rsc.StartCoroutine(BleedingBiteDoTDamage(Utility.GetFussyCastTime(GetBleedingBiteTickLength()), GetBleedingBiteDoTDamage(), GetNumBleedingBiteTicks(), m_currentTarget));
    }

    IEnumerator BleedingBiteDoTDamage(float castTime, int damage, int ticks, RaiderScript target)
    {
        yield return new WaitForSeconds(castTime);

        target.TakeDamage(damage);
        if (!IsDead() && !target.IsDead() && ticks > 0)
        {
            ticks--;
            m_rsc.StartCoroutine(BleedingBiteDoTDamage(castTime, damage, ticks, target));
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
            m_rsc.EndCastingAbility(m_currentAbility);
        }
    }
}
