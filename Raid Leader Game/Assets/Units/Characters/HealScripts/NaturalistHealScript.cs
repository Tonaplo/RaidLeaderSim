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
        m_castTime = 1.2f;
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

    public override void StartFight(int index, Raider caster, RaiderScript rs)
    {
        List<RaiderScript> targets = new List<RaiderScript>();
        GetBestTargets(ref targets);
        rs.StartCoroutine(DoHeal(Utility.GetFussyCastTime(m_castTime), index, targets, rs));
    }

    IEnumerator DoHeal(float castTime, int index, List<RaiderScript> targets, RaiderScript caster)
    {
        yield return new WaitForSeconds(castTime);

        if (!caster.IsBossDead() && !caster.IsDead())
        {
            HealStruct thisHeal = new HealStruct(m_healStruct);
            
            int numTargets = targets.Count;
            thisHeal.m_healMultiplier *= (1.0f / numTargets);

            for (int i = 0; i < numTargets; i++)
            {
                int hotHeal = caster.DoHealing(index, Name, ref thisHeal, targets[i]);
                hotHeal = (int)(hotHeal * thisHeal.m_HoTMultiplier);
                hotHeal = hotHeal == 0 ? 1 : hotHeal;

                caster.StartCoroutine(DoHoT(healInterval, m_maxSeconds, hotHeal, index, caster, targets[i]));
            }

            List<RaiderScript> newTargets = new List<RaiderScript>();
            GetBestTargets(ref newTargets);
            caster.StartCoroutine(DoHeal(Utility.GetFussyCastTime(caster.ApplyCooldownCastTimeMultiplier(m_castTime)), index, newTargets, caster));
        }
    }

    IEnumerator DoHoT(float castTime, int counter, int heal, int index, RaiderScript caster, RaiderScript target)
    {
        yield return new WaitForSeconds(castTime);

        if (counter > 0 && !caster.IsBossDead() && !caster.IsDead() && !target.IsDead())
        {
            counter--;
            target.TakeHealing(Name, caster.Raider.GetName(), index, heal);
            caster.StartCoroutine(DoHoT(healInterval, counter, heal, index, caster, target));
        }
    }
}
