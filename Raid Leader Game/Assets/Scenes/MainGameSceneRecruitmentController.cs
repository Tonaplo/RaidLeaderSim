using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameSceneRecruitmentController : MonoBehaviour {

    public GameObject TopLeft;
    public GameObject TopRight;
    public GameObject BottomLeft;
    public GameObject BottomRight;

    // Use this for initialization
    void Start () {
        GenerateRecruits();
    }

    void GenerateRecruits()
    {
        List<string> previousNames = new List<string>();
        for (int i = 0; i < PlayerData.Roster.Count; i++)
        {
            previousNames.Add(PlayerData.Roster[i].GetName());
        }

        List<string> newNameList = new List<string>(previousNames);
        Utility.GetRandomCharacterName(ref newNameList, 4);
        newNameList.RemoveAll(x => previousNames.Contains(x));

        TopLeft.GetComponent<RecruitScript>().Initialize(new Raider(newNameList[0], RaiderStats.GenerateRaiderStatsFromSpec((Enums.CharacterSpec)Random.Range(0, (int)Enums.CharacterSpec.Necromancer + 1), PlayerData.GetRosterAverageItemLevel())));
        TopRight.GetComponent<RecruitScript>().Initialize(new Raider(newNameList[1], RaiderStats.GenerateRaiderStatsFromSpec((Enums.CharacterSpec)Random.Range(0, (int)Enums.CharacterSpec.Necromancer + 1), PlayerData.GetRosterAverageSkillAndItemLevel())));
        BottomLeft.GetComponent<RecruitScript>().Initialize(new Raider(newNameList[2], RaiderStats.GenerateRaiderStatsFromSpec((Enums.CharacterSpec)Random.Range(0, (int)Enums.CharacterSpec.Necromancer + 1), PlayerData.GetRosterAverageSkillAndItemLevel())));
        BottomRight.GetComponent<RecruitScript>().Initialize(new Raider(newNameList[3], RaiderStats.GenerateRaiderStatsFromSpec((Enums.CharacterSpec)Random.Range(0, (int)Enums.CharacterSpec.Necromancer + 1), PlayerData.GetRosterAverageSkillLevel())));
    }

    // Update is called once per frame
    void Update () {
		
	}
}
