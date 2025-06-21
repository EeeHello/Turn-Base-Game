using UnityEngine;

public class PlayerRuntime : MonoBehaviour
{
    public PlayerData data;

    public void Initialize(PlayerData playerData)
    {
        data = playerData;
        Debug.Log($"Initialized Player: {data.Username} | Level: {data.Stats.Level} | Health: {data.Stats.Health}");
        // You can now apply this data to health, inventory, abilities, etc.
    }
}