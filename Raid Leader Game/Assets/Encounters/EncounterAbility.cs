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

    bool AttemptToCounter() {
        BaseAbility ability = m_counter.RaiderStats().GetAbility();

        //Cant counter something we're not used for
        if (ability.Ability() != Ability())
            return false;
        else { 
            //Handle the persons skill to counter the ability
            //Before this logic is implemented assume he countered
            return true;
        }
    }
}
