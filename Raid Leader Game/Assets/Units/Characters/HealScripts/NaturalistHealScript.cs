using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class NaturalistHealScript : BaseHealScript
{
    float m_HoTMultiplier = 0.10f;
    int m_maxSeconds = 6;
    float healInterval = 1.0f;

    public override string GetDescription() { return "Healed targets are healed again for " + GetPercentIncreaseString(m_HoTMultiplier + 1.0f) + " initial heal every second for " + m_maxSeconds + " seconds"; }

    public override void Setup()
    {
        m_castTime = 1.7f;
        m_baseMultiplier = 1.6f;
        m_name = "Nature's Touch";

        PriorityList = new List<Priority> {

                                            new Priority(1, Enums.RaidHealingState.TankHeavyDamage),
                                            new Priority(2, Enums.RaidHealingState.RaidMultiMediumDamage),
                                            new Priority(3, Enums.RaidHealingState.RaidSingleMediumDamage),
                                            new Priority(1, Enums.RaidHealingState.TankMediumDamage),
                                            new Priority(5, Enums.RaidHealingState.RaidMultiHeavyDamage),
                                            new Priority(6, Enums.RaidHealingState.RaidSingleHeavyDamage),
                                            new Priority(7, Enums.RaidHealingState.RandomTargets), };
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
            int heal = Mathf.RoundToInt(caster.RaiderStats.GetSpellAmount(m_baseMultiplier) / (numTargets*1.1f));

            for (int i = 0; i < numTargets; i++)
            {
                int actualHealing = targets[i].TakeHealing(heal);
                rsc.DoHeal(actualHealing, caster.GetName(), GetName(), index);
                int hotHeal = (int)(heal * m_HoTMultiplier);
                hotHeal = hotHeal == 0 ? 1 : hotHeal;
                rs.StartCoroutine(DoHoT(healInterval, m_maxSeconds, hotHeal, index, caster, rsc, rs, targets[i]));
            }

            rs.StartCoroutine(DoHeal(Utility.GetFussyCastTime(m_castTime), index, caster, rsc, rs));
        }
    }

    IEnumerator DoHoT(float castTime, int counter, int heal, int index, Raider caster, RaidSceneController rsc, RaiderScript rs, RaiderScript target)
    {
        yield return new WaitForSeconds(castTime);

        if (counter > 0 && !rsc.IsBossDead() && !rs.IsDead() && !target.IsDead())
        {
            counter--;
            int actualHealing = target.TakeHealing(heal);
            rsc.DoHeal(actualHealing, caster.GetName(), GetName(), index);
            rs.StartCoroutine(DoHoT(healInterval, counter, heal, index, caster, rsc, rs, target));
        }
    }
}
