using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RaidData {

    [Serializable]
    public class EncounterData
    {
        public bool BeatenOnEasy = false;
        public bool BeatenOnNormal = false;
        public bool BeatenOnHard = false;
        public Enums.EncounterEnum Encounter;
        public Enums.EncounterEnum PreReq;
        public string Name;

        public EncounterData() { }
        public EncounterData(string n, Enums.EncounterEnum e, Enums.EncounterEnum p) { Name = n; Encounter = e; PreReq = p; }
    }

    public string m_name;
    public Enums.RaidEnum m_raid;
    public List<EncounterData> m_encounters;

    public RaidData() { }

    public static List<RaidData> CreateNewRaidData()
    {
        return new List<RaidData>()
        {
            CreateNewMinesOfAtrea(),
        };
    }

    public static RaidData CreateDataFromRaidEnum(Enums.RaidEnum r)
    {
        switch (r)
        {
            case Enums.RaidEnum.MinesOfAtrea:
                return CreateNewMinesOfAtrea();
            default:
                Debug.LogAssertion("You have forgotten to add a new case handling the new raid!");
                break;
        }
        return null;
    }

    static RaidData CreateNewMinesOfAtrea()
    {
        return new RaidData()
        {
            m_name = "Mines of Atrea",
            m_raid = Enums.RaidEnum.MinesOfAtrea,
            m_encounters = new List<EncounterData>
            {
                new EncounterData("Keeper of the Mine", Enums.EncounterEnum.MoAKeeperOfTheMine, Enums.EncounterEnum.None),
                new EncounterData("Vampiricus", Enums.EncounterEnum.MoAVampiricus, Enums.EncounterEnum.MoAKeeperOfTheMine),
                new EncounterData("Council of Stone", Enums.EncounterEnum.MoACouncilOfStone, Enums.EncounterEnum.MoAVampiricus),
                new EncounterData("Mineking Atrea", Enums.EncounterEnum.MoAMinekingAtrea,Enums.EncounterEnum.MoACouncilOfStone)
            }
        };
    }
}
