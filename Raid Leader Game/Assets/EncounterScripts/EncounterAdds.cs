using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterAdds {

    private string m_name;
    private HealthBarScript m_healthbar;
    private Enums.EncounterAdds m_addType;
    private int m_index;

    public string Name { get { return m_name; } }
    public HealthBarScript Healthbar { get { return m_healthbar; } }
    public Enums.EncounterAdds AddType { get { return m_addType; } }
    public int Index { get { return m_index; } }

    public EncounterAdds(string n, HealthBarScript hbs, int maxHealth, int i, Enums.EncounterAdds t)
    {
        m_index = i;
        m_name = n;
        m_addType = t;
        m_healthbar = hbs;
        m_healthbar.SetupHealthBar((m_index % 5) * 195 + 140, 310 - (m_index / 5) * 45, 40, 190, maxHealth);
        m_healthbar.SetUseName(m_name, true);
        m_healthbar.SetUseSingleLine(true);
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
