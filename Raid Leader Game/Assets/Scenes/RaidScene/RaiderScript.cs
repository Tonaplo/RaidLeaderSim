﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiderScript : MonoBehaviour {

    Raider m_raider;
    public HealthBarScript HealthBar;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public bool IsDead()
    {
        return HealthBar.HealthBarSlider.value <= 0;
    }

    public int GetHealth() { return (int)HealthBar.HealthBarSlider.value; }
    public int GetMaxHealth() { return m_raider.GetMaxHealth(); }

    public void Initialize(Raider raider, HealthBarScript hbs, Canvas parent, int index) {
        m_raider = raider;
        HealthBar = hbs;
        HealthBar.SetupHealthBar((index % 3) * 80 + 465, 310 - (index / 3) * 60, 100, 70, m_raider.GetMaxHealth());
        HealthBar.SetUseName(m_raider.GetName(), true);
        switch (m_raider.RaiderStats().GetRole())
        {
            case Enums.CharacterRole.Tank:
                HealthBar.Fill.color = Color.grey;
                break;
            case Enums.CharacterRole.Healer:
                HealthBar.Fill.color = Color.green;
                break;
            case Enums.CharacterRole.RangedDPS:
                HealthBar.Fill.color = Color.cyan;
                break;
            case Enums.CharacterRole.MeleeDPS:
                HealthBar.Fill.color = Color.red;
                break;
            default:
                break;
        }
    }

    public void StartFight(int index, Raider attacker, RaidSceneController rsc)
    {
        Enums.CharacterAttack attack = attacker.RaiderStats().GetBaseAttack();
        float castTime = Utility.GetAttackBaseValue(attack, Enums.AttackValueTypes.CastTime);
        int damage = attacker.RaiderStats().GetSpellAmount(Utility.GetAttackBaseValue(attack, Enums.AttackValueTypes.BaseDamageMultiplier));
        rsc.StartCoroutine(attacker.RaiderStats().DoAttack(castTime, damage, index, attacker, rsc, this));

        if(attacker.RaiderStats().GetRole() == Enums.CharacterRole.Healer)
            rsc.StartCoroutine(attacker.RaiderStats().DoHeal(2.5f, this, index, rsc, rsc.GetRaid()));
    }

    public void TakeDamage(int damage) {
        HealthBar.ModifyHealth(-damage);
    }
}