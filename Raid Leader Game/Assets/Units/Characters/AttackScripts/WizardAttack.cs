using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class WizardAttack : BaseHealOrAttackScript
{
    float m_dotDuration = 15.0f;
    float m_damagePerTick = 0.5f;
    int m_numTicks = 5;

    public override string GetDescription() { return "Each attack burns the target for an additional " + Utility.GetPercentString(m_damagePerTick *m_numTicks) + " damage over " + m_dotDuration + " seconds."; }

    public override void Setup()
    {
        m_damageStruct = new DamageStruct();
        m_castTime = 5.0f;
        m_damageStruct.m_baseMultiplier = 2.5f;
        m_name = "Scorch";
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
            EncounterEnemy target = rs.DealDamage(index, Name, thisAttack, out unused, null);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(rs.ApplyCooldownCastTimeMultiplier(m_castTime)), index, attacker, rs));

            rs.StartCoroutine(DoDoTTick(m_dotDuration / m_numTicks, index, m_numTicks, attacker, rs, target));
        }
    }

    IEnumerator DoDoTTick(float castTime,int index, int tickNumber, Raider attacker, RaiderScript rs, EncounterEnemy target)
    {
        yield return new WaitForSeconds(castTime);

        if (!rs.IsBossDead() && !rs.IsDead())
        {
            DamageStruct thisAttack = new DamageStruct(m_damageStruct);
            thisAttack.m_baseMultiplier *=  m_damagePerTick;

            int unused = 0;
            rs.DealDamage(index, Name, thisAttack, out unused, target);
            tickNumber--;
            if(tickNumber >= 0)
                rs.StartCoroutine(DoDoTTick(m_dotDuration / m_numTicks, index, tickNumber, attacker, rs, target));
        }
    }
}
