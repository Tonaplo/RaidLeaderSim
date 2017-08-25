using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class WizardAttack : BaseHealOrAttackScript
{
    float m_minMultiplier = 0.80f;
    float m_maxMultiplier = 1.80f;

    public override string GetDescription() { return "Deals between " + Utility.GetPercentString(m_minMultiplier) + " and " + Utility.GetPercentString(m_maxMultiplier) + " damage on every attack"; }

    public override void Setup()
    {
        m_damageStruct = new DamageStruct();
        m_castTime = 2.0f;
        m_damageStruct.m_baseMultiplier = 2.8f;
        m_name = "Arcane Blast";
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
            thisAttack.m_baseMultiplier *= UnityEngine.Random.Range(m_minMultiplier, m_maxMultiplier);

            rs.DealDamage(index, Name, thisAttack);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(rs.ApplyCooldownCastTimeMultiplier(m_castTime)), index, attacker, rs));
        }
    }
}
