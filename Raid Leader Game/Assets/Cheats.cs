using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cheats : MonoBehaviour {

    public InputField input;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetSkill()
    {
        PlayerData.SetSkillOfRaid(int.Parse(input.text));
        DataController.controller.Save();
    }

    public void SetGear()
    {
        PlayerData.SetGearOfRaid(int.Parse(input.text));
        DataController.controller.Save();
    }
}
