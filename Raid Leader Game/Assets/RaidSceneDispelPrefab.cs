using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaidSceneDispelPrefab : MonoBehaviour {

    public Text DispelText;

    RaiderScript m_raider;
    RaidSceneController m_rsc;
    RaidSceneControlPanel m_rscp;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Initialize(RaiderScript r, int index, RaidSceneController rsc, RaidSceneControlPanel rscp)
    {
        m_raider = r;
        m_rsc = rsc;
        m_rscp = rscp;
        DispelText.text = m_raider.Raider.GetName();

        transform.SetPositionAndRotation(new Vector3(75f + (100 * (index % 2)), 160 - ((index / 2) * 30), 0), Quaternion.identity);
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
