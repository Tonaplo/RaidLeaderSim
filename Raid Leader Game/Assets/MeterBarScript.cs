using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeterBarScript : MonoBehaviour {
    public Slider BarSlider;
    public Image Fill;
    public Text NameText;
    public Text AmountText;

	// Use this for initialization
	void Start () {
        NameText.fontSize = 12;
        NameText.resizeTextForBestFit = false;
        AmountText.fontSize = 12;
        AmountText.resizeTextForBestFit = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateEntry(MeterControllerScript.Entry entry)
    {
        BarSlider.value = entry.Amount;
        NameText.text = entry.Name;
        AmountText.text = entry.Amount.ToString();
        Fill.color = Utility.GetColorFromClass(entry.Class);
    }

    public void FinalizeEntry(MeterControllerScript.Entry entry, float time)
    {
        BarSlider.value = entry.Amount;
        NameText.text = entry.Name;
        AmountText.text = System.Math.Round((entry.Amount / time), 1).ToString() + "/sec";
        Fill.color = Utility.GetColorFromClass(entry.Class);
    }

    public void UpdateMax(int newMax)
    {
        BarSlider.maxValue = newMax;
    }
}
