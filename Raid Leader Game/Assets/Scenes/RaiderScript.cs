using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaiderScript : MonoBehaviour {

    public HealthBarScript HealthBar;

    Raider m_raider;
    public Raider Raider { get { return m_raider; } }

    public Button HealthBarButton;
    RaidSceneController m_rsc;

    public class ActiveCooldowns
    {
        public ActiveCooldowns(BaseCooldown cd)
        {
            m_cooldown = cd;
            m_progress = 0.0f;
        }

        public BaseCooldown m_cooldown;
        public float m_progress = 0.0f;
    }

    List<ActiveCooldowns> m_activeCooldowns;
    BaseCooldown.CooldownEffects m_currentEffects;
    ConsumableItem m_activeConsumable = null;

    List<GameObject> m_activeDebuffs = new List<GameObject>();

    bool m_cooldownUsed = false;
    public bool CooldownUsed { get { return m_cooldownUsed; } }

    // Use this for initialization
    void Start() {
        m_activeCooldowns = new List<ActiveCooldowns>();
        m_currentEffects = new BaseCooldown.CooldownEffects();
    }

    // Update is called once per frame
    void Update() {
        List<ActiveCooldowns> toBeRemoved = new List<ActiveCooldowns>();
        foreach (var acd in m_activeCooldowns)
        {
            acd.m_progress += Time.deltaTime;

            if (acd.m_progress > acd.m_cooldown.Duration)
            {
                toBeRemoved.Add(acd);
                RemoveCooldown(acd);
            }
        }
        
        m_activeCooldowns.RemoveAll(x => toBeRemoved.Contains(x));
    }

    public bool IsDead()
    {
        return HealthBar.IsDead();
    }

    public bool IsBossDead()
    {
        return m_rsc.IsBossDead();
    }

    public int GetHealth() { return (int)HealthBar.CurrentHealth; }
    public int GetMaxHealth() {
        if(m_activeConsumable != null && m_activeConsumable.ConsumableType == Enums.ConsumableType.HealthIncrease)
            return Mathf.RoundToInt(m_raider.GetMaxHealth() * m_activeConsumable.GetMultiplier());
        else
            return m_raider.GetMaxHealth();
    }
    public float GetHealthPercent() { return ((float)HealthBar.CurrentHealth / (float)m_raider.GetMaxHealth()) * 100.0f;  }

    public void Initialize(Raider raider, HealthBarScript hbs, Canvas parent, int index) {
        m_raider = raider;
        HealthBar = hbs;
        HealthBar.SetupHealthBar(raider.GetName(), index, Enums.HealthBarSetting.Raider, m_raider.GetMaxHealth());
        HealthBar.Fill.color = Utility.GetColorFromClass(m_raider.RaiderStats.GetClass());
        HealthBarButton = HealthBar.BarButton;
        HealthBar.BarButton.onClick.AddListener( delegate () { AttemptToCounterAbility(); });
    }

    public IEnumerator StartFight(float offset, int index, Raider attacker, RaidSceneController rsc)
    {
        yield return new WaitForSeconds(offset);

        m_rsc = rsc;
        m_cooldownUsed = false;
        BaseHealOrAttackScript attackScript;
        attacker.RaiderStats.GetBaseAttackScript(out attackScript);
        attackScript.StartFight(index, attacker, this);

        if (attacker.RaiderStats.GetRole() == Enums.CharacterRole.Healer)
        {
            BaseHealScript healScript;
            attacker.RaiderStats.GetBaseHealingScript(out healScript);
            healScript.SetupHealScript(m_rsc);
            healScript.StartFight(index, attacker, this);
        }
    }

    public int DealDamage(int index, string attackName, DamageStruct ds)
    {
        int baseDamage = Raider.RaiderStats.GetAttackOrHealAmount();

        if (m_activeConsumable != null && m_activeConsumable.ConsumableType == Enums.ConsumableType.ThroughputIncrease)
            baseDamage = Mathf.RoundToInt(baseDamage * m_activeConsumable.GetMultiplier());

        float baseMultiplier = ds.m_baseMultiplier;

        int roll = UnityEngine.Random.Range(0, 100);
        int chance = ApplyCooldownCritChanceMultiplier(ds.m_baseCritChance);

        if (roll < chance)
            baseMultiplier *= ApplyCooldownCritEffectMultiplier(ds.m_baseCritEffect);

        baseMultiplier = ApplyCooldownDamageMultiplier(baseMultiplier);

        int damage = Mathf.RoundToInt(baseMultiplier * baseDamage);
        damage = (damage == 0) ? 1 : damage;

        /* Uncomment the below and enter the raid name of the class you want to see the performance of here. 
        if (Raider.GetName() == "Renmu")
        {
            Debug.Log("baseDamage: " + baseDamage + ", baseMultiplier: " + baseMultiplier + ", finaldamage: " + damage + ", critroll: " + roll + ", critchance: " + chance);
        }*/

        HandleLeechCooldownLeech(damage, index, ds.m_baseLeech);
        m_rsc.DealDamage(damage, this, index);
        return damage;
    }

    public int DoHealing(int index, string healName, ref HealStruct hs, RaiderScript target)
    {
        hs.m_deepHealingMultiplier = ApplyDeepHealingMultiplier(hs.m_deepHealingMultiplier);
        hs.m_HoTMultiplier = ApplyHoTMultiplier(hs.m_HoTMultiplier);

        int baseHealing = Raider.RaiderStats.GetAttackOrHealAmount();

        if (m_activeConsumable != null && m_activeConsumable.ConsumableType == Enums.ConsumableType.ThroughputIncrease)
            baseHealing = Mathf.RoundToInt(baseHealing * m_activeConsumable.GetMultiplier());

        if (!Mathf.Approximately(hs.m_deepHealingMultiplier, 0.0f))
        {
            hs.m_healMultiplier *= (((100.0f - target.GetHealthPercent()) / 100.0f) * hs.m_deepHealingMultiplier) + 1.0f;
        }

        int actualHealing = Mathf.RoundToInt(baseHealing * hs.m_healMultiplier);
        actualHealing = (actualHealing == 0) ? 1 : actualHealing;
        target.TakeHealing(healName, Raider.GetName(), index, actualHealing);
        return actualHealing;
    }

    public void TakeDamage(int damage, string attackName) {

        if (IsDead())
            return;

        float reduction = 0.0f;

        if (Raider.RaiderStats.Traits.Contains(Enums.TraitType.Cauticious))
            damage = Mathf.RoundToInt(damage * 0.9f);

        if (Raider.RaiderStats.Traits.Contains(Enums.TraitType.Clumsy))
            damage = Mathf.RoundToInt(damage * 1.1f);


        //Tanks take up for 50% reduced damage, depending on their skill.
        //The idea is that they mitigate better, the more skilled they are.
        if (Raider.RaiderStats.GetRole() == Enums.CharacterRole.Tank)
        {
            
            reduction += ((float)Raider.RaiderStats.Skills.GetSkillLevel(Enums.SkillTypes.Throughput) / (float)StaticValues.MaxSkill) * 0.5f;

            //Guardians take 15% reduced damage
            if (Raider.RaiderStats.GetCurrentSpec() == Enums.CharacterSpec.Guardian)
                reduction *= 1.15f;
        }

        damage = ApplyCooldownDamageReduction(damage, reduction);

        HealthBar.ModifyHealth(-damage);
        if (IsDead())
            Die(attackName);
    }

    public void TakeHealing(string healName, string healerName, int index, int healing) {
        if (IsDead())
            return;

        float priorValue = HealthBar.CurrentHealth;
        HealthBar.ModifyHealth(healing);

        int actualHealing = Mathf.RoundToInt(HealthBar.CurrentHealth - priorValue);
        m_rsc.DoHeal(actualHealing, healerName, healName, index);
    }

    void Die(string killedBy)
    {
        m_cooldownUsed = true;
        enabled = false;
        m_rsc.RaiderDied(Raider.GetName(), killedBy);
    }

    void AttemptToCounterAbility()
    {
        m_rsc.AttemptToCounterCurrentEncounterAbility(m_raider);
    }

    public void AddDebuff(GameObject debuff)
    {
        m_activeDebuffs.Add(debuff);
    }

    public void RemoveDebuff()
    {
        if (m_activeDebuffs.Count > 0)
        {
            Destroy(m_activeDebuffs[0]);
            m_activeDebuffs.RemoveAt(0);
        }
    }

    public bool HasDebuffs()
    {
        return m_activeDebuffs.Count > 0;
    }

    public bool UseCooldown()
    {
        if (m_rsc != null)
        {
            m_cooldownUsed = true;
            m_rsc.UseRaiderCooldown(this);
            return true;
        }
        return false;
    }

    public void AddCooldown(BaseCooldown cd)
    {
        m_currentEffects.m_castTimeMultiplier *= cd.Cooldowneffects.m_castTimeMultiplier;
        m_currentEffects.m_critChanceIncrease += cd.Cooldowneffects.m_critChanceIncrease;
        m_currentEffects.m_critEffectIncrease += cd.Cooldowneffects.m_critEffectIncrease;
        m_currentEffects.m_damageMultiplier *= cd.Cooldowneffects.m_damageMultiplier;
        m_currentEffects.m_damageReductionMultiplier += cd.Cooldowneffects.m_damageReductionMultiplier;
        m_currentEffects.m_deepHealingMultiplier += cd.Cooldowneffects.m_deepHealingMultiplier;
        m_currentEffects.m_healingMultiplier *= cd.Cooldowneffects.m_healingMultiplier;
        m_currentEffects.m_HoTMultiplier += cd.Cooldowneffects.m_HoTMultiplier;
        m_currentEffects.m_leechMultiplier += cd.Cooldowneffects.m_leechMultiplier;

        m_activeCooldowns.Add(new ActiveCooldowns(cd));
    }

    void RemoveCooldown(ActiveCooldowns cd)
    {
        m_currentEffects.m_castTimeMultiplier /= cd.m_cooldown.Cooldowneffects.m_castTimeMultiplier;
        m_currentEffects.m_critChanceIncrease -= cd.m_cooldown.Cooldowneffects.m_critChanceIncrease;
        m_currentEffects.m_critEffectIncrease -= cd.m_cooldown.Cooldowneffects.m_critEffectIncrease;
        m_currentEffects.m_damageMultiplier /= cd.m_cooldown.Cooldowneffects.m_damageMultiplier;
        m_currentEffects.m_damageReductionMultiplier -= cd.m_cooldown.Cooldowneffects.m_damageReductionMultiplier;
        m_currentEffects.m_deepHealingMultiplier -= cd.m_cooldown.Cooldowneffects.m_deepHealingMultiplier;
        m_currentEffects.m_healingMultiplier /= cd.m_cooldown.Cooldowneffects.m_healingMultiplier;
        m_currentEffects.m_HoTMultiplier -= cd.m_cooldown.Cooldowneffects.m_HoTMultiplier;
        m_currentEffects.m_leechMultiplier -= cd.m_cooldown.Cooldowneffects.m_leechMultiplier;
    }

    public void AddConsumable(ConsumableItem item)
    {
        m_activeConsumable = item;
        if (item.ConsumableType == Enums.ConsumableType.HealthIncrease)
        {
            HealthBar.SetNewMaxHealth(GetMaxHealth());
        }
    }

    public float ApplyCooldownCastTimeMultiplier(float multiplier)
    {
        if (m_activeConsumable != null && m_activeConsumable.ConsumableType == Enums.ConsumableType.CastTimeDecrease)
            multiplier *= m_activeConsumable.GetMultiplier();

        return multiplier * m_currentEffects.m_castTimeMultiplier;
    }

    float ApplyCooldownDamageMultiplier(float multiplier)
    {
        return multiplier * m_currentEffects.m_damageMultiplier;
    }

    float ApplyCooldownHealingMultiplier(float multiplier)
    {
        return multiplier * m_currentEffects.m_healingMultiplier;
    }

    int ApplyCooldownCritChanceMultiplier(int chance)
    {
        return chance + m_currentEffects.m_critChanceIncrease;
    }

    float ApplyCooldownCritEffectMultiplier(float multiplier)
    {
        return multiplier + m_currentEffects.m_critEffectIncrease;
    }

    float ApplyDeepHealingMultiplier(float multiplier)
    {
        return multiplier + m_currentEffects.m_deepHealingMultiplier;
    }

    float ApplyHoTMultiplier(float multiplier)
    {
        return multiplier + m_currentEffects.m_HoTMultiplier;
    }

    void HandleLeechCooldownLeech(int damage, int index, float leechMultiplier)
    {
        float finalMultiplier = (m_currentEffects.m_leechMultiplier + leechMultiplier);
        if (Mathf.Approximately(finalMultiplier, 0.0f))
            return;

        int finalHealing = Mathf.RoundToInt(damage * finalMultiplier);
        finalHealing = (finalHealing == 0) ? 1 : finalHealing;
        TakeHealing("Leech", m_raider.GetName(), index, finalHealing);
    }
    
    int ApplyCooldownDamageReduction(int damage, float currentReduction)
    {
       return Mathf.RoundToInt(damage * (1.0f - (currentReduction + m_currentEffects.m_damageReductionMultiplier)));
    }
}