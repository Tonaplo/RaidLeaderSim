using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConsumableItem {

    Enums.ConsumableType m_type;
    Enums.ConsumableRarity m_rarity;
    float m_throughputMultiplier = 1.0f;
    float m_castTimeMultiplier = 1.0f;
    float m_healthMultiplier = 1.0f;
    int m_cost = 0;
    string m_name;

    public int Cost { get { return m_cost; } }
    public string Name { get { return m_name; } }
    public Enums.ConsumableType ConsumableType { get { return m_type; } }
    public Enums.ConsumableRarity Rarity { get { return m_rarity; } }

    public ConsumableItem() { }

    public ConsumableItem(ConsumableItem rhs)
    {
        m_type = rhs.m_type;
        m_rarity = rhs.m_rarity;
        m_throughputMultiplier = rhs.m_throughputMultiplier;
        m_castTimeMultiplier = rhs.m_castTimeMultiplier;
        m_healthMultiplier = rhs.m_healthMultiplier;
        m_cost = rhs.m_cost;
        m_name = rhs.m_name;
    }

    public string GetMultiplierString()
    {
        switch (m_type)
        {
            case Enums.ConsumableType.ThroughputIncrease:
            default:
                return "+" + Utility.GetPercentIncreaseString(m_throughputMultiplier) + " throughput.";
            case Enums.ConsumableType.CastTimeDecrease:
                return "-" + Utility.GetPercentString(1.0f- m_castTimeMultiplier) + " cast time";
            case Enums.ConsumableType.HealthIncrease:
                return "+" + Utility.GetPercentIncreaseString(m_healthMultiplier) + " health.";
        }
    }

    public float GetMultiplier()
    {
        switch (m_type)
        {
            case Enums.ConsumableType.ThroughputIncrease:
            default:
                return m_throughputMultiplier;
            case Enums.ConsumableType.CastTimeDecrease:
                return m_castTimeMultiplier;
            case Enums.ConsumableType.HealthIncrease:
                return m_healthMultiplier;
        }
    }

    public void Initialize(Enums.ConsumableType t, Enums.ConsumableRarity r)
    {
        m_type = t;
        m_rarity = r;

        switch (m_type)
        {
            case Enums.ConsumableType.ThroughputIncrease:
            default:
                InitializeThroughPutType();
                break;
            case Enums.ConsumableType.CastTimeDecrease:
                InitializeCastTimeType();
                break;
            case Enums.ConsumableType.HealthIncrease:
                InitializeHealthType();
                break;
        }
    }

    void InitializeThroughPutType()
    {
        switch (m_rarity)
        {
            case Enums.ConsumableRarity.Normal:
            default:
                m_name = "Power Pills";
                m_cost = 40;
                m_throughputMultiplier = 1.05f;
                break;
            case Enums.ConsumableRarity.Rare:
                m_name = "Blessing of Power";
                m_cost = 70;
                m_throughputMultiplier = 1.12f;
                break;
            case Enums.ConsumableRarity.Epic:
                m_name = "Power Infusion";
                m_cost = 110;
                m_throughputMultiplier = 1.20f;
                break;
        }
    }

    void InitializeHealthType()
    {
        switch (m_rarity)
        {
            case Enums.ConsumableRarity.Normal:
            default:
                m_name = "Hearthy Meal";
                m_cost = 40;
                m_healthMultiplier = 1.05f;
                break;
            case Enums.ConsumableRarity.Rare:
                m_name = "Blessing of Fortitude";
                m_cost = 70;
                m_healthMultiplier = 1.12f;
                break;
            case Enums.ConsumableRarity.Epic:
                m_name = "Potion of Invincibility";
                m_cost = 110;
                m_healthMultiplier = 1.20f;
                break;
        }
    }

    void InitializeCastTimeType()
    {
        switch (m_rarity)
        {
            case Enums.ConsumableRarity.Normal:
            default:
                m_name = "Sugar Tablet";
                m_cost = 40;
                m_castTimeMultiplier = 0.97f;
                break;
            case Enums.ConsumableRarity.Rare:
                m_name = "Blessing of Speed";
                m_cost = 70;
                m_castTimeMultiplier = 0.94f;
                break;
            case Enums.ConsumableRarity.Epic:
                m_name = "Heroism";
                m_cost = 110;
                m_castTimeMultiplier = 0.90f;
                break;
        }
    }
}
