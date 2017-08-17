using UnityEngine;
using System;

[Serializable]
public class CharacterItem {

    Enums.GearTypes m_geartype;
    int m_itemlevel;

    public Enums.GearTypes GearSlot { get { return m_geartype; } }
    public int ItemLevel { get { return m_itemlevel; } }

    public CharacterItem(Enums.GearTypes t, int i)
    {
        m_geartype = t;
        SetItemLevel(i);
    }

    public CharacterItem(int i)
    {
        m_geartype = (Enums.GearTypes)(UnityEngine.Random.Range(0, (int)Enums.GearTypes.NumGearTypes));
        SetItemLevel(i);
    }

    void SetItemLevel(int i)
    {
        if (i > 0)
            m_itemlevel = i;
        else
            m_itemlevel = 1;
    }
}
