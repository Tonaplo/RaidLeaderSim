using System;
using UnityEngine;

[Serializable]
public class GearStats {

    CharacterItem[] m_gearItemLevels = new CharacterItem[(int)Enums.GearTypes.NumGearTypes];
    int m_averageItemLevel = 0;
    int m_totalItemLevel = 0;

    public int AverageItemLevel { get { return m_averageItemLevel; } }
    public int TotalItemLevel { get { return m_totalItemLevel; } }

    public GearStats(int itemLevel) {
        for (int i = 0; i < (int)Enums.GearTypes.NumGearTypes; i++)
        {
            m_gearItemLevels[i] = new CharacterItem((Enums.GearTypes) i, itemLevel);
        }
        CalculateItemlevels();
    }

    public GearStats(int[] itemLevel)
    {
        for (int i = 0; i < (int)Enums.GearTypes.NumGearTypes; i++)
        {
            m_gearItemLevels[i] = new CharacterItem((Enums.GearTypes)i, itemLevel[i]);
        }
        CalculateItemlevels();
    }

    public void CalculateItemlevels()
    {
        CalculateAverageItemlevel();
        CalculateTotalItemlevel();
    }

    void CalculateAverageItemlevel()
    {
        int average = 0;
        for (int i = 0; i < (int)Enums.GearTypes.NumGearTypes; i++)
        {
            average += m_gearItemLevels[i].ItemLevel;
        }
        m_averageItemLevel = Mathf.RoundToInt((float)average / (float) Enums.GearTypes.NumGearTypes);
    }

    void CalculateTotalItemlevel()
    {
        m_totalItemLevel = 0;
        for (int i = 0; i < (int)Enums.GearTypes.NumGearTypes; i++)
        {
            m_totalItemLevel += m_gearItemLevels[i].ItemLevel;
        }
    }

    public void AddGearPieceToSlot(CharacterItem item)
    {
        m_gearItemLevels[(int)item.GearSlot] = item;

        CalculateItemlevels();

        DataController.controller.Save();
    }

    public int CompareNewItemToSlot(CharacterItem item)
    {
        return (item.ItemLevel - m_gearItemLevels[(int)item.GearSlot].ItemLevel);
    }

    public int GetItemLevelOfSlot(Enums.GearTypes slot)
    {
        return m_gearItemLevels[(int)slot].ItemLevel;
    }
}
