using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;


public class HoverScrollButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float alphaThreshold = 0.1f;
    private bool isHovering = false;
    public int scrollIndex = 1;
    private Vector2 hoverPosition;
    private Vector2 originalPosition;
    public Vector2 hoverOffset;

    [Header("Fight Data")]
    private FightDataManager fdm;
    private PlayerRuntime pr;
    private PlayerData pd;
    private List<WeaponInventoryEntry> weapons;
    public List<List<string>> actions;
    public List<string> basicActions;
    public List<string> skillActions;
    public List<string> burstActions;

    private void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = alphaThreshold;
        originalPosition = transform.position;
        hoverPosition = originalPosition - hoverOffset;

        fdm = FindFirstObjectByType<FightDataManager>();

        if (fdm != null)
        {
            pd = fdm.FetchMyPlayersData(transform.parent.parent.gameObject);
            weapons = fdm.FetchMyWeapons(pd);
            actions = fdm.FetchMyActions(weapons[0]);

            if (actions != null)
            {
                basicActions = actions[0];
                skillActions = actions[1];
                burstActions = actions[2];
            }
            else
            {
                Debug.LogError("actions == null");
            }

        }
        else
        {
            Debug.LogError("FightDataManager not found in the scene.");
        }
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


        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        transform.localScale = new Vector3(2, 13.333334f, 2);
        transform.position = hoverPosition ;
;
    }

public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        transform.localScale = new Vector3(1.5f, 10, 1.5f);
        transform.position = originalPosition;
        
    }
}
