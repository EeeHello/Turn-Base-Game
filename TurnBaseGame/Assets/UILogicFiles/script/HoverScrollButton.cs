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
    public int _scrollIndex = 1;
    public int ScrollIndex
    {
        get => _scrollIndex;
        set
        {
            _scrollIndex = value;
            if (_scrollIndex > 3) _scrollIndex = 1;
            if (_scrollIndex < 1) _scrollIndex = 3;

            OnScrollIndexChanged(_scrollIndex);
        }
    }



    private Vector2 hoverPosition;
    private Vector2 originalPosition;
    public Vector2 hoverOffset;
    public float scrollCooldown = 0.2f;
    private float lastScrollTime;
    private GameObject Player;

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
        Player = transform.parent.parent.gameObject;

        GetComponent<Image>().alphaHitTestMinimumThreshold = alphaThreshold;
        originalPosition = transform.position;
        hoverPosition = originalPosition - hoverOffset;

        fdm = FindFirstObjectByType<FightDataManager>();

        if (fdm != null)
        {
            pd = fdm.FetchMyPlayersData(Player);
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

        float rawScroll = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(rawScroll) < 0.1f) return; // Ignore tiny/no movement

        float scroll = Mathf.Sign(rawScroll);

        if (Time.time - lastScrollTime > scrollCooldown)
        {
            if (scroll > 0)
                ScrollIndex++;
            else
                ScrollIndex--;

            lastScrollTime = Time.time;
        }
    }
    private void OnScrollIndexChanged(int index)
    {
        // Example: Change visual, trigger update, etc.
        Debug.Log($"Scroll index changed to: {index}");

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
