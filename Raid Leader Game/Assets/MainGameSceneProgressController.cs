using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameSceneProgressController : MonoBehaviour {

    public Dropdown RaidSelector;
    public Image EasyBackground;
    public Text EasyText;
    public Image NormalBackground;
    public Text NormalText;
    public Image HardBackground;
    public Text HardText;
    public GameObject BossPrefab;

    List<GameObject> m_bossObjects = new List<GameObject>();

    private void Start()
    {
        BossPrefab.gameObject.SetActive(false);
    }

    public void Reactivate()
    {
        gameObject.SetActive(true);
        if (m_bossObjects != null)
        {
            for (int i = 0; i < m_bossObjects.Count; i++)
            {
                Destroy(m_bossObjects[i]);
            }
            m_bossObjects.Clear();
        }
        else
        {
            m_bossObjects = new List<GameObject>();
        }

        EasyBackground.gameObject.SetActive(true);
        NormalBackground.gameObject.SetActive(true);
        HardBackground.gameObject.SetActive(true);

        RaidSelector.gameObject.SetActive(true);
        RaidSelector.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        for (int i = 0; i < PlayerData.Progress.Count; i++)
        {
            options.Add(new Dropdown.OptionData(PlayerData.Progress[i].m_name));
        }

        RaidSelector.AddOptions(options);
        SetupProgress();
    }

    public void RaidChanged()
    {
        if (m_bossObjects != null)
        {
            for (int i = 0; i < m_bossObjects.Count; i++)
            {
                Destroy(m_bossObjects[i]);
            }
            m_bossObjects.Clear();
        }
        else
        {
            m_bossObjects = new List<GameObject>();
        }

        SetupProgress();
    }

    void SetupProgress()
    {
        RaidData raidData = PlayerData.Progress[RaidSelector.value];
        int easyProgress = 0;
        int normalProgress = 0;
        int hardProgress = 0;
        for (int i = 0; i < raidData.m_encounters.Count; i++)
        {
            GameObject temp = GameObject.Instantiate(BossPrefab);
            temp.SetActive(true);
            temp.transform.SetParent(transform, false);
            temp.GetComponent<ProgressBossPrefab>().Initialize(raidData.m_encounters[i], i);
            m_bossObjects.Add(temp);

            if (raidData.m_encounters[i].BeatenOnEasy)
                easyProgress++;

            if (raidData.m_encounters[i].BeatenOnNormal)
                normalProgress++;

            if (raidData.m_encounters[i].BeatenOnHard)
                hardProgress++;
        }

        EasyText.text = "Easy: " + easyProgress + "/" + raidData.m_encounters.Count;
        NormalText.text = "Normal: " + normalProgress + "/" + raidData.m_encounters.Count;
        HardText.text = "Hard: " + hardProgress + "/" + raidData.m_encounters.Count;
    }
}
