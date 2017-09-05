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
     */
     
    bool m_vampiricBiteInterrupted = false;

    public MoAVampiricus() : base("Vampiricus", 50000) { }

    public override void BeginEncounter()
    {
        m_counter = 0;
        bool hasHitTank = false;
        for (int i = 0; i < m_raid.Count; i++)
        {
            if (m_raid[i].Raider.RaiderStats.GetRole() == Enums.CharacterRole.Tank && !hasHitTank)
            {
                hasHitTank = true;
                m_currentTarget = m_raid[i];
            }
        }

        if (!hasHitTank)
        {
            m_currentTarget = m_raid[0];
        }


        m_rsc.StartCoroutine(DoTankAttack(0.0f, m_currentTarget));
        m_rsc.StartCoroutine(WaitForVampiricBite(GetVampiricBiteWaitTime()));
        m_rsc.StartCoroutine(WaitForSummonBats(GetBatlingSummonWaitTime()));

        if (m_difficulty == Enums.Difficulties.Hard)
            m_rsc.StartCoroutine(WaitForVenomSpray(GetVenomSprayWaitTime()/2.0f));
    }

    public override void SetupLoot()
    {
        GenerateLoot(15, 5);
    }

    public override void SetupDescription()
    {
        m_attacks = new List<EncounterAttackDescription> {
            new EncounterAttackDescription(new List<Enums.CharacterRole>{ Enums.CharacterRole.Tank}, "Bleeding Bite", Name + " sinks his teeth into his target, dealing " + GetBleedingBiteDamage() + " damage and causing the target to bleed for " + GetBleedingBiteDoTDamage() + " damage every second. Every bite on the same target increases the bleed damage."),
        };
    }

    public override void SetupAbilities()
    {
        m_encounterAbilities = new List<EncounterAbility> {
            new EncounterAbility("Vampiric Bite", Name, "Every " + GetVampiricBiteWaitTime() + " seconds, " + Name + " rears up for a bite of a random target, dealing " + GetVampiricBiteDamage() + " damage and healing for " + Utility.GetPercentString(GetVampiricBiteHealAmount()) +" of his maximum health if successful.", GetVampiricBiteCastTime(),Enums.Ability.Immune ),
            new EncounterAbility("Summon Batling", Name, "Every " + GetBatlingSummonWaitTime() + " seconds, "  + Name + " summons Batlings with " + GetBatlingHealth() + " health. The Batlings use the Screech ability.", GetBatlingSummonTime(), Enums.Ability.Uncounterable ),
            new EncounterAbility("Screech", "Batling", "Batlings summoned by " + Name + " will constantly screech, dealing " + GetScreechDamage() + " damage to the raid until defeated.", GetScreechCastTime(),Enums.Ability.Uncounterable),
        };

        if (m_difficulty == Enums.Difficulties.Hard)
        {
            m_encounterAbilities.Add(new EncounterAbility("Venom Spray", Name, "Every " + GetVenomSprayWaitTime() + " seconds, " + Name + " will spray venom on " + GetNumVenomSprayTargets() + " raiders, causing them to suffer " + GetVenomSprayDoTDamage() + " damage every second until dispelled.", GetVenomSprayCastTime(), Enums.Ability.Dispel));
        }
    }

    public override void CurrentAbilityCountered()
    {
        HandleAbilityTypeCountered(m_currentAbility.Ability);
        m_vampiricBiteInterrupted = true;
    }

    public override int TakeDamage(int damage)
    {
        if (m_adds.Count > 0) {
            int previousHealth = (int)m_adds[0].Healthbar.CurrentHealth;
            m_adds[0].Healthbar.ModifyHealth(-damage);
            int actualdamage = previousHealth - (int)m_adds[0].Healthbar.CurrentHealth;

            if (m_adds[0].Healthbar.IsDead())
            {
                m_rsc.EndCastingAbility(m_currentAbility);
                m_adds[0].DestroyHealthBar();
                m_adds.RemoveAt(0);
                m_currentAbility = null;
            }

            return actualdamage;
        }
        else
        {
            int previousHealth = (int)HealthBar.CurrentHealth;
            HealthBar.ModifyHealth(-damage);
            return previousHealth - (int)HealthBar.CurrentHealth;
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
                return 150;
            case Enums.Difficulties.Hard:
                return 200;
        }
    }

    int GetBleedingBiteDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 50 * (m_counter + 1);
            case Enums.Difficulties.Normal:
            default:
                return 75 * (m_counter + 1);
            case Enums.Difficulties.Hard:
                return 100 * (m_counter + 1);
        }
    }

    int GetBleedingBiteDoTDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 4 * (m_counter + 1);
            case Enums.Difficulties.Normal:
            default:
                return 8 * (m_counter + 1);
            case Enums.Difficulties.Hard:
                return 16 * (m_counter + 1);
        }
    }

    float GetBleedingBiteCastTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 4.0f;
            case Enums.Difficulties.Normal:
            default:
                return 3.5f;
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
                return 1500;
            case Enums.Difficulties.Normal:
            default:
                return 2000;
            case Enums.Difficulties.Hard:
                return 3000;
        }
    }

    float GetBatlingSummonWaitTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 25.0f;
            case Enums.Difficulties.Normal:
            default:
                return 23.0f;
            case Enums.Difficulties.Hard:
                return 20f;
        }
    }

    float GetBatlingSummonTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 3.0f;
            case Enums.Difficulties.Normal:
            default:
                return 3.0f;
            case Enums.Difficulties.Hard:
                return 3.0f;
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
                return 3.0f;
            case Enums.Difficulties.Hard:
                return 3.0f;
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
                return 70;
            case Enums.Difficulties.Hard:
                return 90;
        }
    }

    float GetVenomSprayWaitTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
            case Enums.Difficulties.Normal:
            default:
                return 0.0f; // This should only be active on Hard
            case Enums.Difficulties.Hard:
                return 20.0f;
        }
    }

    float GetVenomSprayCastTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
            case Enums.Difficulties.Normal:
            default:
                return 0.0f; // This should only be active on Hard
            case Enums.Difficulties.Hard:
                return 2.0f;
        }
    }

    int GetNumVenomSprayTargets()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
            case Enums.Difficulties.Normal:
            default:
                return 0; // This should only be active on Hard
            case Enums.Difficulties.Hard:
                return 2;
        }
    }

    int GetVenomSprayDoTDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
            case Enums.Difficulties.Normal:
            default:
                return 0; // This should only be active on Hard
            case Enums.Difficulties.Hard:
                return 30;
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
                m_counter = 0;
                target.TakeDamage(GetBleedingBiteDamage());
            }

        }
        else if (target.IsDead())
        {
            m_currentTarget = null;
            m_counter = 0;
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

        if (!m_rsc.IsRaidDead() && !IsDead())
        {
            m_rsc.TankAbilityUsed();
            m_rsc.StartCoroutine(DoTankAttack(Utility.GetFussyCastTime(GetBleedingBiteCastTime()), m_currentTarget));
            m_rsc.StartCoroutine(BleedingBiteDoTDamage(Utility.GetFussyCastTime(GetBleedingBiteTickLength()), GetBleedingBiteDoTDamage(), GetNumBleedingBiteTicks(), m_currentTarget));
        }
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
                m_rsc.StartCoroutine(CastVampiricBite(GetVampiricBiteCastTime()));
                m_vampiricBiteInterrupted = false;
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
            m_rsc.EndCastingAbility(m_currentAbility);
            if (!m_vampiricBiteInterrupted)
            {
                HealthBar.ModifyHealth(HealthBar.MaxHealth * GetVampiricBiteHealAmount());

                List<RaiderScript> randomTarget = GetRandomRaidTargets(1);

                if (randomTarget.Count > 0)
                    randomTarget[0].TakeDamage(GetVampiricBiteDamage());
            }

            m_rsc.StartCoroutine(WaitForVampiricBite(GetVampiricBiteWaitTime()));
            m_currentAbility = null;
        }

        m_vampiricBiteInterrupted = false;
    }

    IEnumerator WaitForSummonBats(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (!m_rsc.IsRaidDead() && !IsDead())
        { 

            if (m_currentAbility == null)
            {
                m_currentAbility = m_encounterAbilities.Find(x => x.Name == "Summon Batling");
                m_rsc.StartCoroutine(SummonBats(GetBatlingSummonTime()));
                m_rsc.BeginCastingAbility(m_currentAbility);
            }
            else
            {
                m_rsc.StartCoroutine(WaitForSummonBats(0.5f));
            }
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

            if (index != -1)
            {
                GameObject batlingGO = GameObject.Instantiate(m_healthBarPrefab);
                batlingGO.name = "Batling " + index;
                batlingGO.transform.SetParent(m_healthBarPrefab.transform);
                m_adds.Add(new EncounterAdds("Batling", batlingGO.GetComponent<HealthBarScript>(), GetBatlingHealth(), index, Enums.EncounterAdds.SingleAdd));

                m_currentAbility = m_encounterAbilities.Find(x => x.Name == "Screech");
                m_rsc.StartCoroutine(CastScreech(GetScreechCastTime()));
                m_rsc.BeginCastingAbility(m_currentAbility);
            }

            m_rsc.StartCoroutine(WaitForSummonBats(GetBatlingSummonWaitTime()));
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
            
            m_rsc.StartCoroutine(CastScreech(GetScreechCastTime()));
            m_currentAbility = m_encounterAbilities.Find(x => x.Name == "Screech");
            m_rsc.BeginCastingAbility(m_currentAbility);
        }
        else
        {
            if (m_currentAbility != null && m_currentAbility.Name == "Screech")
            {
                m_rsc.EndCastingAbility(m_currentAbility);
                m_currentAbility = null;
            }
        }
    }

    IEnumerator WaitForVenomSpray(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (!m_rsc.IsRaidDead() && !IsDead())
        {

            if (m_currentAbility == null)
            {
                m_currentAbility = m_encounterAbilities.Find(x => x.Name == "Venom Spray");
                m_rsc.StartCoroutine(CastVenomSpray(GetVenomSprayCastTime()));
                m_rsc.BeginCastingAbility(m_currentAbility);
            }
            else
            {
                m_rsc.StartCoroutine(WaitForVenomSpray(0.5f));
            }
        }

    }

    IEnumerator CastVenomSpray(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        
        if (!m_rsc.IsRaidDead() && !IsDead())
        {
            List<RaiderScript> debuffTargets = GetRandomRaidTargets(GetNumVenomSprayTargets());

            for (int i = 0; i < debuffTargets.Count; i++)
            {
                GameObject temp = new GameObject();
                temp.AddComponent<RaiderDebuff>().Initialize(debuffTargets[i], GetVenomSprayDoTDamage());
                debuffTargets[i].AddDebuff(temp);
            }

            m_rsc.DebuffsAdded();

            if(m_currentAbility != null && m_currentAbility.Name == "Venom Spray")
                m_currentAbility = null;

            m_rsc.StartCoroutine(WaitForVenomSpray(GetVenomSprayWaitTime()));
        }
    }
}
