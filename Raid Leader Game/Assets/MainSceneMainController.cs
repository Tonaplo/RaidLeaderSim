﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneMainController : MonoBehaviour {

    public GameObject RosterControllerPrefab;
    public Canvas canvas;

    RosterControllerScript m_rosterScript;

	// Use this for initialization
	void Start () {
        Utility.Initialize();
        RosterControllerPrefab = GameObject.Instantiate(RosterControllerPrefab);
        RosterControllerPrefab.transform.SetParent(canvas.transform);
        m_rosterScript = RosterControllerPrefab.GetComponent<RosterControllerScript>();
        m_rosterScript.SetActive(false);
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
        SceneManager.LoadScene("RaidScene");
    }
}