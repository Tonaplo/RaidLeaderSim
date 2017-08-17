using System;
using UnityEngine;

[Serializable]
public class SkillStats {

    int[] m_skillLevels = new int[(int)Enums.SkillTypes.NumSkillTypes];
    int m_averageSkillLevel = 0;

    public int AverageSkillLevel { get { return m_averageSkillLevel; } }

    public SkillStats(int baseLevel)
    {
        for (int i = 0; i < (int)Enums.SkillTypes.NumSkillTypes; i++)
        {
            m_skillLevels[i] = baseLevel;
        }
        CalculateAverageSkillLevel();
    }

    public SkillStats(int[] skillLevels)
    {
        for (int i = 0; i < (int)Enums.SkillTypes.NumSkillTypes; i++)
        {
            m_skillLevels[i] = skillLevels[i];
        }
        CalculateAverageSkillLevel();
    }

    public void CalculateAverageSkillLevel()
    {
        int average = 0;
        for (int i = 0; i < (int)Enums.SkillTypes.NumSkillTypes; i++)
        {
            average += m_skillLevels[i];
        }
        m_averageSkillLevel = Mathf.RoundToInt((float)average / (float)Enums.SkillTypes.NumSkillTypes);
    }

    public void ModifySkill(int newSkillLevel, Enums.SkillTypes skillType)
    {
        if (newSkillLevel < 0)
            m_skillLevels[(int)skillType] = 0;
        else if (newSkillLevel > StaticValues.MaxSkill)
            m_skillLevels[(int)skillType] = StaticValues.MaxSkill;
        else
            m_skillLevels[(int)skillType] = newSkillLevel;

        CalculateAverageSkillLevel();
    }

    public void TrainingFinished()
    {
        int slot = UnityEngine.Random.Range(0, (int)Enums.SkillTypes.NumSkillTypes);
        ModifySkill(m_skillLevels[slot] + 1, (Enums.SkillTypes)slot);
    }

    public int GetSkillLevel(Enums.SkillTypes skillType)
    {
        return m_skillLevels[(int)skillType];
    }
}
