using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainSceneMainController : MonoBehaviour {

    public Canvas canvas;
    public Text ItemsText;
    public GameObject RosterControllerPrefab;
    public GameObject ProgressControllerPrefab;
    public GameObject RecruitmentControllerPrefab;
    public GameObject BankControllerPrefab;


    RosterControllerScript m_rosterScript;
    MainGameSceneRecruitmentController m_recruitmentScript;
    MainSceneItemsController m_itemScript;

	// Use this for initialization
	void Start () {
        m_rosterScript = RosterControllerPrefab.GetComponent<RosterControllerScript>();
        m_recruitmentScript = RecruitmentControllerPrefab.GetComponent<MainGameSceneRecruitmentController>();
        m_itemScript = BankControllerPrefab.GetComponent<MainSceneItemsController>();

        m_rosterScript.gameObject.SetActive(false);
        m_recruitmentScript.gameObject.SetActive(false);
        m_itemScript.gameObject.SetActive(false);

        OnClickRoster();
    }
	
	// Update is called once per frame
	void Update () {
        ItemsText.text = "Purchase Items\n(" + PlayerData.RaidTeamGold + " gold available)";

    }

    public void OnClickRoster()
    {
        m_rosterScript.Reactivate();
        m_recruitmentScript.gameObject.SetActive(false);
        m_itemScript.gameObject.SetActive(false);
    }

    public void OnClickProgress()
    {
        m_rosterScript.gameObject.SetActive(false);
        m_recruitmentScript.gameObject.SetActive(false);
        m_itemScript.gameObject.SetActive(false);
    }

    public void OnClickRecruitment()
    {
        m_rosterScript.gameObject.SetActive(false);
        m_recruitmentScript.gameObject.SetActive(true);
        m_itemScript.gameObject.SetActive(false);
    }

    public void OnClickItems()
    {
        m_rosterScript.gameObject.SetActive(false);
        m_recruitmentScript.gameObject.SetActive(false);
        m_itemScript.Reactivate();
    }

    public void OnClickRaid()
    {
        if(PlayerData.CanRaidWithRoster())
            SceneManager.LoadScene("ChooseEncounterScene");
    }
}
