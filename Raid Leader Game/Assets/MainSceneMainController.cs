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

	// Use this for initialization
	void Start () {
        m_rosterScript = RosterControllerPrefab.GetComponent<RosterControllerScript>();

        m_rosterScript.SetActive(false);
        OnClickRoster();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClickRoster()
    {
        m_rosterScript.SetActive(true);
    }

    public void OnClickProgress()
    {
        m_rosterScript.SetActive(false);
    }

    public void OnClickRaid()
    {
        if(PlayerData.CanRaidWithRoster())
            SceneManager.LoadScene("ChooseEncounterScene");
    }
}
