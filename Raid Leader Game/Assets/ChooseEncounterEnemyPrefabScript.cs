using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseEncounterEnemyPrefabScript : MonoBehaviour {

    public Text Name;
    public Text Description;
    public Button ViewButton;

    ChooseEncounterSceneController m_cesc;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Initialize(string n, string desc, ChooseEncounterSceneController c)
    {
        Name.text = n;
        Description.text = desc;
        m_cesc = c;
    }

    public void ClickButton()
    {
        m_cesc.ClickViewButton(Name.text);
        ViewButton.interactable = false;
    }
}
