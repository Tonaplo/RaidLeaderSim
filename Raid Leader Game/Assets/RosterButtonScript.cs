using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RosterButtonScript : MonoBehaviour {

    public Text ButtonText;

    public Raider Raider { get { return m_raider; } }

    Image Fill;
    Raider m_raider;
    Text m_headerText;
    Text m_bodyText;
    RosterControllerScript m_rcs;

    BaseHealOrAttackScript attackOrHealScript;

    // Use this for initialization
    void Start () {
        Fill = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetupButton(Raider r, ref Text header, ref Text body, RosterControllerScript rcs)
    {
        m_raider = r;
        m_headerText = header;
        m_bodyText = body;
        m_rcs = rcs;
        ButtonText.text = r.GetName() + "\n" + r.RaiderStats.GetRole();

        if(!Fill)
            Fill = GetComponent<Image>();

        Fill.color = Utility.GetColorFromClass(r.RaiderStats.GetClass());

        if (r.RaiderStats.GetRole() == Enums.CharacterRole.Healer)
        {
            BaseHealScript healScript;
            m_raider.RaiderStats.GetBaseHealingScript(out healScript);
            attackOrHealScript = healScript;
        }
        else
            m_raider.RaiderStats.GetBaseAttackScript(out attackOrHealScript);
    }

    public void OnClick() {
        m_headerText.text = m_raider.GetName();

        m_bodyText.text = "Class: " + m_raider.RaiderStats.GetClass() + "\n";
        m_bodyText.text += "Current Spec: " + m_raider.RaiderStats.GetCurrentSpec() + " (" + m_raider.RaiderStats.GetRole() + ")\n";
        m_bodyText.text += "Off Spec: " + m_raider.RaiderStats.GetOffSpec() + " (" + m_raider.RaiderStats.GetOffSpecRole() + ")\n\n";
        m_bodyText.text += "Skill Level: " + m_raider.RaiderStats.GetSkillLevel() + " \\ 100";

        if (m_raider.IsInStatus(Enums.CharacterStatus.InTraining))
        {
            TimeSpan remaining = (m_raider.ActivityFinished - DateTime.Now);
            m_bodyText.text += "                In training: " + ((int)remaining.TotalSeconds).ToString() + " sec left\n";
        }
        else
            m_bodyText.text += "\n";

        m_bodyText.text += "Gear Level: " + m_raider.RaiderStats.GetGearLevel() + "\n";
        m_bodyText.text += "Performance Variance: " + m_raider.RaiderStats.GetVariance() + "%\n";
        m_bodyText.text += "Average Throughput: " + m_raider.RaiderStats.GetAverageThroughput() + "\n";

        


        m_bodyText.text += "\nAbility - " + attackOrHealScript.GetName() + ":";
        m_bodyText.text += "\nCast Time: " + attackOrHealScript.GetBaseCastTimeAsString();
        m_bodyText.text += "\nMultiplier: " + attackOrHealScript.GetBaseMultiplierAsString();
        m_bodyText.text += "\n\n" + attackOrHealScript.GetDescription();

        m_rcs.SetCurrentRaider(m_raider);
    }
}
