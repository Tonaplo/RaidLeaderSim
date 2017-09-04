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

    public MoAVampiricus() : base("Vampiricus", 50000) { }

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
        m_rsc.StartCoroutine(SummonBats(GetBatlingSummonTime()));
    }

    public override void SetupLoot()
    {
        GenerateLoot(15, 5);
    }

    public override void SetupDescription()
    {
        m_attacks = new List<EncounterAttackDescription> {
            new EncounterAttackDescription(new List<Enums.CharacterRole>{ Enums.CharacterRole.Tank}, "Bleeding Bite", Name + " sinks his teeth into his target, dealing " + GetBleedingBiteDamage() + " damage and causing the target to bleed for " + GetBleedingBiteDoTDamage() + " every second. Every bite on the same target increases the bleed damage."),
            new EncounterAttackDescription(new List<Enums.CharacterRole>{ }, "Summon Batling", "Every " + GetBatlingSummonTime() + " seconds, "  + Name + " summons Batlings with " + GetBatlingHealth() + " health. The Batlings use the Screech ability."),
        };
    }

    public override void SetupAbilities()
    {
        m_encounterAbilities = new List<EncounterAbility> {
            new EncounterAbility("Vampiric Bite", "Every " + GetVampiricBiteWaitTime() + " seconds, " + Name + " rears up for a bite of a random target, dealing " + GetVampiricBiteDamage() + " and healing for " + Utility.GetPercentString(GetVampiricBiteHealAmount()) +" of his maximum health if successful.", GetVampiricBiteCastTime(),Enums.Ability.Immune ),
            new EncounterAbility("Screech", "The Batlings summon by " + Name + " will constantly screech, dealing " + GetScreechDamage() + " to the raid until defeated.", GetScreechCastTime(),Enums.Ability.Uncounterable),
        };
    }

    public override void CurrentAbilityCountered()
    {
        m_rsc.StopCoroutine(m_currentAbilityCoroutine);
        m_rsc.StartCoroutine(WaitForVampiricBite(GetVampiricBiteWaitTime()));
        HandleAbilityTypeCountered(m_currentAbility.Ability);
        m_currentAbility = null;
    }

    public override int TakeDamage(int damage)
    {
        if (m_adds.Count > 0) {
            int previousHealth = (int)m_adds[0].Healthbar.HealthBarSlider.value;
            m_adds[0].Healthbar.ModifyHealth(-damage);
            int actualdamage = previousHealth - (int)m_adds[0].Healthbar.HealthBarSlider.value;

            if (m_adds[0].Healthbar.HealthBarSlider.value <= 0)
            {
                m_adds[0].DestroyHealthBar();
                m_adds.RemoveAt(0);
                m_rsc.EndCastingAbility(m_currentAbility);
                m_currentAbility = null;
            }

            return actualdamage;
        }
        else
        {
            int previousHealth = (int)HealthBar.HealthBarSlider.value;
            HealthBar.ModifyHealth(-damage);
            return previousHealth - (int)HealthBar.HealthBarSlider.value;
        }
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
                return 15.0f;
            case Enums.Difficulties.Normal:
            default:
                return 12.0f;
            case Enums.Difficulties.Hard:
                return 9.0f;
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
                return 15 * m_counter;
            case Enums.Difficulties.Hard:
                return 25 * m_counter;
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

    int GetBatlingHealth()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 2000;
            case Enums.Difficulties.Normal:
            default:
                return 3500;
            case Enums.Difficulties.Hard:
                return 5000;
        }
    }

    float GetBatlingSummonTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 20.0f;
            case Enums.Difficulties.Normal:
            default:
                return 18f;
            case Enums.Difficulties.Hard:
                return 15f;
        }
    }

    float GetScreechCastTime()
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

    int GetScreechDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 50;
            case Enums.Difficulties.Normal:
            default:
                return 75;
            case Enums.Difficulties.Hard:
                return 90;
        }
    }
    
    IEnumerator DoTankAttack(float castTime, RaiderScript target)
    {
        yield return new WaitForSeconds(castTime);

        if (!m_rsc.IsRaidDead() && !target.IsDead() && !IsDead())
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
        else if (!m_rsc.IsRaidDead() && target.IsDead())
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
        if (!m_rsc.IsRaidDead() && !IsDead() && !target.IsDead() && ticks > 0)
        {
            ticks--;
            m_rsc.StartCoroutine(BleedingBiteDoTDamage(castTime, damage, ticks, target));
        }
    }

    IEnumerator WaitForVampiricBite(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (!m_rsc.IsRaidDead() && !IsDead())
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

        if (!IsDead() && !m_rsc.IsRaidDead())
        {
            HealthBar.ModifyHealth(HealthBar.HealthBarSlider.maxValue * GetVampiricBiteHealAmount());

            List<RaiderScript> randomTarget = GetRandomRaidTargets(1);

            if(randomTarget.Count > 0)
                randomTarget[0].TakeDamage(GetVampiricBiteDamage());
            
            m_rsc.StartCoroutine(WaitForVampiricBite(GetVampiricBiteWaitTime()));
            m_rsc.EndCastingAbility(m_currentAbility);
            m_currentAbility = null;
        }
    }
    
    IEnumerator SummonBats(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (!m_rsc.IsRaidDead() && !IsDead())
        {
            int index = -1;
            for (int i = 0; i < StaticValues.MaxNumberOfAliveAdds; i++)
            {
                if (m_adds.FindAll(x => x.Index == i).Count == 0)
                {
                    index = i;
                    break;
                }
            }

            if (m_currentAbility == null && index != -1)
            {
                GameObject batlingGO = GameObject.Instantiate(m_healthBarPrefab);
                batlingGO.name = "Batling " + index;
                batlingGO.transform.SetParent(m_healthBarPrefab.transform);
                m_adds.Add(new EncounterAdds("Batling", batlingGO.GetComponent<HealthBarScript>(), GetBatlingHealth(), index, Enums.EncounterAdds.SingleAdd));

                m_currentAbility = m_encounterAbilities.Find(x => x.Name == "Screech");
                m_currentAbilityCoroutine = CastScreech(GetScreechCastTime());
                m_rsc.StartCoroutine(m_currentAbilityCoroutine);
                m_rsc.BeginCastingAbility(m_currentAbility);
                m_rsc.StartCoroutine(SummonBats(GetBatlingSummonTime()));
            }
            else
            {
                m_rsc.StartCoroutine(SummonBats(0.5f));
            }
        }
    }
    
    IEnumerator CastScreech(float castTime)
    {
        yield return new WaitForSeconds(castTime);

        if (!m_rsc.IsRaidDead() && !IsDead() && m_adds.Count > 0)
        {
            for (int i = 0; i < m_raid.Count; i++)
            {
                m_raid[i].TakeDamage(GetScreechDamage());
            }

            m_rsc.EndCastingAbility(m_currentAbility);
            m_currentAbilityCoroutine = CastScreech(GetScreechCastTime());
            m_rsc.StartCoroutine(m_currentAbilityCoroutine);
            m_currentAbility = m_encounterAbilities.Find(x => x.Name == "Screech");
            m_rsc.BeginCastingAbility(m_currentAbility);
        }
        else
        {
            m_currentAbility = null;
        }
    }
}
