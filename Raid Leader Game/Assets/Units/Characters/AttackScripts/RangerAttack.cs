using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class RangerAttack : BaseHealOrAttackScript
{
    int m_counter;
    int m_maxCount = 4;
    float m_multiplier = 3.1f;

    public override string GetDescription() { return "Every " + m_maxCount + "th attack will deal " + Utility.GetPercentIncreaseString(m_multiplier) + " damage."; }

    public override void Setup()
    {
        m_damageStruct = new DamageStruct();
        m_castTime = 1.9f;
        m_damageStruct.m_baseMultiplier = 2.3f;
        m_name = "Aimed Shot";
    }

    public override void StartFight(int index, Raider attacker, RaiderScript rs)
    {
        m_counter = 0;
        //Since Rangers are higly dependent on their cast time, if they have a good attempt, lower the casttime a bit
        int averageThroughput = attacker.RaiderStats.GetAverageThroughput();
        int throughput = attacker.RaiderStats.GetThroughput();
        if (averageThroughput < throughput)
            m_castTime *= (averageThroughput) / (float)(throughput);

        rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rs));
    }

    IEnumerator DoAttack(float castTime, int index, Raider attacker, RaiderScript rs)
    {
        yield return new WaitForSeconds(castTime);
        if (!rs.IsBossDead() && !rs.IsDead())
        {
            DamageStruct thisAttack = new DamageStruct(m_damageStruct);

            m_counter++;

            if (m_counter == m_maxCount)
            {
                thisAttack.m_baseMultiplier *= m_multiplier;
                m_counter = 0;
            }
            rs.DealDamage(index, Name, thisAttack);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(rs.ApplyCooldownCastTimeMultiplier(m_castTime)), index, attacker, rs));
        }
    }
}
