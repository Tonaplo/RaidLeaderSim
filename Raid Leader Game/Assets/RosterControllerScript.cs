using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RosterControllerScript : MonoBehaviour {

    public GameObject RaiderButtonPrefab;
    public Text HeaderText;
    public Text BodyText;
    public Image TextBackGround;

    List<GameObject> buttons;

    // Use this for initialization
    void Start () {
        buttons = new List<GameObject>();
        int height = 40;
        int width = 150;
        int xPosStart = 75;
        int yPosStart = 350;
        for (int i = 0; i < PlayerData.GetRoster().Count; i++)
        {
            GameObject temp = GameObject.Instantiate(RaiderButtonPrefab);
            temp.SetActive(true);
            temp.transform.SetParent(transform);
            temp.transform.SetPositionAndRotation(new Vector3(xPosStart + ((i / 8) * (width) + 10), yPosStart - (((height + 5) * (i % 8))), 0), Quaternion.identity);
            temp.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            temp.GetComponent<RosterButtonScript>().SetupButton(PlayerData.GetRoster()[i], ref HeaderText, ref BodyText);
            buttons.Add(temp);
        }
        buttons[0].GetComponent<RosterButtonScript>().OnClick();
        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    public void SetActive(bool on)
    {
        gameObject.SetActive(on);
    }
}
