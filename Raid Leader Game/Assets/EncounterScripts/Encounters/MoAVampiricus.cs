using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoAVampiricus : BaseEncounter
{     
    bool m_vampiricBiteInterrupted = false;
    EncounterEnemy m_Vampiricus;
    string ConsortString = "Consort of the Queen";
    string SummonConsortString = "Summon Consort of the Queen";
    string ScreechString = "Screech";
    string SwarmOfYounglingsString = "Swarm of Younglings";
    string VenomSprayString = "Venom Spray";

    public MoAVampiricus() : base("Vampiricus") { m_encounterEnum = Enums.EncounterEnum.MoAVampiricus; }

    public override void SetupEncounter()
    {
        CreateEnemy(m_name, Mathf.RoundToInt(45000 * GetDifficultyMultiplier()), Enums.EncounterEnemyType.Boss);
        m_Vampiricus = m_enemies[0];
    }

    public override void BeginEncounter()
    {
        m_counter = 0;
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
        m_rsc.StartCoroutine(CastSwarmOfYounglings(0.5f));
        m_rsc.StartCoroutine(WaitForVampiricBite(GetVampiricBiteWaitTime()));
        m_rsc.StartCoroutine(WaitForSummonConsorts(GetConsortSummonWaitTime()));

        if (m_difficulty == Enums.Difficulties.Hard)
            m_rsc.StartCoroutine(WaitForVenomSpray(GetVenomSprayWaitTime()/2.0f));
    }

    public override void SetupLoot()
    {
        GenerateLoot(50, 5);
    }

    public override void SetupDescription()
    {
        m_description = "In the depths of the Mine, the Vampire Bat Queen lurks. Her and her flok are drawn by the blood in your veins - and seek to drain it for themselves.";

        m_attacks = new List<EncounterAttackDescription> {
            new EncounterAttackDescription(new List<Enums.CharacterRole>{ Enums.CharacterRole.Tank}, "Bleeding Bite", Name + " sinks her teeth into her target, dealing " + GetBleedingBiteDamage() + " damage and causing the target to bleed for " + GetBleedingBiteDoTDamage() * GetNumBleedingBiteTicks() + " damage over " + GetBleedingBiteTickLength()*GetNumBleedingBiteTicks() + " seconds. Every bite on the same target increases the bleed damage."),
            new EncounterAttackDescription(new List<Enums.CharacterRole>(), SwarmOfYounglingsString, "The younger bats come to the aid of their queen, biting random raidmembers for " + GetSwarmOfYounglingsDamage() + " damage. Their incredible numbers translate to two attacks every second."),
        };
    }

    public override void SetupAbilities()
    {
        m_encounterAbilities = new List<EncounterAbility> {
            new EncounterAbility("Vampiric Bite", Name, "Every " + GetVampiricBiteWaitTime() + " seconds, " + Name + " rears up for a bite of a random target, dealing " + GetVampiricBiteDamage() + " damage and healing for " + Utility.GetPercentString(GetVampiricBiteHealAmount()) +" of her maximum health if successful.", GetVampiricBiteCastTime(),Enums.Ability.Immune, Enums.AbilityCastType.Cast),
            new EncounterAbility(SummonConsortString, Name, "Every " + GetConsortSummonWaitTime() + " seconds, "  + Name + " summons her Consorts with " + GetConsortHealth() + " health. The Consorts use the Screech ability.", GetConsortSummonTime(), Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast),
            new EncounterAbility(ScreechString, ConsortString, "Consorts summoned by " + Name + " will constantly screech, dealing " + GetScreechDamage() + " damage to the raid every second until defeated.", GetScreechCastTime(),Enums.Ability.Uncounterable, Enums.AbilityCastType.Channel),
        };

        if (m_difficulty == Enums.Difficulties.Hard)
        {
            m_encounterAbilities.Add(new EncounterAbility(VenomSprayString, Name, "Every " + GetVenomSprayWaitTime() + " seconds, " + Name + " will spray venom on " + GetNumVenomSprayTargets() + " raiders, causing them to suffer " + GetVenomSprayDoTDamage() + " damage every second until dispelled.", GetVenomSprayCastTime(), Enums.Ability.Dispel, Enums.AbilityCastType.Channel));
        }
    }

    public override void CurrentAbilityCountered()
    {
        HandleAbilityTypeCountered(m_currentAbility.Ability);
        m_vampiricBiteInterrupted = true;
    }

    public override void HandleOnTakeDamageEvent(int damage, RaiderScript attacker)
    {
        if (m_enemies.FindAll(e => e.Name == ConsortString).Count == 0)
        {
            if (m_currentAbility != null && m_currentAbility.Name == ScreechString)
            {
                m_rsc.EndCastingAbility(m_currentAbility);
                m_currentAbility = null;
            }
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
                return 225;
            case Enums.Difficulties.Normal:
            default:
                return 275;
            case Enums.Difficulties.Hard:
                return 400;
        }
    }

    int GetBleedingBiteDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 25 * (m_counter + 1);
            case Enums.Difficulties.Normal:
            default:
                return 60 * (m_counter + 1);
            case Enums.Difficulties.Hard:
                return 100 * (m_counter + 1);
        }
    }

    int GetBleedingBiteDoTDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 10 * (m_counter + 1);
            case Enums.Difficulties.Normal:
            default:
                return 30 * (m_counter + 1);
            case Enums.Difficulties.Hard:
                return 50 * (m_counter + 1);
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

    int GetSwarmOfYounglingsDamage()
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

    int GetConsortHealth()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 3000;
            case Enums.Difficulties.Normal:
            default:
                return 4000;
            case Enums.Difficulties.Hard:
                return 5000;
        }
    }

    float GetConsortSummonWaitTime()
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

    float GetConsortSummonTime()
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
                return 4.0f;
            case Enums.Difficulties.Normal:
            default:
                return 4.0f;
            case Enums.Difficulties.Hard:
                return 4.0f;
        }
    }

    int GetScreechDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 15;
            case Enums.Difficulties.Normal:
            default:
                return 25;
            case Enums.Difficulties.Hard:
                return 40;
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
                return 75;
        }
    }

    IEnumerator DoTankAttack(float castTime, RaiderScript target)
    {
        yield return new WaitForSeconds(castTime);

        if (!m_rsc.IsRaidDead() && !target.IsDead() && !IsDead())
        {
            if (m_currentRaiderTarget == target)
            {
                target.TakeDamage(GetBleedingBiteDamage(), "Bleeding Bite");
                m_counter++;
            }
            else
            {
                m_counter = 0;
                target.TakeDamage(GetBleedingBiteDamage(), "Bleeding Bite");
            }

        }
        else if (target.IsDead())
        {
            m_currentRaiderTarget = null;
            m_counter = 0;
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
            m_rsc.TankAbilityUsed();
            m_rsc.StartCoroutine(DoTankAttack(Utility.GetFussyCastTime(GetBleedingBiteCastTime()), m_currentRaiderTarget));
            m_rsc.StartCoroutine(BleedingBiteDoTDamage(Utility.GetFussyCastTime(GetBleedingBiteTickLength()), GetBleedingBiteDoTDamage(), GetNumBleedingBiteTicks(), m_currentRaiderTarget));
        }
    }

    IEnumerator BleedingBiteDoTDamage(float castTime, int damage, int ticks, RaiderScript target)
    {
        yield return new WaitForSeconds(castTime);

        target.TakeDamage(damage, "Bleeding Bite");
        if (!m_rsc.IsRaidDead() && !IsDead() && !target.IsDead() && ticks > 0)
        {
            ticks--;
            m_rsc.StartCoroutine(BleedingBiteDoTDamage(castTime, damage, ticks, target));
        }
    }

    IEnumerator CastSwarmOfYounglings(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (!m_rsc.IsRaidDead() && !IsDead())
        {
            GetRandomRaidTargets(1)[0].TakeDamage(GetSwarmOfYounglingsDamage(), SwarmOfYounglingsString);
            m_rsc.StartCoroutine(CastSwarmOfYounglings(0.5f));
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
                m_Vampiricus.Healthbar.ModifyHealth(m_Vampiricus.Healthbar.MaxHealth * GetVampiricBiteHealAmount());
                
                List<RaiderScript> randomTarget = GetRandomRaidTargets(1);

                if (randomTarget.Count > 0)
                    randomTarget[0].TakeDamage(GetVampiricBiteDamage(), "Vampiric Bite");
            }

            m_rsc.StartCoroutine(WaitForVampiricBite(GetVampiricBiteWaitTime()));
            m_currentAbility = null;
        }

        m_vampiricBiteInterrupted = false;
    }

    IEnumerator WaitForSummonConsorts(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (!m_rsc.IsRaidDead() && !IsDead())
        { 

            if (m_currentAbility == null)
            {
                m_currentAbility = m_encounterAbilities.Find(x => x.Name == SummonConsortString);
                m_rsc.StartCoroutine(SummonConsorts(GetConsortSummonTime()));
                m_rsc.BeginCastingAbility(m_currentAbility);
            }
            else
            {
                m_rsc.StartCoroutine(WaitForSummonConsorts(0.5f));
            }
        }

    }

    IEnumerator SummonConsorts(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (!m_rsc.IsRaidDead() && !IsDead())
        {
            if (CreateEnemy(ConsortString, GetConsortHealth(), Enums.EncounterEnemyType.Add)) { 
                m_currentAbility = m_encounterAbilities.Find(x => x.Name == ScreechString);
                m_rsc.StartCoroutine(CastScreech(GetScreechCastTime()));
                m_rsc.StartCoroutine(DealScreechDamage(1.0f));
                m_rsc.BeginCastingAbility(m_currentAbility);
            }

            m_rsc.StartCoroutine(WaitForSummonConsorts(GetConsortSummonWaitTime()));
        }
    }
    
    IEnumerator CastScreech(float castTime)
    {
        yield return new WaitForSeconds(castTime);

        if (!m_rsc.IsRaidDead() && !IsDead() && m_enemies.FindAll(x => x.Name == ConsortString).Count > 0)
        {
            m_rsc.StartCoroutine(CastScreech(GetScreechCastTime()));
            m_currentAbility = m_encounterAbilities.Find(x => x.Name == ScreechString);
            m_rsc.BeginCastingAbility(m_currentAbility);
        }
        else
        {
            if (m_currentAbility != null && m_currentAbility.Name == ScreechString)
            {
                m_rsc.EndCastingAbility(m_currentAbility);
                m_currentAbility = null;
            }
        }
    }

    IEnumerator DealScreechDamage(float castTime)
    {
        yield return new WaitForSeconds(castTime);

        if (!m_rsc.IsRaidDead() && !IsDead() && m_enemies.FindAll(x => x.Name == ConsortString).Count > 0)
        {
            for (int i = 0; i < m_raid.Count; i++)
            {
                m_raid[i].TakeDamage(GetScreechDamage(), ScreechString);
            }

            m_rsc.StartCoroutine(DealScreechDamage(1.0f));
        }
    }

    IEnumerator WaitForVenomSpray(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (!m_rsc.IsRaidDead() && !IsDead())
        {

            if (m_currentAbility == null)
            {
                m_currentAbility = m_encounterAbilities.Find(x => x.Name == VenomSprayString);
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
                temp.AddComponent<RaiderDebuff>().Initialize(debuffTargets[i], VenomSprayString, GetVenomSprayDoTDamage());
                debuffTargets[i].AddDebuff(temp);
            }

            m_rsc.DebuffsAdded();

            if(m_currentAbility != null && m_currentAbility.Name == VenomSprayString)
                m_currentAbility = null;

            m_rsc.StartCoroutine(WaitForVenomSpray(GetVenomSprayWaitTime()));
        }
    }
}
