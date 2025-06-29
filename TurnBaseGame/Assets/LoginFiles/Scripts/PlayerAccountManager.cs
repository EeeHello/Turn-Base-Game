using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Unity.VisualScripting;

[System.Serializable]
public class PlayerData
{
    public string Username;
    public int SaveSlot;
    public List<WeaponInventoryEntry> Weapons;
    public Ability[] Abilities;
    public Stats Stats;
    public Effect[] Effects;
}


[System.Serializable]
public class WeaponInventoryEntry
{
    public string weaponID; // ID or name used to reference a WeaponSO or prefab
    public bool isEquipped; // Whether this weapon is currently active in battle

    [Range(0f, 1f)]
    public float trust; // Trust level with the weapon

    // Actions unlocked and chosen from this weapon
    public List<string> unlockedBasicActionIDs;
    public List<string> unlockedSkillActionIDs;
    public string equippedBurstActionID;

    // These are the currently selected actions from this weapon
    public List<string> selectedBasicActionIDs;
    public List<string> selectedSkillActionIDs;
    public string selectedBurstActionID;
}

[System.Serializable]
public class Ability
{
    public string name;
    public string otherInfo;
}

[System.Serializable]
public class Stats
{
    public int Level;
    public int Health;
    public int PhyAtk;
    public int MagAtk;
    public int PhyDef;
    public int MagDef;
    public int Luck;
}

[System.Serializable]
public class Effect
{
    public string name;
    public int duration;
}

[System.Serializable]
public class LoginInfo
{
    public string Username;
    public string Password;
    public int SaveSlotCount;
}

[System.Serializable]
public class LoginListWrapper
{
    public List<LoginInfo> accounts = new List<LoginInfo>();
}

public class PlayerAccountManager : MonoBehaviour
{
    [Header("Player Prefab")]
    public GameObject playerPrefab;

    [Header("Starting Weapons")]
    public List<WeaponSO> startingWeapons;

    [Header("Scene Settings")]
    public string gameSceneName; 

    [Header("UI Panels")]
    public GameObject MainMenuPanel;
    public GameObject NewUserPanel;
    public GameObject LoginPanel;

    [Header("Inputs")]
    public InputField NewUsernameInput;
    public InputField NewPasswordInput;

    public InputField LoginUsernameInput;
    public InputField LoginPasswordInput;
    public Text SaveSlotDisplay;

    [Header("Slot Buttons")]
    public Button[] SlotButtons;

    [Header("File Paths")]
    public string loginPath;
    public string templatePath;
    public string saveFolder;
    private LoginListWrapper loginList;

    private void Start()
    {
       

        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);

