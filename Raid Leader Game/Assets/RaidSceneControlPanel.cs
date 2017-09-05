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

    public GameObject DispelPrefab;
    public GameObject DispelCooldownPrefab;
    public GameObject DispellerBackground;

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
    List<GameObject> m_DispelButtons;
    List<GameObject> m_DispelCooldownButtons;

    // Use this for initialization
    void Start () {
        CooldownPrefab.SetActive(false);
        TauntPrefab.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void Initialize()
    {
        shownWidth = (int)Background.GetComponent<RectTransform>().sizeDelta.x;
        shownHeight = (int)Background.GetComponent<RectTransform>().sizeDelta.y;
        m_Cooldownbuttons = new List<GameObject>();
        m_Tauntbuttons = new List<GameObject>();
        m_DispelButtons = new List<GameObject>();
        m_DispelCooldownButtons = new List<GameObject>();
        gameObject.SetActive(true);
        SetupDispellers();
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

        HandleTauntsUI(shouldShow);
    }

    void HandleTauntsUI(bool shouldShow)
    {
        ReDrawTauntUI(shouldShow);
        HandleStackUI();
    }

    void ReDrawTauntUI(bool shouldShow)
    {
        CurrentTargetBackground.gameObject.SetActive(shouldShow);
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
        ReDrawDispelUI(shouldShow);
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
                temp.transform.SetParent(Background.transform);
                temp.GetComponent<RaidSceneDispelPrefab>().Initialize(debuffedRaiders[i], i, RSC, this);
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
        int dispellerCount = 0;
        for (int i = 0; i < RSC.GetRaid().Count; i++)
        {
            if (RSC.GetRaid()[i].Raider.RaiderStats.GetRole() == Enums.CharacterRole.Healer) {
                GameObject temp = GameObject.Instantiate(DispelCooldownPrefab);
                temp.transform.SetParent(Background.transform);
                temp.transform.SetPositionAndRotation(new Vector3(195 + (75 * (i % 3)), 110 - ((i / 3)) * 65, 0), Quaternion.identity);
                temp.GetComponent<RaidSceneDispellerCooldownPrefab>().Initialize(RSC.GetRaid()[i],dispellerCount, this);
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
        if (RSC.Encounter.CurrentTarget == null || RSC.Encounter.CurrentTarget.IsDead())
            CurrentTargetText.text = "No\ncurrent\ntarget!";
        else
            CurrentTargetText.text = "Current target:\n\n" + RSC.Encounter.CurrentTarget.Raider.GetName() + "\n\n" + RSC.Encounter.Stacks + ((RSC.Encounter.Stacks == 1) ? " Stack" : " Stacks");


        if (m_state == ButtonState.Taunt)
        {
            ReDrawTauntUI(false);
            ReDrawTauntUI(true);
        }
    }
}
