using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Healer : BaseCharacter {

    int baseHealing = 30;

    public int GetHealing() { return ComputeThroughput(baseHealing); }

	// Use this for initialization
	void Start () {
        role = Enums.CharacterRole.Healer;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
