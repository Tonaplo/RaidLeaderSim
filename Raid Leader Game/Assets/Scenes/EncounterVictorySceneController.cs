using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EncounterVictorySceneController : MonoBehaviour {

    public GameObject m_RaidMemberPrefab;
    public GameObject m_RaiderMemberBackground;
    public GameObject m_LootInstancePrefab;
    public GameObject m_LootBackground;
    public Text m_SellButtonText;

    List<CharacterItem> m_loot;
    CharacterItem m_currentItem;
    List<GameObject> m_raiderButtons = new List<GameObject>();
    List<GameObject> m_lootButtons = new List<GameObject>();


    public void ItemAwarded()
    {
        m_loot.Remove(m_currentItem);
        if (m_loot.Count > 0)
        {
            ReCreateLoot();
            ReCreateRaid();
        }
        else {
            SceneManager.LoadScene("MainGameScene");
        }
    }

    public void SetCurrentItem(CharacterItem item)
    {
        m_currentItem = item;
        ReCreateRaid();
        for (int i = 0; i < m_lootButtons.Count; i++)
        {
            m_lootButtons[i].GetComponent<UnityEngine.UI.Button>().interactable = true;
        }
        m_SellButtonText.text = "Sell " + m_currentItem.GearSlot + " for " + StaticValues.LootSellValue + " gold.";
    }

    public void SellCurrentItem()
    {
        PlayerData.AwardGold(StaticValues.LootSellValue);
        ItemAwarded();
    }

    // Use this for initialization
    void Start () {
        //Utility.DebugInitalize();
        m_loot = Utility.CurrentEncounter.Loot;
        ReCreateLoot();
        ReCreateRaid();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void ReCreateRaid()
    {
        for (int i = 0; i < m_raiderButtons.Count; i++)
        {
            Destroy(m_raiderButtons[i]);
        }

        m_raiderButtons.Clear();
        
        PlayerData.SortRaidForLoot(m_currentItem.GearSlot);

        float scale = GameObject.FindGameObjectWithTag("Canvas").transform.localScale.x;

        float xPos = 300 * scale;
        float yPos = 380 * scale;
        float height = 31.5f * scale;

        for (int i = 0; i < PlayerData.RaidTeam.Count; i++)
        {
            GameObject temp = Instantiate(m_RaidMemberPrefab);
            temp.SetActive(true);
            temp.transform.SetParent(m_RaiderMemberBackground.gameObject.transform, false);
            temp.transform.SetPositionAndRotation(new Vector3(xPos, yPos - (height * i), 0), Quaternion.identity);
            temp.GetComponent<RaidMemberItemReward>().Initialize(PlayerData.RaidTeam[i], m_currentItem, this);
            m_raiderButtons.Add(temp);
        }
    }

    void ReCreateLoot()
    {
        for (int i = 0; i < m_lootButtons.Count; i++)
        {
            Destroy(m_lootButtons[i]);
        }

        m_lootButtons.Clear();
        float scale = GameObject.FindGameObjectWithTag("Canvas").transform.localScale.x;

        float xPos = 500 * scale;
        float yPos = 375 * scale;
        float height = 41.5f * scale;
        float width = 95 * scale;

        for (int i = 0; i < m_loot.Count; i++)
        {
            GameObject temp = Instantiate(m_LootInstancePrefab);
            temp.SetActive(true);
            temp.transform.SetParent(m_LootBackground.gameObject.transform, false);
            temp.transform.SetPositionAndRotation(new Vector3(xPos + ((i % 3) * width), yPos - (height * (i/3)), 0), Quaternion.identity);
            temp.GetComponent<AwardLootItemInstanceScript>().Initialize(m_loot[i], this);
            m_lootButtons.Add(temp);
        }
        m_lootButtons[0].GetComponent<AwardLootItemInstanceScript>().OnClick();
    }
}
