using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DivinerHealScript : BaseHealScript
{


    float m_LowestMultiplier = 0.7f;
    float m_cooldownCastTimeMultiplier = 0.5f;

    public override string GetDescription() { return "Also heals the lowest health target for " + GetPercentIncreaseString(m_LowestMultiplier + 1.0f) + " of throughput"; }

    public override void Setup()
    {
        m_castTime = 1.8f;
        m_baseMultiplier = 2.5f;
        m_name = "Arcane Mending";

        m_cooldownDuration = 15.0f;
        m_cooldown = new BaseCooldown();
        m_cooldown.Initialize("Quickening", "Reduces casttime of " + m_name + " by " + GetPercentIncreaseString(m_cooldownCastTimeMultiplier + 1.0f) + " for " + m_cooldownDuration + " seconds.", Enums.Cooldowns.HealingCooldown);

        PriorityList = new List<Priority> { new Priority(1, Enums.RaidHealingState.TankHeavyDamage),
                                            new Priority(2, Enums.RaidHealingState.TankMediumDamage),
                                            new Priority(3, Enums.RaidHealingState.RaidMultiHeavyDamage),
                                            new Priority(4, Enums.RaidHealingState.RaidMultiMediumDamage),
                                            new Priority(5, Enums.RaidHealingState.RaidSingleHeavyDamage),
                                            new Priority(6, Enums.RaidHealingState.RaidSingleMediumDamage),
                                            new Priority(7, Enums.RaidHealingState.LowestHealthPercent), };
    }

    public override void StartFight(int index, Raider attacker, RaidSceneController rsc, RaiderScript rs)
    {
        Raid = rsc.GetRaid();
        rs.StartCoroutine(DoHeal(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
    }

    IEnumerator DoHeal(float castTime, int index, Raider caster, RaidSceneController rsc, RaiderScript rs)
    {
        yield return new WaitForSeconds(castTime);

        if (!rsc.IsBossDead() && !rs.IsDead())
        {
            List<RaiderScript> targets = new List<RaiderScript>();
            GetBestTargets(ref targets);
            int numTargets = targets.Count;
            int heal = Mathf.RoundToInt(caster.RaiderStats.GetSpellAmount(m_baseMultiplier) / (numTargets * 1.1f));
            
            for (int i = 0; i < numTargets; i++)
            {
                int actualHealing = targets[i].TakeHealing(heal);
                rsc.DoHeal(actualHealing, caster.GetName(), Name, index);
            }
            
            int abilityHealAmount = Mathf.RoundToInt(caster.RaiderStats.GetSpellAmount(m_baseMultiplier) * m_LowestMultiplier);
            List<RaiderScript> lowest = new List<RaiderScript>(Raid);
            TrimToLowestXFromList(ref lowest, 1);

            for (int i = 0; i < lowest.Count; i++)
            {
                int actualHealing = lowest[i].TakeHealing(abilityHealAmount);
                rsc.DoHeal(actualHealing, caster.GetName(), Name, index);
            }

            rs.StartCoroutine(DoHeal(Utility.GetFussyCastTime(m_castTime), index, caster, rsc, rs));
        }
    }
}
