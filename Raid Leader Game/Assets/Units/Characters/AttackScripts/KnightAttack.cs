using System.Collections;
using UnityEngine;

public class KnightAttack : BaseAttackScript
{

    /*
     * Heals self for 15% of damage dealt			
     * */

    float healPercent = 0.15f;

    public override void SetupAttack()
    {
        m_castTime = 1.0f;
        m_baseMultiplier = 0.5f;
        m_attackName = "Shield Bash";
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
            int damage = attacker.RaiderStats().GetSpellAmount(m_baseMultiplier);
            rs.TakeHealing((int)(damage * healPercent));
            rsc.DealDamage(damage, attacker.GetName(), m_attackName, index);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
        }
    }
}

