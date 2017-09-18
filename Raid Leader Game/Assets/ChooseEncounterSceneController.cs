using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChooseEncounterSceneController : MonoBehaviour {

    public Button FightButton;
    public Text FightButtonText;
    public Text DescriptionText;

    public Dropdown EncounterSelectorDropdown;
    public Dropdown EncounterDifficultyDropdown;

    public GameObject AttackPrefab;
    public RectTransform AttackContent;
    public Scrollbar AttackScrollBar;

    public GameObject AbilityPrefab;
    public RectTransform AbilityContent;
    public Scrollbar AbilityScrollBar;

    Enums.Difficulties m_encounterDifficulty;
    List<BaseEncounter> m_encounters;
    BaseEncounter m_currentlySelectedEncounter;

    List<GameObject> m_attackObjects = new List<GameObject>();
    List<GameObject> m_abilityObjects = new List<GameObject>();

    // Add new Encounters here
    void Start () {
        m_encounters = new List<BaseEncounter> {
            new MoAKeeperOfTheMine(),
            new MoAVampiricus(),
            new MoACouncilOfStone(),
            new MoAMineKingAtrea(),
        };

        for (int i = 0; i < m_encounters.Count; i++)
        {
            m_encounters[i].InitializeForChoice(m_encounterDifficulty);
        }

        EncounterSelectorDropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        
        for (int i = 0; i < m_encounters.Count; i++)
        {
            options.Add(new Dropdown.OptionData(m_encounters[i].Name));
        }

        EncounterSelectorDropdown.AddOptions(options);

        OnEncounterChanged();
        OnDifficultyChanged();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnFightClicked()
    {
        Utility.SetCurrentEncounter(m_currentlySelectedEncounter);

        SceneManager.LoadScene("ChooseRaidTeamForAttempt");
    }

    public void OnBackClicked()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    public void OnDifficultyChanged()
    {
        m_encounterDifficulty = (Enums.Difficulties) EncounterDifficultyDropdown.value;

        for (int i = 0; i < m_encounters.Count; i++)
        {
            m_encounters[i].InitializeForChoice(m_encounterDifficulty);
        }

        PopulateLists();
    }

    public void OnEncounterChanged()
    {
        m_currentlySelectedEncounter = m_encounters[EncounterSelectorDropdown.value];
        FightButtonText.text = "Fight " + m_currentlySelectedEncounter.Name + "!";
        PopulateLists();
    }

    void PopulateLists() {
        ClearAndPopulateAttackList();
        ClearAndPopulateAbilityList();

        string errorText = "";
        if (Utility.CanAttemptEncounter(m_currentlySelectedEncounter.EncounterEnum, m_currentlySelectedEncounter.Difficulty, out errorText))
        {
            DescriptionText.text = m_currentlySelectedEncounter.Description;
            FightButton.interactable = true;
        }
        else
        {
            DescriptionText.text = errorText;
            FightButton.interactable = false;
        }
    }

    void ClearAndPopulateAttackList()
    {
        for (int i = 0; i < m_attackObjects.Count; i++)
        {
            Destroy(m_attackObjects[i]);
        }
        m_attackObjects.Clear();

        List<EncounterAttackDescription> attacks = m_currentlySelectedEncounter.EncounterAttacks;

        //calculate the width and height of each child item.
        int columnCount = attacks.Count;
        float width = 200;
        float height = 135;
        int rowCount = 1;

        //adjust the width of the container so that it will just barely fit all its children
        float scrollWidth = width * columnCount;
        AttackContent.offsetMin = new Vector2(-scrollWidth / 2, AttackContent.offsetMin.y);
        AttackContent.offsetMax = new Vector2(scrollWidth / 2, AttackContent.offsetMax.y);

        int j = 0;
        for (int i = 0; i < attacks.Count; i++)
        {
            //this is used instead of a double for loop because itemCount may not fit perfectly into the rows/columns
            if (i % columnCount == 0)
                j++;

            //create a new item, name it, and set the parent
            GameObject newItem = Instantiate(AttackPrefab) as GameObject;
            newItem.SetActive(true);
            newItem.name = " item at (" + i + "," + j + ")";
            newItem.transform.SetParent(AttackContent.transform);

            //move and size the new item
            RectTransform rectTransform = newItem.GetComponent<RectTransform>();

            float x = -AttackContent.rect.width / 2 + width * (i % columnCount);
            float y = -AttackContent.rect.height / 2 - height * (j % rowCount) + 20;
            rectTransform.offsetMin = new Vector2(x, y);

            
            x = rectTransform.offsetMin.x + width;
            y = rectTransform.offsetMin.y + height;
            rectTransform.offsetMax = new Vector2(x, y);

            newItem.GetComponent<ChooseEncounterAttackPrefabScript>().Initialize(attacks[i].AttackName, attacks[i].AttackDescription);
            m_attackObjects.Add(newItem);
        }

        AttackScrollBar.value = 1.0f;
    }

    void ClearAndPopulateAbilityList()
    {
        for (int i = 0; i < m_abilityObjects.Count; i++)
        {
            Destroy(m_abilityObjects[i]);
        }
        m_abilityObjects.Clear();

        List<EncounterAbility> abilities = m_currentlySelectedEncounter.EncounterAbilities;

        //calculate the width and height of each child item.
        int columnCount = abilities.Count;
        float width = 200;
        float height = 135;
        int rowCount = 1;

        //adjust the width of the container so that it will just barely fit all its children
        float scrollWidth = width * columnCount;
        AbilityContent.offsetMin = new Vector2(-scrollWidth / 2, AttackContent.offsetMin.y);
        AbilityContent.offsetMax = new Vector2(scrollWidth / 2, AttackContent.offsetMax.y);

        int j = 0;
        for (int i = 0; i < abilities.Count; i++)
        {
            //this is used instead of a double for loop because itemCount may not fit perfectly into the rows/columns
            if (i % columnCount == 0)
                j++;

            //create a new item, name it, and set the parent
            GameObject newItem = Instantiate(AbilityPrefab) as GameObject;
            newItem.SetActive(true);
            newItem.name = " item at (" + i + "," + j + ")";
            newItem.transform.SetParent(AbilityContent.transform);

            //move and size the new item
            RectTransform rectTransform = newItem.GetComponent<RectTransform>();

            float x = -AbilityContent.rect.width / 2 + width * (i % columnCount);
            float y = -AbilityContent.rect.height / 2 - height * (j % rowCount) + 20;
            rectTransform.offsetMin = new Vector2(x, y);


            x = rectTransform.offsetMin.x + width;
            y = rectTransform.offsetMin.y + height;
            rectTransform.offsetMax = new Vector2(x, y);

            newItem.GetComponent<ChooseEncounterAbilityPrefabScript>().Initialize(abilities[i].Name, abilities[i].Description, abilities[i].Ability);
            m_abilityObjects.Add(newItem);
        }

        AbilityScrollBar.value = 1.0f;
    }
}
