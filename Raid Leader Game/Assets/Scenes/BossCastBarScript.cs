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
    Enums.AbilityCastType m_castType;
    bool isCasting;

    float m_realWidth;
    float m_height = 20.0f;
    bool m_adjusted = false;

	// Use this for initialization
	void Start () {
        m_realWidth = Fill.rectTransform.sizeDelta.x;

    }
	
	// Update is called once per frame
	void Update () {
        if (isCasting)
        {
            string castOrChannel = "Casting ";
            switch (m_castType)
            {
                case Enums.AbilityCastType.Cast:
                    m_progress += Time.deltaTime;
                    break;
                case Enums.AbilityCastType.Channel:
                    m_progress -= Time.deltaTime;
                    castOrChannel = "Channeling ";
                    break;
                default:
                    break;
            }
            
            Fill.rectTransform.sizeDelta = new Vector2((m_progress / m_totalCastTime) * m_realWidth, m_height);
            CastBarText.text = castOrChannel + m_spellName + ": " + System.Math.Round(m_totalCastTime - m_progress, 1);

            if (m_progress >= m_totalCastTime || m_progress < 0.0f)
            {
                StopCasting();
            }
        }
    }

    public void InitiateCast(EncounterAbility ability)
    {
        //Fuck variable screen sizes!
        if (!m_adjusted)
        {
            float scale = GameObject.FindGameObjectWithTag("Canvas").transform.localScale.x;
            gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
            gameObject.transform.SetPositionAndRotation(new Vector3(50* scale, 360* scale, 0), Quaternion.identity);
        }

        m_totalCastTime = ability.CastTime;
        m_spellName = ability.Name;
        m_castType = ability.CastType;

        switch (m_castType)
        {
            case Enums.AbilityCastType.Cast:
                m_progress = 0.0f;
                break;
            case Enums.AbilityCastType.Channel:
                m_progress = m_totalCastTime;
                break;
            default:
                break;
        }
        isCasting = true;
        gameObject.SetActive(true);
    }

    public void StopCasting()
    {
        gameObject.SetActive(false);
        isCasting = false;
    }
}
