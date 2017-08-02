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
        List<Enums.CharacterSpec> tankSpecs = new List<Enums.CharacterSpec> { Enums.CharacterSpec.Knight, Enums.CharacterSpec.Guardian };
        List<Enums.CharacterSpec> healerSpecs = new List<Enums.CharacterSpec> { Enums.CharacterSpec.Cleric, Enums.CharacterSpec.Diviner, Enums.CharacterSpec.Naturalist };
        List<Enums.CharacterSpec> DPSSpecs = new List<Enums.CharacterSpec> { Enums.CharacterSpec.Assassin, Enums.CharacterSpec.Berserker, Enums.CharacterSpec.Elementalist, Enums.CharacterSpec.Necromancer, Enums.CharacterSpec.Ranger, Enums.CharacterSpec.Scourge, Enums.CharacterSpec.Wizard };
        int numTanks = 2;
        int numHealers = 3;
        int numDPS = 7;

        switch (player.RaiderStats().GetRole())
        {
            case Enums.CharacterRole.Tank:
                numTanks--;
                tankSpecs.Remove(player.RaiderStats().GetCurrentSpec());
                break;
            case Enums.CharacterRole.Healer:
                numHealers--;
                healerSpecs.Remove(player.RaiderStats().GetCurrentSpec());
                break;
            case Enums.CharacterRole.RangedDPS:
            case Enums.CharacterRole.MeleeDPS:
                numDPS--;
                DPSSpecs.Remove(player.RaiderStats().GetCurrentSpec());
                break;
            default:
                break;
        }
        
        
        List<string> names = new List<string>();
        names.Add(player.GetName());
        Utility.GetRandomCharacterName(ref names, numDPS+numHealers+numTanks);
        names.Remove(player.GetName());

        roster = new List<Raider> { player };
        int namecounter = 0;
        for (int i = 0; i < numTanks; i++)
        {
            roster.Add(new Raider(names[namecounter++], RaiderStats.GenerateRaiderStatsFromSpec(tankSpecs[i % tankSpecs.Count], baseLevel)));
        }
        for (int i = 0; i < numHealers; i++)
        {
            roster.Add(new Raider(names[namecounter++], RaiderStats.GenerateRaiderStatsFromSpec(healerSpecs[i % healerSpecs.Count], baseLevel)));
        }
        for (int i = 0; i < numDPS; i++)
        {
            roster.Add(new Raider(names[namecounter++], RaiderStats.GenerateRaiderStatsFromSpec(DPSSpecs[i % DPSSpecs.Count], baseLevel)));
        }

        for (int i = 0; i < roster.Count; i++)
        {
            //roster[i].RaiderStats().SetTestValue();
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
