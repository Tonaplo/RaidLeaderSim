using System.Collections;
using UnityEngine;

public class RangerAttack : BaseAttackScript
{

    /*
     * Every 4th attack will deal quadruple damage			
     * */
    int counter;

    public override void SetupAttack()
    {
        m_castTime = 1.8f;
        m_baseMultiplier = 2.1f;
        m_attackName = "Aimed Shot";
    }

    public override void StartFight(int index, Raider attacker, RaidSceneController rsc, RaiderScript rs)
    {
        counter = 0;
        rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
    }

    IEnumerator DoAttack(float castTime, int index, Raider attacker, RaidSceneController rsc, RaiderScript rs)
    {
        yield return new WaitForSeconds(castTime);
        if (!rsc.IsBossDead() && !rs.IsDead())
        {
            float damage = attacker.RaiderStats().GetSpellAmount(m_baseMultiplier);

            counter++;

            if (counter == 4)
            {
                damage *= 4.0f;
                counter = 0;
            }

            rsc.DealDamage((int)damage, attacker.GetName(), m_attackName, index);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
        }
    }
}
