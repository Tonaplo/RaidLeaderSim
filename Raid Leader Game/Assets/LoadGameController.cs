using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadGameController : MonoBehaviour {

    public Image TextBackGround;
    public Text Header;
    public Text CharacterText;
    public Text RaidText;
    public Text NotFoundText;
    public InputField input;

    public Button StartButton;
    public Button BackButton;

	// Use this for initialization
	void Start () {
        TextBackGround.gameObject.SetActive(true);
        LoadFailed();
        NotFoundText.text = "Enter a character name and press 'Load'!";
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartGameClicked()
    {
        PlayerData.RecalculateRoster();
        SceneManager.LoadScene("MainGameScene");
    }

    public void LoadButtonClicked()
    {
        if (StartButton.interactable != true)
        {
            if (input.text.Length != 0)
            {
                if (DataController.controller.Load(input.text))
                    LoadSuccessful();
                else
                    LoadFailed();
            }
        }
        else
            StartGameClicked();
    }

    void LoadSuccessful()
    {
        Header.gameObject.SetActive(true);
        CharacterText.gameObject.SetActive(true);
        RaidText.gameObject.SetActive(true);
        NotFoundText.gameObject.SetActive(false);

        Header.text = PlayerData.PlayerCharacter.GetName();

        CharacterText.text = "Class: " + PlayerData.PlayerCharacter.RaiderStats.GetClass() +
                             "\nMain Spec: " + PlayerData.PlayerCharacter.RaiderStats.GetCurrentSpec() +
                             "\nSkill Level: " + PlayerData.PlayerCharacter.RaiderStats.Skills.AverageSkillLevel +
                             "\nGear Level: " + PlayerData.PlayerCharacter.RaiderStats.Gear.AverageItemLevel +
                             "\nAverage Throughout: " + PlayerData.PlayerCharacter.RaiderStats.GetAverageThroughput();

        float averageGearLevel = 0.0f;
        float averageSkillLevel = 0.0f;
        int numMembers = PlayerData.Roster.Count;

        for (int i = 0; i < numMembers; i++)
        {
            averageGearLevel += PlayerData.Roster[i].RaiderStats.Gear.AverageItemLevel;
            averageSkillLevel += PlayerData.Roster[i].RaiderStats.Skills.AverageSkillLevel;
        }

        averageGearLevel /= numMembers;
        averageSkillLevel /= numMembers;

        averageGearLevel *= 100.0f;
        averageSkillLevel *= 100.0f;

        averageGearLevel = Mathf.Round(averageGearLevel);
        averageSkillLevel = Mathf.Round(averageSkillLevel);

        averageGearLevel /= 100.0f;
        averageSkillLevel /= 100.0f;

        RaidText.text =  PlayerData.RaidTeamName +
                        "\nTeam Size: " + numMembers +
                        "\nAverage Skill Level: " + averageSkillLevel +
                        "\nAverage Gear Level: " + averageGearLevel +
                        "\nProgress functionality pending";

        StartButton.interactable = true;
    }

    void LoadFailed()
    {
        Header.gameObject.SetActive(false);
        CharacterText.gameObject.SetActive(false);
        RaidText.gameObject.SetActive(false);
        NotFoundText.gameObject.SetActive(true);
        NotFoundText.text = "No data found for that Character name!";
    }
}
