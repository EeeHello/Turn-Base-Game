using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class HoverScrollButton : MonoBehaviour, IPointerEnterHandler
{
    public float alphaThreshold = 0.1f;
    private bool isHovering = false;
    public int _scrollIndex = 1;
    public ActionType currentActionType;
    public enum ActionType
    {
        basic, skill, burst
    }
    public int ScrollIndex
    {
        get => _scrollIndex;
        set
        {
            int maxIndex;

            switch (currentActionType)
            {
                case ActionType.basic:
                    maxIndex = basicActions != null ? basicActions.Count : 3;

                    break;
                case ActionType.skill:
                    maxIndex = skillActions != null ? skillActions.Count : 3;

                    break;
                case ActionType.burst:
                    maxIndex = burstActions != null ? burstActions.Count : 3;

                    break;
                default:
                    maxIndex = 0;
                    Debug.LogError("currentActionType isn't set");
                    break;
            }

            _scrollIndex = value;
            if (_scrollIndex > maxIndex) _scrollIndex = 1;
            if (_scrollIndex < 1) _scrollIndex = maxIndex;
            OnScrollIndexChanged(_scrollIndex);
        }
    }

    private Vector2 hoverPosition;
    private Vector2 originalPosition;
    public Vector2 hoverOffset;
    public float scrollCooldown = 0.2f;
    private float lastScrollTime;

    [Header("GameObjects")]
    private GameObject Player;
    public GameObject ButtonPrefab;
    public GameObject SubFightUI;

    [Header("Fight Data")]
    private FightDataManager fdm;
    private PlayerRuntime pr;
    private PlayerData pd;
    private List<WeaponInventoryEntry> weapons;
    public List<List<string>> actions;
    public List<string> basicActions;
    public List<string> skillActions;
    public List<string> burstActions;

    private int currentWeaponIndex = 0;

    private void Start()
    {
        Player = transform.parent.parent.gameObject;

        GetComponent<Image>().alphaHitTestMinimumThreshold = alphaThreshold;
        

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
        // Weapon switching with Q/E
        if (Keyboard.current != null)
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                SwitchWeapon(-1);
            }
            else if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                SwitchWeapon(1);
            }
        }

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

    private void SwitchWeapon(int direction)
    {
        if (weapons == null || weapons.Count == 0) return;
        currentWeaponIndex += direction;
        if (currentWeaponIndex < 0) currentWeaponIndex = weapons.Count - 1;
        if (currentWeaponIndex >= weapons.Count) currentWeaponIndex = 0;
        // Fetch actions for the new weapon
        actions = fdm.FetchMyActions(weapons[currentWeaponIndex]);
        if (actions != null)
        {
            basicActions = actions[0];
            skillActions = actions[1];
            burstActions = actions[2];
        }
        else
        {
            Debug.LogError("actions == null after weapon switch");
        }
        ScrollIndex = 1; // Reset scroll index
    }

    private void OnScrollIndexChanged(int index)
    {
        // Find the correct action list based on currentActionType
        List<string> actionList = null;
        switch (currentActionType)
        {
            case ActionType.basic:
                actionList = basicActions;
                break;
            case ActionType.skill:
                actionList = skillActions;
                break;
            case ActionType.burst:
                actionList = burstActions;
                break;
        }

        
        /* text gen, not efficient but "works"
        // Always create a new TMP_Text component to display the action name
        TMP_Text actionLabel = null;
        if (actionLabel == null)
        {
            GameObject labelObj = new GameObject("ActionLabel");
            labelObj.transform.SetParent(transform, false);
            actionLabel = labelObj.AddComponent<TextMeshProUGUI>();
        }

        // Set font if provided
        if (font != null)
        {
            actionLabel.font = font;
        }

        // Update the label text
        actionLabel.text = actionName;
        actionLabel.fontSize = 10;
        actionLabel.alignment = TextAlignmentOptions.Center;
        actionLabel.color = Color.black;*/
    }
    public List<GameObject> spawnedActionBars = new List<GameObject>();
    public void spawnActionBars(List<string> actionList)
    {
        for (int i = 0; i < actionList.Count; i++)
        {
            GameObject actionButton = Instantiate(ButtonPrefab);
            ActionBarScript actionBarScript = actionButton.GetComponent<ActionBarScript>();
            actionBarScript.actionList = actionList;
            actionBarScript.selectedActionIndex = i + 1;
            actionBarScript.OGButton = this;
            actionButton.gameObject.transform.SetParent(this.gameObject.transform.parent);
            actionButton.transform.localScale = new Vector3(3.55071568f, 4.16068888f, 2.02025962f);
            spawnedActionBars.Add(actionButton);
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        List<string> actionList = null;
        switch (currentActionType)
        {
            case ActionType.basic:
                actionList = basicActions;
                break;
            case ActionType.skill:
                actionList = skillActions;
                break;
            case ActionType.burst:
                actionList = burstActions;
                break;
        }
        spawnActionBars(actionList);

    }

    public void PointerhasExited()
    {
        isHovering = false;
        foreach (var item in spawnedActionBars)
        {
            Destroy(item);
        }

        spawnedActionBars.Clear();

    }
}
