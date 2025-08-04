using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FightDataManager : MonoBehaviour
{
    public List<PlayerData>  allPlayerssData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerRuntime[] playerRuntimes =  FindObjectsOfType<PlayerRuntime>();
        foreach (var item in playerRuntimes)
        {
            allPlayerssData.Add(item.data);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public PlayerData FetchMyPlayersData(GameObject Player)
    {

        if (allPlayerssData == null)
            return null;

        PlayerData data;

        foreach (var item in allPlayerssData)
        {
            if (item.Username == Player.GetComponent<PlayerRuntime>().data.Username)
            {
                data = item;
                return data;
            }
        }
        return null;
    }

    public List<WeaponInventoryEntry> FetchMyWeapons(PlayerData pd)
    {
        List<WeaponInventoryEntry> equipedWeapons = new List<WeaponInventoryEntry>();

        foreach (var weapon in pd.Weapons)
        {
            if (weapon.isEquipped) equipedWeapons.Add(weapon);
        }

        return equipedWeapons;
    }

    public List<List<string>> FetchMyActions(WeaponInventoryEntry weapon)
    {
        List<string> bActions = new List<string>();
        List<string> sActions = new List<string>();
        List<string> buAction = new List<string>();

    
            foreach (var action in weapon.selectedBasicActionIDs)
            {
                bActions.Add(action);
            }

            foreach (var action in weapon.selectedSkillActionIDs)
            {
                sActions.Add(action);
            }

            buAction.Add(weapon.selectedBurstActionID);

        

        return new List<List<string>> { bActions, sActions, buAction};
    }

}
