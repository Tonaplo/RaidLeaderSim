using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

[Serializable]
public class NaturalistHealScript : BaseHealScript
{
    float m_HoTMultiplier = 0.10f;
    int m_maxSeconds = 6;
    float healInterval = 1.0f;

    public override string GetDescription() { return "Healed targets are healed again for " + Utility.GetPercentString(m_HoTMultiplier) + " of the initial heal every second for " + m_maxSeconds + " seconds"; }

    public override void Setup()
    {
        m_healStruct = new HealStruct();
        m_castTime = 1.7f;
        m_healStruct.m_healMultiplier = 1.6f;
        m_name = "Nature's Touch";

        PriorityList = new List<Priority> {
                                            new Priority(1, Enums.RaidHealingState.TankHeavyDamage),
                                            new Priority(2, Enums.RaidHealingState.RaidMultiHeavyDamage),
                                            new Priority(3, Enums.RaidHealingState.RaidSingleHeavyDamage),
                                            new Priority(4, Enums.RaidHealingState.RaidMultiMediumDamage),
                                            new Priority(5, Enums.RaidHealingState.RaidSingleMediumDamage),
                                            new Priority(6, Enums.RaidHealingState.TankMediumDamage),
                                            new Priority(7, Enums.RaidHealingState.LowestHealthPercent), };
    }

    public override void StartFight(int index, Raider attacker, RaiderScript rs)
    {
        rs.StartCoroutine(DoHeal(Utility.GetFussyCastTime(m_castTime), index, attacker, rs));
    }

    IEnumerator DoHeal(float castTime, int index, Raider caster, RaiderScript rs)
    {
        yield return new WaitForSeconds(castTime);

        if (!rs.IsBossDead() && !rs.IsDead())
        {
            HealStruct thisHeal = new HealStruct(m_healStruct);
            List<RaiderScript> targets = new List<RaiderScript>();
            GetBestTargets(ref targets);
            int numTargets = targets.Count;
            thisHeal.m_healMultiplier *= (1.0f / numTargets);

            for (int i = 0; i < numTargets; i++)
            {
                int hotHeal = rs.DoHealing(index, Name, ref thisHeal, targets[i]);
                hotHeal = (int)(hotHeal * thisHeal.m_HoTMultiplier);
                hotHeal = hotHeal == 0 ? 1 : hotHeal;

                rs.StartCoroutine(DoHoT(healInterval, m_maxSeconds, hotHeal, index, caster, rs, targets[i]));
            }

            rs.StartCoroutine(DoHeal(Utility.GetFussyCastTime(rs.ApplyCooldownCastTimeMultiplier(m_castTime)), index, caster, rs));
        }
    }

    IEnumerator DoHoT(float castTime, int counter, int heal, int index, Raider caster, RaiderScript rs, RaiderScript target)
    {
        yield return new WaitForSeconds(castTime);

        if (counter > 0 && !rs.IsBossDead() && !rs.IsDead() && !target.IsDead())
        {
            counter--;
            target.TakeHealing(Name, caster.GetName(), index, heal);
            rs.StartCoroutine(DoHoT(healInterval, counter, heal, index, caster, rs, target));
        }
    }
}
