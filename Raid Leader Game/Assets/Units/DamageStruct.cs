using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageStruct
{
    public DamageStruct() { }

        public DamageStruct(DamageStruct rhs)
    {
        m_baseMultiplier = rhs.m_baseMultiplier;
        m_baseLeech = rhs.m_baseLeech;
        m_baseCritEffect = rhs.m_baseCritEffect;
        m_baseCritChance = rhs.m_baseCritChance;
    }

    public float m_baseMultiplier = 1.0f;
    public float m_baseLeech = 0.0f;
    public float m_baseCritEffect = 1.0f;
    public int m_baseCritChance = 0;
}
