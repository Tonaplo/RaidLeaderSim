using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

[Serializable]
public class ClericHealScript : BaseHealScript
{


    float m_maxHealIncrease = 1.5f;

    public override string GetDescription() { return "Heals targets up to " + Utility.GetPercentString(m_maxHealIncrease) + " more, based on targets health. Lower health means more healing."; }

    public override void Setup()
    {
        m_healStruct = new HealStruct();
        m_castTime = 1.5f;
        m_healStruct.m_healMultiplier = 1.5f;
        m_name = "Divine Light";

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
                caster.DoHealing(index, Name, ref thisHeal, targets[i]);
            }

            List<RaiderScript> newTargets = new List<RaiderScript>();
            GetBestTargets(ref newTargets);
            caster.StartCoroutine(DoHeal(Utility.GetFussyCastTime(caster.ApplyCooldownCastTimeMultiplier(m_castTime)), index, newTargets, caster));
        }
    }
}
