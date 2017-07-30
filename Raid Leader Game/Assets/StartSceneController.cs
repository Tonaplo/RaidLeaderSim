using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Utility.Initialize();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnStartGameClick()
    {
        SceneManager.LoadScene("MainGameScene");
    }
}
