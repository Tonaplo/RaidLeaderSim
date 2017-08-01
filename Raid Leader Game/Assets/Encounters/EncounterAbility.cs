using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EncounterAbility : BaseAbility
{
    public EncounterAbility(string _name, string _description, Enums.Ability _ability, EncounterAbilityEffect _effect) : base(_name, _description,_ability) 
    {
    
    }

    Raider m_counter;
    EncounterAbilityEffect m_effect;


    void AssignCounter(Raider character)
    {
        m_counter = character;
    }

    public bool CanCounterAbility(Raider character)
    {
        //Cant counter something we're not used for
        return character.RaiderStats().GetAbility().Ability() == Ability();
    }

    bool AttemptToCounter() {
        BaseAbility ability = m_counter.RaiderStats().GetAbility();
        
        if (!CanCounterAbility(m_counter))
            return false;
        else { 
            //Handle the persons skill to counter the m_ability
            //Before this logic is implemented assume he countered
            return true;
        }
    }
}
