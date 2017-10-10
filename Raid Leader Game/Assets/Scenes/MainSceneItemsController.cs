﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneItemsController : MonoBehaviour {

    public GameObject CategoryPrefab;

    List<GameObject> buttons;

    // Use this for initialization
    void Start () {
        CategoryPrefab.SetActive(false);
    }

    public void Reactivate()
    {
        gameObject.SetActive(true);
        if (buttons != null)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                Destroy(buttons[i]);
            }
            buttons.Clear();
        }
        else
        {
            buttons = new List<GameObject>();
        }

        SetupCategories();
    }

    void SetupCategories()
    {
        float scale = GameObject.FindGameObjectWithTag("Canvas").transform.localScale.x;
        float width = 200 * scale;
        float height = 190 * scale;
        float xPosStart = 125 * scale;

        for (int i = 0; i < (int)Enums.ConsumableType.NumTypes; i++)
        {
            GameObject temp = GameObject.Instantiate(CategoryPrefab);
            temp.SetActive(true);
            temp.transform.SetParent(transform, false);
            temp.transform.SetPositionAndRotation(new Vector3(xPosStart + (i * width), height, 0), Quaternion.identity);
            temp.GetComponent<MainSceneItemCategoryController>().Initialize((Enums.ConsumableType)i, this);
            buttons.Add(temp);
        }
    }
}
