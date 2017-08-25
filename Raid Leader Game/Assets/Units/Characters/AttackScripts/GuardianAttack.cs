using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class GuardianAttack : BaseHealOrAttackScript
{
    public override string GetDescription() { return "Counterattacks the enemy that hits him. Guardians also have 15% increased health and take 15% reduced damage."; }

    public override void Setup() {
        m_damageStruct = new DamageStruct();
        m_castTime = 1.0f;
        m_damageStruct.m_baseMultiplier = 0.5f;
        m_name = "Counterattack";
    }
    
    public override void StartFight(int index, Raider attacker, RaiderScript rs) {
        rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rs));
    }

    IEnumerator DoAttack(float castTime, int index, Raider attacker, RaiderScript rs)
    {
        yield return new WaitForSeconds(castTime);

        if (!rs.IsBossDead() && !rs.IsDead())
        {
            DamageStruct thisAttack = new DamageStruct(m_damageStruct);
            rs.DealDamage(index, Name, thisAttack);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(rs.ApplyCooldownCastTimeMultiplier(m_castTime)), index, attacker, rs));
        }
    }
}
