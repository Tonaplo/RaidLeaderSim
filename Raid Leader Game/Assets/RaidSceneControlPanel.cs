using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaidSceneControlPanel : MonoBehaviour {

    public Image Background;
    public Button EventLogButton;
    public Button DispelButton;
    public Button TauntButton;
    public Button CooldownButton;

    public GameObject EventLogBackScrollView;
    
    public GameObject CooldownPrefab;
    public GameObject DefensiveCooldownsBackGround;
    public GameObject OffensiveCooldownsBackGround;

    public GameObject TauntPrefab;
    public GameObject CurrentTargetBackground;
    public Text CurrentTargetText;

    public GameObject DispelPrefab;
    public GameObject DispelCooldownPrefab;
    public GameObject DispellerBackground;

    public RaidSceneController RSC;

    enum ButtonState
    {
        Cooldown,
        Taunt,
        Dispel,
        EventLog
    }

    float m_scale;
    ButtonState m_state = ButtonState.EventLog;

    List<GameObject> m_Cooldownbuttons;
    List<GameObject> m_Tauntbuttons;
    List<GameObject> m_DispelButtons;
    List<GameObject> m_DispelCooldownButtons;

    // Use this for initialization
    void Start () {
        CooldownPrefab.SetActive(false);
        TauntPrefab.SetActive(false);
        m_scale = GameObject.FindGameObjectWithTag("Canvas").transform.localScale.x;
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void Initialize()
    {
        m_scale = GameObject.FindGameObjectWithTag("Canvas").transform.localScale.x;
        m_Cooldownbuttons = new List<GameObject>();
        m_Tauntbuttons = new List<GameObject>();
        m_DispelButtons = new List<GameObject>();
        m_DispelCooldownButtons = new List<GameObject>();
        gameObject.SetActive(true);
        SetupDispellers();
        SetupCooldownButtons();
        OnClickEventLog();
    }

    public void OnClickEventLog()
    {
        m_state = ButtonState.EventLog;
        HandleEventLogUI(true);
        HandleCooldownUI(false);
        HandleTauntsUI(false);
        HandleDispelsUI(false);
    }

    void HandleEventLogUI(bool shouldShow)
    {
        EventLogButton.interactable = !shouldShow;
        EventLogBackScrollView.gameObject.SetActive(shouldShow);
    }

    public void OnClickCooldown()
    {
        m_state = ButtonState.Cooldown;
        HandleEventLogUI(false);
        HandleCooldownUI(true);
        HandleTauntsUI(false);
        HandleDispelsUI(false);
    }

    void HandleCooldownUI(bool shouldShow)
    {
        for (int i = 0; i < m_Cooldownbuttons.Count; i++)
        {
            m_Cooldownbuttons[i].SetActive(shouldShow);
        }

        CooldownButton.interactable = !shouldShow;
        DefensiveCooldownsBackGround.SetActive(shouldShow);
        OffensiveCooldownsBackGround.SetActive(shouldShow);
    }

    void SetupCooldownButtons()
    {
        for (int i = 0; i < m_Cooldownbuttons.Count; i++)
        {
            Destroy(m_Cooldownbuttons[i]);
        }
        m_Cooldownbuttons.Clear();

        m_scale = GameObject.FindGameObjectWithTag("Canvas").transform.localScale.x;
        int perRow = 4;
        float height = 35 *m_scale;
        float width = 81.75f * m_scale;
        float xPos = 75 * m_scale;
        float healerStartY = 139 * m_scale;

        List<RaiderScript> tanksAndHeals = RSC.GetRaid().FindAll(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.Tank || x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.Healer);

        for (int i = 0; i < tanksAndHeals.Count; i++)
        {
            GameObject temp = GameObject.Instantiate(CooldownPrefab);
            temp.SetActive(true);
            temp.transform.SetParent(Background.transform, false);
            temp.transform.SetPositionAndRotation(new Vector3(xPos + (width * (i % perRow)), healerStartY - ((i / perRow)) * height, 0), Quaternion.identity);
            temp.GetComponent<RaidSceneCooldownPrefabScript>().Initialize(tanksAndHeals[i], RSC);
            m_Cooldownbuttons.Add(temp);
        }

        float dpsStart = 55;
        if (tanksAndHeals.Count <= 4)
        {
            OffensiveCooldownsBackGround.transform.SetPositionAndRotation(new Vector3(dpsStart, 125*m_scale, 0), Quaternion.identity);
            dpsStart = 90;
        }

        dpsStart *= m_scale;
        List<RaiderScript> dps = RSC.GetRaid().FindAll(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.MeleeDPS || x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.RangedDPS);

        for (int i = 0; i < dps.Count; i++)
        {
            GameObject temp = GameObject.Instantiate(CooldownPrefab);
            temp.SetActive(true);
            temp.transform.SetParent(Background.transform, false);
            temp.transform.SetPositionAndRotation(new Vector3(xPos + (width * (i % perRow)), dpsStart - ((i / perRow)) * height, 0), Quaternion.identity);
            temp.GetComponent<RaidSceneCooldownPrefabScript>().Initialize(dps[i], RSC);
            m_Cooldownbuttons.Add(temp);
        }

        
    }

    public void OnClickTaunts()
    {
        m_state = ButtonState.Taunt;
        HandleEventLogUI(false);
        HandleCooldownUI(false);
        HandleTauntsUI(true);
        HandleDispelsUI(false);
    }

    void HandleTauntsUI(bool shouldShow)
    {
        ReDrawTauntUI(shouldShow);
        HandleStackUI();
        TauntButton.interactable = !shouldShow;
    }

    void ReDrawTauntUI(bool shouldShow)
    {
        float width = 75 * m_scale;
        float height = 65 * m_scale;
        float xPos = 195 * m_scale;
        float yPos = 110 * m_scale;

        CurrentTargetBackground.gameObject.SetActive(shouldShow);
        if (shouldShow)
        {
            List<RaiderScript> tanks = RSC.GetRaid().FindAll(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.Tank);

            tanks.Remove(tanks.Find(x => x == RSC.Encounter.CurrentRaiderTarget));

            for (int i = 0; i < tanks.Count; i++)
            {
                GameObject temp = GameObject.Instantiate(TauntPrefab);
                temp.SetActive(true);
                temp.transform.SetParent(Background.transform, false);
                temp.transform.SetPositionAndRotation(new Vector3(xPos + (width * (i % 3)), yPos - ((i / 3)) * height, 0), Quaternion.identity);
                temp.GetComponent<RaidSceneTauntPrefabScript>().Initialize(tanks[i], RSC);
                m_Tauntbuttons.Add(temp);
            }
        }
        else
        {

            for (int i = 0; i < m_Tauntbuttons.Count; i++)
            {
                Destroy(m_Tauntbuttons[i]);
            }
            m_Tauntbuttons.Clear();
        }
    }

    public void OnClickDispels()
    {
        m_state = ButtonState.Dispel;
        HandleEventLogUI(false);
        HandleCooldownUI(false);
        HandleTauntsUI(false);
        HandleDispelsUI(true);
    }

    void HandleDispelsUI(bool shouldShow)
    {
        ReDrawDispelUI(shouldShow);
        DispelButton.interactable = !shouldShow;
    }

    public void TryToUpdateDispellUI()
    {
        if (m_state == ButtonState.Dispel)
        {
            OnClickDispels();
            OnClickDispels();
        }
    }

    public void ReDrawDispelUI(bool shouldShow)
    {
        DispellerBackground.SetActive(shouldShow);
        for (int i = 0; i < m_DispelCooldownButtons.Count; i++)
        {
            if (shouldShow)
                m_DispelCooldownButtons[i].GetComponent<RaidSceneDispellerCooldownPrefab>().UnPause();
            else
                m_DispelCooldownButtons[i].GetComponent<RaidSceneDispellerCooldownPrefab>().Pause();

            m_DispelCooldownButtons[i].SetActive(shouldShow);
        }

        if (shouldShow)
        {
            List<RaiderScript> debuffedRaiders = RSC.GetRaid().FindAll(x => x.HasDebuffs());

            for (int i = 0; i < debuffedRaiders.Count; i++)
            {
                GameObject temp = GameObject.Instantiate(DispelPrefab);
                temp.SetActive(true);
                temp.transform.SetParent(Background.transform, false);
                temp.GetComponent<RaidSceneDispelPrefab>().Initialize(debuffedRaiders[i], i, this);
                m_DispelButtons.Add(temp);
            }
        }
        else
        {

            for (int i = 0; i < m_DispelButtons.Count; i++)
            {
                Destroy(m_DispelButtons[i]);
            }
            m_DispelButtons.Clear();
        }
    }

    void SetupDispellers()
    {
        float width = 75 * m_scale;
        float height = 65 * m_scale;
        float xPos = 195 * m_scale;
        float yPos = 110 * m_scale;

        int dispellerCount = 0;
        for (int i = 0; i < RSC.GetRaid().Count; i++)
        {
            if (RSC.GetRaid()[i].Raider.RaiderStats.GetRole() == Enums.CharacterRole.Healer) {
                GameObject temp = GameObject.Instantiate(DispelCooldownPrefab);
                temp.transform.SetParent(Background.transform, false);
                temp.transform.SetPositionAndRotation(new Vector3(xPos + (width * (i % 3)), yPos - ((i / 3)) * height, 0), Quaternion.identity);
                temp.GetComponent<RaidSceneDispellerCooldownPrefab>().Initialize(RSC.GetRaid()[i],dispellerCount);
                m_DispelCooldownButtons.Add(temp);
                dispellerCount++;
            }
        }
    }

    public RaiderScript GetDispeller()
    {
        for (int i = 0; i < m_DispelCooldownButtons.Count; i++)
        {
            if (!m_DispelCooldownButtons[i].GetComponent<RaidSceneDispellerCooldownPrefab>().OnCooldown)
            {
                m_DispelCooldownButtons[i].GetComponent<RaidSceneDispellerCooldownPrefab>().PutOnCooldown();
                return m_DispelCooldownButtons[i].GetComponent<RaidSceneDispellerCooldownPrefab>().Dispeller;
            }
        }

        return null;
    }

    public void HandleStackUI()
    {
        if (RSC.Encounter.CurrentRaiderTarget == null || RSC.Encounter.CurrentRaiderTarget.IsDead())
            CurrentTargetText.text = "No\ncurrent\ntarget!";
        else
            CurrentTargetText.text = "Current target:\n\n" + RSC.Encounter.CurrentRaiderTarget.Raider.GetName() + "\n\n" + RSC.Encounter.Stacks + ((RSC.Encounter.Stacks == 1) ? " Stack" : " Stacks");


        if (m_state == ButtonState.Taunt)
        {
            ReDrawTauntUI(false);
            ReDrawTauntUI(true);
        }
    }
}
