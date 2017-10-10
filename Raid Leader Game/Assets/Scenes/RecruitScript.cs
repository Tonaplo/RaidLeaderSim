using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class RecruitScript : MonoBehaviour {

    public Text Header;
    public Text Description;
    public Button RecruitButton;
    public Button RejectButton;

    int m_infoIndex;
    bool m_isDisabled = false;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (PlayerData.RecruitLockOut[m_infoIndex].IsDisabled)
        {
            UpdateNextRecruitString();
        }
        else if (PlayerData.Roster.Count >= StaticValues.RosterMaxSize)
        {
            if(!m_isDisabled)
                Disable();

            Description.text = "You already have " + StaticValues.RosterMaxSize + " members on your roster!";
        }
	}

    public void Initialize(int i)
    {
        m_infoIndex = i;
        if (!PlayerData.RecruitLockOut[m_infoIndex].IsDisabled && PlayerData.Roster.Count < StaticValues.RosterMaxSize)
        {
            Enable();
        }
        else if (PlayerData.Roster.Count >= StaticValues.RosterMaxSize)
        {
            Disable();
            Description.text = "You already have " + StaticValues.RosterMaxSize + " members on your roster!";
        }
        else
        {
            Disable();
            UpdateNextRecruitString();
        }
    }

    public void OnClickRecruit()
    {
        PlayerData.AddRecruitToRoster(PlayerData.RecruitLockOut[m_infoIndex].Recruit);
        PlayerData.RecruitLockOut[m_infoIndex].Disable();
        Disable();
        UpdateNextRecruitString();
    }

    public void OnClickReject()
    {
        PlayerData.RecruitLockOut[m_infoIndex].Disable();
        Disable();
        UpdateNextRecruitString();
    }

    void Disable()
    {
        RecruitButton.gameObject.SetActive(false);
        RejectButton.gameObject.SetActive(false);
        Header.text = "";
        m_isDisabled = true;
    }

    void Enable()
    {
        Header.text = PlayerData.RecruitLockOut[m_infoIndex].Recruit.GetName() + ", " + PlayerData.RecruitLockOut[m_infoIndex].Recruit.RaiderStats.GetCurrentSpec().ToString() + " (" + Utility.GetRoleString(PlayerData.RecruitLockOut[m_infoIndex].Recruit.RaiderStats.GetRole()) + ")\n";
        Header.text += "Average Skill/Gear: " + PlayerData.RecruitLockOut[m_infoIndex].Recruit.RaiderStats.Skills.AverageSkillLevel + "/" + PlayerData.RecruitLockOut[m_infoIndex].Recruit.RaiderStats.Gear.AverageItemLevel;
        Description.text = Utility.GetDescriptionOfSpec(PlayerData.RecruitLockOut[m_infoIndex].Recruit.RaiderStats.GetCurrentSpec());
        RecruitButton.gameObject.SetActive(true);
        RejectButton.gameObject.SetActive(true);
        m_isDisabled = false;
    }

    void UpdateNextRecruitString()
    {
        int hours = 24 - DateTime.Now.Hour;
        int minutes = 60 -DateTime.Now.Minute;
        int seconds = 60 -DateTime.Now.Second;
        string timeLeftString = (hours > 9 ? hours.ToString() : "0" + hours.ToString()) + ":" + (minutes > 9 ? minutes.ToString() : "0" + minutes.ToString()) + ":" + (seconds > 9 ? seconds.ToString() : "0" + seconds.ToString());
        Description.text = "Next recruit available in " + timeLeftString;
        m_isDisabled = !PlayerData.RecruitLockOut[m_infoIndex].CheckForNewRecruit();
        
        if (!m_isDisabled)
            Enable();
    }
}
