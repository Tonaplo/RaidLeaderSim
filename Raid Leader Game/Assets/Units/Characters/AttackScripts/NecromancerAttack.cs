using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class NecromancerAttack : BaseHealOrAttackScript
{

    public override string GetDescription() { return "Has a " + m_damageStruct.m_baseCritChance + "% chance to deal " + Utility.GetPercentString(m_damageStruct.m_baseCritEffect) + " damage"; }

    public override void Setup()
    {
        m_damageStruct = new DamageStruct();
        m_castTime = 2.0f;
        m_damageStruct.m_baseMultiplier = 2.8f;
        m_damageStruct.m_baseCritChance = 10;
        m_damageStruct.m_baseCritEffect = 4.0f;
        m_name = "Spear of Death";
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
            rs.DealDamage(index, Name, thisAttack);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(rs.ApplyCooldownCastTimeMultiplier(m_castTime)), index, attacker, rs));
        }
    }
}
