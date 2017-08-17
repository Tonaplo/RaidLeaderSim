using System.Collections;
using UnityEngine;

public class NecromancerAttack : BaseHealOrAttackScript
{
    float m_multiplier = 4.0f;
    int m_chance = 10;
    int m_cooldownIncrease = 40;

    public override string GetDescription() { return "Has a " + m_chance + "% chance to deal " + GetPercentIncreaseString(m_multiplier+1.0f) + " damage"; }

    public override void Setup()
    {
        m_castTime = 2.0f;
        m_baseMultiplier = 2.8f;
        m_name = "Spear of Death";
        m_cooldownDuration = 15.0f;
        m_cooldown = new BaseCooldown();
        m_cooldown.Initialize("Amplified " + m_name, "Increases the chance for " + m_name + " to deal " + GetPercentIncreaseString(m_multiplier + 1.0f) + " damage to " + (m_chance + m_cooldownIncrease) + "% for " + m_cooldownDuration + " seconds.", Enums.Cooldowns.DPSCooldown);
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
