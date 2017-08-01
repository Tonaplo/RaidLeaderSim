using System.Collections;
using UnityEngine;

public class GuardianAttack : BaseAttackScript
{

    /*
     * The attack of the Guardian is quite boring.
     * It just deals damage on a cast time, no modifiers
     * */
     
    public override void SetupAttack() {
        m_castTime = 1.0f;
        m_baseMultiplier = 0.5f;
        m_attackName = "Counterattack";
    }
    
    public override void StartFight(int index, Raider attacker, RaidSceneController rsc, RaiderScript rs) {
        rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
    }

    IEnumerator DoAttack(float castTime, int index, Raider attacker, RaidSceneController rsc, RaiderScript rs)
    {
        yield return new WaitForSeconds(castTime);

        if (!rsc.IsBossDead() && !rs.IsDead())
        {
            int damage = attacker.RaiderStats().GetSpellAmount(m_baseMultiplier);
            rsc.DealDamage(damage, attacker.GetName(), m_attackName, index);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
        }
    }
}
