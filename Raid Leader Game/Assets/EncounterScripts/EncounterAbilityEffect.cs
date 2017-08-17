using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterAbilityEffect {

    public struct EffectResults
    {
        int m_damageDealtToRaid;
        int m_healingDoneToBoss;
    }

    int m_numOccurences;
    int m_baseDamage;

    public EncounterAbilityEffect(int numOccu, int damage) 
    {
        m_numOccurences = numOccu;
        m_baseDamage = damage;
    }

    public void EvaluateEffect(out EffectResults results)
    {
        results = new EffectResults();
        //Do nothing for now
    }
}
