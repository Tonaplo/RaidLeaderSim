using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseEncounterAbilityPrefabScript : MonoBehaviour {

    public Text Name;
    public Text Description;
    public Text Counter;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Initialize(string n, string desc, Enums.Ability counter)
    {
        Name.text = n;
        Description.text = desc;
        Counter.text = "Countered by: " + counter.ToString();
    }
}
