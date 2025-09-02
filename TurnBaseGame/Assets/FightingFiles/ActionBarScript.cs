using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionBarScript : MonoBehaviour, IPointerExitHandler
{
    private float alphaThreshold = 0.1f;

    public List<string> actionList;
    public int selectedActionIndex = 0;
    public TMP_FontAsset font;
    private string actionName;
    public HoverScrollButton OGButton;

    public Vector3 spawnPoint;
    private TextMeshProUGUI actionText; // Text element for displaying action name

    public int fontSizeMin = 1;
    public int fontSizeMax = 36;
    void Start()
    {
        this.transform.position = spawnPoint;

        // Match the color and threshold from the original button
        GetComponent<Image>().color = OGButton.GetComponent<Image>().color;
        GetComponent<Image>().alphaHitTestMinimumThreshold = alphaThreshold;

        // Fix indexing (avoid out of range)
        if (actionList != null && selectedActionIndex > 0 && selectedActionIndex <= actionList.Count)
            actionName = actionList[selectedActionIndex - 1];
        else
            actionName = "Unknown";

        // Create a TextMeshProUGUI object if not already present
        actionText = GetComponentInChildren<TextMeshProUGUI>();
        if (actionText == null)
        {
            GameObject textObj = new GameObject("ActionNameText");
            textObj.transform.SetParent(transform, false);
            actionText = textObj.AddComponent<TextMeshProUGUI>();

            // Stretch text to fit ActionBar image
            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.3f,0.5f);
            rect.anchorMax = new Vector2(0.9f, 0.6f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.rotation = new Quaternion(0,0,25,1);
        }

        // Apply font and text
        actionText.font = font;
        actionText.text = actionName;
        actionText.alignment = TextAlignmentOptions.Center;
        actionText.enableAutoSizing = true;   // scale font to fit
        actionText.fontSizeMin = fontSizeMin;          // smallest size allowed
        actionText.fontSizeMax = fontSizeMax;         // largest size allowed
        actionText.color = Color.black;       // adjust for contrast
    }

    void Update()
    {
        actionText.fontSizeMin = fontSizeMin;          // smallest size allowed
        actionText.fontSizeMax = fontSizeMax;         // largest size allowed
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OGButton.PointerhasExited();
    }
}
