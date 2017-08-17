using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RosterControllerScript : MonoBehaviour {

    public GameObject RaiderButtonPrefab;
    public Text HeaderText;
    public Button GearButton;
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

    List<GameObject> buttons;
    GameObject m_currentButton;
    Raider m_currentRaider;

    // Use this for initialization
    void Start () {
        buttons = new List<GameObject>();
        PlayerData.SortRoster();
        SetupRoster();
        SetCurrentRaider(buttons[0].GetComponent<RosterButtonScript>().Raider);
        SkillButtonOnClick();
        MoveButtonOnClick();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void SetupRoster()
    {
        int height = 40;
        int width = 150;
        int xPosStart = 75;
        int yPosStart = 350;
        for (int i = 0; i < PlayerData.Roster.Count; i++)
        {
            GameObject temp = GameObject.Instantiate(RaiderButtonPrefab);
            temp.SetActive(true);
            temp.transform.SetParent(transform);
            temp.transform.SetPositionAndRotation(new Vector3(xPosStart + ((i / 8) * (width) + 10), yPosStart - (((height + 5) * (i % 8))), 0), Quaternion.identity);
            temp.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            temp.GetComponent<RosterButtonScript>().SetupButton(PlayerData.Roster[i], ref HeaderText, ref LeftBodyText, ref RightBodyText, ref AbilityText, this);
            buttons.Add(temp);
        }
        buttons[0].GetComponent<RosterButtonScript>().OnClick();
        m_currentButton = buttons[0];
    }
    
    public void SetActive(bool on)
    {
        gameObject.SetActive(on);
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
        m_currentButton.GetComponent<RosterButtonScript>().OnClick();
    }

    public void SkillButtonOnClick()
    {
        SkillButton.interactable = false;
        GearButton.interactable = true;
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
