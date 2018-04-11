using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class ScourgeAttack : BaseHealOrAttackScript
{
    float m_percentCutoff = 0.80f;
    float m_buff = 1.0f;
    float m_buffDuration = 10.0f;
    bool m_hasbuff = false;
    IEnumerator m_coroutine;

    public override string GetDescription() { return "Dealing damage to a target with over " + Utility.GetPercentString(m_percentCutoff) + " health increases damage done by " + Utility.GetPercentString(m_buff) + " for " + m_buffDuration +" secs."; }


    public override void Setup()
    {
        m_damageStruct = new DamageStruct();
        m_castTime = 1.0f;
        m_damageStruct.m_baseMultiplier = 1.2f;
        m_name = "Hatred of Life";
    }

    public override void StartFight(int index, Raider attacker, RaiderScript rs)
    {
        rs.StartCoroutine(HatredOfLife(Utility.GetFussyCastTime(m_castTime), index, attacker, rs));
    }

    IEnumerator HatredOfLife(float castTime, int index, Raider attacker, RaiderScript rs)
    {
        yield return new WaitForSeconds(castTime);

        if (!rs.IsBossDead() && !rs.IsDead())
        {
            DamageStruct thisAttack = new DamageStruct(m_damageStruct);

            if (m_hasbuff) {
                thisAttack.m_baseMultiplier *=(m_buff + 1.0f);
            }

            int unused = 0;
            EncounterEnemy thisAttackEnemy = rs.DealDamage(index, Name, thisAttack, out unused, null);
            if (thisAttackEnemy.Healthbar.GetHealthPercent() >= (m_percentCutoff * 100.0f))
            {
                m_hasbuff = true;
                if (m_coroutine != null)
                    rs.StopCoroutine(m_coroutine);

                m_coroutine = CancelBuff(m_buffDuration, attacker, rs);
                rs.StartCoroutine(m_coroutine);
            }
            
            rs.StartCoroutine(HatredOfLife(Utility.GetFussyCastTime(rs.ApplyCooldownCastTimeMultiplier(m_castTime)), index, attacker, rs));
        }
    }

    IEnumerator CancelBuff(float castTime, Raider attacker, RaiderScript rs)
    {
        yield return new WaitForSeconds(castTime);
        m_hasbuff = false;
    }
}
