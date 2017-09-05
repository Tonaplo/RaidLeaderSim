using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour {
    
    public Image Background;
    public Image Fill;
    public Text HealthBarText;
    public Button RaiderButton;

    //variables for position and setup
    int m_height;
    int m_width;
    int m_maxHealth;
    int m_currentHealth;

    //variables for text
    string m_name;
    bool m_useName = false;
    bool m_usePercent = false;
    bool m_useSingleLine = false;

    //getters
    public int CurrentHealth { get { return m_currentHealth; } }
    public int MaxHealth { get { return m_maxHealth; } }

    // Use this for initialization
    void Start() {
        RaiderButton.interactable = false;
        RaiderButton.GetComponent<Image>().color = new Color(0,0,0,0);
    }

    public void SetUseName(string name, bool on) {
        m_name = name;
        m_useName = on;
        SetText();
    }

    public void SetUsePercent(bool on) {
        m_usePercent = on;
        SetText();
    }

    public void SetUseSingleLine(bool on) {
        m_useSingleLine = on;
        SetText();
    }

    public void SetupHealthBar(int xPos, int yPos, int height, int width, int maxHealth) {

        m_height = height;
        m_width = width;
        Background.transform.SetPositionAndRotation(new Vector3(xPos, yPos, 0), Quaternion.identity);
        Background.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        RaiderButton.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height * 0.8f);
        Fill.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        m_maxHealth = maxHealth;
        m_currentHealth = maxHealth;

        SetText();
    }

    public void SetNewMaxHealth(int maxHealth)
    {
        m_maxHealth = maxHealth;
        m_currentHealth = maxHealth;
        SetText();
    }

    public bool IsDead()
    {
        return m_currentHealth == 0;
    }

    public void ModifyHealth(int amount) {
        ModifyHealth((float)amount);
    }

    public void ModifyHealth(float amount)
    {
        if(m_currentHealth == 0)
            return;

        int newAmount = (int)(m_currentHealth + amount);

        if (newAmount >= m_maxHealth)
            m_currentHealth = m_maxHealth;
        else if (newAmount < 0)
        {
            m_currentHealth = 0;
        }
        else
            m_currentHealth = newAmount;

        SetText();
    }

    public int GetHealthPercent()
    {
        return (int)(((float)m_currentHealth / (float)m_maxHealth) * 100.0f);
    }

    void SetText()
    {
        string healthString = m_currentHealth + "/" + m_maxHealth;
        if (IsDead())
        {
            healthString = "DEAD";
        }
        else if (m_usePercent)
            healthString += " (" + GetHealthPercent() + " %)";

        if (!m_useName)
            HealthBarText.text = healthString;
        else
        {
            string singleLine = "\n";

            if (m_useSingleLine)
                singleLine = " ";

            HealthBarText.text = m_name + singleLine + healthString;
        }

        if (IsDead())
            Fill.gameObject.SetActive(false);
        else
        {
            float newWidthForFill = (((float)m_currentHealth / (float)m_maxHealth) * m_width);
            Fill.GetComponent<RectTransform>().sizeDelta = new Vector2(newWidthForFill, m_height);
        }
    }
}
