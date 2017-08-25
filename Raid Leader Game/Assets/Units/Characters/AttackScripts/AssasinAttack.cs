using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class AssasinAttack : BaseHealOrAttackScript
{
    
    float m_multiplier = 0.15f;
    int m_poisonDuration = 5;

    public override string GetDescription() { return "Leaves a poison, dealing " + Utility.GetPercentString(m_multiplier) + " damage to enemies every second for  " + m_poisonDuration + " seconds."; }

    public override void Setup()
    {
        m_damageStruct = new DamageStruct();
        m_castTime = 0.5f;
        m_damageStruct.m_baseMultiplier = 0.6f;
        m_name = "Vein Slit";
    }

    public override void StartFight(int index, Raider attacker, RaiderScript rs)
    {
        rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(rs.ApplyCooldownCastTimeMultiplier(m_castTime)), index, attacker, rs));
    }

    IEnumerator DoAttack(float castTime, int index, Raider attacker, RaiderScript rs)
    {
        yield return new WaitForSeconds(castTime);

        if (!rs.IsBossDead() && !rs.IsDead())
        {
            DamageStruct thisAttack = new DamageStruct(m_damageStruct);

            int damageDealt = rs.DealDamage(index, Name, thisAttack);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(rs.ApplyCooldownCastTimeMultiplier(m_castTime)), index, attacker, rs));

            rs.StartCoroutine(DoPoisonDoT(1.0f, m_poisonDuration, index, attacker, rs));
        }
    }

    IEnumerator DoPoisonDoT(float castTime, int counter, int index, Raider attacker, RaiderScript rs)
    {
        yield return new WaitForSeconds(castTime);

        if (counter > 0 && !rs.IsBossDead() && !rs.IsDead())
        {
            counter--;
            DamageStruct thisAttack = new DamageStruct(m_damageStruct);
            thisAttack.m_baseMultiplier *= m_multiplier;
            rs.DealDamage(index, Name, thisAttack);
            rs.StartCoroutine(DoPoisonDoT(1.0f, counter, index, attacker, rs));
        }
    }
}

