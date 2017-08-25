using UnityEngine;
using System.Collections.Generic;
using System;

public class BaseHealOrAttackScript
{

    protected float m_castTime;
    protected DamageStruct m_damageStruct;
    protected string m_name;

    public string Name { get { return m_name; } }
    public string GetBaseCastTimeAsString() { return m_castTime + " seconds"; }
    public virtual string GetBaseMultiplierAsString(Raider r) { return (r.RaiderStats.GetAverageThroughput() * m_damageStruct.m_baseMultiplier).ToString() + " average damage per attack."; }

    //used to Initialize the correct values
    public virtual void Setup() { Debug.LogAssertion("Setup() should be overridden!"); }

    //Derived version will do the actual damage and such
    public virtual void StartFight(int index, Raider attacker, RaiderScript rs) { Debug.LogAssertion("StartFight() should be overridden!"); }

    //Derived versions should describe what they do
    public virtual string GetDescription() {
        Debug.LogAssertion("GetDescription() should be overridden!");
        return "";
    }
}