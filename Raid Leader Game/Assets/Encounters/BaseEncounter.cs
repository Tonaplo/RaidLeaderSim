using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseEncounter : MonoBehaviour
{

    #region variables and getters and setters
    int tanksNeeded = 0;

    public int TanksNeeded
    {
        get { return tanksNeeded; }
        set { tanksNeeded = value; }
    }
    int tanksUsed = 0;

    public int TanksUsed
    {
        get { return tanksUsed; }
        set { tanksUsed = value; }
    }
    int baseAoEDPSNeeded = 0;

    public int BaseAoEDPSNeeded
    {
        get { return baseAoEDPSNeeded; }
        set { baseAoEDPSNeeded = value; }
    }
    int baseSingleTargetDPSNeeded = 0;

    public int BaseSingleTargetDPSNeeded
    {
        get { return baseSingleTargetDPSNeeded; }
        set { baseSingleTargetDPSNeeded = value; }
    }
    int baseAoEHealingNeeded = 0;

    public int BaseAoEHealingNeeded
    {
        get { return baseAoEHealingNeeded; }
        set { baseAoEHealingNeeded = value; }
    }
    int baseSingleTargetHealingNeeded = 0;

    public int BaseSingleTargetHealingNeeded
    {
        get { return baseSingleTargetHealingNeeded; }
        set { baseSingleTargetHealingNeeded = value; }
    }
    int actualAoEDPSNeeded = 0;

    public int ActualAoEDPSNeeded
    {
        get { return actualAoEDPSNeeded; }
        set { actualAoEDPSNeeded = value; }
    }
    int actualSingleTargetDPSNeeded = 0;

    public int ActualSingleTargetDPSNeeded
    {
        get { return actualSingleTargetDPSNeeded; }
        set { actualSingleTargetDPSNeeded = value; }
    }
    int actualAoEHealingNeeded = 0;

    public int ActualAoEHealingNeeded
    {
        get { return actualAoEHealingNeeded; }
        set { actualAoEHealingNeeded = value; }
    }
    int actualSingleTargetHealingNeeded = 0;

    public int ActualSingleTargetHealingNeeded
    {
        get { return actualSingleTargetHealingNeeded; }
        set { actualSingleTargetHealingNeeded = value; }
    }
    Enums.Difficulties difficulty = Enums.Difficulties.Normal;

    public Enums.Difficulties Difficulty
    {
        get { return difficulty; }
        set { difficulty = value; }
    }
    List<EncounterAbility> encounterAbilities = new List<EncounterAbility>();

    public List<EncounterAbility> EncounterAbilities
    {
        get { return encounterAbilities; }
        set { encounterAbilities = value; }
    }
    List<BaseCooldown> encounterCooldowns = new List<BaseCooldown>();

    public List<BaseCooldown> EncounterCooldowns
    {
        get { return encounterCooldowns; }
        set { encounterCooldowns = value; }
    }
    #endregion

    public BaseEncounter(   int m_tanksNeeded,
                            int m_baseAoEDPSNeeded,
                            int m_baseSingleTargetDPSNeeded,
                            int m_baseAoEHealingNeeded,
                            int m_baseSingleTargetHealingNeeded,
                            Enums.Difficulties m_difficulty, 
                            List<EncounterAbility> m_encounterAbilities, 
                            List<BaseCooldown> m_encounterCooldowns)
    {
        tanksNeeded = m_tanksNeeded;
        baseAoEDPSNeeded = m_baseAoEDPSNeeded;
        baseSingleTargetDPSNeeded = m_baseSingleTargetDPSNeeded;
        baseAoEHealingNeeded = m_baseAoEHealingNeeded;
        baseSingleTargetHealingNeeded = m_baseSingleTargetHealingNeeded;
        difficulty = m_difficulty;
        encounterAbilities = m_encounterAbilities;
        encounterCooldowns = m_encounterCooldowns;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
