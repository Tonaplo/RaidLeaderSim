using System.Collections;
using UnityEngine;

public class NecromancerAttack : BaseHealOrAttackScript
{
    float m_multiplier = 4.0f;
    int m_chance = 10;

    public override string GetDescription() { return "Has a " + m_chance + "% chance to deal " + GetPercentIncreaseString(m_multiplier+1.0f) + " damage"; }

    public override void Setup()
    {
        m_castTime = 2.0f;
        m_baseMultiplier = 2.8f;
        m_name = "Spear of Death";
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

            rsc.DealDamage((int)damage, attacker.GetName(), GetName(), index);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
        }
    }
}
