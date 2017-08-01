using System.Collections;
using UnityEngine;

public class ElementalistAttack : BaseAttackScript
{

    /*
     * Deals 25% less damage, but has a 50% chance to deal double damage
     * */

    float m_multiplier = 2.0f;
    float m_percentReduction = 0.75f;
    int m_chance = 50;

    public override void SetupAttack()
    {
        m_castTime = 2.0f;
        m_baseMultiplier = 2.9f;
        m_attackName = "Fireball";
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
            float damage = attacker.RaiderStats().GetSpellAmount(m_baseMultiplier) * m_percentReduction;
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
 