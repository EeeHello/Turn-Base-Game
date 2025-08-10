using BehaviorTree;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class LoadoutPanelManager : MonoBehaviour
{
    [SerializeField] private Transform weaponPanelCustomizationParent;
    [SerializeField] private GameObject weaponPanelPrefab;
    [SerializeField] private PlayerRuntime playerData;

    private List<WeaponInventoryEntry> equippedWeapons;

    private int maxBasics = 5;
    private int maxSkills = 5;
    private int currentBasics = 0;
    private int currentSkills = 0;

    void Start()
    {
        // Get equipped weapons from loaded player data

        if (playerData == null)
        {
            Debug.LogError("No player data loaded! Cannot populate loadout UI.");
            return;
        }

        equippedWeapons = playerData.data.Weapons.FindAll(w => w.isEquipped);

        PopulateUI();
    }
    private void SetupBasicsDropdown(TMP_Dropdown basicsDropdown, WeaponInventoryEntry weapon)
    {
        basicsDropdown.ClearOptions();
        basicsDropdown.AddOptions(new List<string>(weapon.unlockedBasicActionIDs));

        // Store previous bitmask selection per dropdown
        int prevBitmask = 0;
        // Initialize prevBitmask from current weapon.selectedBasicActionIDs
        for (int i = 0; i < weapon.unlockedBasicActionIDs.Count; i++)
        {
            if (weapon.selectedBasicActionIDs.Contains(weapon.unlockedBasicActionIDs[i]))
                prevBitmask |= (1 << i);
        }

        basicsDropdown.value = prevBitmask;

        basicsDropdown.onValueChanged.AddListener(newBitmask =>
        {
            // Decode new selection from bitmask
            List<string> newSelectedBasics = new List<string>();
            for (int i = 0; i < weapon.unlockedBasicActionIDs.Count; i++)
            {
                if ((newBitmask & (1 << i)) != 0)
                    newSelectedBasics.Add(weapon.unlockedBasicActionIDs[i]);
            }

            // Calculate total basics selected across all weapons *if* this change applied
            int totalBasicsIfChanged = 0;
            foreach (var w in equippedWeapons)
            {
                if (w == weapon)
                    totalBasicsIfChanged += newSelectedBasics.Count;
                else
                    totalBasicsIfChanged += w.selectedBasicActionIDs.Count;
            }

            if (totalBasicsIfChanged > maxBasics)
            {
                // Revert dropdown selection to previous bitmask because max exceeded
                basicsDropdown.SetValueWithoutNotify(prevBitmask);
                basicsDropdown.captionText.text = "Max basics reached!";
                return;
            }

            // Accept new selection
            weapon.selectedBasicActionIDs = newSelectedBasics;
            prevBitmask = newBitmask;
            basicsDropdown.captionText.text = string.Join(", ", weapon.selectedBasicActionIDs);
        });
    }
    private void SetupSkillsDropdown(TMP_Dropdown skillsDropdown, WeaponInventoryEntry weapon)
    {
        skillsDropdown.ClearOptions();
        skillsDropdown.AddOptions(new List<string>(weapon.unlockedSkillActionIDs));

        int prevBitmask = 0;
        for (int i = 0; i < weapon.unlockedSkillActionIDs.Count; i++)
        {
            if (weapon.selectedSkillActionIDs.Contains(weapon.unlockedSkillActionIDs[i]))
                prevBitmask |= (1 << i);
        }

        skillsDropdown.value = prevBitmask;

        skillsDropdown.onValueChanged.AddListener(newBitmask =>
        {
            List<string> newSelectedSkills = new List<string>();
            for (int i = 0; i < weapon.unlockedSkillActionIDs.Count; i++)
            {
                if ((newBitmask & (1 << i)) != 0)
                    newSelectedSkills.Add(weapon.unlockedSkillActionIDs[i]);
            }

            int totalSkillsIfChanged = 0;
            foreach (var w in equippedWeapons)
            {
                if (w == weapon)
                    totalSkillsIfChanged += newSelectedSkills.Count;
                else
                    totalSkillsIfChanged += w.selectedSkillActionIDs.Count;
            }

            if (totalSkillsIfChanged > maxSkills)
            {
                skillsDropdown.SetValueWithoutNotify(prevBitmask);
                skillsDropdown.captionText.text = "Max skills reached!";
                return;
            }

            weapon.selectedSkillActionIDs = newSelectedSkills;
            prevBitmask = newBitmask;
            skillsDropdown.captionText.text = string.Join(", ", weapon.selectedSkillActionIDs);
        });
    }

    private void PopulateUI()
    {
        foreach (var weapon in equippedWeapons)
        {
            GameObject panel = Instantiate(weaponPanelPrefab, weaponPanelCustomizationParent);
            panel.GetComponentInChildren<TMP_Text>().text = weapon.weaponID;

            // --- Basics Dropdown ---
            TMP_Dropdown basicsDropdown = panel.transform.Find("Content/BasicsSection/BasicsDropdown").GetComponent<TMP_Dropdown>();
            SetupBasicsDropdown(basicsDropdown, weapon);

            // --- Skills Dropdown ---
            TMP_Dropdown skillsDropdown = panel.transform.Find("Content/SkillsSection/SkillsDropdown").GetComponent<TMP_Dropdown>();
            SetupSkillsDropdown(skillsDropdown, weapon);


            // --- Burst Dropdown (single select) ---
            TMP_Dropdown burstDropdown = panel.transform.Find("Content/BurstSection/BurstDropdown").GetComponent<TMP_Dropdown>();
            burstDropdown.ClearOptions();

            List<string> burstIDs = new List<string>();
            if (!string.IsNullOrEmpty(weapon.unlockedBurstActionID))
                burstIDs.Add(weapon.unlockedBurstActionID);

            // If second burst is unlocked, add it
            // (assuming it's in selectedBurstActionID when unlocked)
            if (!string.IsNullOrEmpty(weapon.selectedBurstActionID) && weapon.selectedBurstActionID != weapon.unlockedBurstActionID)
                burstIDs.Add(weapon.selectedBurstActionID);

            burstDropdown.AddOptions(burstIDs);
            burstDropdown.value = burstIDs.IndexOf(weapon.selectedBurstActionID);
            burstDropdown.onValueChanged.AddListener(index =>
            {
                weapon.selectedBurstActionID = burstIDs[index];
            });
        }
    }

    private Toggle CreateToggle(string label, Transform parent)
    {
        GameObject toggleObj = new GameObject(label, typeof(Toggle), typeof(LayoutElement));
        TextMeshProUGUI text = new GameObject("Label", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
        text.text = label;
        text.transform.SetParent(toggleObj.transform, false);

        Toggle toggle = toggleObj.GetComponent<Toggle>();
        toggleObj.transform.SetParent(parent, false);
        return toggle;
    }
}