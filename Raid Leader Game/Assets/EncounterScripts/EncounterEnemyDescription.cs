using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterEnemyDescription {

    string m_name;
    string m_description;

    public string Name { get { return m_name; } }
    public string Description { get { return m_description; } }

    public EncounterEnemyDescription(string n, string d)
    {
        m_name = n;
        m_description = d;
    }


}
