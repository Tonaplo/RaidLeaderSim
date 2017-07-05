using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class PlayerData
{
    static List<BaseCharacter> roster;
    static BaseCharacter playerChar;

    public static List<BaseCharacter> GetRoster()
    {
        return roster;
    }

    public static bool IsNameDuplicateOfRosterNames(string name) {
        for (int i = 0; i < roster.Count; i++)
        {
            if (roster[i].GetName() == name)
                return true;
        }
        return false;
    }
}
