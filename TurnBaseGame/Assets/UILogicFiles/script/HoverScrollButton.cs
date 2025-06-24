using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class HoverScrollButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float alphaThreshold = 0.1f;
    private bool isHovering = false;
    public int scrollIndex = 1;
    private string originalText;

    public TMPro.TextMeshProUGUI buttonText;

    private void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = alphaThreshold;
        buttonText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        originalText = buttonText.text;
    }

    private void Update()
    {
        if (!isHovering) return;

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scroll) > 0.1f)
        {
            if (scroll > 0)
                scrollIndex++;
            else
                scrollIndex--;

            // Loop scrollIndex between 1 and 3
            if (scrollIndex > 3) scrollIndex = 1;
            if (scrollIndex < 1) scrollIndex = 3;

            buttonText.text = "\n" + scrollIndex.ToString();

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        buttonText.text = "\n" + scrollIndex.ToString();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        buttonText.text = originalText;
    }
}
