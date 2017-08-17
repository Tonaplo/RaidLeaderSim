using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseEncounterAttackPrefabScript : MonoBehaviour {

    public Text Name;
    public Text Description;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Initialize(string n, string desc)
    {
        Name.text = n;
        Description.text = desc;
    }
}
