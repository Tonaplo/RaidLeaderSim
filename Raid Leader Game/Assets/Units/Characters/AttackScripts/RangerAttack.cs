using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class RangerAttack : BaseHealOrAttackScript
{
    EncounterEnemy m_currentTarget = null;
    int m_stacks = 0;
    int m_maxStacks = 3;
    float m_castTimeReduction = 0.50f;

    public override string GetDescription() { return "Each attack against the same target reduces casttime by " + Utility.GetPercentString(m_castTimeReduction) + ". Stacks up to " + m_maxStacks +" times."; }


    public override void Setup()
    {
        m_damageStruct = new DamageStruct();
        m_castTime = 5.0f;
        m_damageStruct.m_baseMultiplier = 2.0f;
        m_name = "Aimed Shot";
    }

    public override void StartFight(int index, Raider attacker, RaiderScript rs)
    {
        //Since Rangers are higly dependent on their cast time, if they have a good attempt, lower the casttime a bit
        int averageThroughput = attacker.RaiderStats.GetAverageThroughput();
        int throughput = attacker.RaiderStats.GetThroughput();
        if (averageThroughput < throughput)
            m_castTime *= (averageThroughput) / (float)(throughput);

        rs.StartCoroutine(AimedShot(Utility.GetFussyCastTime(m_castTime), index, attacker, rs));
    }

    IEnumerator AimedShot(float castTime, int index, Raider attacker, RaiderScript rs)
    {
        yield return new WaitForSeconds(castTime);
        if (!rs.IsBossDead() && !rs.IsDead())
        {
            DamageStruct thisAttack = new DamageStruct(m_damageStruct);
            int unused = 0;
            EncounterEnemy thisAttackEnemy = rs.DealDamage(index, Name, thisAttack, out unused, null);
            if (m_currentTarget != null && thisAttackEnemy == m_currentTarget)
            {
                m_stacks = m_stacks >= m_maxStacks ? m_maxStacks : m_stacks+1;
            }
            else if (m_stacks > 0)
                m_stacks = 0;

            float actualCastTime = m_castTime * Mathf.Pow(m_castTimeReduction, m_stacks);
            m_currentTarget = thisAttackEnemy;
            rs.StartCoroutine(AimedShot(Utility.GetFussyCastTime(rs.ApplyCooldownCastTimeMultiplier(actualCastTime)), index, attacker, rs));
        }
    }
}
