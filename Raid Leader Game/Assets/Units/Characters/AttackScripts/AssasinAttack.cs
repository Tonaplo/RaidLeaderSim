using System.Collections;
using UnityEngine;

public class AssasinAttack : BaseHealOrAttackScript
{
    
    float m_multiplier = 2.0f;
    int m_bossHealthPercent = 75;

    public override string GetDescription() { return "Deals " + GetPercentIncreaseString(m_multiplier+1.0f) + " damage to enemies above " + m_bossHealthPercent + "% health"; }

    public override void Setup()
    {
        m_castTime = 0.5f;
        m_baseMultiplier = 0.7f;
        m_name = "Vein Slit";
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
            if (rsc.GetBossHealthPercent() > m_bossHealthPercent)
            {
                damage *= m_multiplier;
            }

            rsc.DealDamage((int)damage, attacker.GetName(), Name, index);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
        }
    }
}

