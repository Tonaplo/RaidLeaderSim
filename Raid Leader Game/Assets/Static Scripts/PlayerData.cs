﻿using UnityEngine;
using UnityEngine.Timeline;
using System.Globalization;
using System;
using System.Collections.Generic;

public static class PlayerData
{
    static List<Raider> m_roster;
    static List<Raider> m_raidTeam;
    static List<ConsumableItem> m_consumables;
    static List<RaidData> m_progress;
    static List<RaidData> m_weeklyLockOut;
    static List<RecruitInfo> m_recruitLockOut;
    static DateTime m_thisLockout;
    static int m_attemptsLeft = StaticValues.AttemptsPerDay;
    static Raider m_playerChar;
    static string m_raidTeamName;
    static int m_raidTeamGold = 0;
    static bool m_tutorialEnabled = false;

    public static List<Raider> Roster { get { return m_roster; } }
    public static List<Raider> RaidTeam { get { return m_raidTeam; } }
    public static List<ConsumableItem> Consumables { get { return m_consumables; } }
    public static List<RaidData> Progress { get { return m_progress; } }
    public static List<RaidData> WeeklyLockOut { get { return m_weeklyLockOut; } }
    public static List<RecruitInfo> RecruitLockOut { get { return m_recruitLockOut; } }
    public static DateTime ThisWeek { get { return m_thisLockout; } }
    public static int AttemptsLeft { get { return m_attemptsLeft; } }
    public static Raider PlayerCharacter { get { return m_playerChar; } }
    public static string RaidTeamName { get { return m_raidTeamName; } }
    public static int RaidTeamGold { get { return m_raidTeamGold; } }
    public static bool TutorialEnabled { get { return m_tutorialEnabled; } set { m_tutorialEnabled = value; } }
    
    public static void Initialize()
    {
        m_roster = new List<Raider>();
        m_raidTeam = new List<Raider>();
        m_consumables = new List<ConsumableItem>();
    }

    public static void InitializeDataFromSaveData(DataController.SaveData data)
    {
        m_playerChar = data.Player;

        if (data.Roster != null)
            m_roster = data.Roster;
        else
            m_roster = new List<Raider>();

        if (data.Consumables != null)
            m_consumables = data.Consumables;
        else
            m_consumables = new List<ConsumableItem>();

        if (data.ProgressData != null)
        {
            List<RaidData> FullNewData = RaidData.CreateNewRaidData();
            m_progress = data.ProgressData;

            //We need to add any new raids that where added in this update
            foreach (var newRaid in FullNewData)
            {
                bool found = false;
                foreach (var existingRaid in m_progress)
                {
                    if (existingRaid.m_raid == newRaid.m_raid)
                        found = true;
                }

                if (!found)
                    m_progress.Add(RaidData.CreateDataFromRaidEnum(newRaid.m_raid));
            }
        }
        else
            m_progress = RaidData.CreateNewRaidData();

        if (data.RecruitLockOut != null)
            m_recruitLockOut = data.RecruitLockOut;
        else
        {
            m_recruitLockOut = new List<RecruitInfo>() { new RecruitInfo(), new RecruitInfo(), new RecruitInfo(), new RecruitInfo(), };
            for (int i = 0; i < m_recruitLockOut.Count; i++)
            {
                m_recruitLockOut[i].Init(i);
            }
        }

        if (data.LockOutData != null)
        {
            List<RaidData> FullNewData = RaidData.CreateNewRaidData();
            m_weeklyLockOut = data.LockOutData;

            //We need to add any new raids that where added in this update
            foreach (var newRaid in FullNewData)
            {
                bool found = false;
                foreach (var existingRaid in m_weeklyLockOut)
                {
                    if (existingRaid.m_raid == newRaid.m_raid)
                        found = true;
                }

                if (!found)
                    m_weeklyLockOut.Add(RaidData.CreateDataFromRaidEnum(newRaid.m_raid));
            }
        }
        else
            m_weeklyLockOut = RaidData.CreateNewRaidData();

        SetRaidTeamName(data.TeamName);
        m_raidTeamGold = data.TeamGold;
        m_thisLockout = data.LockOutDate;
        m_attemptsLeft = data.AttemptsLeft;
        CheckWeeklyReset();
    }

