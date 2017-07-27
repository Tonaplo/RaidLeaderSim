using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseEncounter
{

    #region variables and getters and setters

    private string m_name;
    private RaidSceneController m_rsc;
    private List<RaiderScript> m_raid;
    private HealthBarScript m_healthBar;

    public string Name
    {
        get { return m_name; }
    }

    public HealthBarScript HealthBar
    {
        get { return m_healthBar; }
    }

    Enums.Difficulties m_difficulty = Enums.Difficulties.Normal;

    public Enums.Difficulties Difficulty
    {
        get { return m_difficulty; }
        set { m_difficulty = value; }
    }
    List<EncounterAbility> m_encounterAbilities = new List<EncounterAbility>();

    public List<EncounterAbility> EncounterAbilities
    {
        get { return m_encounterAbilities; }
        set { m_encounterAbilities = value; }
    }
    List<BaseCooldown> m_encounterCooldowns = new List<BaseCooldown>();

    public List<BaseCooldown> EncounterCooldowns
    {
        get { return m_encounterCooldowns; }
        set { m_encounterCooldowns = value; }
    }
    #endregion

    public BaseEncounter(   string name,
                            int health,
                            Enums.Difficulties difficulty, 
                            List<EncounterAbility> encounterAbilities, 
                            List<BaseCooldown> encounterCooldowns,
                            List<RaiderScript> raiders,
                            RaidSceneController rsc,
                            HealthBarScript healthBar)
    {
        m_name = name;
        m_difficulty = difficulty;
        m_encounterAbilities = encounterAbilities;
        m_encounterCooldowns = encounterCooldowns;
        m_raid = raiders;
        m_rsc = rsc;
        m_healthBar = healthBar;
        m_healthBar.SetupHealthBar(350, 375, 100, 600, health);
        m_healthBar.SetUseName(name, true);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool IsDead()
    {
        return HealthBar.IsDead();
    }

    public virtual void BeginEncounter() {
        for (int i = 0; i < m_raid.Count; i++)
        {
            m_rsc.StartCoroutine(DoBasicAttack(2.5f + Random.Range(0, 2.5f), (int) (25 * Random.value), m_raid[i]));
        }
        
    }

    public virtual IEnumerator DoBasicAttack(float castTime, int damage, RaiderScript target) {
        yield return new WaitForSeconds(castTime);

        if (!target.IsDead() && !IsDead())
        {
            target.TakeDamage(damage);
            m_rsc.StartCoroutine(DoBasicAttack(2.5f + Random.Range(0, 2.5f), (int)(25 * Random.value), target));
        }
    }
}
