using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneMainController : MonoBehaviour {

    public Canvas canvas;
    public GameObject RosterControllerPrefab;
    public GameObject ProgressControllerPrefab;
    public GameObject RecruitmentControllerPrefab;
    public GameObject BankControllerPrefab;


    RosterControllerScript m_rosterScript;
    MainGameSceneRecruitmentController m_recruitmentScript;

	// Use this for initialization
	void Start () {
        m_rosterScript = RosterControllerPrefab.GetComponent<RosterControllerScript>();
        m_recruitmentScript = RecruitmentControllerPrefab.GetComponent<MainGameSceneRecruitmentController>();

        m_rosterScript.gameObject.SetActive(false);
        m_recruitmentScript.gameObject.SetActive(false);

        OnClickRoster();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClickRoster()
    {
        m_rosterScript.Reactivate();
        m_recruitmentScript.gameObject.SetActive(false);
    }

    public void OnClickProgress()
    {
        m_rosterScript.gameObject.SetActive(false);
        m_recruitmentScript.gameObject.SetActive(false);
    }

    public void OnClickRecruitment()
    {
        m_rosterScript.gameObject.SetActive(false);
        m_recruitmentScript.gameObject.SetActive(true);
    }

    public void OnClickItems()
    {
        m_rosterScript.gameObject.SetActive(false);
        m_recruitmentScript.gameObject.SetActive(false);
    }

    public void OnClickRaid()
    {
        if(PlayerData.CanRaidWithRoster())
            SceneManager.LoadScene("ChooseEncounterScene");
    }
}
