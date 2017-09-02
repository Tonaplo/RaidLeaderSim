using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneItemCategoryController : MonoBehaviour {

    public Text Header;
    public Text NormalText;
    public Text RareText;
    public Text EpicText;
    public Button NormalButton;
    public Button RareButton;
    public Button EpicButton;

    Enums.ConsumableType m_type;
    MainSceneItemsController m_msic;

    public void Initialize(Enums.ConsumableType type, MainSceneItemsController msic)
    {
        m_msic = msic;
        m_type = type;
        Header.text = m_type.ToString();
        ReInitializeButtons();
    }
    
    public void PurchaseNormal() {
        ConsumableItem item = new ConsumableItem();
        item.Initialize(m_type, Enums.ConsumableRarity.Normal);
        PlayerData.PurchaseConsumable(item);
        m_msic.Reactivate();
    }

    public void PurchaseRare() {
        ConsumableItem item = new ConsumableItem();
        item.Initialize(m_type, Enums.ConsumableRarity.Rare);
        PlayerData.PurchaseConsumable(item);
        m_msic.Reactivate();
    }

    public void PurchaseEpic() {
        ConsumableItem item = new ConsumableItem();
        item.Initialize(m_type, Enums.ConsumableRarity.Epic);
        PlayerData.PurchaseConsumable(item);
        m_msic.Reactivate();
    }

    void SetupButton(ConsumableItem item, Text text, Button button)
    {
        List<ConsumableItem> existing = PlayerData.Consumables.FindAll(x => x.ConsumableType == item.ConsumableType && x.Rarity == item.Rarity);
        text.text = item.Name + "\n\n" + item.GetMultiplierString() + "\n\nCost: " + item.Cost + "\n(" + existing.Count + " owned)";

        if (PlayerData.RaidTeamGold < item.Cost)
        {
            text.color = Color.red;
            button.interactable = false;
        }
        else
        {
            text.color = Color.black;
            button.interactable = true;
        }
    }

    void ReInitializeButtons()
    {
        ConsumableItem item = new ConsumableItem();

        item.Initialize(m_type, Enums.ConsumableRarity.Normal);
        SetupButton(item, NormalText, NormalButton);

        item.Initialize(m_type, Enums.ConsumableRarity.Rare);
        SetupButton(item, RareText, RareButton);

        item.Initialize(m_type, Enums.ConsumableRarity.Epic);
        SetupButton(item, EpicText, EpicButton);

    }

}
