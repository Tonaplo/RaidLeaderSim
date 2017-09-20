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
       

        int width = 300;
        int height = 150;
        for (int i = 0; i < 4; i++)
        {
            GameObject temp = GameObject.Instantiate(RecruitPrefab);
            temp.SetActive(true);
            temp.transform.SetParent(transform);
            temp.transform.SetPositionAndRotation(new Vector3(200 + (width * (i % 2)), 300 - ((i / 2)) * height, 0), Quaternion.identity);
            temp.GetComponent<RecruitScript>().Initialize(i);
            m_recruitObjects.Add(temp);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