    public static void GenerateNewGameRoster(Raider player, int baseLevel)
    {
        m_playerChar = player;
        List<Enums.CharacterSpec> tankSpecs = new List<Enums.CharacterSpec> { Enums.CharacterSpec.Knight, Enums.CharacterSpec.Guardian };
        List<Enums.CharacterSpec> healerSpecs = new List<Enums.CharacterSpec> { Enums.CharacterSpec.Cleric, Enums.CharacterSpec.Diviner, Enums.CharacterSpec.Naturalist };
        List<Enums.CharacterSpec> DPSSpecs = new List<Enums.CharacterSpec> { Enums.CharacterSpec.Assassin, Enums.CharacterSpec.Berserker, Enums.CharacterSpec.Elementalist, Enums.CharacterSpec.Necromancer, Enums.CharacterSpec.Ranger, Enums.CharacterSpec.Scourge, Enums.CharacterSpec.Wizard };
        int numTanks = 2;
        int numHealers = 1;
        int numDPS = 9;

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
            m_roster.Add(new Raider(names[namecounter++], RaiderStats.GenerateRaiderStatsFromSpec(tankSpecs[i % tankSpecs.Count], baseLevel, baseLevel)));
        }
        for (int i = 0; i < numHealers; i++)
        {
            m_roster.Add(new Raider(names[namecounter++], RaiderStats.GenerateRaiderStatsFromSpec(healerSpecs[i % healerSpecs.Count], baseLevel, baseLevel)));
        }
        for (int i = 0; i < numDPS; i++)
        {
            m_roster.Add(new Raider(names[namecounter++], RaiderStats.GenerateRaiderStatsFromSpec(DPSSpecs[i % DPSSpecs.Count], baseLevel, baseLevel)));
        }
        
