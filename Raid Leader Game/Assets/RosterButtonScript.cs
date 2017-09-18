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
    Text m_leftBodyText;
    Text m_rightBodyText;
    Text m_abilityText;
    RosterControllerScript m_rcs;

    BaseHealOrAttackScript attackOrHealScript;

    // Use this for initialization
    void Start () {
        Fill = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetupButton(Raider r, ref Text header, ref Text lbody, ref Text rbody, ref Text abilityT, RosterControllerScript rcs)
    {
        m_raider = r;
        m_headerText = header;
        m_leftBodyText = lbody;
        m_rightBodyText = rbody;
        m_abilityText = abilityT;
        m_rcs = rcs;
        ButtonText.text = r.GetName() + "\n" + r.RaiderStats.GetRole();

        if(!Fill)
            Fill = GetComponent<Image>();

        Fill.color = Utility.GetColorFromClass(r.RaiderStats.GetClass());
    }

    public void OnClick() {
        m_headerText.text = m_raider.GetName();
        SetupClass();

        if (m_rcs.SkillButton.IsInteractable())
            SetupGear();
        else
            SetupSkills();

        if (!m_rcs.MoveButton.IsInteractable())
            SetupMove();
        else if (!m_rcs.CounterButton.IsInteractable())
            SetupCounter();
        else
            SetupCooldown();

        m_rcs.SetCurrentRaider(m_raider);
    }

    void SetupClass()
    {
        if (m_raider.RaiderStats.GetRole() == Enums.CharacterRole.Healer)
        {
            BaseHealScript healScript;
            m_raider.RaiderStats.GetBaseHealingScript(out healScript);
            attackOrHealScript = healScript;
        }
        else
            m_raider.RaiderStats.GetBaseAttackScript(out attackOrHealScript);

        m_leftBodyText.text = "Class: " + m_raider.RaiderStats.GetClass() + "\n";
        m_leftBodyText.text += "Current Spec: " + m_raider.RaiderStats.GetCurrentSpec() + " (" + m_raider.RaiderStats.GetRole() + ")\n";
        m_leftBodyText.text += "Off Spec: " + m_raider.RaiderStats.GetOffSpec() + " (" + m_raider.RaiderStats.GetOffSpecRole() + ")\n";
        m_leftBodyText.text += "Performance Variance: " + m_raider.RaiderStats.GetVariance() + "%\n";
        m_leftBodyText.text += "Average Throughput: " + m_raider.RaiderStats.GetAverageThroughput() + "\n";
    }

    void SetupSkills()
    {
        m_rightBodyText.text = "Skills:\n";

        for (int i = 0; i < (int)Enums.SkillTypes.NumSkillTypes; i++)
        {
            m_rightBodyText.text += ((Enums.SkillTypes)i).ToString() + ": " + m_raider.RaiderStats.Skills.GetSkillLevel((Enums.SkillTypes)i) + " \\ 100\n";
        }

        if (m_raider.IsInStatus(Enums.CharacterStatus.InTraining))
        {
            TimeSpan remaining = (m_raider.ActivityFinished - DateTime.Now);
            m_rightBodyText.text += "In training: " + ((int)remaining.TotalSeconds).ToString() + " sec left";
        }
    }

    void SetupGear()
    {
        m_rightBodyText.text = "Gear:\n";
        for (int i = 0; i < (int)Enums.GearTypes.NumGearTypes; i++)
        {
            m_rightBodyText.text += ((Enums.GearTypes)i).ToString() + ": " + m_raider.RaiderStats.Gear.GetItemLevelOfSlot((Enums.GearTypes)i) + " \\ 100\n";
        }
        m_rightBodyText.text += "\nAverage Itemlevel: " + m_raider.RaiderStats.Gear.AverageItemLevel;// + "\n";
        //m_rightBodyText.text += "Total Itemlevel: " + m_raider.RaiderStats.Gear.TotalItemLevel;
    }

    void SetupMove()
    {
        m_abilityText.text = "Special Move - " + attackOrHealScript.Name + ":";
        m_abilityText.text += "\nCast Time: " + attackOrHealScript.GetBaseCastTimeAsString();
        m_abilityText.text += "                     " + attackOrHealScript.GetBaseMultiplierAsString(m_raider);
        m_abilityText.text += "\n\n" + attackOrHealScript.GetDescription();
    }

    void SetupCounter()
    {
        m_abilityText.text = "Counter Ability - " + m_raider.RaiderStats.Ability.Name + ":";
        m_abilityText.text += "\nCan be used to counter " + m_raider.RaiderStats.Ability.Ability;
        m_abilityText.text += "\n\n" + m_raider.RaiderStats.Ability.Description;
    }

    void SetupCooldown()
    {
        m_abilityText.text = "Cooldown - " + m_raider.RaiderStats.Cooldown.Name + ":";
        m_abilityText.text += "\nCan be used as a " + m_raider.RaiderStats.Cooldown.Cooldown;
        m_abilityText.text += "\n\n" + m_raider.RaiderStats.Cooldown.Description;
    }
}
