using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class AssasinAttack : BaseHealOrAttackScript
{
    float m_maxBuffPerStack = 0.30f;
    float m_minBuffPerStack = 0.05f;
    float m_tickDuration = 1.0f;
    float m_poisonDuration = 5.0f;
    IEnumerator m_coroutine;  

    public override string GetDescription() { return "Dealing damage to a target also applies a poison that deals between " + Utility.GetPercentString(m_minBuffPerStack *(m_poisonDuration/m_tickDuration)) + " and " + Utility.GetPercentString(m_maxBuffPerStack * (m_poisonDuration / m_tickDuration)) + " additional damage over " + m_poisonDuration +" seconds. The higher health the target is, the stronger the poison."; }

    public override void Setup()
    {
        m_damageStruct = new DamageStruct();

        m_name = "Poisonous Blades";
        m_castTime = 1.2f;
        m_damageStruct.m_baseMultiplier = 1.2f;
    }

    public override void StartFight(int index, Raider attacker, RaiderScript rs)
    {
        rs.StartCoroutine(BasicAttack(Utility.GetFussyCastTime(rs.ApplyCooldownCastTimeMultiplier(m_castTime)), index, attacker, rs));
    }

    IEnumerator BasicAttack(float castTime, int index, Raider attacker, RaiderScript rs)
    {
        yield return new WaitForSeconds(castTime);

        if (!rs.IsBossDead() && !rs.IsDead())
        {
            DamageStruct thisAttack = new DamageStruct(m_damageStruct);
            int unused = 0;
            EncounterEnemy thisAttackEnemy = rs.DealDamage(index, Name, thisAttack, out unused, null);

            float poisonMultiplier = (thisAttackEnemy == null) ? 0.0f : (thisAttackEnemy.Healthbar.GetHealthPercent() / 100.0f) * m_maxBuffPerStack;

            //Make sure it never falls below the minimum
            poisonMultiplier = poisonMultiplier < m_minBuffPerStack ? m_minBuffPerStack : poisonMultiplier;

            rs.StartCoroutine(DoPoisonTick(m_tickDuration, index, poisonMultiplier,(int)(m_poisonDuration/m_tickDuration), attacker, rs, thisAttackEnemy));
            
            rs.StartCoroutine(BasicAttack(Utility.GetFussyCastTime(rs.ApplyCooldownCastTimeMultiplier(m_castTime)), index, attacker, rs));
        }
    }

    IEnumerator DoPoisonTick(float castTime, int index, float multiplier, int tickNumber, Raider attacker, RaiderScript rs, EncounterEnemy target)
    {
        yield return new WaitForSeconds(castTime);

        if (!rs.IsBossDead() && !rs.IsDead() && (target != null) && !target.Healthbar.IsDead())
        {
            DamageStruct thisAttack = new DamageStruct(m_damageStruct);
            thisAttack.m_baseMultiplier *= multiplier;

            int unused = 0;
            rs.DealDamage(index, Name, thisAttack, out unused, target);
            tickNumber--;
            if (tickNumber >= 0)
                rs.StartCoroutine(DoPoisonTick(m_tickDuration, index, multiplier, tickNumber, attacker, rs, target));
        }
    }
}