        LoadLoginList();
    }

    void LoadLoginList()
    {
        if (File.Exists(loginPath))
        {
            loginList = JsonUtility.FromJson<LoginListWrapper>(File.ReadAllText(loginPath));
            if (loginList == null)  Debug.Log("loginList = Null"+ "  loginPath: " + loginPath);
        }
        else
        {
            loginList = new LoginListWrapper();
            File.WriteAllText(loginPath, JsonUtility.ToJson(loginList, true));
        }
    }

    public void OnNewPlayerClicked()
    {
        MainMenuPanel.SetActive(false);
        NewUserPanel.SetActive(true);
    }

    public void OnExistingPlayerClicked()
    {
        MainMenuPanel.SetActive(false);
        LoginPanel.SetActive(true);
    }

    public void OnBackButtonClicked()
    {
        MainMenuPanel.SetActive(true);
        NewUserPanel.SetActive(false);
        LoginPanel.SetActive(false);
    }

    public void CreateNewAccount()
    {
        string username = NewUsernameInput.text;
        string password = NewPasswordInput.text;

        if (loginList.accounts.Exists(a => a.Username == username))
        {
            Debug.LogWarning("Username already exists!");
            return;
        }

        Debug.Log("Username is available. Creating account...");

        LoginInfo newLogin = new LoginInfo
        {
            Username = username,
            Password = password,
            SaveSlotCount = 1
        };

        loginList.accounts.Add(newLogin);
        File.WriteAllText(loginPath, JsonUtility.ToJson(loginList, true));

        string templateJson = File.ReadAllText(templatePath);
        PlayerData player = JsonUtility.FromJson<PlayerData>(templateJson);
        player.Username = username;
        player.SaveSlot = 1;
        player.Stats.Level = 1;
        player.Stats.Health = 100;
        player.Stats.PhyAtk = 20;
        player.Stats.PhyDef = 5;
        player.Stats.MagAtk = 30;
        player.Stats.MagDef = 5;
        player.Stats.Luck = 5;
        player.Abilities = new Ability[0];
        player.Effects = new Effect[0];
        player.Weapons = new List<WeaponInventoryEntry>();

        foreach (var weapon in startingWeapons)
        {
            WeaponInventoryEntry entry = new WeaponInventoryEntry
            {
                weaponID = weapon.weaponName,
                isEquipped = true,
                trust = 0.0f,

                unlockedBasicActionIDs = new List<string>(),
                unlockedSkillActionIDs = new List<string>(),
                selectedBasicActionIDs = new List<string>(),
                selectedSkillActionIDs = new List<string>(),
                equippedBurstActionID = null,
                selectedBurstActionID = null
            };

            // Give first basic/skill/burst
            if (weapon.basicActions.Count > 0)
            {
                var basicID = weapon.basicActions[0].action.name;
                entry.unlockedBasicActionIDs.Add(basicID);
                entry.selectedBasicActionIDs.Add(basicID);
            }

            if (weapon.skillActions.Count > 0)
            {
                var skillID = weapon.skillActions[0].action.name;
                entry.unlockedSkillActionIDs.Add(skillID);
                entry.selectedSkillActionIDs.Add(skillID);
            }

            if (weapon.baseBurst != null)
            {
                var burstID = weapon.baseBurst.name;
                entry.equippedBurstActionID = burstID;
                entry.selectedBurstActionID = burstID;
            }

            player.Weapons.Add(entry);
        }

        string savePath = Path.Combine(saveFolder, $"{username}_Slot1.json");
        File.WriteAllText(savePath, JsonUtility.ToJson(player, true));

        Debug.Log("Account created and Slot 1 initialized.");

        // Auto-fill login fields for auto-login
        LoginUsernameInput.text = username;
        LoginPasswordInput.text = password;

        // Hide new user panel and show login panel if needed
        NewUserPanel.SetActive(false);
        LoginPanel.SetActive(true);

        // Call login to simulate automatic login
        Login();
    }


    public void Login()
    {
        string username = LoginUsernameInput.text;
        string password = LoginPasswordInput.text;

        LoginInfo account = loginList.accounts.Find(a => a.Username == username && a.Password == password);
        if (account == null)
        {
            Debug.LogWarning("Invalid username or password.");
            return;
        }

        // Show slot buttons according to available slots
        SaveSlotDisplay.text = $"Available Slots: {account.SaveSlotCount}";

        for (int i = 0; i < SlotButtons.Length; i++)
        {
            SlotButtons[i].interactable = i < account.SaveSlotCount;
        }
    }

    public void LoadSlot(int slot)
    {
        string username = LoginUsernameInput.text;
        string path = Path.Combine(saveFolder, $"{username}_Slot{slot}.json");

        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            PlayerData player = JsonUtility.FromJson<PlayerData>(data);
            Debug.Log($"Loaded data for {username}, Slot {slot}");

            if (PlayerDataCarrier.Instance == null)
            {
                Debug.LogError("PlayerDataCarrier not found in scene!");
                return;
            }

            PlayerDataCarrier.Instance.LoadedPlayerData = player;

            // Load the gameplay scene
            SceneManager.LoadScene(gameSceneName);
            this.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Save file does not exist.");
        }
    }
}
