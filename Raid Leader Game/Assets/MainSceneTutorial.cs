using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneTutorial : MonoBehaviour {

    public GameObject Tooltip;

	// Use this for initialization
	void Start () {
        if (PlayerData.TutorialEnabled)
        {
            string tooltipText = "This is an example tooltip. I'm just trying to figure out how this is supposed to work. I've copied it once more to make it a it longer This is an example tooltip. I'm just trying to figure out how this is supposed to work.";
            GameObject temp = Object.Instantiate(Tooltip);
            temp.transform.SetParent(transform);
            temp.GetComponent<TooltipScript>().UpdateTooltip(new Vector2(200, 200), TooltipScript.TooltipArrowDirection.BottomLeft, true, tooltipText);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
