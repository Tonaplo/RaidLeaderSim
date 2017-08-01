using System.Collections;
using UnityEngine;

public class BerserkerAttack : BaseAttackScript
{

    /*
     * Sacrifices 2% health for 35% more damage done while above 50% health.
     * */

    int m_healthPercent = 50;
    float m_damageTakenPercent = 0.02f;
    float m_multiplier = 1.35f;
     
    public override void SetupAttack()
    {
        m_castTime = 1.0f;
        m_baseMultiplier = 1.6f;
        m_attackName = "Raging Blow";
    }

    public override void StartFight(int index, Raider attacker, RaidSceneController rsc, RaiderScript rs)
    {
        rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
    }

    IEnumerator DoAttack(float castTime, int index, Raider attacker, RaidSceneController rsc, RaiderScript rs)
    {
        yield return new WaitForSeconds(castTime);

        if (!rsc.IsBossDead() && !rs.IsDead())
        {
            float damage = attacker.RaiderStats().GetSpellAmount(m_baseMultiplier);
            if (rs.GetHealthPercent() > m_healthPercent) {
                rs.TakeDamage((int)(rs.GetMaxHealth() * m_damageTakenPercent));
                m_multiplier *= 1.25f;
            }

            rsc.DealDamage((int)damage, attacker.GetName(), m_attackName, index);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
        }
    }
}
