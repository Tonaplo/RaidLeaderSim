using System.Collections;
using UnityEngine;

public class ScourgeAttack : BaseHealOrAttackScript
{
    float m_multiplier = 3.5f;
    int m_bossHealthPercent = 20;

    public override string GetDescription() { return "Deals " + GetPercentIncreaseString(m_multiplier+1.0f) + " damage to enemies below " + m_bossHealthPercent + "% health."; }

    public override void Setup()
    {
        m_castTime = 0.5f;
        m_baseMultiplier = 0.68f;
        m_name = "Scent of Death";
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
            float damage = attacker.RaiderStats.GetSpellAmount(m_baseMultiplier);
            if (rsc.GetBossHealthPercent() < m_bossHealthPercent)
            {
                damage *= m_multiplier;
            }

            rsc.DealDamage((int)damage, attacker.GetName(), GetName(), index);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
        }
    }
}
