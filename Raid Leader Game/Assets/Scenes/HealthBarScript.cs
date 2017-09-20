using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour {
    
    public Image Background;
    public Image Fill;
    public Text HealthBarText;
    public Button BarButton;

    //variables for position and setup
    int m_height;
    int m_width;
    int m_index;
    int m_maxHealth;
    int m_currentHealth;

    //variables for text
    string m_name;
    Enums.HealthBarSetting m_setting; 

    //getters
    public int CurrentHealth { get { return m_currentHealth; } }
    public int MaxHealth { get { return m_maxHealth; } }
    public Enums.HealthBarSetting HealthBarSetting { get { return m_setting; } }

    // Use this for initialization
    void Start() {
        BarButton.GetComponent<Image>().color = new Color(0,0,0,0);
    }
    
    public void SetupHealthBar(string name, int index, Enums.HealthBarSetting s, int maxHealth) {

        m_name = name;
        m_index = index;
        m_setting = s;
        m_maxHealth = maxHealth;
        m_currentHealth = maxHealth;
        
        HandleNewSetting();
    }

    public void ChangeHealthbarSetting(Enums.HealthBarSetting s, int newIndex)
    {
        m_index = newIndex;
        m_setting = s;
        HandleNewSetting();
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

    void HandleNewSetting()
    {
        int xPos = 0;
        int xPosOffset = 0;
        int yPos = 0;
        int yPosOffset = 0;
        int indexMod = 1;
        float buttonHeightMultiplier = 0.0f;
        

        switch (m_setting)
        {
            case Enums.HealthBarSetting.CurrentTarget:
                xPos = 350;
                yPos = 390;
                m_width = 630;
                m_height = 60;
                buttonHeightMultiplier = 1.0f;
                BarButton.interactable = false;
                break;
            case Enums.HealthBarSetting.Enemy:
                xPos = 110;
                xPosOffset = 195;
                yPos = 310;
                yPosOffset = 45;
                m_width = 190;
                m_height = 45;
                indexMod = 2;
                buttonHeightMultiplier = 1.0f;
                BarButton.interactable = true;
                break;
            case Enums.HealthBarSetting.Raider:
                xPos = 460;
                xPosOffset = 95;
                yPos = 310;
                yPosOffset = 45;
                m_width = 95;
                m_height = 45;
                buttonHeightMultiplier = 0.8f;
                indexMod = 3;
                break;
            default:
                break;
        }

        Background.transform.SetPositionAndRotation(new Vector3(xPos + (m_index % indexMod)*xPosOffset, yPos - (m_index / indexMod) * yPosOffset, 0), Quaternion.identity);
        Background.GetComponent<RectTransform>().sizeDelta = new Vector2(m_width, m_height);
        BarButton.GetComponent<RectTransform>().sizeDelta = new Vector2(m_width, m_height * buttonHeightMultiplier);
        SetText();
    }

    void SetText()
    {
        string healthString = "";
        string deathstring = "";

        switch (m_setting)
        {
            case Enums.HealthBarSetting.CurrentTarget:
                healthString = m_name + "\n" +m_currentHealth + "/" + m_maxHealth + " (" + GetHealthPercent() + " %)";
                deathstring = m_name + "\nDEAD!";
                break;
            case Enums.HealthBarSetting.Enemy:
                healthString = m_name + " - " + m_currentHealth + "/" + m_maxHealth + " (" + GetHealthPercent() + " %)";
                deathstring = m_name + " - DEAD!";
                break;
            case Enums.HealthBarSetting.Raider:
                healthString = m_name + "\n" + m_currentHealth + "/" + m_maxHealth;
                deathstring = m_name + "\nDEAD!";
                break;
            default:
                break;
        }
        if (IsDead())
        {
            healthString = deathstring;
        }

        HealthBarText.text = healthString;

        if (IsDead())
            Fill.gameObject.SetActive(false);
        else
        {
            float newWidthForFill = (((float)m_currentHealth / (float)m_maxHealth) * m_width);
            Fill.GetComponent<RectTransform>().sizeDelta = new Vector2(newWidthForFill, m_height);
        }
    }
}
