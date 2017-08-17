using System.Collections;
using UnityEngine;

public class WizardAttack : BaseHealOrAttackScript
{
    float m_minMultiplier = 0.80f;
    float m_maxMultiplier = 1.80f;

    public override string GetDescription() { return "Deals between " + GetPercentIncreaseString(m_minMultiplier+1.0f) + " and " + GetPercentIncreaseString(m_maxMultiplier + 1.0f) + " damage on every attack"; }

    public override void Setup()
    {
        m_castTime = 2.0f;
        m_baseMultiplier = 2.8f;
        m_name = "Arcane Blast";
        m_cooldownDuration = 15.0f;
        m_cooldown = new BaseCooldown();
        m_cooldown.Initialize("Not Sure Yet", "Dont know yet.", Enums.Cooldowns.DPSCooldown);
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
            damage *= Random.Range(m_minMultiplier, m_maxMultiplier);

            rsc.DealDamage((int)damage, attacker.GetName(), Name, index);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
        }
    }
}
