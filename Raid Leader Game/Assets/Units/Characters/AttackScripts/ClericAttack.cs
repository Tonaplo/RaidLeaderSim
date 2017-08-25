using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class ClericAttack : BaseHealOrAttackScript
{


    public override string GetDescription() { return "Simply smites the target"; }

    public override void Setup()
    {
        m_damageStruct = new DamageStruct();
        m_castTime = 1.5f;
        m_damageStruct.m_baseMultiplier = 0.3f;
        m_name = "Smite";
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
            rs.DealDamage(index, Name,thisAttack);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(rs.ApplyCooldownCastTimeMultiplier(m_castTime)), index, attacker, rs));
        }
    }
}

