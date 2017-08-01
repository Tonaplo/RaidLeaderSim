using System.Collections;
using UnityEngine;

public class ScourgeAttack : BaseAttackScript
{

    /*
     * Scent of Death: Deals triple damage to enemies below 20% health			
     * */

    float m_multiplier = 3.0f;
    int m_bossHealthPercent = 20;

    public override void SetupAttack()
    {
        m_castTime = 0.5f;
        m_baseMultiplier = 0.7f;
        m_attackName = "Scent of Death";
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
            if (rsc.GetBossHealthPercent() < m_bossHealthPercent)
            {
                damage *= m_multiplier;
            }

            rsc.DealDamage((int)damage, attacker.GetName(), m_attackName, index);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
        }
    }
}
