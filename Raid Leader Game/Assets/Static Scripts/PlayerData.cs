using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class PlayerData
{
    static List<Raider> roster;
    static Raider playerChar;

    public static void Initialize()
    {
        
    }

    public static void InitializeDataFromSaveData(Raider player, List<Raider> r)
    {
        roster = r;
        playerChar = player;
    }

    public static void GenerateNewGameRoster(Raider player, int baseLevel)
    {
        playerChar = player;
        int numTanks = 2;
        int numHealers = 4;
        int numDPS = 6;

        switch (player.RaiderStats().GetRole())
        {
            case Enums.CharacterRole.Tank:
                numTanks--;
                break;
            case Enums.CharacterRole.Healer:
                numHealers--;
                break;
            case Enums.CharacterRole.RangedDPS:
            case Enums.CharacterRole.MeleeDPS:
                numDPS--;
                break;
            default:
                break;
        }
        
        
        List<string> names = new List<string>();
        names.Add(player.GetName());
        Utility.GetRandomCharacterName(ref names, numDPS+numHealers+numTanks);
        names.Remove(player.GetName());

        roster = new List<Raider>();
        roster.Add(player);
        int namecounter = 0;
        for (int i = 0; i < numTanks; i++)
        {
            roster.Add(new Raider(names[namecounter++], RaiderStats.GenerateRaiderStatsFromRole(Enums.CharacterRole.Tank, baseLevel)));
        }
        for (int i = 0; i < numHealers; i++)
        {
            roster.Add(new Raider(names[namecounter++], RaiderStats.GenerateRaiderStatsFromRole(Enums.CharacterRole.Healer, baseLevel)));
        }
        int randomValue = 0;
        for (int i = 0; i < numDPS; i++)
        {
            randomValue = Random.Range(0, 1);
            if(randomValue == 0)
                roster.Add(new Raider(names[namecounter++], RaiderStats.GenerateRaiderStatsFromRole(Enums.CharacterRole.MeleeDPS, baseLevel)));
            else
                roster.Add(new Raider(names[namecounter++], RaiderStats.GenerateRaiderStatsFromRole(Enums.CharacterRole.RangedDPS, baseLevel)));
        }

        for (int i = 0; i < roster.Count; i++)
        {
            roster[i].CalculateMaxHealth();
        }
    }

    public static List<Raider> GetRoster()
    {
        return roster;
    }

    public static Raider GetPlayerCharacter() { return playerChar; }

    public static bool IsNameDuplicateOfRosterNames(string name)
    {
        for (int i = 0; i < roster.Count; i++)
        {
            if (roster[i].GetName() == name)
                return true;
        }
        return false;
    }
}
