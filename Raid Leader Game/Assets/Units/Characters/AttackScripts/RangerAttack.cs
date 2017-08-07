using System.Collections;
using UnityEngine;

public class RangerAttack : BaseHealOrAttackScript
{
    int m_counter;
    int m_maxCount = 4;
    float m_multiplier = 3.1f;

    public override string GetDescription() { return "Every " + m_maxCount + "th attack will deal " + GetPercentIncreaseString(m_multiplier) + " damage."; }

    public override void Setup()
    {
        m_castTime = 1.9f;
        m_baseMultiplier = 2.2f;
        m_name = "Aimed Shot";
    }

    public override void StartFight(int index, Raider attacker, RaidSceneController rsc, RaiderScript rs)
    {
        m_counter = 0;
        //Since Rangers are higly dependent on their cast time, if they have a good attempt, lower the casttime a bit
        int averageThroughput = attacker.RaiderStats().GetAverageThroughput();
        int throughput = attacker.RaiderStats().GetThroughput();
        if (averageThroughput < throughput)
            m_castTime *= (float)(averageThroughput) / (float)(throughput);

        rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
    }

    IEnumerator DoAttack(float castTime, int index, Raider attacker, RaidSceneController rsc, RaiderScript rs)
    {
        yield return new WaitForSeconds(castTime);
        if (!rsc.IsBossDead() && !rs.IsDead())
        {
            float damage = attacker.RaiderStats().GetSpellAmount(m_baseMultiplier);

            m_counter++;

            if (m_counter == m_maxCount)
            {
                damage *= m_multiplier;
                m_counter = 0;
            }

            rsc.DealDamage((int)damage, attacker.GetName(), GetName(), index);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
        }
    }
}
