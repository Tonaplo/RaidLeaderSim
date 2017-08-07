using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseTeamForAttemptTeamMember : MonoBehaviour
{

    public Text NameText;
    public Text SpecRoleText;
    public Text ThroughPutText;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setup(string name, Enums.CharacterSpec spec, Enums.CharacterRole role, int throughput)
    {
        NameText.text = name;
        SpecRoleText.text = spec.ToString() + " - " + role.ToString();
        ThroughPutText.text = throughput.ToString();
    }
}
