using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaidSceneControlPanel : MonoBehaviour {

    public Image Background;
    public Button CooldownButton;
    public Text CooldownButtonText;
    public Button TauntButton;
    public Text TauntButtonText;
    public Button DispelButton;
    public Text DispelButtonText;
    
    public GameObject CooldownPrefab;
    public Button CooldownDamageButton;
    public Button CooldownHealingButton;

    public GameObject TauntPrefab;
    public GameObject CurrentTargetBackground;
    public Text CurrentTargetText;

    public RaidSceneController RSC;

    enum ButtonState
    {
        Cooldown,
        Taunt,
        Dispel,
        Hidden
    }

    int shownWidth;
    int shownHeight;
    int hiddenWidth = 70;
    int hiddenHeight = 140;
    ButtonState m_state = ButtonState.Cooldown;

    List<GameObject> m_Cooldownbuttons;
    List<GameObject> m_Tauntbuttons;
    List<GameObject> m_Dispelbuttons;

    // Use this for initialization
    void Start () {
        CooldownPrefab.SetActive(false);
        TauntPrefab.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        HandleStackUI();
	}

    public void Initialize()
    {
        shownWidth = (int)Background.GetComponent<RectTransform>().sizeDelta.x;
        shownHeight = (int)Background.GetComponent<RectTransform>().sizeDelta.y;
        m_Cooldownbuttons = new List<GameObject>();
        m_Tauntbuttons = new List<GameObject>();
        m_Dispelbuttons = new List<GameObject>();
        gameObject.SetActive(true);
        OnClickDamage();
        OnClickCooldown();
    }

    public void OnClickCooldown()
    {
        bool shouldShow = m_state != ButtonState.Cooldown;
        HandleCooldownUI(shouldShow);

        if (m_state == ButtonState.Cooldown)
        {
            m_state = ButtonState.Hidden;
            CooldownButtonText.text = "Cooldown";
            DispelButton.gameObject.SetActive(true);
            TauntButton.gameObject.SetActive(true);
            Background.GetComponent<RectTransform>().sizeDelta = new Vector2(hiddenWidth, hiddenHeight);
        }
        else
        {
            m_state = ButtonState.Cooldown;
            CooldownButtonText.text = "Hide";
            DispelButton.gameObject.SetActive(false);
            TauntButton.gameObject.SetActive(false);
            Background.GetComponent<RectTransform>().sizeDelta = new Vector2(shownWidth, shownHeight);
        }
    }

    void HandleCooldownUI(bool shouldShow)
    {
        for (int i = 0; i < m_Cooldownbuttons.Count; i++)
        {
            m_Cooldownbuttons[i].SetActive(shouldShow);
        }
        CooldownDamageButton.gameObject.SetActive(shouldShow);
        CooldownHealingButton.gameObject.SetActive(shouldShow);
    }

    public void OnClickDamage()
    {
        for (int i = 0; i < m_Cooldownbuttons.Count; i++)
        {
            Destroy(m_Cooldownbuttons[i]);
        }
        m_Cooldownbuttons.Clear();

        List<RaiderScript> dps = RSC.GetRaid().FindAll(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.MeleeDPS || x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.RangedDPS);

        for (int i = 0; i < dps.Count; i++)
        {
            GameObject temp = GameObject.Instantiate(CooldownPrefab);
            temp.SetActive(true);
            temp.transform.SetParent(Background.transform);
            temp.transform.SetPositionAndRotation(new Vector3(75 + (65 * (i % 5)), 110 - ((i / 5)) * 65, 0), Quaternion.identity);
            temp.GetComponent<RaidSceneCooldownPrefabScript>().Initialize(dps[i], RSC);
            m_Cooldownbuttons.Add(temp);
        }
    }

    public void OnClickHealing()
    {
        for (int i = 0; i < m_Cooldownbuttons.Count; i++)
        {
            Destroy(m_Cooldownbuttons[i]);
        }
        m_Cooldownbuttons.Clear();

        List<RaiderScript> tanksAndHeals = RSC.GetRaid().FindAll(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.Tank || x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.Healer);

        for (int i = 0; i < tanksAndHeals.Count; i++)
        {
            GameObject temp = GameObject.Instantiate(CooldownPrefab);
            temp.SetActive(true);
            temp.transform.SetParent(Background.transform);
            temp.transform.SetPositionAndRotation(new Vector3(75 + (65 * (i % 5)), 110 - ((i / 5)) * 65, 0), Quaternion.identity);
            temp.GetComponent<RaidSceneCooldownPrefabScript>().Initialize(tanksAndHeals[i], RSC);
            m_Cooldownbuttons.Add(temp);
        }
    }

    public void OnClickTaunts()
    {
        bool shouldShow = m_state != ButtonState.Taunt;
        HandleTauntsUI(shouldShow);

        if (m_state == ButtonState.Taunt)
        {
            m_state = ButtonState.Hidden;
            TauntButtonText.text = "Taunt";
            CooldownButton.gameObject.SetActive(true);
            DispelButton.gameObject.SetActive(true);
            Background.GetComponent<RectTransform>().sizeDelta = new Vector2(hiddenWidth, hiddenHeight);
        }
        else
        {
            m_state = ButtonState.Taunt;
            TauntButtonText.text = "Hide";
            CooldownButton.gameObject.SetActive(false);
            DispelButton.gameObject.SetActive(false);
            Background.GetComponent<RectTransform>().sizeDelta = new Vector2(shownWidth, shownHeight);
        }
    }

    void HandleTauntsUI(bool shouldShow)
    {
        CurrentTargetBackground.gameObject.SetActive(shouldShow);
        HandleStackUI();
        
        if (shouldShow)
        {
            List<RaiderScript> tanks = RSC.GetRaid().FindAll(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.Tank);

            tanks.Remove(tanks.Find(x => x == RSC.Encounter.CurrentTarget));

            for (int i = 0; i < tanks.Count; i++)
            {
                GameObject temp = GameObject.Instantiate(TauntPrefab);
                temp.SetActive(true);
                temp.transform.SetParent(Background.transform);
                temp.transform.SetPositionAndRotation(new Vector3(195 + (75 * (i % 3)), 110 - ((i / 3)) * 65, 0), Quaternion.identity);
                temp.GetComponent<RaidSceneTauntPrefabScript>().Initialize(tanks[i], RSC, this);
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
        bool shouldShow = m_state != ButtonState.Dispel;
        HandleDispelsUI(shouldShow);
        
        if (m_state == ButtonState.Dispel)
        {
            m_state = ButtonState.Hidden;
            DispelButtonText.text = "Dispel";
            CooldownButton.gameObject.SetActive(true);
            TauntButton.gameObject.SetActive(true);
            Background.GetComponent<RectTransform>().sizeDelta = new Vector2(hiddenWidth, hiddenHeight);
        }
        else
        {
            m_state = ButtonState.Dispel;
            DispelButtonText.text = "Hide";
            CooldownButton.gameObject.SetActive(false);
            TauntButton.gameObject.SetActive(false);
            Background.GetComponent<RectTransform>().sizeDelta = new Vector2(shownWidth, shownHeight);
        }
    }

    void HandleDispelsUI(bool shouldShow)
    {
    }

    void HandleStackUI()
    {
        int currentCounter = RSC.Encounter.Stacks;
        if (RSC.Encounter.CurrentTarget == null || RSC.Encounter.CurrentTarget.IsDead())
            CurrentTargetText.text = "No\ncurrent\ntarget!";
        else
            CurrentTargetText.text = "Current target:\n\n" + RSC.Encounter.CurrentTarget.Raider.GetName() + "\n\n" + RSC.Encounter.Stacks + ((RSC.Encounter.Stacks == 1) ? " Stack" : " Stacks");
    }
}
