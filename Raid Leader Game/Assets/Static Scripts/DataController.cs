using UnityEngine;
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
        public List<RaidData> ProgressData;
        public List<RaidData> LockOutData;
        public DateTime LockOutDate;
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
        PlayerData.CheckWeeklyReset();

        FileStream file = File.Open(Application.persistentDataPath + "/" + PlayerData.PlayerCharacter.GetName() + ".dat", FileMode.OpenOrCreate);
        BinaryFormatter bf = new BinaryFormatter();

        SaveData data = new SaveData()
        {
            Player = PlayerData.PlayerCharacter,
            Roster = PlayerData.Roster,
            TeamName = PlayerData.RaidTeamName,
            TeamGold = PlayerData.RaidTeamGold,
            Consumables = PlayerData.Consumables,
            ProgressData = PlayerData.Progress,
            LockOutData = PlayerData.WeeklyLockOut,
            LockOutDate = PlayerData.ThisWeek,
        };

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

            PlayerData.InitializeDataFromSaveData(data);

            return true;
        }
        return false;
    }
}
