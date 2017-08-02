using UnityEngine;
using System.Collections.Generic;

public class BaseHealOrAttackScript
{

    protected float m_castTime;
    protected float m_baseMultiplier;
    protected string m_name;

    public string GetName() { return m_name; }

    public string GetBaseCastTimeAsString() { return m_castTime + " seconds"; }
    public string GetBaseMultiplierAsString() { return m_baseMultiplier.ToString() + " times throughput"; }

    //used to Initialize the correct values
    public virtual void Setup() {; }

    //Derived version will do the actual damage and such
    public virtual void StartFight(int index, Raider attacker, RaidSceneController rsc, RaiderScript rs) {; }

    //Derived versions should describe what they do
    public virtual string GetDescription() { return ""; }

    protected string GetPercentIncreaseString(float multiplier)
    {
        int percentIncrease = Mathf.RoundToInt((multiplier - 1.0f) * 100.0f);

        return percentIncrease + "%";
    }

}