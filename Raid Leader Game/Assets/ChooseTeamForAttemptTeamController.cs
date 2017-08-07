using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseTeamForAttemptTeamController : MonoBehaviour {

    public GameObject teamMemberPrefab;

    int m_numMembers = 0;

	// Use this for initialization
	void Start () {
        teamMemberPrefab.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddMember(string name, Enums.CharacterSpec spec, Enums.CharacterRole role, int throughput)
    {
        GameObject temp = GameObject.Instantiate(teamMemberPrefab);
        temp.SetActive(true);
        temp.transform.SetParent(this.transform);
        Vector3 position = teamMemberPrefab.transform.position + new Vector3(0, -(30 * m_numMembers), 0);
        temp.transform.SetPositionAndRotation(position, Quaternion.identity);
        temp.GetComponent<ChooseTeamForAttemptTeamMember>().Setup(name, spec, role, throughput);
        m_numMembers++;
    }
}
