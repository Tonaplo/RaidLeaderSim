using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TooltipScript : MonoBehaviour {

    public Text TooltipText;
    public Image Arrow;
    public Image Background;
    public Button NextButton;
    Vector2 m_position;

    public enum TooltipArrowDirection
    {
        TopRight,
        BottomRight,
        BottomLeft,
        TopLeft,
    }

    public void UpdateTooltip(Vector2 tooltipPosition, TooltipArrowDirection arrow, bool includeButton, string text)
    {
        NextButton.gameObject.SetActive(includeButton);
        m_position = tooltipPosition;
        TooltipText.text = text + "\n";
        transform.SetPositionAndRotation(tooltipPosition, Quaternion.identity);
        StartCoroutine(Resize(0.00001f, arrow));
    }

    IEnumerator Resize(float wait, TooltipArrowDirection dir)
    {
        yield return new WaitForSeconds(wait);

        Vector2 arrowPosition = new Vector2(m_position.x, m_position.y);

        switch (dir)
        {
            case TooltipArrowDirection.TopRight:
                arrowPosition.x += 58;
                arrowPosition.y += 13;
                Arrow.transform.SetPositionAndRotation(arrowPosition, Quaternion.identity);
                Arrow.transform.Rotate(0, 0, 45f);
                break;
            case TooltipArrowDirection.BottomRight:
                arrowPosition.x += 78;
                arrowPosition.y -= TooltipText.GetComponent<RectTransform>().rect.height-2;
                Arrow.transform.SetPositionAndRotation(arrowPosition, Quaternion.identity);
                Arrow.transform.Rotate(0, 0, -45f);
                break;
            case TooltipArrowDirection.BottomLeft:
                arrowPosition.x -= 59;
                arrowPosition.y -= TooltipText.GetComponent<RectTransform>().rect.height-2;
                Arrow.transform.SetPositionAndRotation(arrowPosition, Quaternion.identity);
                Arrow.transform.Rotate(0, 0, -135f);
                break;
            case TooltipArrowDirection.TopLeft:
                arrowPosition.x -= 78;
                arrowPosition.y -= 2;
                Arrow.transform.SetPositionAndRotation(arrowPosition, Quaternion.identity);
                Arrow.transform.Rotate(0, 0, 135f);
                break;
            default:
                break;
        }

        Background.GetComponent<RectTransform>().sizeDelta = new Vector2(150, TooltipText.GetComponent<RectTransform>().rect.height + 30);
    }
    
	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update ()
    {

    }
}
