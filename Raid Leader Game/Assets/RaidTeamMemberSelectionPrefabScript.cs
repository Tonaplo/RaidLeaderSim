using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaidTeamMemberSelectionPrefabScript : MonoBehaviour {


    public Text RosterMemberText;
    public Text MainSpecThroughPutText;
    public Button MainSpecButton;
    public Text MainSpecButtonText;
    public Text OffSpecThroughPutText;
    public Button OffSpecButton;
    public Text OffSpecButtonText;

    Raider m_rosterMember;
    ChooseRaidTeamForAttemptControllerScript m_controller;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Initialize(Raider r, Enums.CharacterRole currentRole, ChooseRaidTeamForAttemptControllerScript c)
    {
        m_rosterMember = r;
        m_controller = c;
        RosterMemberText.text = m_rosterMember.GetName() + "\n" + m_rosterMember.RaiderStats().GetClass();
        bool isCurrentRoleDPS = (currentRole == Enums.CharacterRole.MeleeDPS) || (currentRole == Enums.CharacterRole.RangedDPS);

        m_rosterMember.RecalculateRaider();
        int throughPut = m_rosterMember.RaiderStats().GetAverageThroughput();
        MainSpecThroughPutText.text = throughPut.ToString();
        OffSpecThroughPutText.text = throughPut.ToString();

        //Rewrite this once you have differences between the two specs
        Enums.CharacterRole mainSpecRole = m_rosterMember.RaiderStats().GetRole();
        bool isMainSpecDPS = (mainSpecRole == Enums.CharacterRole.MeleeDPS) || (mainSpecRole == Enums.CharacterRole.RangedDPS);
        

        if (currentRole != mainSpecRole && !(isCurrentRoleDPS && isMainSpecDPS))
        {
            MainSpecButton.interactable = false;
        }


        MainSpecButtonText.text = m_rosterMember.RaiderStats().GetCurrentSpec() + "\n" + GetRoleString(mainSpecRole);

        Enums.CharacterRole offSpecRole = m_rosterMember.RaiderStats().GetOffSpecRole();
        bool isOffSpecDPS = (offSpecRole == Enums.CharacterRole.MeleeDPS) || (offSpecRole == Enums.CharacterRole.RangedDPS);

        if (currentRole != offSpecRole && !(isOffSpecDPS && isCurrentRoleDPS))
        {
            OffSpecButton.interactable = false;
        }

        OffSpecButtonText.text = m_rosterMember.RaiderStats().GetOffSpec() + "\n" + GetRoleString(offSpecRole);
    }

    string GetRoleString(Enums.CharacterRole role) {
        if (role == Enums.CharacterRole.RangedDPS)
            return "RDPS";

        if (role == Enums.CharacterRole.MeleeDPS)
            return "MDPS";

        return role.ToString();
    }

    public void ChooseMainSpec()
    {
        MainSpecButton.interactable = false;
        OffSpecButton.interactable = false;
        m_controller.AddRaiderToTeam(m_rosterMember);
    }

    public void ChooseOffSpec()
    {
        MainSpecButton.interactable = false;
        OffSpecButton.interactable = false;
        m_rosterMember.ChangeSpec();
        m_controller.AddRaiderToTeam(m_rosterMember);
    }
}
