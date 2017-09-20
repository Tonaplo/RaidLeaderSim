using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RecruitInfo {
    Raider m_recruit;
    bool m_isDisabled = false;
    DateTime m_lockoutDate;
    int m_index;

    public Raider Recruit { get{ return m_recruit; } }
    public bool IsDisabled { get { return m_isDisabled; } }
    public DateTime LockoutDate { get { return m_lockoutDate; } }
    public int Index { get { return m_index; } }

    public RecruitInfo()
    {
        m_recruit = new Raider("random", RaiderStats.GenerateRaiderStatsFromClass(Enums.CharacterClass.Fighter, 5, 5));
    }

    public void Init(int index)
    {
        m_index = index;
        GenerateNewRecruit();
    }

    public void CheckForNewRecruit()
    {
        if (m_isDisabled && m_lockoutDate != DateTime.Now.Date)
        {
            GenerateNewRecruit();
        }
    }

    public void Disable()
    {
        m_lockoutDate = DateTime.Now.Date;
        m_isDisabled = true;
        DataController.controller.Save();
    }

    void GenerateNewRecruit()
    {
        List<string> previousNames = new List<string>();
        for (int i = 0; i < PlayerData.Roster.Count; i++)
        {
            previousNames.Add(PlayerData.Roster[i].GetName());
        }

        for (int i = 0; i < PlayerData.RecruitLockOut.Count; i++)
        {
            previousNames.Add(PlayerData.RecruitLockOut[i].Recruit.GetName());
        }

        List<string> newNameList = new List<string>(previousNames);
        Utility.GetRandomCharacterName(ref newNameList, 1);
        newNameList.RemoveAll(x => previousNames.Contains(x));

        m_recruit = new Raider(newNameList[0], RaiderStats.GenerateRaiderStatsFromSpec((Enums.CharacterSpec)UnityEngine.Random.Range(0, (int)Enums.CharacterSpec.Necromancer + 1), PlayerData.GetRosterAverageSkillLevel(), PlayerData.GetRosterAverageItemLevel()));
        m_isDisabled = false;
        DataController.controller.Save();
    }
}
