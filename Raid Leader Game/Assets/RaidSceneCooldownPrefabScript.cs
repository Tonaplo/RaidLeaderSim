using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaidSceneCooldownPrefabScript : MonoBehaviour {

    public Text m_text;
    Image m_background;
    Button m_button;
    RaiderScript m_raiderScript;
    RaidSceneController m_rsc;

    public void Initialize(RaiderScript r, RaidSceneController rsc)
    {
        m_rsc = rsc;
        m_raiderScript = r;

        m_background = GetComponent<Image>();
        m_button = GetComponent<Button>();

        m_background.color = Utility.GetColorFromClass(m_raiderScript.Raider.RaiderStats.GetClass());
        m_text.text = m_raiderScript.Raider.GetName() + "\n\n";
        m_text.text += "\"" + m_raiderScript.Raider.RaiderStats.Cooldown.Name + "\"";
        m_button.interactable = !m_raiderScript.CooldownUsed;

    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UseCooldown()
    {
        m_raiderScript.UseCooldown();
        m_button.interactable = !m_raiderScript.CooldownUsed;
    }
}
