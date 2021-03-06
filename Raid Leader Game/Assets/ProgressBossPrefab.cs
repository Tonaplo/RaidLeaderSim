﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBossPrefab : MonoBehaviour {

    public Image EasyImage;
    public Image NormalImage;
    public Image HardImage;
    public Text BossName;
    

    public void Initialize(RaidData.EncounterData e, int index)
    {
        float scale = GameObject.FindGameObjectWithTag("Canvas").transform.localScale.x;

        float width = 223 * scale;
        float height = 110 * scale;
        float xPos = 10 * scale;
        float yPos = 335 * scale;
        transform.SetPositionAndRotation(new Vector3(xPos + ((index % 3) * (width)), yPos - ((height * (index / 3))), 0), Quaternion.identity);

        BossName.text = e.Name;

        if (e.BeatenOnEasy)
            EasyImage.color = Color.white;

        if (e.BeatenOnNormal)
            NormalImage.color = Color.white;

        if (e.BeatenOnHard)
            HardImage.color = Color.white;

        foreach (var raid in PlayerData.WeeklyLockOut)
        {
            foreach (var encounter in raid.m_encounters)
            {
                if (encounter.Encounter == e.Encounter)
                {
                    if (encounter.BeatenOnEasy)
                        EasyImage.color = Color.red;

                    if (encounter.BeatenOnNormal)
                        NormalImage.color = Color.red;

                    if (encounter.BeatenOnHard)
                        HardImage.color = Color.red;
                }
            }

        }
    }
}
