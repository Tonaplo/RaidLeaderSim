using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseEncounter
{

    #region variables and getters and setters

    protected string m_name;
    protected string m_description;
    protected int m_counter = 1;
    protected Enums.EncounterEnum m_encounterEnum;
    protected RaiderScript m_currentRaiderTarget;
    protected EncounterAbility m_currentAbility;
    protected Enums.Difficulties m_difficulty = Enums.Difficulties.Normal;
    protected List<EncounterAbility> m_encounterAbilities = new List<EncounterAbility>();
    protected List<EncounterAttackDescription> m_attacks;
    protected List<CharacterItem> m_loot;
    protected List<EncounterEnemy> m_enemies;

    public string Name { get { return m_name; } }
    public string Description { get { return m_description; } }
    public int Stacks { get { return m_counter; } }
    public Enums.EncounterEnum EncounterEnum { get { return m_encounterEnum; } }
    public RaiderScript CurrentRaiderTarget { get { return m_currentRaiderTarget; } }
    public Enums.Difficulties Difficulty { get { return m_difficulty; } }
    public EncounterAbility CurrentAbility { get { return m_currentAbility; } }
    public List<EncounterAbility> EncounterAbilities { get { return m_encounterAbilities; } }
    public List<EncounterAttackDescription> EncounterAttacks { get { return m_attacks; } }
    public List<CharacterItem> Loot { get { return m_loot; } }
    public List<EncounterEnemy> Enemies { get { return m_enemies; } }


    protected GameObject m_healthBarPrefab;
    protected RaidSceneController m_rsc;
    protected List<RaiderScript> m_raid;
    protected List<RaiderScript> m_positionalTargets;

    #endregion

    public BaseEncounter(string name)
    {
        m_name = name;
        m_difficulty = Enums.Difficulties.Normal;
        m_encounterAbilities = new List<EncounterAbility>();
    }

    public void InitializeForRaid(List<RaiderScript> raiders, RaidSceneController rsc, GameObject healthBar)
    {
        InitializeForChoice(m_difficulty);
        m_healthBarPrefab = healthBar;
        m_healthBarPrefab.SetActive(false);
        m_raid = raiders;
        m_rsc = rsc;
        m_enemies = new List<EncounterEnemy>();
        SetupEncounter();
        TargetEnemy(m_enemies.Find(e => e.EnemyType == Enums.EncounterEnemyType.Boss).Index);
        for (int i = 0; i < m_enemies.Count; i++)
        {
            if (m_enemies[i].IsCurrentTarget())
                m_enemies[i].Healthbar.gameObject.SetActive(true);
        }
    }

    public virtual void SetupEncounter()
    {
        Debug.LogAssertion("Calling SetupEncounter on the BaseEncounter - this should always be overridden!");
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
        return m_enemies.FindAll(x => x.EnemyType == Enums.EncounterEnemyType.Boss).Count == 0;
    }

    public bool AttemptToCounterCurrentAbility(Raider counter)
    {
        if (m_currentAbility != null)
        {
            m_currentAbility.AssignCounter(counter);
            return m_currentAbility.AttemptToCounter();
        }
        return false;
    }

    public void SetCurrentRaiderTarget(RaiderScript raider)
    {
        m_currentRaiderTarget = raider;
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

    public virtual void BeginEncounter()
    {
        Debug.LogAssertion("Calling BeginEncounter on the BaseEncounter - this should always be overridden!");
        for (int i = 0; i < m_raid.Count; i++)
        {
            m_raid[i].TakeDamage(Mathf.RoundToInt(m_raid[i].GetMaxHealth() * 2.5f), "Death by Debug");
        }
    }

    public virtual void CurrentAbilityCountered()
    {
        Debug.LogAssertion("Calling CurrentAbilityCountered on the BaseEncounter - this should always be overridden!");
    }

    public int TakeDamage(int damage, RaiderScript attacker)
    {
        EncounterEnemy currentTarget = m_enemies.Find(e => e.IsCurrentTarget());
        if (currentTarget != null)
        {
            int previousHealth = currentTarget.Healthbar.CurrentHealth;
            currentTarget.Healthbar.ModifyHealth(-damage);

            if (currentTarget.Healthbar.IsDead())
            {
                int newIndex = -1;
                for (int i = 0; i < m_enemies.Count; i++)
                {
                    if (m_enemies.FindAll(e => e.Index == i).Count == 0) {
                        newIndex = i;
                        break;
                    }
                }
                currentTarget.Index = newIndex;
                currentTarget.ToggleTargetSetting();
                currentTarget.DestroyHealthBar();
                m_enemies.Remove(currentTarget);
            }

            int actualDamage = previousHealth - (int)currentTarget.Healthbar.CurrentHealth;
            HandleOnTakeDamageEvent(actualDamage, attacker);

            return actualDamage;
        }

        return 0;
    }

    public virtual void HandleOnTakeDamageEvent(int damage, RaiderScript attacker)
    {
        ;//This function can be overridden to deal with the event of taking damage by each encounter
    }

    protected void HandleAbilityTypeCountered(Enums.Ability abilityType)
    {
        /*
         So the idea is that different abilities behave differently when countered
            - Stuns and Interrupts mean that the caster should stop casting immediately
            - Immunes mean that the caster should continue casting, but his ability has no effect on his target
            - Dispell should'nt have anything to do with a cast bar at all and thus should do nothing
         */
        switch (abilityType)
        {
            case Enums.Ability.Interrupt:
            case Enums.Ability.Stun:
                m_rsc.EndCastingAbility(m_currentAbility);
                break;
            case Enums.Ability.Immune:
                break;
            case Enums.Ability.Dispel:
                break;
            default:
                break;
        }
    }

    protected float GetDifficultyMultiplier()
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                return 0.8f;
            case Enums.Difficulties.Normal:
            default:
                return 1.0f;
            case Enums.Difficulties.Hard:
                return 1.2f;
        }
    }

    protected List<RaiderScript> GetRandomRaidTargets(int numTargets)
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < m_raid.Count; i++)
        {
            if (!m_raid[i].IsDead())
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

    public void InitiatePositionalAbility()
    {
        m_positionalTargets = new List<RaiderScript>(m_raid);
    }

    public bool AttemptToMove()
    {
        List<RaiderScript> toBeRemoved = new List<RaiderScript>();
        for (int i = 0; i < m_positionalTargets.Count; i++)
        {
            int chance = m_raid[i].Raider.RaiderStats.Skills.GetSkillLevel(Enums.SkillTypes.Positional);
            int roll = Random.Range(0, StaticValues.MaxSkill + 1);
            if (roll < chance)
            {
                toBeRemoved.Add(m_positionalTargets[i]);
            }
        }

        for (int i = 0; i < toBeRemoved.Count; i++)
        {
            m_positionalTargets.Remove(toBeRemoved[i]);
        }

        return m_positionalTargets.Count == 0;
    }

    protected void GenerateLoot(int normalItemLevel, int numNormalLootPieces)
    {
        switch (m_difficulty)
        {
            case Enums.Difficulties.Easy:
                normalItemLevel -= 5;
                numNormalLootPieces -= 1;
                break;
            case Enums.Difficulties.Normal:
            default:
                break;
            case Enums.Difficulties.Hard:
                normalItemLevel += 5;
                numNormalLootPieces += 1;
                break;
        }

        m_loot = new List<CharacterItem>();

        for (int i = 0; i < numNormalLootPieces; i++)
        {
            m_loot.Add(new CharacterItem(normalItemLevel + Random.Range(0, StaticValues.ItemLevelTitanforge + 1)));
        }
    }

    protected bool CreateEnemy(string name, int addHealth, Enums.EncounterEnemyType enemyType)
    {
        int index = -1;
        for (int i = 0; i < StaticValues.MaxNumberOfAliveAdds; i++)
        {
            if (m_enemies.FindAll(x => x.Index == i).Count == 0)
            {
                index = i;
                break;
            }
        }

        if (index != -1)
        {
            GameObject addGO = GameObject.Instantiate(m_healthBarPrefab);
            addGO.SetActive(true);
            addGO.name = name + index;
            addGO.transform.SetParent(m_rsc.canvas.transform);
            m_enemies.Add(new EncounterEnemy(name, addGO.GetComponent<HealthBarScript>(), addHealth, index, enemyType, this));
            return true;
        }
        return false;
    }

    public void TargetEnemy(int index)
    {
        EncounterEnemy currentTarget = m_enemies.Find(e => e.IsCurrentTarget());

        if (currentTarget != null)
        {
            m_enemies.Find(e => e.Index == index).Index = -1;
            currentTarget.Index = index;
        }
        else
        {
            EncounterEnemy newTarget = m_enemies.Find(e => e.Index == index);
            newTarget.Index = -1;
        }

        for (int i = 0; i < m_enemies.Count; i++)
        {
            m_enemies[i].ToggleTargetSetting();
        }
    }
}
