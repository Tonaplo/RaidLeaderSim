using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AwardLootItemInstanceScript : MonoBehaviour {

    public Text ButtonText;

    CharacterItem m_item;
    EncounterVictorySceneController m_evsc;

    public void Initialize(CharacterItem i, EncounterVictorySceneController e)
    {
        m_evsc = e;
        m_item = i;
        ButtonText.text = i.GearSlot + ":\nItemLevel: " + i.ItemLevel;
    }

    public void OnClick()
    {
        m_evsc.SetCurrentItem(m_item);
        GetComponent<Button>().interactable = false;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
