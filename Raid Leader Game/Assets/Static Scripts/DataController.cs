﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DataController : MonoBehaviour
{

    [Serializable]
    public class SaveData
    {

        public Raider Player;
        public List<Raider> Roster;
        public List<ConsumableItem> Consumables;
        public string TeamName;
        public int TeamGold;

    }
    public static DataController controller;

    void Awake()
    {
        if (controller == null)
        {
            DontDestroyOnLoad(gameObject);
            controller = this;
        }
        else if (controller != this)
        {
            Destroy(gameObject);
        }
    }

    public void Save()
    {
        FileStream file = File.Open(Application.persistentDataPath + "/" + PlayerData.PlayerCharacter.GetName() + ".dat", FileMode.OpenOrCreate);
        BinaryFormatter bf = new BinaryFormatter();

        SaveData data = new SaveData();
        data.Player = PlayerData.PlayerCharacter;
        data.Roster = PlayerData.Roster;
        data.TeamName = PlayerData.RaidTeamName;
        data.TeamGold = PlayerData.RaidTeamGold;
        data.Consumables = PlayerData.Consumables;

        bf.Serialize(file, data);
        file.Close();
    }

    public bool Load(string healerName)
    {
        if (File.Exists(Application.persistentDataPath + "/" + healerName + ".dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + healerName + ".dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();

            PlayerData.InitializeDataFromSaveData(data.Player, data.Roster, data.Consumables, data.TeamName, data.TeamGold);

            return true;
        }
        return false;
    }
}
