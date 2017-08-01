using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiderScript : MonoBehaviour {

    Raider m_raider;
    public HealthBarScript HealthBar;

    public Raider Raider { get { return m_raider; } }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public bool IsDead()
    {
        return HealthBar.IsDead();
    }

    public int GetHealth() { return (int)HealthBar.HealthBarSlider.value; }
    public int GetMaxHealth() { return m_raider.GetMaxHealth(); }

    public void Initialize(Raider raider, HealthBarScript hbs, Canvas parent, int index) {
        m_raider = raider;
        HealthBar = hbs;
        HealthBar.SetupHealthBar((index % 3) * 80 + 465, 310 - (index / 3) * 60, 100, 70, m_raider.GetMaxHealth());
        HealthBar.SetUseName(m_raider.GetName(), true);
        HealthBar.Fill.color = Utility.GetColorFromClass(m_raider.RaiderStats().GetClass());
    }

    public void StartFight(int index, float offset, Raider attacker, RaidSceneController rsc)
    {
        Enums.CharacterAttack attack = attacker.RaiderStats().GetBaseAttack();
        float castTime = Utility.GetAttackBaseValue(attack, Enums.AttackValueTypes.CastTime) + offset;
        int damage = attacker.RaiderStats().GetSpellAmount(Utility.GetAttackBaseValue(attack, Enums.AttackValueTypes.BaseDamageMultiplier));
        rsc.StartCoroutine(attacker.RaiderStats().DoAttack(castTime, damage, index, attacker, attack, rsc, this));

        if(attacker.RaiderStats().GetRole() == Enums.CharacterRole.Healer)
            rsc.StartCoroutine(attacker.RaiderStats().DoHeal(2.5f, this, index, rsc, rsc.GetRaid()));
    }

    public void TakeDamage(int damage) {
        HealthBar.ModifyHealth(-damage);
        if (IsDead())
            Die();
    }

    public void TakeHealing(int healing) {
        HealthBar.ModifyHealth(healing);
    }

    void Die()
    {
        enabled = false;
    }
}