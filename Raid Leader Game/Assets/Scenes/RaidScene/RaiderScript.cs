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
    public float GetHealthPercent() { return (HealthBar.HealthBarSlider.value / m_raider.GetMaxHealth()) * 100.0f;  }

    public void Initialize(Raider raider, HealthBarScript hbs, Canvas parent, int index) {
        m_raider = raider;
        HealthBar = hbs;
        HealthBar.SetupHealthBar((index % 3) * 80 + 465, 310 - (index / 3) * 60, 100, 70, m_raider.GetMaxHealth());
        HealthBar.SetUseName(m_raider.GetName(), true);
        HealthBar.Fill.color = Utility.GetColorFromClass(m_raider.RaiderStats().GetClass());
    }

    public IEnumerator StartFight(float offset, int index, Raider attacker, RaidSceneController rsc)
    {
        yield return new WaitForSeconds(offset);

        BaseHealOrAttackScript script;
        attacker.RaiderStats().GetBaseAttackScript(out script);
        script.StartFight(index, attacker, rsc, this);

        if (attacker.RaiderStats().GetRole() == Enums.CharacterRole.Healer)
        {
            BaseHealScript healScript;
            attacker.RaiderStats().GetBaseHealingScript(out healScript);
            healScript.StartFight(index, attacker, rsc, this);
        }
    }

    public void TakeDamage(int damage) {

        //Tanks take up for 50% reduced damage, depending on their skill.
        //The idea is that they mitigate better, the more skilled they are.
        if (Raider.RaiderStats().GetRole() == Enums.CharacterRole.Tank)
        {
            
            float reduction = ((float)Raider.RaiderStats().GetSkillThisAttempt() / (float)Enums.StaticValues.maxSkill) * 0.5f;

            //Guardians take 15% reduced damage
            if (Raider.RaiderStats().GetCurrentSpec() == Enums.CharacterSpec.Guardian)
                reduction *= 1.15f;

            //Debug.Log("Tank should have taken " + damage + ", but has " + reduction.ToString() + "% damage reduction, so will only take " + Mathf.RoundToInt(damage * (1.0f - reduction)));
            damage = Mathf.RoundToInt(damage * (1.0f - reduction));
        }

        HealthBar.ModifyHealth(-damage);
        if (IsDead())
            Die();
    }

    public int TakeHealing(int healing) {
        float priorValue = HealthBar.HealthBarSlider.value;
        HealthBar.ModifyHealth(healing);

        return Mathf.RoundToInt(HealthBar.HealthBarSlider.value - priorValue);
    }

    void Die()
    {
        enabled = false;
    }
}