using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterEnemy {

    private string m_name;
    private HealthBarScript m_healthbar;
    private Enums.EncounterEnemyType m_enemyType;
    private int m_index;
    private BaseEncounter m_encounter;

    public string Name { get { return m_name; } }
    public HealthBarScript Healthbar { get { return m_healthbar; } }
    public Enums.EncounterEnemyType EnemyType { get { return m_enemyType; } }
    public int Index { get { return m_index; } set { m_index = value; } }

    public EncounterEnemy(string n, HealthBarScript hbs, int maxHealth, int i, Enums.EncounterEnemyType et, BaseEncounter e)
    {
        m_index = i;
        m_name = n;
        m_healthbar = hbs;
        m_enemyType = et;
        m_encounter = e;
        m_healthbar.SetupHealthBar(m_name, i, Enums.HealthBarSetting.Enemy, maxHealth);
        Healthbar.BarButton.onClick.AddListener(() =>  TargetMe() );
        Healthbar.BarButton.interactable = true;
    }

    void TargetMe()
    {
        m_encounter.TargetEnemy(m_index);
    }

    public bool IsCurrentTarget() { return Healthbar.HealthBarSetting == Enums.HealthBarSetting.CurrentTarget; }

    public void ToggleTargetSetting()
    {
        Enums.HealthBarSetting oldSetting = Healthbar.HealthBarSetting;
        Enums.HealthBarSetting newSetting = (m_index == -1) ? Enums.HealthBarSetting.CurrentTarget : Enums.HealthBarSetting.Enemy;
        Healthbar.ChangeHealthbarSetting(newSetting, m_index);

        if (oldSetting != newSetting) {
            if (oldSetting == Enums.HealthBarSetting.CurrentTarget)
            {
                Healthbar.BarButton.onClick.AddListener(() => TargetMe());
                Healthbar.BarButton.interactable = true;
            }
            else
                Healthbar.BarButton.onClick.RemoveAllListeners();
        }

    }

    public void DestroyHealthBar()
    {
        m_healthbar.StartCoroutine(DestroyHealthBarInternal(2.0f));
    }

    IEnumerator DestroyHealthBarInternal(float castTime)
    {
        yield return new WaitForSeconds(castTime);
        Object.Destroy(m_healthbar.gameObject);
    }
}
