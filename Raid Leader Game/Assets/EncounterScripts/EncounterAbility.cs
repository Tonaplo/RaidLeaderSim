using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EncounterAbility : BaseAbility
{
    float m_castTime;

    public float CastTime { get { return m_castTime; } }
    public EncounterAbility(string _name, string _description, float _castTime, Enums.Ability _ability, EncounterAbilityEffect _effect) : base(_name, _description,_ability) 
    {
        m_castTime = _castTime;
    }

    Raider m_counter;
    EncounterAbilityEffect m_effect;


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

            if(m_counter.RaiderStats.Skills.GetSkillLevel(Enums.SkillTypes.Mechanical) >= roll)
                return true;

            return false;
        }
    }
}
