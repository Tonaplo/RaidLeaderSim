﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DivinerHealScript : BaseHealScript
{


    float m_LowestMultiplier = 0.6f;

    public override string GetDescription() { return "Also heals the lowest health target for " + GetPercentIncreaseString(m_LowestMultiplier + 1.0f) + " of throughput"; }

    public override void Setup()
    {
        m_castTime = 1.5f;
        m_baseMultiplier = 2.0f;
        m_name = "Arcane Mending";
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
            List<RaiderScript> targets = GetRandomTargets(); // This asshole needs to be rewritten.
            int numTargets = targets.Count;
            int heal = Mathf.RoundToInt(caster.RaiderStats().GetSpellAmount(m_baseMultiplier) / (numTargets * 1.1f));
            
            for (int i = 0; i < numTargets; i++)
            {
                int actualHealing = targets[i].TakeHealing(heal);
                rsc.DoHeal(actualHealing, caster.GetName(), GetName(), index);
            }
            
            int abilityHealAmount = Mathf.RoundToInt(caster.RaiderStats().GetSpellAmount(m_baseMultiplier) * m_LowestMultiplier);
            List<RaiderScript> lowest = new List<RaiderScript>(Raid);
            TrimToLowestXFromList(ref lowest, 1);

            for (int i = 0; i < lowest.Count; i++)
            {
                int actualHealing = lowest[i].TakeHealing(abilityHealAmount);
                rsc.DoHeal(actualHealing, caster.GetName(), GetName(), index);
            }

            rs.StartCoroutine(DoHeal(Utility.GetFussyCastTime(m_castTime), index, caster, rsc, rs));
        }
    }
}