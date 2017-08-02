using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class NaturalistHealScript : BaseHealScript
{


    float m_HoTMultiplier = 0.05f;
    int m_maxSeconds = 3;
    float healInterval = 1.0f;

    public override string GetDescription() { return "Healed targets are healed again for " + GetPercentIncreaseString(m_HoTMultiplier + 1.0f) + " throughput every second for " + m_maxSeconds + " seconds"; }

    public override void Setup()
    {
        m_castTime = 1.5f;
        m_baseMultiplier = 1.1f;
        m_name = "Nature's Touch";
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
            int heal = Mathf.RoundToInt(caster.RaiderStats().GetSpellAmount(m_baseMultiplier) / (numTargets*1.1f));

            for (int i = 0; i < numTargets; i++)
            {
                int actualHealing = targets[i].TakeHealing(heal);
                rsc.DoHeal(actualHealing, caster.GetName(), GetName(), index);
                rs.StartCoroutine(DoHoT(healInterval, m_maxSeconds, index, caster, rsc, rs, targets[i]));
            }

            rs.StartCoroutine(DoHeal(Utility.GetFussyCastTime(m_castTime), index, caster, rsc, rs));
        }
    }

    IEnumerator DoHoT(float castTime, int counter, int index, Raider caster, RaidSceneController rsc, RaiderScript rs, RaiderScript target)
    {
        yield return new WaitForSeconds(castTime);

        if (counter > 0 && !rsc.IsBossDead() && !rs.IsDead() && !target.IsDead())
        {
            counter--;
            int heal = Mathf.RoundToInt(caster.RaiderStats().GetSpellAmount(m_baseMultiplier) * m_HoTMultiplier);
            int actualHealing = target.TakeHealing(heal);
            rsc.DoHeal(actualHealing, caster.GetName(), GetName(), index);
            rs.StartCoroutine(DoHoT(healInterval, counter, index, caster, rsc, rs, target));
        }
    }
}
