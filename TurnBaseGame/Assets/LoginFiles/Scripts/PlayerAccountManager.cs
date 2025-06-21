using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerData
{
    public string Username;
    public int SaveSlot;
    public string[] Inventory;
    public Ability[] Abilities;
    public Stats Stats;
    public Effect[] Effects;
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
        player.Stats.Health = 10;
        player.Inventory = new string[0];
        player.Abilities = new Ability[0];
        player.Effects = new Effect[0];

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
