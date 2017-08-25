using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaidSceneTauntPrefabScript : MonoBehaviour {

    public Text m_text;
    Image m_background;
    Button m_button;
    RaiderScript m_raiderScript;
    RaidSceneControlPanel m_rscp;
    RaidSceneController m_rsc;

    public void Initialize(RaiderScript r, RaidSceneController rsc, RaidSceneControlPanel rscp)
    {
        m_rscp = rscp;
        m_rsc = rsc;
        m_raiderScript = r;

        m_background = GetComponent<Image>();
        m_button = GetComponent<Button>();

        m_background.color = Utility.GetColorFromClass(m_raiderScript.Raider.RaiderStats.GetClass());
        m_text.text = "Have\n" + m_raiderScript.Raider.GetName() + "\nTaunt";

    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Taunt()
    {
        m_rsc.RaiderTaunt(m_raiderScript);
        m_rscp.OnClickTaunts();
        m_rscp.OnClickTaunts();
    }
}
