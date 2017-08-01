using System.Collections;
using UnityEngine;

public class WizardAttack : BaseAttackScript
{

    /*
     * Has a 15% chance to deal sextuple damage			
     * */

    float m_multiplier = 6.0f;
    int m_chance = 15;

    public override void SetupAttack()
    {
        m_castTime = 2.0f;
        m_baseMultiplier = 2.9f;
        m_attackName = "Arcane Blast";
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
            int roll = Random.Range(0, 100);
            if (m_chance >= roll)
            {
                damage *= m_multiplier;
            }

            rsc.DealDamage((int)damage, attacker.GetName(), m_attackName, index);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
        }
    }
}
