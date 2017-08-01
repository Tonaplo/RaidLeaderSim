using System.Collections;
using UnityEngine;

public class NecromancerAttack : BaseAttackScript
{

    /*
     * Leaves a plague on targets, causing 100% of throughput after 10 secs.			
     * */

    bool m_isDoTRolling;
    float m_DoTCastTime = 10.0f;

    public override void SetupAttack()
    {
        m_castTime = 2.0f;
        m_baseMultiplier = 2.9f;
        m_attackName = "Death Missile";
    }

    public override void StartFight(int index, Raider attacker, RaidSceneController rsc, RaiderScript rs)
    {
        m_isDoTRolling = false;
        rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
    }

    IEnumerator DoAttack(float castTime, int index, Raider attacker, RaidSceneController rsc, RaiderScript rs)
    {
        yield return new WaitForSeconds(castTime);

        if (!rsc.IsBossDead() && !rs.IsDead())
        {
            float damage = attacker.RaiderStats().GetSpellAmount(m_baseMultiplier);
            if (!m_isDoTRolling) {
                rs.StartCoroutine(DealDelayedDamage(m_DoTCastTime, (int)damage, index, attacker, rsc, rs));
                m_isDoTRolling = true;
            }

            rsc.DealDamage((int)damage, attacker.GetName(), m_attackName, index);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
        }
    }

    IEnumerator DealDelayedDamage(float castTime, int damageAmount, int index, Raider attacker, RaidSceneController rsc, RaiderScript rs)
    {
        yield return new WaitForSeconds(castTime);

        if (!rsc.IsBossDead() && !rs.IsDead())
        {
            rsc.DealDamage((int)damageAmount, attacker.GetName(), m_attackName, index);
            m_isDoTRolling = false;
        }
    }
}
