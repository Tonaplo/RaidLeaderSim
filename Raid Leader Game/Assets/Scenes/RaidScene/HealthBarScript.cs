using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour {

    public Slider HealthBarSlider;
    public Image Fill;
    public Text HealthBarText;
    public Button RaiderButton;

    bool m_useName = false;
    string m_name;

    bool m_usePercent = false;

    // Use this for initialization
    void Start() {
        RaiderButton.interactable = false;
        RaiderButton.GetComponent<Image>().color = new Color(0,0,0,0);
        
    }

    public void SetUseName(string name, bool on) {
        if (on)
        {
            HealthBarText.text = name + "\n" + HealthBarSlider.value + "/" + HealthBarSlider.maxValue;
            HealthBarText.resizeTextForBestFit = true;
        }

        m_name = name;
        m_useName = on;
    }

    public void SetUsePercent(bool on) { m_usePercent = on; }

    public void SetupHealthBar(int xPos, int yPos, int height, int width, int maxHealth) {

        HealthBarSlider.transform.SetPositionAndRotation(new Vector3(xPos, yPos, 0), Quaternion.identity);
        HealthBarSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        HealthBarSlider.maxValue = maxHealth;
        HealthBarSlider.value = maxHealth;

        SetText();
    }

    public void SetNewMaxHealth(int maxHealth)
    {
        HealthBarSlider.maxValue = maxHealth;
        HealthBarSlider.value = maxHealth;
        SetText();
    }

    public bool IsDead()
    {
        return HealthBarSlider.value <= 0;
    }

    public void ModifyHealth(int amount) {
        ModifyHealth((float)amount);
    }

    public void ModifyHealth(float amount)
    {
        if(HealthBarSlider.value == 0)
            return;

        float newAmount = (HealthBarSlider.value + amount);

        if (newAmount >= HealthBarSlider.maxValue)
            HealthBarSlider.value = HealthBarSlider.maxValue;
        else if (newAmount < 0)
        {
            HealthBarSlider.value = 0;
        }
        else
            HealthBarSlider.value = newAmount;

        SetText();
    }

    public int GetHealthPercent()
    {
        return (int)(HealthBarSlider.value / HealthBarSlider.maxValue * 100.0f);
    }

    void SetText()
    {
        string healthString = HealthBarSlider.value + "/" + HealthBarSlider.maxValue;
        if (HealthBarSlider.value == 0)
            healthString = "DEAD";
        else if (m_usePercent)
            healthString += " (" + GetHealthPercent() + " %)";

        if (!m_useName)
            HealthBarText.text = healthString;
        else
        {
            HealthBarText.text = m_name + "\n" + healthString;
        }
    }
}
