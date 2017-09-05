using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaidSceneCooldownPrefabScript : MonoBehaviour {

    public Text m_text;
    Image m_background;
    Button m_button;
    RaiderScript m_raiderScript;

    public void Initialize(RaiderScript r, RaidSceneController rsc)
    {
        m_raiderScript = r;

        m_background = GetComponent<Image>();
        m_button = GetComponent<Button>();

        m_background.color = Utility.GetColorFromClass(m_raiderScript.Raider.RaiderStats.GetClass());

        if (m_raiderScript.IsDead())
        {
            m_text.text = m_raiderScript.Raider.GetName() + "\n\nDied!";
            m_button.interactable = false;
        }
        else if (m_raiderScript.CooldownUsed)
        {
            m_text.text = m_raiderScript.Raider.GetName() + "\n\nUsed!";
            m_button.interactable = !m_raiderScript.CooldownUsed;
        }
        else
        {
            m_text.text = m_raiderScript.Raider.GetName() + "\n\n";
            m_text.text += "\"" + m_raiderScript.Raider.RaiderStats.Cooldown.Name + "\"";
            m_button.interactable = !m_raiderScript.CooldownUsed;
        }
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!m_raiderScript.CooldownUsed && m_raiderScript.IsDead())
        {
            m_text.text = m_raiderScript.Raider.GetName() + "\n\nDied!";
            m_button.interactable = !m_raiderScript.CooldownUsed;
        }
	}

    public void UseCooldown()
    {
        m_raiderScript.UseCooldown();
        m_text.text = m_raiderScript.Raider.GetName() + "\n\nUsed!";
        m_button.interactable = !m_raiderScript.CooldownUsed;
    }
}
