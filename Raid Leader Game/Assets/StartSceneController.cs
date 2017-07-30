using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartSceneController : MonoBehaviour {

    public Button StartGameButton;
    public Button LoadDataButton;
    public Button NewGameButton;
    public InputField LoadDataInputField;
    public GameObject CreateNewPrefab;

	// Use this for initialization
	void Start () {
        Utility.Initialize();
        CreateNewPrefab.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnNewGameClick()
    {
        NewGameButton.gameObject.SetActive(false);
        LoadDataButton.gameObject.SetActive(false);
        CreateNewPrefab.SetActive(true);
    }

    public void BackFromCreateNewGameClicked()
    {
        NewGameButton.gameObject.SetActive(true);
        LoadDataButton.gameObject.SetActive(true);
        CreateNewPrefab.SetActive(false);
    }
}
