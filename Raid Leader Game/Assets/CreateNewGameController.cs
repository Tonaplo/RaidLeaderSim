using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreateNewGameController : MonoBehaviour {

    public Text DescriptionText;
    public Button WhoAreYouButton;
    public Image ClassImage;
    public Image SpecImage;
    public Image NameImage;

    public Button FighterButton;
    public Button ShadowButton;
    public Button TotemicButton;
    public Button SorcererButton;
    public Button PaladinButton;
    public Button OccultistButton;
    
    public Button SpecOneButton;
    public Button SpecTwoButton;
    public Text SpecOneButtonText;
    public Text SpecTwoButtonText;

    public Text SpecDescriptionText;

    public InputField namefield;
    public Button StartGameButton;
    public Text NameSuffixText;

    Enums.CharacterClass m_playerClass = Enums.CharacterClass.Fighter;
    Enums.CharacterSpec m_playerMainSpec = Enums.CharacterSpec.Berserker;

    // Use this for initialization
    void Start () {
        FighterButton.GetComponent<Image>().color = Utility.GetColorFromClass(Enums.CharacterClass.Fighter);
        ShadowButton.GetComponent<Image>().color = Utility.GetColorFromClass(Enums.CharacterClass.Shadow);
        TotemicButton.GetComponent<Image>().color = Utility.GetColorFromClass(Enums.CharacterClass.Totemic);
        SorcererButton.GetComponent<Image>().color = Utility.GetColorFromClass(Enums.CharacterClass.Sorcerer);
        PaladinButton.GetComponent<Image>().color = Utility.GetColorFromClass(Enums.CharacterClass.Paladin);
        OccultistButton.GetComponent<Image>().color = Utility.GetColorFromClass(Enums.CharacterClass.Occultist);
        ClassImage.gameObject.SetActive(false);
        SpecImage.gameObject.SetActive(false);
        NameImage.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClickWhoAreYou()
    {
        WhoAreYouButton.gameObject.SetActive(false);
        DescriptionText.alignment = TextAnchor.MiddleCenter;
        DescriptionText.text = "Click a class to learn more about them.";
        ClassImage.gameObject.SetActive(true);
    }

    public void OnClickFighterButton()
    {
        FighterButton.interactable = false;
        ShadowButton.interactable = true;
        TotemicButton.interactable = true;
        SorcererButton.interactable = true;
        PaladinButton.interactable = true;
        OccultistButton.interactable = true;

        m_playerMainSpec = Enums.CharacterSpec.Guardian;
        SetClassText(Enums.CharacterClass.Fighter);
    }

    public void OnClickShadowButton()
    {
        FighterButton.interactable = true;
        ShadowButton.interactable = false;
        TotemicButton.interactable = true;
        SorcererButton.interactable = true;
        PaladinButton.interactable = true;
        OccultistButton.interactable = true;

        m_playerMainSpec = Enums.CharacterSpec.Assassin;
        SetClassText(Enums.CharacterClass.Shadow);
    }

    public void OnClickTotemicButton()
    {
        FighterButton.interactable = true;
        ShadowButton.interactable = true;
        TotemicButton.interactable = false;
        SorcererButton.interactable = true;
        PaladinButton.interactable = true;
        OccultistButton.interactable = true;

        m_playerMainSpec = Enums.CharacterSpec.Elementalist;
        SetClassText(Enums.CharacterClass.Totemic);
    }

    public void OnClickSorcererButton()
    {
        FighterButton.interactable = true;
        ShadowButton.interactable = true;
        TotemicButton.interactable = true;
        SorcererButton.interactable = false;
        PaladinButton.interactable = true;
        OccultistButton.interactable = true;

        m_playerMainSpec = Enums.CharacterSpec.Wizard;
        SetClassText(Enums.CharacterClass.Sorcerer);
    }

    public void OnClickPaladinButton()
    {
        FighterButton.interactable = true;
        ShadowButton.interactable = true;
        TotemicButton.interactable = true;
        SorcererButton.interactable = true;
        PaladinButton.interactable = false;
        OccultistButton.interactable = true;

        m_playerMainSpec = Enums.CharacterSpec.Knight;
        SetClassText(Enums.CharacterClass.Paladin);
    }

    public void OnClickOccultistButton()
    {
        FighterButton.interactable = true;
        ShadowButton.interactable = true;
        TotemicButton.interactable = true;
        SorcererButton.interactable = true;
        PaladinButton.interactable = true;
        OccultistButton.interactable = false;

        m_playerMainSpec = Enums.CharacterSpec.Scourge;
        SetClassText(Enums.CharacterClass.Occultist);
    }

    void SetClassText(Enums.CharacterClass c)
    {
        SpecOneButtonText.text = m_playerMainSpec.ToString();
        SpecTwoButtonText.text = Utility.GetOtherSpec(m_playerMainSpec).ToString();
        m_playerClass = c;
        DescriptionText.text = "The " + c + ":\n\n"+ Utility.GetDescriptionOfClass(c);
        SpecImage.gameObject.SetActive(true);
        SpecOneButton.interactable = false;
        SpecTwoButton.interactable = true;
        UpdateSpecDescriptionText();
        UpdateNameSuffixText();
    }

    public void SpecOneButtonClick()
    {
        NameImage.gameObject.SetActive(true);
        m_playerMainSpec = Utility.GetOtherSpec(m_playerMainSpec);
        SpecOneButton.interactable = false;
        SpecTwoButton.interactable = true;
        UpdateSpecDescriptionText();
        UpdateNameSuffixText();
    }

    public void SpecTwoButtonClick()
    {
        NameImage.gameObject.SetActive(true);
        m_playerMainSpec = Utility.GetOtherSpec(m_playerMainSpec);
        SpecOneButton.interactable = true;
        SpecTwoButton.interactable = false;
        UpdateSpecDescriptionText();
        UpdateNameSuffixText();
    }

    public void OnNameFieldChanged()
    {
        if (namefield.text.Length == 0)
            StartGameButton.interactable = false;
        else
            StartGameButton.interactable = true;
    }

    void UpdateSpecDescriptionText()
    {
        SpecDescriptionText.text = "The " + m_playerMainSpec.ToString() + " (" + Utility.GetRoleFromSpec(m_playerMainSpec) + ")\n\n" + Utility.GetDescriptionOfSpec(m_playerMainSpec);
    }

    void UpdateNameSuffixText()
    {
        NameSuffixText.text = ", The " + m_playerClass.ToString() + " " + m_playerMainSpec;
    }

    public void OnStartGameButtonClicked()
    {
        int baseLevel = 10;
        int playerAdvantage = 3;
        Raider player = new Raider(namefield.text, new RaiderStats(baseLevel, baseLevel+ playerAdvantage, 10, Utility.GetRoleFromSpec(m_playerMainSpec), m_playerClass));
        PlayerData.GenerateNewGameRoster(player, baseLevel);
        SceneManager.LoadScene("MainGameScene");
    }
}