        RecalculateRoster();
    }

    public static void FinalizeNewGameGeneration()
    {
        m_consumables = new List<ConsumableItem>();
        m_progress = RaidData.CreateNewRaidData();
        m_weeklyLockOut = RaidData.CreateNewRaidData();
        m_thisLockout = DateTime.Now;
        m_recruitLockOut = new List<RecruitInfo>() { new RecruitInfo(), new RecruitInfo(), new RecruitInfo(), new RecruitInfo(), };
        for (int i = 0; i < m_recruitLockOut.Count; i++)
        {
            m_recruitLockOut[i].Init(i);
        }
        m_attemptsLeft = StaticValues.AttemptsPerDay;
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

    public static void PurchaseAttempts()
    {
        if (m_raidTeamGold >= StaticValues.GoldCostOfAttempts)
        {
            m_raidTeamGold -= StaticValues.GoldCostOfAttempts;
            m_attemptsLeft += StaticValues.AttemptsPerDay;
            DataController.controller.Save();
        }
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

    public static void CheckWeeklyReset()
    {
        CheckWeeklyReset(DateTime.Now);
    }

    public static void ConsumeAttempt()
    {
        if(m_attemptsLeft > 0)
            m_attemptsLeft--;
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

    public static void EncounterBeaten(Enums.EncounterEnum e, Enums.Difficulties d)
    {
        foreach (var raid in m_progress)
        {
            foreach (var encounter in raid.m_encounters)
            {
                if (encounter.Encounter == e)
                {
                    switch (d)
                    {
                        case Enums.Difficulties.Easy:
                            encounter.BeatenOnEasy = true;
                            break;
                        case Enums.Difficulties.Normal:
                            encounter.BeatenOnNormal = true;
                            break;
                        case Enums.Difficulties.Hard:
                            encounter.BeatenOnHard = true;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        foreach (var raid in m_weeklyLockOut)
        {
            foreach (var encounter in raid.m_encounters)
            {
                if (encounter.Encounter == e)
                {
                    switch (d)
                    {
                        case Enums.Difficulties.Easy:
                            encounter.BeatenOnEasy = true;
                            break;
                        case Enums.Difficulties.Normal:
                            encounter.BeatenOnNormal = true;
                            break;
                        case Enums.Difficulties.Hard:
                            encounter.BeatenOnHard = true;
                            break;
                        default:
                            break;
                    }
                    DataController.controller.Save();
                    return;
                }
            }
        }

        Debug.LogAssertion("Encounter was not found in preexisting data! This is a huge error!");
    }

    //=======================================
    //           Internal Functions
    //=======================================

    static void CheckWeeklyReset(DateTime now)
    {
        Calendar cal = DateTimeFormatInfo.CurrentInfo.Calendar;
        if (cal.GetWeekOfYear(now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) != cal.GetWeekOfYear(m_thisLockout, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday))
        {
            m_weeklyLockOut = RaidData.CreateNewRaidData();
            m_attemptsLeft = StaticValues.AttemptsPerDay;
        }
        
        m_thisLockout = now;
    }

    //=======================================
    //           Debug Functions
    //=======================================
    public static void GenerateDebugRoster()
    {
        int averageSkill = 50;
        int averageGear = 60;
        m_roster = new List<Raider>();
        m_roster.Add(new Raider("Guardian", new RaiderStats(averageGear, averageSkill, 0, Enums.CharacterRole.Tank, Enums.CharacterClass.Fighter)));
        m_roster.Add(new Raider("Knight", new RaiderStats(averageGear, averageSkill, 0, Enums.CharacterRole.Tank, Enums.CharacterClass.Paladin)));
        m_roster.Add(new Raider("Cleric", new RaiderStats(averageGear, averageSkill, 0, Enums.CharacterRole.Healer, Enums.CharacterClass.Paladin)));
        m_roster.Add(new Raider("Diviner", new RaiderStats(averageGear, averageSkill, 0, Enums.CharacterRole.Healer, Enums.CharacterClass.Sorcerer)));
        m_roster.Add(new Raider("Naturalist", new RaiderStats(averageGear, averageSkill, 0, Enums.CharacterRole.Healer, Enums.CharacterClass.Totemic)));
        m_roster.Add(new Raider("Berserker", new RaiderStats(averageGear, averageSkill, 0, Enums.CharacterRole.MeleeDPS, Enums.CharacterClass.Fighter)));
        m_roster.Add(new Raider("Assassin", new RaiderStats(averageGear, averageSkill, 0, Enums.CharacterRole.MeleeDPS, Enums.CharacterClass.Shadow)));
        m_roster.Add(new Raider("Scourge", new RaiderStats(averageGear, averageSkill, 0, Enums.CharacterRole.MeleeDPS, Enums.CharacterClass.Occultist)));
        m_roster.Add(new Raider("Ranger", new RaiderStats(averageGear, averageSkill, 0, Enums.CharacterRole.RangedDPS, Enums.CharacterClass.Shadow)));
        m_roster.Add(new Raider("Wizard", new RaiderStats(averageGear, averageSkill, 0, Enums.CharacterRole.RangedDPS, Enums.CharacterClass.Sorcerer)));
        m_roster.Add(new Raider("Elementalist", new RaiderStats(averageGear, averageSkill, 0, Enums.CharacterRole.RangedDPS, Enums.CharacterClass.Totemic)));
        m_roster.Add(new Raider("Necromancer", new RaiderStats(averageGear, averageSkill, 0, Enums.CharacterRole.RangedDPS, Enums.CharacterClass.Occultist)));

        RecalculateRoster();

        m_raidTeam = m_roster;
    }

    //=======================================
    //           Cheat Functions
    //=======================================

    public static void SetSkillOfRaid(int value)
    {
        foreach (var raider in m_roster)
        {
            for (int i = 0; i < (int)Enums.SkillTypes.NumSkillTypes; i++)
            {
                raider.RaiderStats.Skills.ModifySkill(value, (Enums.SkillTypes)i);
            }
        }
    }

    public static void SetGearOfRaid(int value)
    {
        foreach (var raider in m_roster)
        {
            for (int i = 0; i < (int)Enums.GearTypes.NumGearTypes; i++)
            {
                CharacterItem item = new CharacterItem((Enums.GearTypes)i, value);
                raider.RaiderStats.Gear.AddGearPieceToSlot(item);
            }
        }
    }
}
