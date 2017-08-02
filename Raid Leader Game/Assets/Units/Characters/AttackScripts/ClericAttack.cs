using System.Collections;
using UnityEngine;

public class ClericAttack : BaseHealOrAttackScript
{


    public override string GetDescription() { return "Simply smites the target"; }

    public override void Setup()
    {
        m_castTime = 1.5f;
        m_baseMultiplier = 0.3f;
        m_name = "Smite";
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
            rsc.DealDamage((int)damage, attacker.GetName(), GetName(), index);
            rs.StartCoroutine(DoAttack(Utility.GetFussyCastTime(m_castTime), index, attacker, rsc, rs));
        }
    }
}

