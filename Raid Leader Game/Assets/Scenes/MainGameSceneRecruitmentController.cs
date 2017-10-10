using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameSceneRecruitmentController : MonoBehaviour {

    public GameObject RecruitPrefab;


    List<GameObject> m_recruitObjects = new List<GameObject>();
    // Use this for initialization
    void Start () {
        GenerateRecruits();
    }

    void GenerateRecruits()
    {
        float scale = GameObject.FindGameObjectWithTag("Canvas").transform.localScale.x;
        float width = 300 * scale;
        float height = 150 * scale;
        float xPos = 200 * scale;
        float yPos = 300 * scale;
        for (int i = 0; i < 4; i++)
        {
            GameObject temp = GameObject.Instantiate(RecruitPrefab);
            temp.SetActive(true);
            temp.transform.SetParent(transform, false);
            temp.transform.SetPositionAndRotation(new Vector3(xPos + (width * (i % 2)), yPos - ((i / 2)) * height, 0), Quaternion.identity);
            temp.GetComponent<RecruitScript>().Initialize(i);
            m_recruitObjects.Add(temp);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
