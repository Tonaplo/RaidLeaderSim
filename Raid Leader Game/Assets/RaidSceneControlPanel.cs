using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaidSceneControlPanel : MonoBehaviour {

    public Image Background;
    public GameObject EventLogBackScrollView;
    public RaidSceneController RSC;
    
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void Initialize()
    {
        gameObject.SetActive(true);

        HandleEventLogUI(true);
    }

    void HandleEventLogUI(bool shouldShow)
    {
        EventLogBackScrollView.gameObject.SetActive(shouldShow);
    }
    
}
