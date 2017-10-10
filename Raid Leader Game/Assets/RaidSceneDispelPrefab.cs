using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaidSceneDispelPrefab : MonoBehaviour {

    public Text DispelText;

    RaiderScript m_raider;
    RaidSceneControlPanel m_rscp;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Initialize(RaiderScript r, int index, RaidSceneControlPanel rscp)
    {
        float scale = GameObject.FindGameObjectWithTag("Canvas").transform.localScale.x;
        m_raider = r;
        m_rscp = rscp;
        DispelText.text = m_raider.Raider.GetName();

        float xPos = 75 * scale;
        float width = 100 * scale;
        float yPos = 160 * scale;
        float height = 30 * scale;
        transform.SetPositionAndRotation(new Vector3(xPos + (width * (index % 2)), yPos - ((index / 2) * height), 0), Quaternion.identity);
    }

    public void AttemptToDispel()
    {
        RaiderScript dispeller = m_rscp.GetDispeller();
        if (dispeller == null)
            return;

        int roll = Random.Range(0, StaticValues.MaxSkill);

        if (roll <= dispeller.Raider.RaiderStats.Skills.GetSkillLevel(Enums.SkillTypes.Mechanical))
        {
            m_raider.RemoveDebuff();
            m_rscp.ReDrawDispelUI(false);
            m_rscp.ReDrawDispelUI(true);
        }
    }
}
