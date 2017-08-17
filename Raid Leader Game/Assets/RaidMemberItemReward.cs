using UnityEngine;
using UnityEngine.UI;

public class RaidMemberItemReward : MonoBehaviour {

    public Text DescriptionText;
    public Text ButtonText;

    Raider m_raider;
    CharacterItem m_currentItem;
    EncounterVictorySceneController m_evsc;

    public void Initialize(Raider r, CharacterItem i, EncounterVictorySceneController e)
    {
        m_evsc = e;
        m_raider = r;
        m_currentItem = i;
        DescriptionText.text = m_raider.GetName() + " - " + m_raider.RaiderStats.GetCurrentSpec() + " (" + Utility.GetRoleString(m_raider.RaiderStats.GetRole()) + ")";
        int difference = i.ItemLevel - m_raider.RaiderStats.Gear.GetItemLevelOfSlot(i.GearSlot);

        if (difference > 0)
        {
            ButtonText.text = "+" + difference + " increase";
            ButtonText.color = Color.green;
        }
        else if (difference < 0)
        {
            ButtonText.text = difference + " decrease";
            ButtonText.color = Color.red;
        }
        else
        {
            ButtonText.text = "No change";
        }
    }

    public void AwardItemToRaider()
    {
        m_raider.RaiderStats.Gear.AddGearPieceToSlot(m_currentItem);
        m_evsc.ItemAwarded();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
