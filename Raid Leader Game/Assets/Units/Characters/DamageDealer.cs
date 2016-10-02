using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageDealer : BaseCharacter
{
    int baseAoeDamage;
    int baseSingleTargetDamage;
    int maxBaseDamage = 100;

    public int GetAoEDamage() { return (int)((float)ComputeThroughput(maxBaseDamage)*((float)baseAoeDamage/100.0f)); }
    public int GetSingleTargetDamage() { return (int)((float)ComputeThroughput(maxBaseDamage) * ((float)baseSingleTargetDamage / 100.0f)); }

	// Use this for initialization
	void Start () {
        role = Enums.CharacterRole.DamageDealer;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
