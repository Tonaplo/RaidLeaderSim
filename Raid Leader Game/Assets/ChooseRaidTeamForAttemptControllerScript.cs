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
    public RectTransform RaidContainerRectTransform;
    public Scrollbar rosterScrollBar;

    int m_selectionStep = 0;
    List<GameObject> m_listOfRosterButtons = new List<GameObject>();

	// Use this for initialization
	void Start () {
        PlayerData.ClearCurrentRaidTeam();
        RaidButton.interactable = false;
        RosterMemberPrefab.SetActive(false);
        PopulateList();
        NextButtonText.text = "Choose Healers";
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void PopulateList()
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
            Enums.CharacterRole mainRole = PlayerData.Roster[i].RaiderStats.GetRole();
            Enums.CharacterRole offRole = PlayerData.Roster[i].RaiderStats.GetOffSpecRole();
            if (mainRole == currentRole || (currentRole == Enums.CharacterRole.RangedDPS && mainRole == Enums.CharacterRole.MeleeDPS))
                newChoices.Add(PlayerData.Roster[i]);
            else if (offRole == currentRole || (currentRole == Enums.CharacterRole.RangedDPS && offRole == Enums.CharacterRole.MeleeDPS))
                newChoices.Add(PlayerData.Roster[i]);
        }

        newChoices.RemoveAll(x => PlayerData.RaidTeam.Contains(x));

        //calculate the width and height of each child item.
        int columnCount = 1;
        float width = 285;
        float height = 50;
        int rowCount = newChoices.Count;
        if (newChoices.Count % rowCount > 0)
            rowCount++;

        //adjust the height of the container so that it will just barely fit all its children
        float scrollHeight = height * rowCount;
        RaidContainerRectTransform.offsetMin = new Vector2(RaidContainerRectTransform.offsetMin.x, -scrollHeight / 2);
        RaidContainerRectTransform.offsetMax = new Vector2(RaidContainerRectTransform.offsetMax.x, scrollHeight / 2);

        int j = 0;
        for (int i = 0; i < newChoices.Count; i++)
        {
            //this is used instead of a double for loop because itemCount may not fit perfectly into the rows/columns
            if (i % columnCount == 0)
                j++;

            //create a new item, name it, and set the parent
            GameObject newItem = Instantiate(RosterMemberPrefab) as GameObject;
            newItem.SetActive(true);
            newItem.name = gameObject.name + " item at (" + i + "," + j + ")";
            newItem.transform.SetParent(RaidContainerRectTransform.transform, false);

            //move and size the new item
            RectTransform rectTransform = newItem.GetComponent<RectTransform>();

            float x = -RaidContainerRectTransform.rect.width / 2 + width * (i % columnCount);
            float y = RaidContainerRectTransform.rect.height / 2 - height * j;
            rectTransform.offsetMin = new Vector2(x, y);

            x = rectTransform.offsetMin.x + width;
            y = rectTransform.offsetMin.y + height;
            rectTransform.offsetMax = new Vector2(x, y);

            newItem.GetComponent<RaidTeamMemberSelectionPrefabScript>().Initialize(newChoices[i], currentRole, this);
            m_listOfRosterButtons.Add(newItem);
        }

        rosterScrollBar.value = 1.0f;
    }

    public void ResetRaidTeam()
    {
        PlayerData.ClearCurrentRaidTeam();
        m_selectionStep = 0;
    }

    public void AddRaiderToTeam(Raider r)
    {
        PlayerData.AddRaiderToRaidTeam(r);
        TeamListControllerScript.AddMember(r.GetName(), r.RaiderStats.GetCurrentSpec(), r.RaiderStats.GetRole(), r.RaiderStats.GetAverageThroughput());

        if (PlayerData.RaidTeam.Count == StaticValues.RaidTeamSize)
        {
            m_selectionStep++;
            PopulateList();
            RaidButton.interactable = true;
        }
    }

    public void AdvanceSelectionStep() {
        if (m_selectionStep == 0)
            NextButtonText.text = "Choose DPS";
        else
            NextButton.gameObject.SetActive(false);

        m_selectionStep++;
        PopulateList();
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
