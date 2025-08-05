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

    public WeaponInventoryEntry GiveActiveWeapon(int directionOfSwitch, PlayerData pd, WeaponInventoryEntry currentWeapon)
    {
        List<WeaponInventoryEntry> listOfWeapons = FetchMyWeapons(pd);
        int weaponIndex;
        for (int i = 0; i < listOfWeapons.Count; i++)
        {
            if (listOfWeapons[i] == currentWeapon)
            {
                switch (i + directionOfSwitch)
                {
                    default:
                        weaponIndex = i + directionOfSwitch;
                        break;
                    case <0:
                        weaponIndex = 2;
                        break;
                    case >2:
                        weaponIndex = 0;
                        break;
                }
                return listOfWeapons[weaponIndex];
            }
        }
        return currentWeapon;
    }

}
