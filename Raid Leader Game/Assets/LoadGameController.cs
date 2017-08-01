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
        SceneManager.LoadScene("MainGameScene");
    }

    public void LoadButtonClicked()
    {
        if (input.text.Length != 0)
        {
            if (DataController.controller.Load(input.text))
                LoadSuccessful();
            else
                LoadFailed();
        }
    }

    void LoadSuccessful()
    {
        Header.gameObject.SetActive(true);
        CharacterText.gameObject.SetActive(true);
        RaidText.gameObject.SetActive(true);
        NotFoundText.gameObject.SetActive(false);

        Header.text = PlayerData.GetPlayerCharacter().GetName();

        CharacterText.text = "Class: " + PlayerData.GetPlayerCharacter().RaiderStats().GetClass() +
                             "\nMain Spec: " + PlayerData.GetPlayerCharacter().RaiderStats().GetCurrentSpec() +
                             "\nSkill Level: " + PlayerData.GetPlayerCharacter().RaiderStats().GetSkillLevel() +
                             "\nGear Level: " + PlayerData.GetPlayerCharacter().RaiderStats().GetGearLevel() +
                             "\nAverage Throughout: " + PlayerData.GetPlayerCharacter().RaiderStats().GetAverageThroughput();

        float averageGearLevel = 0.0f;
        float averageSkillLevel = 0.0f;
        int numMembers = PlayerData.GetRoster().Count;

        for (int i = 0; i < numMembers; i++)
        {
            averageGearLevel += PlayerData.GetRoster()[i].RaiderStats().GetGearLevel();
            averageSkillLevel += PlayerData.GetRoster()[i].RaiderStats().GetSkillLevel();
        }

        averageGearLevel /= numMembers;
        averageSkillLevel /= numMembers;

        averageGearLevel *= 100.0f;
        averageSkillLevel *= 100.0f;

        averageGearLevel = Mathf.Round(averageGearLevel);
        averageSkillLevel = Mathf.Round(averageSkillLevel);

        averageGearLevel /= 100.0f;
        averageSkillLevel /= 100.0f;

        RaidText.text = "Raid team Name functionality pending" +
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
