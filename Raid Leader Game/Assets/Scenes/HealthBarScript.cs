using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{

    public Image Background;
    public Image Fill;
    public Text HealthBarText;
    public Button BarButton;

    class StackInfo
    {
        private int m_numStacks = 0;
        private float m_remainingTime = 0;
        private float m_totalDuration = 0;

        public int Stacks { get { return m_numStacks; } }
        public float TotalDuration { get { return m_totalDuration; } }
        public float RemainingTime { get { return m_remainingTime; } }

        public void SubtractTime(float delta)
        {
            if (m_numStacks > 0)
            {
                m_remainingTime -= delta;
                if (m_remainingTime < 0.0f)
                {
                    m_totalDuration = 0.0f;
                    m_remainingTime = 0.0f;
                    m_numStacks = 0;
                }
            }
        }

        public void AddStacks(int stacks, float duration)
        {
            m_numStacks += stacks;
            m_totalDuration = duration;
            m_remainingTime = m_totalDuration;
        }
    }

    //variables for position and setup
    float m_scale;
    float m_height;
    float m_width;
    int m_index;
    int m_maxHealth;
    int m_currentHealth;

    //variables for Stacks
    StackInfo m_stackInfo = new StackInfo();
    bool m_isCurrentTank = false;

    //variables for dispelling
    float m_dispellProgress = 0.0f;
    bool m_dispellState = false;
    bool m_fightOver = false;

    //variables for text
    string m_name;
    Enums.HealthBarSetting m_setting;

    //getters
    public int CurrentHealth { get { return m_currentHealth; } }
    public int MaxHealth { get { return m_maxHealth; } }
    public Enums.HealthBarSetting HealthBarSetting { get { return m_setting; } }
    public int Stacks() { return m_stackInfo.Stacks; }

    // Use this for initialization
    void Start()
    {
        m_scale = GameObject.FindGameObjectWithTag("Canvas").transform.localScale.x;
        BarButton.GetComponent<Image>().color = new Color(0,0,0,0);
    }

    private void Update()
    {
        if (IsDead() || m_fightOver)
        {
            SetText();
            return;
        }

        if (m_setting == Enums.HealthBarSetting.Tank)
        {
            m_stackInfo.SubtractTime(Time.deltaTime);
            if (m_stackInfo.Stacks > 0)
                HealthBarText.text = m_name + "\n" + m_stackInfo.Stacks + " stacks - " + UnityEngine.Mathf.Round(m_stackInfo.RemainingTime) + " s";
            else if (m_isCurrentTank)
                HealthBarText.text = m_name + "\ntaunted!";
            else
                SetText();
        }
        else if (m_setting == Enums.HealthBarSetting.Healer)
        {
            
            if (m_dispellProgress > 0)
            {
                m_dispellProgress -= Time.deltaTime;
                if(m_dispellState)
                    HealthBarText.text = m_name + "\n" + " cooldown - " + UnityEngine.Mathf.Round(m_dispellProgress) + " s";
            }
            else if(m_dispellState)
            {
                BarButton.interactable = true;
                BarButton.GetComponent<Image>().color = Color.green;
                m_dispellProgress = 0.0f;
                SetText();
            }
        }
    }

    public void SetupHealthBar(string name, int index, Enums.HealthBarSetting s, int maxHealth)
    {
        m_scale = GameObject.FindGameObjectWithTag("Canvas").transform.localScale.x;
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

    public void SetHealthMultiplier(float multiplier)
    {
        m_maxHealth = Mathf.RoundToInt(m_maxHealth* multiplier);
        m_currentHealth = Mathf.RoundToInt(m_currentHealth * multiplier);
        SetText();
    }

    public bool IsDead()
    {
        return m_currentHealth == 0;
    }

    public int ModifyHealth(int amount)
    {
        return ModifyHealth((float)amount);
    }

    public int ModifyHealth(float amount)
    {
        if (m_currentHealth == 0)
            return 0;

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
        return newAmount;
    }

    public int GetHealthPercent()
    {
        return (int)(((float)m_currentHealth / (float)m_maxHealth) * 100.0f);
    }

    void HandleNewSetting()
    {
        float scale = GameObject.FindGameObjectWithTag("Canvas").transform.localScale.x;
        float xPos = 0;
        float xPosOffset = 0;
        float yPos = 0;
        float yPosOffset = 0;
        int indexMod = 1;
        float buttonHeightMultiplier = 0.0f;


        switch (m_setting)
        {
            case Enums.HealthBarSetting.CurrentTarget:
                xPos = 233;
                yPos = 240;
                m_width = 330;
                m_height = 32;
                buttonHeightMultiplier = 1.0f;
                BarButton.interactable = false;
                break;
            case Enums.HealthBarSetting.Enemy:
                xPos = 65;
                yPos = 175;
                m_width = 80;
                xPosOffset = m_width * 1.4f;
                m_height = 25;
                yPosOffset = m_height * 1.4f;
                indexMod = 2;
                buttonHeightMultiplier = 1.0f;
                BarButton.interactable = true;
                break;

            case Enums.HealthBarSetting.Tank:
            case Enums.HealthBarSetting.Healer:
            case Enums.HealthBarSetting.DPS:
                xPos = 278;
                yPos = 185;
                m_width = 52;
                xPosOffset = m_width*1.4f;
                m_height = 20;
                yPosOffset = m_height*1.4f;
                buttonHeightMultiplier = 0.8f;
                indexMod = 3;
                BarButton.interactable = false;
                break;
            default:
                break;
        }

        m_height *= m_scale;
        m_width *= m_scale;
        xPos *= m_scale;
        xPosOffset *= m_scale;
        yPos *= m_scale;
        yPosOffset *= m_scale;

        if (m_setting == Enums.HealthBarSetting.Tank)
        {
            BarButton.GetComponent<Image>().color = Color.green;
            HealthBarText.text = m_name + "\nTaunt";
            BarButton.interactable = true;
        }

        xPos *= scale;
        xPosOffset *= scale;
        yPos *= scale;
        yPosOffset *= scale;

        Background.transform.SetPositionAndRotation(new Vector3(xPos + (m_index % indexMod) * xPosOffset, yPos - (m_index / indexMod) * yPosOffset, 0), Quaternion.identity);
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
                healthString = m_name + "\n" + m_currentHealth + "/" + m_maxHealth + " (" + GetHealthPercent() + " %)";
                deathstring = m_name + "\nDEAD!";
                break;
            case Enums.HealthBarSetting.Enemy:
                healthString = m_name + " - " + m_currentHealth + "/" + m_maxHealth + " (" + GetHealthPercent() + " %)";
                deathstring = m_name + " - DEAD!";
                break;
            case Enums.HealthBarSetting.DPS:
            case Enums.HealthBarSetting.Tank:
            case Enums.HealthBarSetting.Healer:
                healthString = m_name;// + "\n" + m_currentHealth + "/" + m_maxHealth;
                deathstring = "DEAD!";
                break;
            default:
                break;
        }
        if (IsDead())
        {
            healthString = deathstring;
        }

        if(m_stackInfo.Stacks == 0 || m_fightOver || IsDead())
            HealthBarText.text = healthString;

        if (IsDead())
        {
            Fill.gameObject.SetActive(false);
            BarButton.interactable = false;
            BarButton.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }
        else
        {
            float newWidthForFill = (((float)m_currentHealth / (float)m_maxHealth) * m_width);
            Fill.GetComponent<RectTransform>().sizeDelta = new Vector2(newWidthForFill, m_height);
        }
    }

    public void AddStacks(int stacks, float duration)
    {
        m_stackInfo.AddStacks(stacks, duration);
    }

    public void SetCurrentTank(bool on)
    {
        if (m_setting != Enums.HealthBarSetting.Tank)
            return;

        if (IsDead()) {
            BarButton.interactable = false;
            BarButton.GetComponent<Image>().color = new Color(0,0,0,0);
            return;
        }

        m_isCurrentTank = on;
        
        if (m_isCurrentTank)
        {
            BarButton.interactable = false;
            BarButton.GetComponent<Image>().color = Color.grey;
        }
        else {
            BarButton.interactable = true;
            BarButton.GetComponent<Image>().color = Color.green;
        }
    }

    public void SetDispelState(bool on)
    {
        if (m_setting != Enums.HealthBarSetting.Healer)
            return;

        if (IsDead())
        {
            BarButton.interactable = false;
            BarButton.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            return;
        }

        m_dispellState = on;

        if (!on)
        {
            BarButton.interactable = false;
            BarButton.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            SetText();
        }
        else if (Mathf.Approximately(m_dispellProgress, 0.0f))
        {
            BarButton.interactable = true;
            BarButton.GetComponent<Image>().color = Color.green;
        }
        else {
            BarButton.interactable = false;
            BarButton.GetComponent<Image>().color = Color.grey;
        }
    }

    public void AttemptedToDispell()
    {
        if (IsDead())
        {
            BarButton.interactable = false;
            BarButton.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            return;
        }

        m_dispellProgress = 15.0f;
        BarButton.interactable = false;
        BarButton.GetComponent<Image>().color = Color.grey;
    }

    public void FightEnded()
    {
        m_fightOver = true;
        BarButton.interactable = false;
        BarButton.GetComponent<Image>().color = new Color(0,0,0,0);
    }
}