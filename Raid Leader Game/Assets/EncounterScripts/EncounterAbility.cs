using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EncounterAbility : BaseAbility
{
    float m_castTime;
    string m_casterName;
    Enums.AbilityCastType m_castType;

    public string Caster { get { return m_casterName; } }
    public float CastTime { get { return m_castTime; } }
    public Enums.AbilityCastType CastType { get { return m_castType; } }
    public EncounterAbility(string _name, string _caster, string _description, float _castTime, Enums.Ability _ability, Enums.AbilityCastType _castType) : base(_name, _description,_ability) 
    {
        m_castType = _castType;
        m_castTime = _castTime;
        m_casterName = _caster;
    }

    Raider m_counter;

    public void SetNewCastTime(float newCasttime) { m_castTime = newCasttime; }

    public void AssignCounter(Raider character)
    {
        m_counter = character;
    }

    public bool CanCounterAbility(Raider character)
    {
        //Cant counter something we're not used for
        return character.RaiderStats.Ability.Ability == Ability;
    }

    public bool AttemptToCounter() {
        BaseAbility ability = m_counter.RaiderStats.Ability;
        
        if (!CanCounterAbility(m_counter))
            return false;
        else {
            int roll = Random.Range(0, 100);

            if(m_counter.RaiderStats.GetMechanicalSkillForEncounter() >= roll)
                return true;

            return false;
        }
    }
}
