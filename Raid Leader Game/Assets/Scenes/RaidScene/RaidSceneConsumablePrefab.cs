using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaidSceneConsumablePrefab : MonoBehaviour {

    public Text text;

    ConsumableItem m_item;
    RaidSceneController m_rsc;

    public void Initialize(RaidSceneController rsc, ConsumableItem i)
    {
        m_rsc = rsc;
        m_item = i;
        text.text = m_item.Name + "\n" + m_item.GetMultiplierString();
    }

    public void OnUse()
    {
        m_rsc.UseConsumable(m_item);
    }
}
