using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoACouncilOfStone : BaseEncounter
{
    /*
        This is a council fight. On Easy and Normal Difficulty, only 1 of the three bosses will be active and you kill them in a certain order.
        On Hard, all three are active at the same time and all their abilities come into play at the same time.

        The three council members are:

        1) Granitor:
            - Sweeping blow: DONE!
                - Cast time but not interruptable 
                - Deals heavy damage to a group of raiders every x seconds.
                - At certain percentage intervals, his damage increases.
            - From Earth to Earth: DONE! 
                - Easy and Normal:
                    - Stunnable
                    - Heals Granitor for y% of max health if not interrupted.
                - Hard:
                    - No longer stunnable
                    - Can now only be interrupted by dealing z % of his health in damage.
                    - Now heals the lowest health council members to full. 
        2) Volcanus:
            - Burning Core: DONE!
                - No Cast Time:
                - Deals consistent burn damage to the entire raid, every 2 seconds.
                - Every time he deals damage, it increases by y %
            - Lava Flood: DONE!
                - Post Move:
                - Cast time but not interruptable.
                - Creates pools of lava underneath all raiders dealing continous damage to all raiders
                - Use the movement button to get them away.
        3) Obsidiana
            - Summon Minisidiana: DONE!
                - Summons adds that throws shards on the players. Always summons 1 more add than the last time. 
                - each add deals damage to a random raider
     */

    EncounterEnemy Granitor = null;
    EncounterEnemy Volcanus = null;
    EncounterEnemy Obsidiana = null;
    int m_currentNumberOfCouncilMembers = 3;
    int m_summonNextMemberPercent = 10;
    int m_sweepingBlowHealthPercentage = 20;
    bool m_FETEcountered = false;
    int m_FETEHealthTarget = 0;

    string VolcanusString = "Volcanus";
    string GranitorString = "Granitor";
    string ObsidianaString = "Obsidiana";
    string MinisidianaString = "Minisidiana";

    public MoACouncilOfStone() : base("Council of Stone") { m_encounterEnum = Enums.EncounterEnum.MoACouncilOfStone; }

    public override void SetupEncounter()
    {
        SummonGranitor();
        if (m_difficulty == Enums.Difficulties.Hard)
        {
            SummonVolcanus();
            SummonObsidiana();
        }
    }

    public override void BeginEncounter()
    {
        m_currentNumberOfCouncilMembers = 1;
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

        StartGranitor();
        
        if (m_difficulty == Enums.Difficulties.Hard)
        {
            StartVolcanus();
            StartObsidiana();
        }
    }

    public override void SetupLoot()
    {
        GenerateLoot(50, 5);
    }

    public override void SetupDescriptionAndAbilities()
    {
        m_description = Name + " is comprised of three members, each representing one of the most powerful elements of the earth. Can your team overcome to power of earth?";

        m_enemyDescription = new List<EncounterEnemyDescription> {
            new EncounterEnemyDescription(Name, Name + " is comprised of three members, each representing one of the most powerful elements of the earth"),
            new EncounterEnemyDescription(GranitorString, "The impenetrable " + GranitorString + " is made of solid granite."),
            new EncounterEnemyDescription(VolcanusString, "The molten " + VolcanusString + " is made from a difference"),
            new EncounterEnemyDescription(ObsidianaString, "The spiky " + ObsidianaString + " is made from obsidian, making every inch of her sharp and deadly."),
            new EncounterEnemyDescription(MinisidianaString, ObsidianaString + " breaks of parts of herself to create " + MinisidianaString + "s to destroy her enemies."),
        };
        
        m_encounterAbilities = new List<EncounterAbility> {
            new EncounterAbility("Sweeping Blow", GranitorString , "While Granitor is alive, the council member deals " + GetSweepingBlowDamageBase() + " damage to " + GetNumSweepingBlowTargets() + " raiders, every " + GetSweepingBlowCastTime() + " seconds . For each " + m_sweepingBlowHealthPercentage + "% health Granitor is missing, damage is increased by " + Utility.GetPercentString(GetSweepingBlowMultiplier()) + "%.", GetSweepingBlowCastTime(), Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast),
            new EncounterAbility("Summon Minisidiana", ObsidianaString, "Every " + GetSummonMinisidianaWaitTime() + " seconds, Obsidiana summons Minisidianas with " + GetMinisidianaHealth() + " health with the Pelt attack. The number of Minisidianas summoned increases for each cast.", GetSummonMinisidianaCastTime(), Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast),
            new EncounterAbility("Lava Flood",VolcanusString, "Every " + GetLavaFloodWaitTime() + " seconds, Volcanus floods the raid with lava, dealing " + GetLavaFloodDamage() + " damage to all raiders every seconds they remain in the lava.", GetLavaFloodCastTime(), Enums.Ability.PostMovePositional, Enums.AbilityCastType.Cast),
            new EncounterAbility("Acceleration", Name, "Every " + GetAccelerationCastTime() + " seconds, the current council member smashes his current target for " + GetAccelerationDamage() + " damage. Each hit against the same target decreases cast time of this ability by " + Utility.GetPercentString(1.0f - GetAccelerationCastTimeDecrease()) + ".", 0, Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast),
            new EncounterAbility("Shattered Form", Name, "Every time a council member dies, the shattered form hurls smaller rocks at the raid, dealing between " + GetShatteredFormMinDamage() + " and " + GetShatteredFormMaxDamage() + " damage to all raiders.", 0, Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast),
            new EncounterAbility("Burning Core", VolcanusString, "While Volcanus is alive, the council member deals " + GetBurningCoreDamageBase() + " damage  to all raiders, every " + GetBurningCoreCastTime() + " seconds. Each time cast increases damage by " + Utility.GetPercentIncreaseString(GetBurningCoreMultiplier()) + ".", 0, Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast),
            new EncounterAbility("Pelt", MinisidianaString, "Minisidianas summoned by Obsidiana continuosly cast Pelt on a random raid member, dealing " + GetPeltDamage() + " damage.", 0, Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast),
        };

        Enums.Ability FETEAbility;
        string hardCounter = "";
        if (m_difficulty == Enums.Difficulties.Hard)
        {
            FETEAbility = Enums.Ability.Damage;
            hardCounter = " Removing " + Utility.GetPercentString(GetFETEdamagePercent()) + " of Granitors max health interrupts the cast.";
        }
        else
            FETEAbility = Enums.Ability.Stun;

        if (m_difficulty == Enums.Difficulties.Hard)
        {
            m_encounterAbilities.Add(new EncounterAbility("Council Assembled", Name, "All three council members enter the battle at once.", 0, Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast));
        }
        else
        {
            m_encounterAbilities.Add(new EncounterAbility("Council Apart", Name, "Granitor starts the fight, with Volcanus replacing him if he falls, and with Obsidina as the final obstacle.", 0, Enums.Ability.Uncounterable, Enums.AbilityCastType.Cast));
        }

        m_encounterAbilities.Add(new EncounterAbility("From Earth To Earth", GranitorString, "Every " + GetFETEWaitTime() + " seconds, Granitor mends the wounds of the lowest health council member, restoring " + Utility.GetPercentString(GetFETEHealPercent()) + " max health." + hardCounter, GetFETECastTime(), FETEAbility, Enums.AbilityCastType.Cast));
    }

    public override void CurrentAbilityCountered()
    {
        HandleAbilityTypeCountered(m_currentAbility.Ability);
        if (m_currentAbility.Name == "From Earth To Earth")
            m_FETEcountered = true;
    }

    public override void HandleOnTakeDamageEvent(int damage, RaiderScript attacker)
    {
        int numCouncilMembers = m_enemies.FindAll(e => e.EnemyType == Enums.EncounterEnemyType.Boss).Count;
        if (numCouncilMembers < m_currentNumberOfCouncilMembers)
        {
            for (int i = 0; i < m_raid.Count; i++)
            {
                m_raid[i].TakeDamage(GetShatteredFormDamage(), "Shattered Form");
            }
            m_currentNumberOfCouncilMembers = numCouncilMembers;

            //This ensure that their locals get set to null when they die
            Granitor = m_enemies.Find(e => e.Name == "Granitor");
            Volcanus = m_enemies.Find(e => e.Name == "Volcanus");
            Obsidiana = m_enemies.Find(e => e.Name == "Obsidiana");
        }
        else if(numCouncilMembers > m_currentNumberOfCouncilMembers)
            m_currentNumberOfCouncilMembers = numCouncilMembers;

        //On Normal and Easy, the next boss is summoned at x% left
        if (m_difficulty != Enums.Difficulties.Hard)
        {
            if (ReadyToSummonNextCouncilMember(Granitor, Volcanus))
            {
                SummonVolcanus();
                StartVolcanus();
            }
            else if (ReadyToSummonNextCouncilMember(Volcanus, Obsidiana))
            {
                SummonObsidiana();
                StartObsidiana();
            }
        }
        else if(m_difficulty == Enums.Difficulties.Hard && m_currentAbility != null && m_currentAbility.Name == "From Earth To Earth" && Granitor != null)
        {
            if (m_FETEHealthTarget > Granitor.Healthbar.CurrentHealth)
                m_FETEcountered = true;
        }
    }

    bool ReadyToSummonNextCouncilMember(EncounterEnemy enemy, EncounterEnemy nextEnemy)
    {
        return enemy != null && enemy.Healthbar.GetHealthPercent() <= m_summonNextMemberPercent && nextEnemy == null;
    }

    void SummonGranitor()
    {
        CreateEnemy("Granitor", Mathf.RoundToInt(23000 * GetDifficultyMultiplier()), Enums.EncounterEnemyType.Boss);
        Granitor = m_enemies.Find(e => e.Name == "Granitor");
    }

    void SummonVolcanus()
    {
        CreateEnemy("Volcanus", Mathf.RoundToInt(17000 * GetDifficultyMultiplier()), Enums.EncounterEnemyType.Boss);
        Volcanus = m_enemies.Find(e => e.Name == "Volcanus");
    }
    
    void SummonObsidiana()
    {
        CreateEnemy("Obsidiana", Mathf.RoundToInt(20000 * GetDifficultyMultiplier()), Enums.EncounterEnemyType.Boss);
        Obsidiana = m_enemies.Find(e => e.Name == "Obsidiana");
    }

    void StartGranitor()
    {
        m_rsc.StartCoroutine(WaitForSweepingBlow(Utility.GetFussyCastTime(GetSweepingBlowWaitTime()/3f)));
        m_rsc.StartCoroutine(WaitForFETE(GetFETEWaitTime()));
    }

    void StartVolcanus()
    {
        m_rsc.StartCoroutine(CastBurningCore(Utility.GetFussyCastTime(GetBurningCoreCastTime()), 0));
        m_rsc.StartCoroutine(WaitForLavaFlood(Utility.GetFussyCastTime(GetLavaFloodWaitTime() / 3f))); 
    }

    void StartObsidiana()
    {
        m_rsc.StartCoroutine(WaitForSummonMinisidanas(GetSummonMinisidianaWaitTime() / 3f, 1));
    }

    int GetAccelerationDamage()
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

    float GetAccelerationCastTimeDecrease()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 0.95f;
            case Enums.Difficulties.Normal:
            default:
                return 0.90f;
            case Enums.Difficulties.Hard:
                return 0.80f;
        }
    }

    float GetAccelerationCastTime()
    {
        int numStacks = GetNumStacksForRaider(m_currentRaiderTarget);
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 4.0f * Mathf.Pow(GetAccelerationCastTimeDecrease(), numStacks);
            case Enums.Difficulties.Normal:
            default:
                return 3.5f * Mathf.Pow(GetAccelerationCastTimeDecrease(), numStacks);
            case Enums.Difficulties.Hard:
                return 3.0f * Mathf.Pow(GetAccelerationCastTimeDecrease(), numStacks);
        }
    }

    float GetAccelerationDuration()
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

    int GetShatteredFormMinDamage()
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

    int GetShatteredFormMaxDamage()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 150;
            case Enums.Difficulties.Normal:
            default:
                return 250;
            case Enums.Difficulties.Hard:
                return 350;
        }
    }

    int GetShatteredFormDamage()
    {
        return Random.Range(GetShatteredFormMinDamage(), GetShatteredFormMaxDamage() + 1);
    }

    int GetSweepingBlowDamageBase()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 75;
            case Enums.Difficulties.Normal:
            default:
                return 150;
            case Enums.Difficulties.Hard:
                return 150;
        }
    }

    int GetSweepingBlowDamage()
    {
        int multiplier = (100 - Granitor.Healthbar.GetHealthPercent()) / m_sweepingBlowHealthPercentage;
        return Mathf.RoundToInt(GetSweepingBlowDamageBase() * (1.0f + GetSweepingBlowMultiplier()* multiplier));
    }

    int GetNumSweepingBlowTargets()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 3;
            case Enums.Difficulties.Normal:
            default:
                return 5;
            case Enums.Difficulties.Hard:
                return 7;
        }
    }

    float GetSweepingBlowWaitTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 10.0f;
            case Enums.Difficulties.Normal:
            default:
                return 7.0f;
            case Enums.Difficulties.Hard:
                return 15.0f;
        }
    }

    float GetSweepingBlowCastTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 3.0f;
            case Enums.Difficulties.Normal:
            default:
                return 2.0f;
            case Enums.Difficulties.Hard:
                return 3.0f;
        }
    }

    float GetSweepingBlowMultiplier()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 0.1f;
            case Enums.Difficulties.Normal:
            default:
                return 0.2f;
            case Enums.Difficulties.Hard:
                return 0.3f;
        }
    }
    
    float GetFETEdamagePercent()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
            case Enums.Difficulties.Normal:
            default:
                return 0; //This should only be used on Hard
            case Enums.Difficulties.Hard:
                return 0.05f;
        }
    }

    float GetFETEWaitTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 10.0f;
            case Enums.Difficulties.Normal:
            default:
                return 7.0f;
            case Enums.Difficulties.Hard:
                return 15.0f;
        }
    }

    float GetFETECastTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 3.0f;
            case Enums.Difficulties.Normal:
            default:
                return 2.0f;
            case Enums.Difficulties.Hard:
                return 8.0f;
        }
    }

    float GetFETEHealPercent()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 0.05f;
            case Enums.Difficulties.Normal:
            default:
                return 0.1f;
            case Enums.Difficulties.Hard:
                return 1.0f;
        }
    }

    int GetBurningCoreDamageBase()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 20;
            case Enums.Difficulties.Normal:
            default:
                return 25;
            case Enums.Difficulties.Hard:
                return 30;
        }
    }

    float GetBurningCoreCastTime()
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

    float GetBurningCoreMultiplier()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 1.10f;
            case Enums.Difficulties.Normal:
            default:
                return 1.15f;
            case Enums.Difficulties.Hard:
                return 1.20f;
        }
    }

    int GetLavaFloodDamage()
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

    float GetLavaFloodCastTime()
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

    float GetLavaFloodWaitTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 12.0f;
            case Enums.Difficulties.Normal:
            default:
                return 7.0f;
            case Enums.Difficulties.Hard:
                return 16.0f;
        }
    }

    float GetLavaFloodTickInterval()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 1.0f;
            case Enums.Difficulties.Normal:
            default:
                return 1.0f;
            case Enums.Difficulties.Hard:
                return 1.0f;
        }
    }

    int GetMinisidianaHealth()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 750;
            case Enums.Difficulties.Normal:
            default:
                return 1000;
            case Enums.Difficulties.Hard:
                return 1250;
        }
    }

    float GetSummonMinisidianaWaitTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 12.0f;
            case Enums.Difficulties.Normal:
            default:
                return 10.0f;
            case Enums.Difficulties.Hard:
                return 20.0f;
        }
    }

    float GetSummonMinisidianaCastTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 2f;
            case Enums.Difficulties.Normal:
            default:
                return 2.0f;
            case Enums.Difficulties.Hard:
                return 2.0f;
        }
    }

    int GetPeltDamage()
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

    float GetPeltCastTime()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 1f;
            case Enums.Difficulties.Normal:
            default:
                return 1f;
            case Enums.Difficulties.Hard:
                return 1f;
        }
    }

    IEnumerator DoTankAttack(float castTime, RaiderScript target)
    {
        yield return new WaitForSeconds(castTime);

        if (!m_rsc.IsRaidDead() && !target.IsDead() && !IsDead())
        {
            m_currentRaiderTarget.TakeDamage(GetAccelerationDamage(), "Acceleration");
            AddStackstoRaider(m_currentRaiderTarget, 1, GetAccelerationDuration());

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
            m_rsc.StartCoroutine(DoTankAttack(Utility.GetFussyCastTime(GetAccelerationCastTime()), m_currentRaiderTarget));
        }
    }
    
    IEnumerator WaitForSweepingBlow(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (!m_rsc.IsRaidDead() && Granitor != null)
        {

            if (m_currentAbility == null)
            {
                m_currentAbility = m_encounterAbilities.Find(x => x.Name == "Sweeping Blow");
                m_rsc.StartCoroutine(CastSweepingBlow(GetSweepingBlowCastTime()));
                m_rsc.BeginCastingAbility(m_currentAbility);
            }
            else
            {
                m_rsc.StartCoroutine(WaitForSweepingBlow(0.5f));
            }
        }

    }

    IEnumerator CastSweepingBlow(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (!m_rsc.IsRaidDead() && Granitor != null)
        {
            List<RaiderScript> targets = GetRandomRaidTargets(GetNumSweepingBlowTargets());

            foreach (var raider in targets)
            {
                raider.TakeDamage(GetSweepingBlowDamage(), "Sweeping Blow");
            }

            m_rsc.StartCoroutine(WaitForSweepingBlow(GetSweepingBlowWaitTime()));
        }

        m_currentAbility = null;
    }

    IEnumerator WaitForFETE(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (!m_rsc.IsRaidDead() && !IsDead() && Granitor != null)
        {

            if (m_currentAbility == null)
            {
                if (m_difficulty == Enums.Difficulties.Hard)
                {
                    m_FETEHealthTarget = Granitor.Healthbar.CurrentHealth - Mathf.RoundToInt(Granitor.Healthbar.MaxHealth * GetFETEdamagePercent());
                    m_FETEHealthTarget = m_FETEHealthTarget < 0 ? 0 : m_FETEHealthTarget;
                }
                m_FETEcountered = false;
                m_currentAbility = m_encounterAbilities.Find(x => x.Name == "From Earth To Earth");
                m_rsc.StartCoroutine(CastFETE(GetFETECastTime()));
                m_rsc.BeginCastingAbility(m_currentAbility);
            }
            else
            {
                m_rsc.StartCoroutine(WaitForFETE(0.5f));
            }
        }

    }

    IEnumerator CastFETE(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (Granitor != null && !m_rsc.IsRaidDead())
        {
            if (!m_FETEcountered)
            {
                EncounterEnemy target = Granitor;

                if (m_difficulty == Enums.Difficulties.Hard)
                {
                    foreach (var boss in m_enemies)
                    {
                        if (boss.EnemyType != Enums.EncounterEnemyType.Boss)
                            continue;

                        if (boss.Healthbar.GetHealthPercent() < target.Healthbar.GetHealthPercent())
                            target = boss;
                    }
                }

                target.Healthbar.ModifyHealth(target.Healthbar.MaxHealth * GetFETEHealPercent());
            }
            
            m_rsc.StartCoroutine(WaitForFETE(GetFETEWaitTime()));
        }

        m_rsc.EndCastingAbility(m_currentAbility);
        m_currentAbility = null;
    }

    IEnumerator CastBurningCore(float castTime, int burningCoreCounter)
    {
        yield return new WaitForSeconds(castTime);

        if (m_enemies.Find(e => e.Name == "Volcanus") != null && !m_rsc.IsRaidDead())
        {
            for (int i = 0; i < m_raid.Count; i++)
            {
                m_raid[i].TakeDamage(Mathf.RoundToInt(Mathf.Pow(GetBurningCoreMultiplier(), burningCoreCounter) * GetBurningCoreDamageBase()), "Burning Core");
            }
            burningCoreCounter++;
            m_rsc.StartCoroutine(CastBurningCore(Utility.GetFussyCastTime(GetBurningCoreCastTime()), burningCoreCounter));
        }
    }

    IEnumerator WaitForLavaFlood(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (m_enemies.Find(e => e.Name == "Volcanus") != null && !m_rsc.IsRaidDead())
        {

            if (m_currentAbility == null)
            {
                InitiatePositionalAbility();
                m_currentAbility = m_encounterAbilities.Find(x => x.Name == "Lava Flood");
                m_rsc.StartCoroutine(CastLavaFlood(GetLavaFloodCastTime()));
                m_rsc.BeginCastingAbility(m_currentAbility);
                m_rsc.StartCoroutine(WaitForLavaFlood(GetLavaFloodWaitTime() + GetLavaFloodCastTime()));
            }
            else
            {
                m_rsc.StartCoroutine(WaitForLavaFlood(0.5f));
            }
        }

    }

    IEnumerator CastLavaFlood(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (Volcanus != null && !m_rsc.IsRaidDead() && m_positionalTargets.Count > 0)
        {
            foreach (var raider in m_positionalTargets)
            {
                raider.TakeDamage(GetLavaFloodDamage(), "Lave Flood");
            }

            m_rsc.StartCoroutine(CastLavaFlood(GetLavaFloodTickInterval()));
        }

        if (m_currentAbility != null && m_currentAbility.Name == "Lava Flood")
        {
            m_rsc.EndCastingAbility(m_currentAbility);
            m_currentAbility = null;
        }
    }

    IEnumerator WaitForSummonMinisidanas(float waitTime, int addCounter)
    {
        yield return new WaitForSeconds(waitTime);

        if (!m_rsc.IsRaidDead() && !IsDead())
        {

            if (m_currentAbility == null)
            {
                m_currentAbility = m_encounterAbilities.Find(x => x.Name == "Summon Minisidiana");
                m_rsc.StartCoroutine(SummonMinisidanas(GetSummonMinisidianaCastTime(), addCounter));
                m_rsc.BeginCastingAbility(m_currentAbility);
            }
            else
            {
                m_rsc.StartCoroutine(WaitForSummonMinisidanas(0.5f, addCounter));
            }
        }

    }

    IEnumerator SummonMinisidanas(float waitTime, int addCounter)
    {
        yield return new WaitForSeconds(waitTime);

        if (!m_rsc.IsRaidDead() && !IsDead())
        {
            for (int i = 0; i < (addCounter/2)+1; i++)
            {
                CreateEnemy("Minisidiana", GetMinisidianaHealth(), Enums.EncounterEnemyType.Add);
            }

            m_rsc.StartCoroutine(CastPelt(GetPeltCastTime()));

            addCounter++;
            m_rsc.StartCoroutine(WaitForSummonMinisidanas(GetSummonMinisidianaWaitTime(), addCounter));
        }

        m_currentAbility = null;
    }

    IEnumerator CastPelt(float castTime)
    {
        yield return new WaitForSeconds(castTime);

        int numPelters = m_enemies.FindAll(x => x.Name == "Minisidiana").Count;

        if (!m_rsc.IsRaidDead() && !IsDead() && numPelters > 0)
        {
            for (int i = 0; i < numPelters; i++)
            {
                int index = Random.Range(0, m_raid.Count);
                while (m_raid[index].IsDead())
                {
                    if (index == m_raid.Count - 1)
                        index = 0;
                    else
                        index++;
                }

                m_raid[index].TakeDamage(GetPeltDamage(), "Pelt");
            }

            m_rsc.StartCoroutine(CastPelt(GetPeltCastTime()));
        }
    }
}
