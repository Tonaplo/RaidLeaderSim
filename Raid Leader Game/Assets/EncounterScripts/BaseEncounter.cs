using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseEncounter
{

    #region variables and getters and setters

    protected string m_name;
    protected HealthBarScript m_healthBar;
    protected EncounterAbility m_currentAbility;
    protected Enums.Difficulties m_difficulty = Enums.Difficulties.Normal;
    protected List<EncounterAbility> m_encounterAbilities = new List<EncounterAbility>();
    protected List<EncounterAttackDescription> m_attacks;
    protected List<CharacterItem> m_loot;
    
    public string Name { get { return m_name; }  }
    public HealthBarScript HealthBar  {  get { return m_healthBar; }   }
    public Enums.Difficulties Difficulty  {  get { return m_difficulty; } }
    public EncounterAbility CurrentAbility { get { return m_currentAbility; } }
    public List<EncounterAbility> EncounterAbilities  {  get { return m_encounterAbilities; } }
    public List<EncounterAttackDescription> EncounterAttacks { get { return m_attacks; } }
    public List<CharacterItem> Loot { get { return m_loot; } }
    
    protected int m_baseHealth;
    protected RaidSceneController m_rsc;
    protected List<RaiderScript> m_raid;

    #endregion

    public BaseEncounter(string name, int health)
    {
        m_name = name;
        m_baseHealth = health;
        m_difficulty = Enums.Difficulties.Normal;
        m_encounterAbilities = new List<EncounterAbility>();
    }

    public void InitializeForRaid(List<RaiderScript> raiders, RaidSceneController rsc, HealthBarScript healthBar)
    {
        InitializeForChoice(m_difficulty);
        m_raid = raiders;
        m_rsc = rsc;
        m_healthBar = healthBar;
        m_healthBar.SetupHealthBar(350, 390, 100, 600, Mathf.RoundToInt(m_baseHealth * GetDifficultyMultiplier()));
        m_healthBar.SetUseName(m_name, true);
        m_healthBar.SetUsePercent(true);
    }

    public void InitializeForChoice(Enums.Difficulties diff)
    {
        m_difficulty = diff;
        SetupLoot();
        SetupDescription();
        SetupAbilities();
    }

    public bool IsDead()
    {
        return HealthBar.IsDead();
    }

    public bool AttemptToCounterCurrentAbility(Raider counter)
    {
        m_currentAbility.AssignCounter(counter);
        return m_currentAbility.AttemptToCounter();
    }

    public virtual void SetupLoot()
    {
        Debug.LogAssertion("Calling SetupLoot on the BaseEncounter - this should always be overridden!");
    }

    public virtual void SetupDescription()
    {
        Debug.LogAssertion("Calling SetupDescription on the BaseEncounter - this should always be overridden!");
    }

    public virtual void SetupAbilities()
    {
        Debug.LogAssertion("Calling SetupAbilities on the BaseEncounter - this should always be overridden!");
    }

    public virtual void BeginEncounter() {
        Debug.LogAssertion("Calling BeginEncounter on the BaseEncounter - this should always be overridden!");
        for (int i = 0; i < m_raid.Count; i++)
        {
            m_raid[i].TakeDamage(Mathf.RoundToInt(m_raid[i].GetMaxHealth()*2.5f));
        }
    }

    public virtual void CurrentAbilityCountered()
    {
        Debug.LogAssertion("Calling CurrentAbilityCountered on the BaseEncounter - this should always be overridden!");
    }

    protected float GetDifficultyMultiplier()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 0.6f;
            case Enums.Difficulties.Normal:
            default:
                return 1.0f;
            case Enums.Difficulties.Hard:
                return 1.4f;
        }
    }

    protected List<RaiderScript> GetRandomRaidTargets(int numTargets)
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < m_raid.Count; i++) {
            if(!m_raid[i].IsDead())
                indices.Add(i);
        }

        if (numTargets > indices.Count)
            numTargets = indices.Count;

        while (numTargets != indices.Count)
        {
            indices.RemoveAt(Random.Range(0, indices.Count));
        }

        List<RaiderScript> targets = new List<RaiderScript>();
        for (int i = 0; i < indices.Count; i++)
        {
            targets.Add(m_raid[indices[i]]);
        }

        return targets;
    }
}
