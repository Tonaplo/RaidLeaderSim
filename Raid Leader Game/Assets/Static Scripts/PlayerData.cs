﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class PlayerData
{
    static List<Raider> roster;
    static Raider playerChar;

    public static void Initialize()
    {
        roster = new List<Raider>();
        roster.Add(new Raider("Praerend", new RaiderStats(15, 15, 5, Enums.CharacterRole.Tank, Enums.CharacterClass.Fighter)));
        roster.Add(new Raider("Greybone", new RaiderStats(15, 15, 2, Enums.CharacterRole.Tank, Enums.CharacterClass.Paladin)));

        roster.Add(new Raider("Mallusof", new RaiderStats(15, 15, 2, Enums.CharacterRole.Healer, Enums.CharacterClass.Paladin)));
        roster.Add(new Raider("Amranar", new RaiderStats(15, 15, 5, Enums.CharacterRole.Healer, Enums.CharacterClass.Totemic)));
        roster.Add(new Raider("Granjior", new RaiderStats(15, 15, 7, Enums.CharacterRole.Healer, Enums.CharacterClass.Sorcerous)));
        roster.Add(new Raider("Farahn", new RaiderStats(15, 15, 9, Enums.CharacterRole.Healer, Enums.CharacterClass.Sorcerous)));

        roster.Add(new Raider("Morifa", new RaiderStats(15, 15, 2, Enums.CharacterRole.RangedDPS, Enums.CharacterClass.Sorcerous)));
        roster.Add(new Raider("Kaldorath", new RaiderStats(15, 15, 2, Enums.CharacterRole.MeleeDPS, Enums.CharacterClass.Fighter)));

        roster.Add(new Raider("Faerand", new RaiderStats(15, 15, 5, Enums.CharacterRole.RangedDPS, Enums.CharacterClass.Shadow)));
        roster.Add(new Raider("Rahran", new RaiderStats(15, 15, 5, Enums.CharacterRole.MeleeDPS, Enums.CharacterClass.Shadow)));

        roster.Add(new Raider("Fimwack", new RaiderStats(15, 15, 7, Enums.CharacterRole.RangedDPS, Enums.CharacterClass.Totemic)));
        roster.Add(new Raider("Miriyal", new RaiderStats(15, 15, 7, Enums.CharacterRole.MeleeDPS, Enums.CharacterClass.Shadow)));

        for (int i = 0; i < roster.Count; i++)
        {
            roster[i].CalculateMaxHealth();
        }
    }

    public static List<Raider> GetRoster()
    {
        return roster;
    }

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
