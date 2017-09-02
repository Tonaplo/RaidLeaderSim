using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class PlayerData
{
    static List<Raider> m_roster;
    static List<Raider> m_raidTeam;
    static List<ConsumableItem> m_consumables;
    static Raider m_playerChar;
    static string m_raidTeamName;
    static int m_raidTeamGold = 0;
    
    public static List<Raider> Roster { get { return m_roster; } }
    public static List<Raider> RaidTeam { get { return m_raidTeam; } }
    public static List<ConsumableItem> Consumables { get { return m_consumables; } }
    public static Raider PlayerCharacter { get { return m_playerChar; } }
    public static string RaidTeamName { get { return m_raidTeamName; } }
    public static int RaidTeamGold { get { return m_raidTeamGold; } }


    public static void Initialize()
    {
        m_roster = new List<Raider>();
        m_raidTeam = new List<Raider>();
        m_consumables = new List<ConsumableItem>();
    }

    public static void InitializeDataFromSaveData(Raider player, List<Raider> r, List<ConsumableItem> c, string n, int g)
    {
        if (r != null)
            m_roster = r;
        else
            m_roster = new List<Raider>();

        if (c != null)
            m_consumables = c;
        else
            m_consumables = new List<ConsumableItem>();
        
        m_playerChar = player;
        SetRaidTeamName(n);
        m_raidTeamGold = g;
    }

    public static void GenerateNewGameRoster(Raider player, int baseLevel)
    {
        m_playerChar = player;
        List<Enums.CharacterSpec> tankSpecs = new List<Enums.CharacterSpec> { Enums.CharacterSpec.Knight, Enums.CharacterSpec.Guardian };
        List<Enums.CharacterSpec> healerSpecs = new List<Enums.CharacterSpec> { Enums.CharacterSpec.Cleric, Enums.CharacterSpec.Diviner, Enums.CharacterSpec.Naturalist };
        List<Enums.CharacterSpec> DPSSpecs = new List<Enums.CharacterSpec> { Enums.CharacterSpec.Assassin, Enums.CharacterSpec.Berserker, Enums.CharacterSpec.Elementalist, Enums.CharacterSpec.Necromancer, Enums.CharacterSpec.Ranger, Enums.CharacterSpec.Scourge, Enums.CharacterSpec.Wizard };
        int numTanks = 2;
        int numHealers = 3;
        int numDPS = 7;

        switch (player.RaiderStats.GetRole())
        {
            case Enums.CharacterRole.Tank:
                numTanks--;
                tankSpecs.Remove(player.RaiderStats.GetCurrentSpec());
                break;
            case Enums.CharacterRole.Healer:
                numHealers--;
                healerSpecs.Remove(player.RaiderStats.GetCurrentSpec());
                break;
            case Enums.CharacterRole.RangedDPS:
            case Enums.CharacterRole.MeleeDPS:
                numDPS--;
                DPSSpecs.Remove(player.RaiderStats.GetCurrentSpec());
                break;
            default:
                break;
        }
        
        
        List<string> names = new List<string>();
        names.Add(player.GetName());
        Utility.GetRandomCharacterName(ref names, numDPS+numHealers+numTanks);
        names.Remove(player.GetName());

        m_roster = new List<Raider> { player };
        int namecounter = 0;
        for (int i = 0; i < numTanks; i++)
        {
            m_roster.Add(new Raider(names[namecounter++], RaiderStats.GenerateRaiderStatsFromSpec(tankSpecs[i % tankSpecs.Count], baseLevel)));
        }
        for (int i = 0; i < numHealers; i++)
        {
            m_roster.Add(new Raider(names[namecounter++], RaiderStats.GenerateRaiderStatsFromSpec(healerSpecs[i % healerSpecs.Count], baseLevel)));
        }
        for (int i = 0; i < numDPS; i++)
        {
            m_roster.Add(new Raider(names[namecounter++], RaiderStats.GenerateRaiderStatsFromSpec(DPSSpecs[i % DPSSpecs.Count], baseLevel)));
        }
        
        RecalculateRoster();
    }

    public static void RecalculateRoster()
    {
        for (int i = 0; i < m_roster.Count; i++)
        {
            m_roster[i].RecalculateRaider();
        }
    }

    public static void SortRoster()
    {

        List<Raider> tanks = m_roster.FindAll(x => x.RaiderStats.GetRole() == Enums.CharacterRole.Tank);
        List<Raider> healers = m_roster.FindAll(x => x.RaiderStats.GetRole() == Enums.CharacterRole.Healer);
        List<Raider> dps = m_roster.FindAll(x => x.RaiderStats.GetRole() == Enums.CharacterRole.RangedDPS || x.RaiderStats.GetRole() == Enums.CharacterRole.MeleeDPS);

        m_roster.Clear();

        m_roster.AddRange(tanks);
        m_roster.AddRange(healers);
        m_roster.AddRange(dps);
        /*m_roster.Sort(delegate (Raider x, Raider y)
        {
            if (x.GetName() == y.GetName())
                return 0;

            Enums.CharacterRole xRole = x.RaiderStats.GetRole();
            Enums.CharacterRole yRole = y.RaiderStats.GetRole();

            bool isXDPS = (xRole == Enums.CharacterRole.MeleeDPS) || (xRole == Enums.CharacterRole.RangedDPS);
            bool isYDPS = (yRole == Enums.CharacterRole.MeleeDPS) || (yRole == Enums.CharacterRole.RangedDPS);

            if (xRole == yRole || (isXDPS && isYDPS))
                return 0;

            int result = 0;
            if (xRole == Enums.CharacterRole.Tank && yRole != Enums.CharacterRole.Tank)
                result = 1;
            if (yRole == Enums.CharacterRole.Tank && xRole != Enums.CharacterRole.Tank)
                result = -1;
            if (xRole == Enums.CharacterRole.Healer && yRole != Enums.CharacterRole.Healer)
                result = 1;
            if (yRole == Enums.CharacterRole.Healer && xRole != Enums.CharacterRole.Healer)
                result = -1;
            
            return result;
        });*/
    }

    public static void SortRaidForLoot(Enums.GearTypes slot)
    {
        m_raidTeam.Sort(delegate (Raider x, Raider y)
        {
            int xILevel = x.RaiderStats.Gear.GetItemLevelOfSlot(slot);
            int yILevel = y.RaiderStats.Gear.GetItemLevelOfSlot(slot);

            if (xILevel > yILevel)
                return 1;
            else if (xILevel < yILevel)
                return -1;
            
            return 0;
        });
    }

    public static void ClearCurrentRaidTeam()
    {
        m_raidTeam.Clear();
    }

    public static bool CanRaidWithRoster()
    {
        int count = 0;
        for (int i = 0; i < Roster.Count; i++)
        {
            if (Roster[i].IsEligibleForActivity())
                count++;
        }

        return (count >= StaticValues.RaidTeamSize);
    }

    public static void AddRaiderToRaidTeam(Raider r)
    {
        m_raidTeam.Add(r);
    }

    public static void AddRecruitToRoster(Raider recruit)
    {
        m_roster.Add(recruit);
        DataController.controller.Save();
    }

    public static void AddPlayerToRoster(Raider player)
    {
        m_playerChar = player;
        AddRecruitToRoster(player);
        DataController.controller.Save();
    }

    public static void RemoveMemberFromRoster(Raider r)
    {
        if (r == m_playerChar)
            return;

        m_roster.Remove(r);
        DataController.controller.Save();
    }

    public static void PurchaseConsumable(ConsumableItem item)
    {
        if (item.Cost > m_raidTeamGold)
            return;

        m_raidTeamGold -= item.Cost;
        m_consumables.Add(item);
        DataController.controller.Save();
    }

    public static bool UseConsumable(ConsumableItem item)
    {

        if (m_consumables.FindAll(x => x.Name == item.Name).Count != 0)
        {
            m_consumables.Remove(m_consumables.Find(x => x.Name == item.Name));
            DataController.controller.Save();
            return true;
        }
        
        return false;
    }

    public static int GetRosterAverageItemLevel()
    {
        int average = 0;

        for (int i = 0; i < m_roster.Count; i++)
        {
            average += m_roster[i].RaiderStats.Gear.AverageItemLevel;
        }

        return Mathf.RoundToInt(average/ m_roster.Count);
    }

    public static int GetRosterAverageSkillLevel()
    {
        int average = 0;

        for (int i = 0; i < m_roster.Count; i++)
        {
            average += m_roster[i].RaiderStats.Skills.AverageSkillLevel;
        }

        return Mathf.RoundToInt(average / m_roster.Count);
    }

    public static int GetRosterAverageSkillAndItemLevel()
    {
        int average = 0;

        for (int i = 0; i < m_roster.Count; i++)
        {
            average += m_roster[i].RaiderStats.Skills.AverageSkillLevel;
            average += m_roster[i].RaiderStats.Gear.AverageItemLevel;
        }

        return Mathf.RoundToInt(average / (m_roster.Count*2));
    }

    public static void SetRaidTeamName(string newTeamName)
    {
        m_raidTeamName = newTeamName;
    }

    public static void AwardGold(int gold)
    {
        if (gold < 0)
            return;
        
        m_raidTeamGold += gold;
        DataController.controller.Save();
    }

    public static bool IsNameDuplicateOfRosterNames(string name)
    {
        for (int i = 0; i < Roster.Count; i++)
        {
            if (Roster[i].GetName() == name)
                return true;
        }
        return false;
    }

    //=======================================
    //           Debug Functions
    //=======================================
    public static void GenerateDebugRoster()
    {
        Raider player = new Raider("Mallusof", RaiderStats.GenerateRaiderStatsFromSpec(Enums.CharacterSpec.Naturalist, 10));
        GenerateNewGameRoster(player, 10);
        m_raidTeam = new List<Raider>(m_roster);
    }
}
