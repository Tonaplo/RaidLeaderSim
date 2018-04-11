using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class NaturalistAttack : BaseHealOrAttackScript
{
    public override string GetDescription() { return "Attempts to drown the target by splashing water on it."; }

    public override void Setup()
    {
        m_damageStruct = new DamageStruct();
        m_castTime = 1.5f;
        m_damageStruct.m_baseMultiplier = 0.3f;
        m_name = "Splash";
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
            int unused = 0;
            rs.DealDamage(index, Name, thisAttack, out unused, null);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(rs.ApplyCooldownCastTimeMultiplier(m_castTime)), index, attacker, rs));
        }
    }
}