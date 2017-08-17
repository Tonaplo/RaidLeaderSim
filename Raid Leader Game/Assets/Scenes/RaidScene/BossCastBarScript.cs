using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossCastBarScript : MonoBehaviour {

    public Text CastBarText;
    public Image Fill;

    float m_totalCastTime = 1.0f;
    float m_progress = 0.0f;
    string m_spellName;
    bool isCasting;

    float m_realWidth;
    float m_height = 20.0f;

	// Use this for initialization
	void Start () {
        m_realWidth = Fill.rectTransform.sizeDelta.x;
    }
	
	// Update is called once per frame
	void Update () {
        if (isCasting)
        {
            m_progress += Time.deltaTime;
            Fill.rectTransform.sizeDelta = new Vector2((m_progress / m_totalCastTime) * m_realWidth, m_height);
            CastBarText.text = "Casting " + m_spellName + ": " + System.Math.Round(m_totalCastTime - m_progress, 1);

            if (m_progress >= m_totalCastTime)
            {
                StopCasting();
            }
        }
    }

    public void InitiateCast(float castTime, string spellName)
    {
        m_progress = 0.0f;
        m_totalCastTime = castTime;
        m_spellName = spellName;
        isCasting = true;
        gameObject.SetActive(true);
    }

    public void StopCasting()
    {
        gameObject.SetActive(false);
        isCasting = false;
    }
}
