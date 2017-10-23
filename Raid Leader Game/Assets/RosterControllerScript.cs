using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RosterControllerScript : MonoBehaviour {

    public GameObject RaiderButtonPrefab;
    public Text HeaderText;
    public Button GearButton;
    public Button TraitButton;
    public Button SkillButton;
    public Text LeftBodyText;
    public Text RightBodyText;
    public Button MoveButton;
    public Button CounterButton;
    public Button CooldownButton;
    public Text AbilityText;
    public Image TextBackGround;
    public Button ChangeSpecButton;
    public Button BeginTrainingButton;
    public Button RemoveFromRosterButton;
    public Text RaidTeamName;
    public Text RaidTeamStats;

    List<GameObject> buttons;
    GameObject m_currentButton;
    Raider m_currentRaider;

    // Use this for initialization
    void Start () {
        
        Reactivate();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Reactivate()
    {
        gameObject.SetActive(true);
        if (buttons != null)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                Destroy(buttons[i]);
            }
            buttons.Clear();
        }
        else
        {
            buttons = new List<GameObject>();
        }
        
        PlayerData.SortRoster();
        SetupRoster();
        SetCurrentRaider(buttons[0].GetComponent<RosterButtonScript>().Raider);
        SkillButtonOnClick();
        MoveButtonOnClick();
    }

    void SetupRoster()
    {

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

        RaidTeamName.text = PlayerData.RaidTeamName;
        RaidTeamStats.text = "Average Gear Level: " + averageGearLevel + "             Average Skil Level: " + averageSkillLevel;

        float scale = GameObject.FindGameObjectWithTag("Canvas").transform.localScale.x;
        int height = 35;
        int width = 200;
        float xOffset = (width + 10) * scale;
        float yOffset = (height + 5) * scale;
        float xPosStart = 110 * scale;
        float yPosStart = 300 * scale;
        for (int i = 0; i < PlayerData.Roster.Count; i++)
        {
            GameObject temp = GameObject.Instantiate(RaiderButtonPrefab);
            temp.SetActive(true);
            temp.transform.SetParent(transform, false);
            temp.transform.SetPositionAndRotation(new Vector3(xPosStart + ((i % 2) * xOffset), yPosStart - ((yOffset * (i / 2))), 0), Quaternion.identity);
            temp.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            temp.GetComponent<RosterButtonScript>().SetupButton(PlayerData.Roster[i], ref HeaderText, ref LeftBodyText, ref RightBodyText, ref AbilityText, this);
            buttons.Add(temp);
        }
        buttons[0].GetComponent<RosterButtonScript>().OnClick();
        m_currentButton = buttons[0];
    }

    public void SetCurrentRaider(Raider r)
    {
        m_currentRaider = r;
        if (m_currentRaider == PlayerData.PlayerCharacter)
            RemoveFromRosterButton.interactable = false;
        else
            RemoveFromRosterButton.interactable = true;

        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].GetComponent<RosterButtonScript>().Raider == m_currentRaider)
            {
                m_currentButton = buttons[i];
                break;
            }
        }

        if (m_currentRaider.IsInStatus(Enums.CharacterStatus.InTraining))
            StartCoroutine(UpdateTraining(1.0f, m_currentRaider));
    }

    public void OnChangeSpecButtonClicked()
    {
        m_currentRaider.ChangeSpec();
        m_currentButton.GetComponent<RosterButtonScript>().OnClick();
        Reactivate();
    }

    public void OnBeginTrainingClicked()
    {
        m_currentRaider.TrainRaider();
        StartCoroutine(UpdateTraining(1.0f, m_currentRaider));
        m_currentButton.GetComponent<RosterButtonScript>().OnClick();
    }

    IEnumerator UpdateTraining(float time, Raider r)
    {
        yield return new WaitForSeconds(time);

        if (r == m_currentRaider)
        {
            if (!m_currentRaider.CheckForTrainingEnd())
            {
                StartCoroutine(UpdateTraining(1.0f, m_currentRaider));
            }

            m_currentButton.GetComponent<RosterButtonScript>().OnClick();
        }
    }

    public void OnRemoveFromRosterButtonClicked()
    {
        PlayerData.RemoveMemberFromRoster(m_currentRaider);
        m_currentRaider = null;
        ChangeSpecButton.interactable = false;
        BeginTrainingButton.interactable = false;
        RemoveFromRosterButton.interactable = false;
        for (int i = 0; i < buttons.Count; i++)
        {
            Destroy(buttons[i]);
        }
        buttons = new List<GameObject>();
        SetupRoster();
    }

    public void GearButtonOnClick()
    {
        SkillButton.interactable = true;
        GearButton.interactable = false;
        TraitButton.interactable = true;
        m_currentButton.GetComponent<RosterButtonScript>().OnClick();
    }

    public void TraitButtonOnClick()
    {
        SkillButton.interactable = true;
        GearButton.interactable = true;
        TraitButton.interactable = false;
        m_currentButton.GetComponent<RosterButtonScript>().OnClick();
    }

    public void SkillButtonOnClick()
    {
        SkillButton.interactable = false;
        GearButton.interactable = true;
        TraitButton.interactable = true;
        m_currentButton.GetComponent<RosterButtonScript>().OnClick();
    }

    public void MoveButtonOnClick()
    {
        MoveButton.interactable = false;
        CounterButton.interactable = true;
        CooldownButton.interactable = true;
        m_currentButton.GetComponent<RosterButtonScript>().OnClick();
    }

    public void CounterButtonOnClick()
    {
        MoveButton.interactable = true;
        CounterButton.interactable = false;
        CooldownButton.interactable = true;
        m_currentButton.GetComponent<RosterButtonScript>().OnClick();
    }

    public void CooldownButtonOnClick()
    {
        MoveButton.interactable = true;
        CounterButton.interactable = true;
        CooldownButton.interactable = false;
        m_currentButton.GetComponent<RosterButtonScript>().OnClick();
    }
}
