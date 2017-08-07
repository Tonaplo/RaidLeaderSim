using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChooseRaidTeamForAttemptControllerScript : MonoBehaviour {

    public GameObject RosterMemberPrefab;
    public GameObject RosterMemberImage;
    public ChooseTeamForAttemptTeamController TeamListControllerScript;
    public Button NextButton;
    public Text NextButtonText;
    public Button RaidButton;

    int m_selectionStep = 0;
    List<GameObject> m_listOfRosterButtons = new List<GameObject>();

	// Use this for initialization
	void Start () {
        RaidButton.interactable = false;
        RosterMemberPrefab.SetActive(false);
        SetupChoiceButtons();
        NextButtonText.text = "Choose Healers";
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void SetupChoiceButtons()
    {
        for (int i = 0; i < m_listOfRosterButtons.Count; i++)
        {
            Destroy(m_listOfRosterButtons[i]);
        }
        m_listOfRosterButtons.Clear();

        List<Raider> newChoices = new List<Raider>();
        Enums.CharacterRole currentRole;

        if (m_selectionStep == 0)
            currentRole = Enums.CharacterRole.Tank;
        else if (m_selectionStep == 1)
            currentRole = Enums.CharacterRole.Healer;
        else if (m_selectionStep == 2)
            currentRole = Enums.CharacterRole.RangedDPS;
        else
            return;
        

        for (int i = 0; i < PlayerData.Roster.Count; i++)
        {
            Enums.CharacterRole mainRole = PlayerData.Roster[i].RaiderStats().GetRole();
            Enums.CharacterRole offRole = PlayerData.Roster[i].RaiderStats().GetOffSpecRole();
            if (mainRole == currentRole || (currentRole == Enums.CharacterRole.RangedDPS && mainRole == Enums.CharacterRole.MeleeDPS))
                newChoices.Add(PlayerData.Roster[i]);
             else if (offRole == currentRole || (currentRole == Enums.CharacterRole.RangedDPS && offRole == Enums.CharacterRole.MeleeDPS))
                newChoices.Add(PlayerData.Roster[i]);
        }

        newChoices.RemoveAll(x => PlayerData.RaidTeam.Contains(x));


        for (int i = 0; i < newChoices.Count; i++)
        {
            GameObject temp = GameObject.Instantiate(RosterMemberPrefab);
            temp.SetActive(true);
            temp.transform.SetParent(RosterMemberImage.transform);
            Vector3 position = RosterMemberPrefab.transform.position + new Vector3(0, -(50 * i), 0);
            temp.transform.SetPositionAndRotation(position, Quaternion.identity);
            temp.GetComponent<RaidTeamMemberSelectionPrefabScript>().Initialize(newChoices[i], currentRole, this);
            m_listOfRosterButtons.Add(temp);
        }
    }

    public void ResetRaidTeam()
    {
        PlayerData.ClearCurrentRaidTeam();
        m_selectionStep = 0;
    }

    public void AddRaiderToTeam(Raider r)
    {
        PlayerData.AddRaiderToRaidTeam(r);
        TeamListControllerScript.AddMember(r.GetName(), r.RaiderStats().GetCurrentSpec(), r.RaiderStats().GetRole(), r.RaiderStats().GetAverageThroughput());

        if (PlayerData.RaidTeam.Count == (int)Enums.StaticValues.raidTeamSize)
        {
            m_selectionStep++;
            SetupChoiceButtons();
            RaidButton.interactable = true;
        }
    }

    public void AdvanceSelectionStep() {
        if (m_selectionStep == 0)
            NextButtonText.text = "Choose DPS";
        else
            NextButton.gameObject.SetActive(false);

        m_selectionStep++;
        SetupChoiceButtons();
    }

    public void OnClickExitAttempt()
    {
        ResetRaidTeam();
        SceneManager.LoadScene("MainGameScene");
    }

    public void OnRaidClickButton()
    {
        SceneManager.LoadScene("RaidScene");
    }
}
