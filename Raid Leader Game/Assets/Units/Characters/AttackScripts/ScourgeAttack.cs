using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class ScourgeAttack : BaseHealOrAttackScript
{
    float m_maxCastTime = 0.9f;
    float m_minCastTime = 0.2f;

    public override string GetDescription() { return "Pauses between attacks can last as much as " + m_maxCastTime +" seconds, and as little as " + m_minCastTime + " seconds." ; }

    public override void Setup()
    {
        m_damageStruct = new DamageStruct();
        m_castTime = (m_maxCastTime+m_minCastTime)/2.0f;
        m_damageStruct.m_baseMultiplier = 1.0f;
        m_name = "Chaos";
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
            
            float newRandomCastTime = UnityEngine.Random.Range(m_minCastTime, m_maxCastTime);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(rs.ApplyCooldownCastTimeMultiplier(newRandomCastTime)), index, attacker, rs));
        }
    }
}
