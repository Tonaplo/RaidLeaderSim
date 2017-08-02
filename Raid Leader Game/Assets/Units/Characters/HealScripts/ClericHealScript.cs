using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ClericHealScript : BaseHealScript
{


    float m_maxHealIncrease = 1.5f;

    public override string GetDescription() { return "Heals targets up to " + GetPercentIncreaseString(m_maxHealIncrease+1.0f) + " more, based on targets health. Lower health means more healing."; }

    public override void Setup()
    {
        m_castTime = 1.5f;
        m_baseMultiplier = 1.5f;
        m_name = "Divine Light";
    }

    public override void StartFight(int index, Raider caster, RaidSceneController rsc, RaiderScript rs)
    {
        Raid = rsc.GetRaid();
        rs.StartCoroutine(DoHeal(Utility.GetFussyCastTime(m_castTime), index, caster, rsc, rs));
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
                float increase = (((100.0f - targets[i].GetHealthPercent())/100.0f)* m_maxHealIncrease) + 1.0f;
                int actualHealing = Mathf.RoundToInt(increase * heal);
                int realhealing = targets[i].TakeHealing(actualHealing);
                rsc.DoHeal(realhealing, caster.GetName(), GetName(), index);
            }

            rs.StartCoroutine(DoHeal(Utility.GetFussyCastTime(m_castTime), index, caster, rsc, rs));
        }
    }
}
