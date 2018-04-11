using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class ElementalistAttack : BaseHealOrAttackScript
{
    EncounterEnemy m_currentTarget = null;
    int m_maxStacks = 5;
    int m_stacks = 1;
    float m_damageIncreasePerStack = 1.0f;

    public override string GetDescription() { return "Every attack increases damage done against the target by " + Utility.GetPercentString(m_damageIncreasePerStack) + ". Stacks up to " + m_maxStacks+" times"; }

    public override void Setup()
    {
        m_damageStruct = new DamageStruct();
        m_castTime = 3.2f;
        m_damageStruct.m_baseMultiplier = 1.7f;
        m_name = "Elemental Attunement";
    }

    public override void StartFight(int index, Raider attacker, RaiderScript rs)
    {
        rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rs));
    }

    IEnumerator DoAttack(float castTime, int index, Raider attacker, RaiderScript rs)
    {
        yield return new WaitForSeconds(castTime);

        if (!rs.IsBossDead() && !rs.IsDead())
        {
            DamageStruct thisAttack = new DamageStruct(m_damageStruct);
            if(m_stacks > 1)
                thisAttack.m_baseMultiplier *= (m_stacks * m_damageIncreasePerStack);
            int unused = 0;
            EncounterEnemy thisAttackEnemy = rs.DealDamage(index, Name, thisAttack, out unused, null);

            if (m_currentTarget != null && thisAttackEnemy == m_currentTarget)
            {
                m_stacks = m_stacks >= m_maxStacks ? m_maxStacks : m_stacks + 1;
            }
            else if (m_stacks > 1)
                m_stacks = 1;

            m_currentTarget = thisAttackEnemy;
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(rs.ApplyCooldownCastTimeMultiplier(m_castTime)), index, attacker, rs));
        }
    }
}
 