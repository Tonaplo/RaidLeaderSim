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

    public Button RosterButton;
    public Button ProgressButton;
    public Button RecruitmentButton;
    public Button ItemsButton;

    public Text AttemptsLeftText;


    RosterControllerScript m_rosterScript;
    MainGameSceneRecruitmentController m_recruitmentScript;
    MainSceneItemsController m_itemScript;
    MainGameSceneProgressController m_progressScript;

	// Use this for initialization
	void Start () {
        m_rosterScript = RosterControllerPrefab.GetComponent<RosterControllerScript>();
        m_recruitmentScript = RecruitmentControllerPrefab.GetComponent<MainGameSceneRecruitmentController>();
        m_itemScript = BankControllerPrefab.GetComponent<MainSceneItemsController>();
        m_progressScript = ProgressControllerPrefab.GetComponent<MainGameSceneProgressController>();

        m_rosterScript.gameObject.SetActive(false);
        m_recruitmentScript.gameObject.SetActive(false);
        m_itemScript.gameObject.SetActive(false);
        m_progressScript.gameObject.SetActive(false);
        AttemptsLeftText.text = PlayerData.AttemptsLeft.ToString();

        if (PlayerData.AttemptsLeft > 0)
        {
            AttemptsLeftText.color = Color.green;
        }
        else
        {
            AttemptsLeftText.color = Color.red;
        }

        OnClickRoster();
    }
	
	// Update is called once per frame
	void Update () {
        ItemsText.text = "Purchase Items\n(" + PlayerData.RaidTeamGold + " gold available)";

    }

    public void OnClickRoster()
    {
        HandleButtons(RosterButton);
        m_rosterScript.Reactivate();
        m_recruitmentScript.gameObject.SetActive(false);
        m_itemScript.gameObject.SetActive(false);
        m_progressScript.gameObject.SetActive(false);
    }

    public void OnClickProgress()
    {
        HandleButtons(ProgressButton);
        m_rosterScript.gameObject.SetActive(false);
        m_recruitmentScript.gameObject.SetActive(false);
        m_itemScript.gameObject.SetActive(false);
        m_progressScript.Reactivate();
    }

    public void OnClickRecruitment()
    {
        HandleButtons(RecruitmentButton);
        m_rosterScript.gameObject.SetActive(false);
        m_recruitmentScript.gameObject.SetActive(true);
        m_itemScript.gameObject.SetActive(false);
        m_progressScript.gameObject.SetActive(false);
    }

    public void OnClickItems()
    {
        HandleButtons(ItemsButton);
        m_rosterScript.gameObject.SetActive(false);
        m_recruitmentScript.gameObject.SetActive(false);
        m_itemScript.Reactivate();
        m_progressScript.gameObject.SetActive(false);
    }

    public void OnClickRaid()
    {
        string errorString = "";
        if (Utility.IsAbleToRaid(out errorString))
            SceneManager.LoadScene("ChooseEncounterScene");
        else
        {
            Debug.Log(errorString);
        }
    }

    void HandleButtons(Button b)
    {
        RosterButton.interactable = true;
        ProgressButton.interactable = true;
        RecruitmentButton.interactable = true;
        ItemsButton.interactable = true;
        b.interactable = false;
    }
}
