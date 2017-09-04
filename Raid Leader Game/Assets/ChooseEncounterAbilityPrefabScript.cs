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

        string counterText = counter.ToString();

        if (counter == Enums.Ability.PostMovePositional || counter == Enums.Ability.PreMovePositional)
            counterText = " Moving";

        Counter.text = "Countered by: " + counterText + ".";

        if (counter == Enums.Ability.Uncounterable)
            Counter.text = "Cannot be countered!";
    }
}
