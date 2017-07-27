using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RosterButtonScript : MonoBehaviour {

    public Text ButtonText;
    Raider m_raider;
    Text m_headerText;
    Text m_bodyText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetupButton(Raider r, ref Text header, ref Text body)
    {
        m_raider = r;
        m_headerText = header;
        m_bodyText = body;
        ButtonText.text = r.GetName();
    }

    public void OnClick() {
        m_headerText.text = m_raider.GetName();

        m_bodyText.text = "Class: " + m_raider.RaiderStats().GetClass() + "\n";
        m_bodyText.text = "Roles: " + m_raider.RaiderStats().GetClass() + "\n";
        m_bodyText.text += "Skill: " + m_raider.RaiderStats().GetSkillLevel() + "\n";
        m_bodyText.text += "Gear Level: " + m_raider.RaiderStats().GetGearLevel() + "\n";
        m_bodyText.text += "Performance Variance: " + m_raider.RaiderStats().GetVariance() + "\n";
        m_bodyText.text += "Average Throughput: " + m_raider.RaiderStats().GetAverageThroughput() + "\n";
    }
}
