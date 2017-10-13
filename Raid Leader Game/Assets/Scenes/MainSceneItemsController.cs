using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneItemsController : MonoBehaviour {

    public GameObject CategoryPrefab;
    public Button PurchaseAttemptsButton;
    public Text AttemptsLeftText;

    List<GameObject> buttons;

    // Use this for initialization
    void Start () {
        CategoryPrefab.SetActive(false);
    }

    public void Reactivate()
    {
        PurchaseAttemptsButton.gameObject.SetActive(true);
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

        PurchaseAttemptsButton.interactable = (PlayerData.RaidTeamGold >= StaticValues.GoldCostOfAttempts);

        SetupCategories();
    }

    void SetupCategories()
    {
        float scale = GameObject.FindGameObjectWithTag("Canvas").transform.localScale.x;
        float width = 200 * scale;
        float height = 210 * scale;
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

    public void PurchaseAttempts()
    {
        PlayerData.PurchaseAttempts();
        if (PlayerData.AttemptsLeft > 0)
        {
            AttemptsLeftText.color = Color.green;
        }
        else
        {
            AttemptsLeftText.color = Color.red;
        }
        AttemptsLeftText.text = PlayerData.AttemptsLeft.ToString();

        Reactivate();
    }
}
