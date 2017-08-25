using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealStruct {

    public float m_healMultiplier = 1.0f;
    public float m_deepHealingMultiplier = 0.0f;
    public float m_HoTMultiplier = 0.0f;

    public HealStruct() { }

        public HealStruct(HealStruct rhs)
    {
        m_healMultiplier = rhs.m_healMultiplier;
        m_deepHealingMultiplier = rhs.m_deepHealingMultiplier;
        m_HoTMultiplier = rhs.m_HoTMultiplier;
    }
}
