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
    public Button NextButton;
    public Button StartGameButton;
    public Text NameSuffixText;
    
    public InputField RaidTeamNameField;
    public Text ClassText;
    public Image RaidTeamBackground;
    public Image TeamMemberCreationBackground;
    public Text RaidRoster;

    public Text TeamMemberHeader;
    public Text TeamMemberAbilityDescription;
    public Button AddMember;
    public Button RejectMember;

    Enums.CharacterClass m_playerClass = Enums.CharacterClass.Fighter;
    Enums.CharacterSpec m_playerMainSpec = Enums.CharacterSpec.Berserker;
    string m_playerName;

    int m_tanksNeeded = 2;
    int m_healersNeeded = 3;
    int m_dpsNeeded = 7;
    int m_recruitBaseLevel = 10;
    Enums.CharacterSpec m_recruitSpec;
    string m_recruitName;
    List<string> m_AllRecruitNames = new List<string>();

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
        StartGameButton.gameObject.SetActive(false);
        NextButton.gameObject.SetActive(true);
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
            NextButton.interactable = false;
        else
            NextButton.interactable = true;
    }

    public void OnRaidTeamNameFieldChanged()
    {
        FighterButton.gameObject.SetActive(false);
        ShadowButton.gameObject.SetActive(false);
        TotemicButton.gameObject.SetActive(false);
        SorcererButton.gameObject.SetActive(false);
        PaladinButton.gameObject.SetActive(false);
        OccultistButton.gameObject.SetActive(false);
        ClassText.text = "And so, " + RaidTeamNameField.text + " came to be. But what is a team without a roster?\nLet fill out your core team.\n\nThere are 12 spots in a raid team.\nYou have one spot, so we need to recruit 11 team mates:";
        RaidTeamBackground.gameObject.SetActive(true);
        TeamMemberCreationBackground.gameObject.SetActive(true);
        
        if (RaidTeamNameField.text.Length == 0)
            ClassImage.gameObject.SetActive(false);
        else
            ClassImage.gameObject.SetActive(true);
    }

    public void OnAddRecruitClicked()
    {
        m_AllRecruitNames.Add(m_recruitName);
        Raider recruit = new Raider(m_recruitName, RaiderStats.GenerateRaiderStatsFromSpec(m_recruitSpec, m_recruitBaseLevel));
        PlayerData.AddRecruitToRoster(recruit);

        Enums.CharacterRole newGuyRole = Utility.GetRoleFromSpec(m_recruitSpec);

        switch (newGuyRole)
        {
            case Enums.CharacterRole.Tank:
                m_tanksNeeded--;
                break;
            case Enums.CharacterRole.Healer:
                m_healersNeeded--;
                break;
            case Enums.CharacterRole.RangedDPS:
            case Enums.CharacterRole.MeleeDPS:
                m_dpsNeeded--;
                break;
            default:
                break;
        }
        
        UpdateRaidRosterText();

        if (m_dpsNeeded != 0)
            GenerateNewRecruit();
        else
        {
            TeamCreationFinished();
        }

    }

    public void OnRejectRecruitClicked()
    {
        GenerateNewRecruit();
    }

    void UpdateRaidRosterText()
    {
        if(m_tanksNeeded != 0)
            RaidRoster.text = "Recruiting Tanks - 2 total:\n\n";
        else if (m_healersNeeded != 0)
            RaidRoster.text = "Recruiting Healers - 3 total:\n\n";
        else
            RaidRoster.text = "Recruiting DPS - 7 total:\n\n";
        
        PlayerData.SortRoster();
        for (int i = 0; i < PlayerData.Roster.Count; i++)
        {
            if (PlayerData.Roster[i].RaiderStats.GetRole() == Enums.CharacterRole.Tank && m_tanksNeeded != 0)
            {
                RaidRoster.text += PlayerData.Roster[i].GetName() + " (" + PlayerData.Roster[i].RaiderStats.GetCurrentSpec() + ")\n";
            }
            else if (PlayerData.Roster[i].RaiderStats.GetRole() == Enums.CharacterRole.Healer && m_tanksNeeded == 0 && m_healersNeeded != 0)
            {
                RaidRoster.text += PlayerData.Roster[i].GetName() + " (" + PlayerData.Roster[i].RaiderStats.GetCurrentSpec() + ")\n";
            }
            else if (m_tanksNeeded == 0 && m_healersNeeded == 0 && m_dpsNeeded != 0 && (PlayerData.Roster[i].RaiderStats.GetRole() == Enums.CharacterRole.RangedDPS || PlayerData.Roster[i].RaiderStats.GetRole() == Enums.CharacterRole.MeleeDPS))
            {
                RaidRoster.text += PlayerData.Roster[i].GetName() + " (" + PlayerData.Roster[i].RaiderStats.GetCurrentSpec() + ")\n";
            }
        }
    }

    void UpdateRosterTextWithFullTeam()
    {
        RaidRoster.text = "DPS:\n";
        TeamMemberHeader.text = "Tanks:\n";
        PlayerData.SortRoster();
        bool firstHealer = true;
        for (int i = 0; i < PlayerData.Roster.Count; i++)
        {
            if (PlayerData.Roster[i].RaiderStats.GetRole() == Enums.CharacterRole.Tank)
            {
                TeamMemberHeader.text += PlayerData.Roster[i].GetName() + " (" + PlayerData.Roster[i].RaiderStats.GetCurrentSpec() + ")\n";
            }
            else if (PlayerData.Roster[i].RaiderStats.GetRole() == Enums.CharacterRole.Healer)
            {
                if (firstHealer)
                {
                    firstHealer = false;
                    TeamMemberHeader.text += "\nHealers:\n";
                }
                TeamMemberHeader.text += PlayerData.Roster[i].GetName() + " (" + PlayerData.Roster[i].RaiderStats.GetCurrentSpec() + ")\n";
            }
            else
            {
                RaidRoster.text += PlayerData.Roster[i].GetName() + " (" + PlayerData.Roster[i].RaiderStats.GetCurrentSpec() + ")\n";
            }
        }
    }

    void GenerateNewRecruit()
    {
        Enums.CharacterRole recruitRole = Enums.CharacterRole.Tank;
        Enums.CharacterClass recruitClass = Enums.CharacterClass.Fighter;
        if (m_tanksNeeded != 0)
        {
            recruitClass = Utility.GenerateClassFromRole(recruitRole);
        }
        else if (m_healersNeeded != 0)
        {
            recruitRole = Enums.CharacterRole.Healer;
            recruitClass = Utility.GenerateClassFromRole(recruitRole);
        }
        else
        {
            recruitRole = (Random.value > .5f ? Enums.CharacterRole.MeleeDPS : Enums.CharacterRole.RangedDPS);
            recruitClass = Utility.GenerateClassFromRole(recruitRole);
        }

        m_recruitSpec = Utility.GetSpecFromRoleAndClass(recruitClass, recruitRole);
        List<string> newNameList = new List<string>(m_AllRecruitNames);
        Utility.GetRandomCharacterName(ref newNameList, 1);

        newNameList.RemoveAll(x => m_AllRecruitNames.Contains(x));
        m_recruitName = newNameList[0];
        
        TeamMemberHeader.text = m_recruitName + ", " + recruitClass.ToString() + " " + recruitRole.ToString() + "\n" + m_recruitSpec.ToString() + " spec";
        TeamMemberAbilityDescription.text = Utility.GetDescriptionOfSpec(m_recruitSpec);
    }

    void UpdateSpecDescriptionText()
    {
        SpecDescriptionText.text = "The " + m_playerMainSpec.ToString() + " (" + Utility.GetRoleFromSpec(m_playerMainSpec) + ")\n\n" + Utility.GetDescriptionOfSpec(m_playerMainSpec);
    }

    void UpdateNameSuffixText()
    {
        NameSuffixText.text = ", The " + m_playerClass.ToString() + " " + m_playerMainSpec;
    }

    void UpdateDescriptionTextForRaidCreation()
    {
        DescriptionText.text = "Welcome to the world of raid team leadership, " + m_playerName + 
            ". You've made it clear who you are - now let's define your raid team.\n\nFirst off - What is your raid team called?                                     ";
    }

    void TeamCreationFinished()
    {
        PlayerData.SetRaidTeamName(RaidTeamNameField.text);
        RaidTeamNameField.gameObject.SetActive(false);
        UpdateRosterTextWithFullTeam();
        ClassText.text = "A great collection of talent, ready for raiding!\n\nYou're ready and you've got yourself a team. Good luck!";
        DescriptionText.fontSize = 20;
        DescriptionText.text = PlayerData.RaidTeamName + "\nManaged by " + m_playerName + ", the " + PlayerData.PlayerCharacter.RaiderStats.GetCurrentSpec() +".";
        StartGameButton.interactable = true;
        AddMember.gameObject.SetActive(false);
        RejectMember.gameObject.SetActive(false);
        TeamMemberAbilityDescription.gameObject.SetActive(false);
    }

    public void OnNextButtonClicked()
    {

        m_playerName = namefield.text;
        int baseLevel = 10;
        int playerAdvantage = 3;
        Raider player = new Raider(m_playerName, new RaiderStats(baseLevel, baseLevel + playerAdvantage, 10, Utility.GetRoleFromSpec(m_playerMainSpec), m_playerClass));
        PlayerData.AddPlayerToRoster(player);
        m_AllRecruitNames.Add(m_playerName);

        switch (m_playerMainSpec)
        {
            case Enums.CharacterSpec.Guardian:
            case Enums.CharacterSpec.Knight:
                m_tanksNeeded--;
                break;
            case Enums.CharacterSpec.Cleric:
            case Enums.CharacterSpec.Diviner:
            case Enums.CharacterSpec.Naturalist:
                m_healersNeeded--;
                break;
            case Enums.CharacterSpec.Berserker:
            case Enums.CharacterSpec.Assassin:
            case Enums.CharacterSpec.Scourge:
            case Enums.CharacterSpec.Ranger:
            case Enums.CharacterSpec.Wizard:
            case Enums.CharacterSpec.Elementalist:
            case Enums.CharacterSpec.Necromancer:
            default:
                m_dpsNeeded--;
                break;
        }
        UpdateDescriptionTextForRaidCreation();
        RaidTeamNameField.gameObject.SetActive(true);
        ClassImage.gameObject.SetActive(false);
        SpecImage.gameObject.SetActive(false);
        NameImage.gameObject.SetActive(false);
        StartGameButton.gameObject.SetActive(true);
        NextButton.gameObject.SetActive(false);
        GenerateNewRecruit();
        UpdateRaidRosterText();
    }

    public void OnStartGameButtonClicked()
    {
        PlayerData.SortRoster();
        PlayerData.RecalculateRoster();
        SceneManager.LoadScene("MainGameScene");
    }
}
