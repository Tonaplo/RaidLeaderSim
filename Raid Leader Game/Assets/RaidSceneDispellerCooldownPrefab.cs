using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaidSceneDispellerCooldownPrefab : MonoBehaviour {

    public Image Fill;
    public Text CooldownText;
    
    RaiderScript m_raider;
    int m_width = 110;
    int m_height = 25;
    float m_progress = 0.0f;
    bool m_onCooldown = false;
    bool m_paused = false;
    float m_pauseTime;

    public bool OnCooldown {get { return m_onCooldown; } }
    public RaiderScript Dispeller { get { return m_raider; } }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (m_onCooldown && !m_paused)
        {
            m_progress += Time.deltaTime;

            if (m_progress < StaticValues.DispellCooldown)
            {
                Fill.GetComponent<RectTransform>().sizeDelta = new Vector2(m_progress / StaticValues.DispellCooldown * m_width, m_height);
                CooldownText.text = m_raider.Raider.GetName() + " - " + Mathf.RoundToInt(StaticValues.DispellCooldown - m_progress) + " sec";
            } else
            {
                m_progress = 0.0f;
                m_onCooldown = false;
                Fill.GetComponent<RectTransform>().sizeDelta = new Vector2(m_width, m_height);
                CooldownText.text = m_raider.Raider.GetName();
            }
        }
	}

    public void Initialize(RaiderScript r, int index)
    {
        float scale = GameObject.FindGameObjectWithTag("Canvas").transform.localScale.x;
        float width = 340 * scale;
        float yPos = 140 * scale;
        float height = 25 * scale;
        m_raider = r;
        transform.SetPositionAndRotation(new Vector3(width, yPos - (index * height), 0), Quaternion.identity);
        CooldownText.text = m_raider.Raider.GetName();
    }

    public void PutOnCooldown()
    {
        if (m_onCooldown)
            return;

        m_onCooldown = true;
    }

    public void Pause()
    {
        m_pauseTime = Time.time;
        m_paused = true;
    }

    public void UnPause()
    {
        m_paused = false;
        if (m_onCooldown)
        {
            m_progress += Time.time - m_pauseTime;
        }
    }
}
