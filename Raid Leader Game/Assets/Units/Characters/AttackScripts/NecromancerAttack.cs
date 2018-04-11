using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class NecromancerAttack : BaseHealOrAttackScript
{
    EncounterEnemy m_currentTarget = null;
    float m_damageIncrease = 4.0f;
    int m_chancePerStack = 10;
    int m_stacks = 0;
    int m_maxStacks = 9;

    public override string GetDescription() { return "Every attack has a " + m_chancePerStack + "% chance to deal " + Utility.GetPercentString(m_damageIncrease) + " additional damage. Each attack against the same target increases the chance by an additional " + m_chancePerStack + "%."; }

    public override void Setup()
    {
        m_damageStruct = new DamageStruct();
        m_castTime = 3.0f;
        m_damageStruct.m_baseMultiplier = 2.1f;
        m_damageStruct.m_baseCritChance = m_chancePerStack;
        m_damageStruct.m_baseCritEffect = m_damageIncrease;
        m_name = "Death Curse";
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
            thisAttack.m_baseCritChance += (m_stacks * m_chancePerStack);
            int unused = 0;
            EncounterEnemy thisAttackEnemy = rs.DealDamage(index, Name, thisAttack, out unused, null);
            if (m_currentTarget != null && thisAttackEnemy == m_currentTarget)
            {
                m_stacks = m_stacks >= m_maxStacks ? m_maxStacks : m_stacks + 1;
            }
            else if (m_stacks > 0)
                m_stacks = 0;

            m_currentTarget = thisAttackEnemy;
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(rs.ApplyCooldownCastTimeMultiplier(m_castTime)), index, attacker, rs));
        }
    }
}
