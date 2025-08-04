using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class TabMenuScript : MonoBehaviour
{
    public GameObject WeaponsPanel;
    public GameObject WeaponsCustomPanel;
    public GameObject TabPanel;
    public InputActionReference toggleTabAction;

    private bool tabMenuActive;

    private Dictionary<string, GameObject> panels;

    void Awake()
    {
        tabMenuActive = TabPanel.active;

        panels = new Dictionary<string, GameObject>
        {
            { "tab", TabPanel },
            { "weapons", WeaponsPanel },
            { "weaponsCustom", WeaponsCustomPanel },

        };
    }
    void OnEnable()
    {
        toggleTabAction.action.performed += OnToggleTab;
        toggleTabAction.action.Enable();
    }

    void OnDisable()
    {
        toggleTabAction.action.performed -= OnToggleTab;
        toggleTabAction.action.Disable();
    }

    private void OnToggleTab(InputAction.CallbackContext context)
    {
        ToggleTabMenu();
    } 
    public void SwitchPanel(string panelName)
    {
        foreach (var kvp in panels)
        {
            kvp.Value.SetActive(kvp.Key == panelName);
        }
    }

    public void TurnOffAllPanels()
    {
        foreach (var kvp in panels)
        {
            kvp.Value.SetActive(false);
        }
        tabMenuActive = false;
    }
    public void OpenTabPanel()
    {
        SwitchPanel("tab");
        tabMenuActive = true;
        WeaponsPanel.GetComponent<WeaponsPanelLogic>().ArrangeWeaponSlots();
        WeaponsPanel.GetComponent<WeaponsPanelLogic>().PopulateWeaponCircle();
    }

    public void ToggleTabMenu()
    {
        if (tabMenuActive) { TurnOffAllPanels(); } else OpenTabPanel();
    }
}
