using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class BerserkerAttack : BaseHealOrAttackScript
{
    float m_multiplier = 3.0f;

    public override string GetDescription() { return "Deals up to " + Utility.GetPercentString(m_multiplier) + " more damage, based on missing health. More health missing translates to more damage."; }


    public override void Setup()
    {
        m_damageStruct = new DamageStruct();
        m_castTime = 1.0f;
        m_damageStruct.m_baseMultiplier = 1.1f;
        m_name = "Berserker's Rage";
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
            float actualMultiplier = m_multiplier* (100.0f - rs.GetHealthPercent()) / 100.0f;

            if (actualMultiplier > 1.0f)
                thisAttack.m_baseMultiplier *= m_multiplier;
            
            int unused = 0;
            rs.DealDamage(index, Name, thisAttack, out unused, null);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(rs.ApplyCooldownCastTimeMultiplier(m_castTime)), index, attacker, rs));
        }
    }
}
