using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeterBarScript : MonoBehaviour {
    public Slider BarSlider;
    public Text NameText;
    public Text AmountText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateEntry(MeterControllerScript.Entry entry)
    {
        Debug.Log("Updating Entry for " + entry.Name + " - Previous Amount was " + BarSlider.value + " and new value is " + entry.Amount);
        BarSlider.value = entry.Amount;
        NameText.text = entry.Name;
        AmountText.text = entry.Amount.ToString();
    }

    public void UpdateMax(int newMax)
    {
        BarSlider.maxValue = newMax;
    }
}
