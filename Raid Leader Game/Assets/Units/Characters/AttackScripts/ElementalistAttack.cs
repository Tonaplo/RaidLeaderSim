using System.Collections;
using UnityEngine;

public class ElementalistAttack : BaseHealOrAttackScript
{
    float m_multiplier = 2.0f;
    float m_percentReduction = 0.85f;
    int m_chance = 50;

    public override string GetDescription() { return "Deals " + GetPercentIncreaseString((1.0f-m_percentReduction)+ 1.0f) + " less damage, but has a " + m_chance + "% chance to deal " + GetPercentIncreaseString(m_multiplier + 1.0f) + " damage"; }

    public override void Setup()
    {
        m_castTime = 1.9f;
        m_baseMultiplier = 2.7f;
        m_name = "Fireball";
        m_cooldownDuration = 15.0f;
        m_cooldown = new BaseCooldown();
        m_cooldown.Initialize("Rapid Fire", "Removes the " + GetPercentIncreaseString((1.0f - m_percentReduction) + 1.0f) + " damage penalty for " + m_cooldownDuration + " seconds.", Enums.Cooldowns.DPSCooldown);
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
            float damage = attacker.RaiderStats.GetSpellAmount(m_baseMultiplier) * m_percentReduction;
            int roll = Random.Range(0, 100);
            if (m_chance >= roll)
            {
                damage *= m_multiplier;
            }

            rsc.DealDamage((int)damage, attacker.GetName(), Name, index);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
        }
    }
}
 