using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecruitScript : MonoBehaviour {

    public Text Header;
    public Text Description;
    public Button RecruitButton;
    public Button RejectButton;

    Raider m_recruit;
    bool m_disabled = false;
    float m_disabledTime = 0.0f;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        if (m_disabled)
        {
            m_disabledTime += Time.deltaTime;
            Description.text = "Next recruit available in " + Mathf.RoundToInt(m_disabledTime) + " (actual countdown implementation pending)";
        }
	}

    public void Initialize(Raider r)
    {
        m_recruit = r;
        Header.text = m_recruit.GetName() + ", " + m_recruit.RaiderStats.GetCurrentSpec().ToString() + " (" + Utility.GetRoleString(m_recruit.RaiderStats.GetRole()) + ")\n";
        Header.text += "Average Skill/Gear: " + m_recruit.RaiderStats.Skills.AverageSkillLevel + "/" + m_recruit.RaiderStats.Gear.AverageItemLevel;
        Description.text = Utility.GetDescriptionOfSpec(m_recruit.RaiderStats.GetCurrentSpec());
    }

    public void OnClickRecruit()
    {
        PlayerData.AddRecruitToRoster(m_recruit);
        Disable();
    }

    public void OnClickReject()
    {
        Disable();
    }

    void Disable()
    {
        RecruitButton.gameObject.SetActive(false);
        RejectButton.gameObject.SetActive(false);
        Header.text = "";
        Description.text = "Next recruit available in (countdown implementation pending)";
        m_disabled = true;
        m_disabledTime = 0.0f;
    }
}
