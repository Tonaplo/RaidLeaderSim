using System.Collections;
using UnityEngine;

public class KnightAttack : BaseHealOrAttackScript
{
    float healPercent = 0.15f;

    public override string GetDescription() { return "Heals self for " + GetPercentIncreaseString(1.0f + healPercent) + " of damage dealt."; }

    public override void Setup()
    {
        m_castTime = 1.0f;
        m_baseMultiplier = 0.5f;
        m_name = "Shield Bash";
        m_cooldown = new BaseCooldown();
        m_cooldown.Initialize("Dont Know Yet", "Dont Know Yet", Enums.Cooldowns.TankCooldown);
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
            int damage = attacker.RaiderStats.GetSpellAmount(m_baseMultiplier);
            int healing = (int)(damage * healPercent);
            healing = healing == 0 ? 1 : healing; //Make sure we always heal for at least 1
            int actualHealing = rs.TakeHealing(healing);
            rsc.DoHeal(actualHealing, attacker.GetName(), Name, index);
            rsc.DealDamage(damage, attacker.GetName(), Name, index);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
        }
    }
}

