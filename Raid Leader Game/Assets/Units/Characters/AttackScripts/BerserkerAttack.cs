using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class BerserkerAttack : BaseHealOrAttackScript
{
    int m_healthPercent = 20;
    float m_damageTakenPercent = 0.02f;
    float m_multiplier = 1.5f;

    public override string GetDescription() { return "While above " + m_healthPercent + " % health, " + Utility.GetPercentString(m_damageTakenPercent) + " health is sacrificed to deal " + Utility.GetPercentString(m_multiplier) + " damage."; }


    public override void Setup()
    {
        m_damageStruct = new DamageStruct();
        m_castTime = 1.5f;
        m_damageStruct.m_baseMultiplier = 1.8f;
        m_name = "Raging Blow";
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
            
            if (rs.GetHealthPercent() > m_healthPercent) {
                rs.TakeDamage((int)(rs.GetMaxHealth() * m_damageTakenPercent));
                thisAttack.m_baseMultiplier *= m_multiplier;
            }
            
            rs.DealDamage(index, Name, thisAttack);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(rs.ApplyCooldownCastTimeMultiplier(m_castTime)), index, attacker, rs));
        }
    }
}
