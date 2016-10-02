using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RaidSceneController : MonoBehaviour {

    public GameObject damageDealer;
    public GameObject healer;
    public Text text;

    List<BaseCharacter> allCharacters = new List<BaseCharacter>();
    List<DamageDealer> damageDealers = new List<DamageDealer>();
    List<Healer> healers = new List<Healer>();

    float healInterval;
    float healProgress = 0.0f;

    float damageInterval = 1.1f;
    float damageProgress = 0.0f;
    int currentHealer = 0;

    bool encounterStarted = false;

	// Use this for initialization
	void Start () {
        GameObject dd1 = GameObject.Instantiate(damageDealer);
        GameObject dd2 = GameObject.Instantiate(damageDealer);
        GameObject dd3 = GameObject.Instantiate(damageDealer);
        GameObject h1 = GameObject.Instantiate(healer);
        GameObject h2 = GameObject.Instantiate(healer);

        dd1.name = "Morifa";
        dd2.name = "Rahran";
        dd3.name = "Kaldorath";
        h1.name = "Mallusof";
        h2.name = "Amranar";

        dd1.transform.Translate(-4, 0, 0);
        dd2.transform.Translate(-2, 0, 0);
        dd3.transform.Translate(0, 0, 0);
        h1.transform.Translate(2, 0, 0);
        h2.transform.Translate(4, 0, 0);

        allCharacters.Add(dd1.GetComponent<DamageDealer>());
        allCharacters.Add(dd2.GetComponent<DamageDealer>());
        allCharacters.Add(dd3.GetComponent<DamageDealer>());
        allCharacters.Add(h1.GetComponent<Healer>());
        allCharacters.Add(h2.GetComponent<Healer>());

        damageDealers.Add(dd1.GetComponent<DamageDealer>());
        damageDealers.Add(dd2.GetComponent<DamageDealer>());
        damageDealers.Add(dd3.GetComponent<DamageDealer>());
        healers.Add(h1.GetComponent<Healer>());
        healers.Add(h2.GetComponent<Healer>());
        healProgress = 0.0f;
        damageInterval = 1.1f;
        damageProgress = 0.0f;
        ComputeHealInterval();
	}

    public void ComputeHealInterval() {
        healInterval = (2.0f / (float)healers.Count);
    }
	
	// Update is called once per frame
	void Update () {
        //if (!encounterStarted)
        //    return;

        healProgress += Time.deltaTime;
        damageProgress += Time.deltaTime;

        //If it's more than the interval amount of time since we healed, HEAL!
        if (healProgress > healInterval) {

            int healing = healers[currentHealer].GetHealing();
            print(healing);
            for (int i = 0; i < allCharacters.Count; i++) {
                
                allCharacters[i].ModifyHealth(healing);
            }
            currentHealer++;

            if (currentHealer == healers.Count-1)
            {
                currentHealer = 0;
            }

           

            healProgress = 0.0f;
        }

        if (damageProgress > damageInterval)
        {
            for (int i = 0; i < allCharacters.Count; i++)
            {
                allCharacters[i].ModifyHealth(-25);
            }
            damageProgress = 0.0f;
        }

        text.text = "Party:\n";
        for (int i = 0; i < allCharacters.Count; i++)
        {
            text.text += allCharacters[i].name + ": " + allCharacters[i].GetHealth() + "/" + allCharacters[i].GetMaxHealth() + "\n";
        }
    }
}
