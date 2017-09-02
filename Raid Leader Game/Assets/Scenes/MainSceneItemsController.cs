using System.Collections;
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
        int width = 150;
        int xPosStart = 75;
        for (int i = 0; i < (int)Enums.ConsumableType.NumTypes; i++)
        {
            GameObject temp = GameObject.Instantiate(CategoryPrefab);
            temp.SetActive(true);
            temp.transform.SetParent(transform);
            temp.transform.SetPositionAndRotation(new Vector3(xPosStart + (i * (width + 10)), 190, 0), Quaternion.identity);
            temp.GetComponent<MainSceneItemCategoryController>().Initialize((Enums.ConsumableType)i, this);
            buttons.Add(temp);
        }
    }
}
