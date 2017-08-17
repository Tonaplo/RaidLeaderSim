using System.Collections;
using UnityEngine;

public class RangerAttack : BaseHealOrAttackScript
{
    int m_counter;
    int m_maxCount = 4;
    float m_multiplier = 3.1f;
    float m_rapidFireMultipler = 0.5f;

    public override string GetDescription() { return "Every " + m_maxCount + "th attack will deal " + GetPercentIncreaseString(m_multiplier) + " damage."; }

    public override void Setup()
    {
        m_castTime = 1.9f;
        m_baseMultiplier = 2.2f;
        m_name = "Aimed Shot";
        m_cooldownDuration = 15.0f;
        m_cooldown = new BaseCooldown();
        m_cooldown.Initialize("Rapid Fire", "Reduces casttime by " + GetPercentIncreaseString(m_rapidFireMultipler) + " for " + m_cooldownDuration + " seconds.", Enums.Cooldowns.DPSCooldown);
    }

    public override void StartFight(int index, Raider attacker, RaidSceneController rsc, RaiderScript rs)
    {
        m_counter = 0;
        //Since Rangers are higly dependent on their cast time, if they have a good attempt, lower the casttime a bit
        int averageThroughput = attacker.RaiderStats.GetAverageThroughput();
        int throughput = attacker.RaiderStats.GetThroughput();
        if (averageThroughput < throughput)
            m_castTime *= (float)(averageThroughput) / (float)(throughput);

        rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
    }

    IEnumerator DoAttack(float castTime, int index, Raider attacker, RaidSceneController rsc, RaiderScript rs)
    {
        yield return new WaitForSeconds(castTime);
        if (!rsc.IsBossDead() && !rs.IsDead())
        {
            float damage = attacker.RaiderStats.GetSpellAmount(m_baseMultiplier);

            m_counter++;

            if (m_counter == m_maxCount)
            {
                damage *= m_multiplier;
                m_counter = 0;
            }

            rsc.DealDamage((int)damage, attacker.GetName(), Name, index);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
        }
    }
}
