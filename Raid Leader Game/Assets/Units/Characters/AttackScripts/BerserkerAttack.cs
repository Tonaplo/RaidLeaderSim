using System.Collections;
using UnityEngine;

public class BerserkerAttack : BaseHealOrAttackScript
{
    int m_healthPercent = 20;
    float m_damageTakenPercent = 0.02f;
    float m_multiplier = 1.50f;
    float m_berserkMultiplier = 3.00f;


    public override string GetDescription() { return "While above " + m_healthPercent + " % health, " + GetPercentIncreaseString(m_damageTakenPercent + 1.0f) + " health is sacrificed to deal " + GetPercentIncreaseString(m_multiplier+1.0f) + " damage."; }


    public override void Setup()
    {
        m_castTime = 1.4f;
        m_baseMultiplier = 2.6f;
        m_cooldownDuration = 15.0f;
        m_name = "Raging Blow";
        m_cooldown = new BaseCooldown();
        m_cooldown.Initialize("Berserk", "The Berserker now heals for " + GetPercentIncreaseString(m_damageTakenPercent + 1.0f) + " and deals " + GetPercentIncreaseString(m_berserkMultiplier + 1.0f) + " for " + m_cooldownDuration + " seconds.", Enums.Cooldowns.DPSCooldown);
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
            if (rs.GetHealthPercent() > m_healthPercent) {
                rs.TakeDamage((int)(rs.GetMaxHealth() * m_damageTakenPercent));
                m_multiplier *= 1.25f;
            }

            rsc.DealDamage((int)damage, attacker.GetName(), Name, index);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
        }
    }
}
