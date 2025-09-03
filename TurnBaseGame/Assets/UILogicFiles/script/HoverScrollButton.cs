using System;
using System.Collections;
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

    public List<GameObject> spawnedActionBars = new List<GameObject>();

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

    [Header("Animation")]
    public float animationDuration = 999f;
    public float offsetDistance = 100f; // how far to the left newAction starts
    public Vector3 defaultScale = Vector3.one; // editable in inspector

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

    private int lastWeaponIndex = 0;
    private void OnScrollIndexChanged(int index)
    {
        if (lastWeaponIndex == currentWeaponIndex)
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

            if (actionList != null && actionList.Count > 0)
                AnimateToNewAction(actionList);
        }
        lastWeaponIndex = currentWeaponIndex;
    }

    public void SpawnActionBar(List<string> actionList, int index)
    {
        GameObject actionButton = Instantiate(ButtonPrefab);
        ActionBarScript actionBarScript = actionButton.GetComponent<ActionBarScript>();
        actionBarScript.actionList = actionList;
        actionBarScript.selectedActionIndex = index;
        actionBarScript.OGButton = this;
        actionButton.gameObject.transform.SetParent(this.gameObject.transform.parent);
        actionButton.transform.localScale = new Vector3(3.55071568f, 4.16068888f, 2.02025962f);
        spawnedActionBars.Add(actionButton);
    }
    public void AnimateToNewAction(List<string> actionList)
    {
        if (actionList == null || actionList.Count == 0) return;

        if (spawnedActionBars.Count == 0)
        {
            //2.make original button disappear
            this.GetComponent<Image>().enabled = false;

            //3.spawn new button
            SpawnActionBar(actionList, ScrollIndex);
        }
        else
        {
            //1.spawn new button
            SpawnActionBar(actionList, ScrollIndex);

            //2.animate curent button
            AnimateButton(spawnedActionBars);
        }
    }

    public void AnimateButton(List<GameObject> spawnedActionBars)
    {
        if (spawnedActionBars.Count < 2) return;

        GameObject oldAction = spawnedActionBars[0];
        GameObject newAction = spawnedActionBars[1];

        RectTransform oldRect = oldAction.GetComponent<RectTransform>();
        RectTransform newRect = newAction.GetComponent<RectTransform>();

        // Starting position of newAction = slightly left of oldAction
        Vector3 startPos = oldRect.position + (-oldRect.right * offsetDistance);
        Vector3 targetPos = oldRect.position;

        newRect.position = startPos;

        // Make newAction transparent
        CanvasGroup newGroup = newAction.GetComponent<CanvasGroup>();
        if (newGroup == null) newGroup = newAction.AddComponent<CanvasGroup>();
        newGroup.alpha = 0f;

        // Make sure oldAction has CanvasGroup too
        CanvasGroup oldGroup = oldAction.GetComponent<CanvasGroup>();
        if (oldGroup == null) oldGroup = oldAction.AddComponent<CanvasGroup>();
        oldGroup.alpha = 1f;

        // Start coroutine
        StartCoroutine(AnimateTransition(oldAction, oldRect, oldGroup, newRect, newGroup, targetPos));
    }

    private IEnumerator AnimateTransition(GameObject oldAction,
                                          RectTransform oldRect, CanvasGroup oldGroup,
                                          RectTransform newRect, CanvasGroup newGroup,
                                          Vector3 targetPos)
    {
        float elapsed = 0f;
        Vector3 oldStart = oldRect != null ? oldRect.position : Vector3.zero;
        Vector3 newStart = newRect != null ? newRect.position : Vector3.zero;

        while (elapsed < animationDuration)
        {
            if (oldRect == null || newRect == null) yield break;

            float t = elapsed / animationDuration;
            float smoothT = Mathf.SmoothStep(0, 1, t);

            // Move positions
            oldRect.position = Vector3.Lerp(oldStart, targetPos, smoothT);
            newRect.position = Vector3.Lerp(newStart, targetPos, smoothT);

            // Fade
            oldGroup.alpha = Mathf.Lerp(1f, 0f, smoothT);
            newGroup.alpha = Mathf.Lerp(0f, 1f, smoothT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (oldRect != null) oldRect.position = targetPos;
        if (newRect != null) newRect.position = targetPos;

        if (oldGroup != null) oldGroup.alpha = 0f;
        if (newGroup != null) newGroup.alpha = 1f;

        if (spawnedActionBars.Count > 0 && spawnedActionBars[0] == oldAction)
        {
            spawnedActionBars.RemoveAt(0);
            Destroy(oldAction);
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
        AnimateToNewAction(actionList);
    }

    public void PointerhasExited()
    {
        isHovering = false;
        this.GetComponent<Image>().enabled = true;

        StopAllCoroutines(); // prevent animations from updating destroyed objects

        foreach (var item in spawnedActionBars)
        {
            if (item != null) Destroy(item);
        }

        spawnedActionBars.Clear();
    }
}
