using UnityEngine;

public class PlayerDataCarrier : MonoBehaviour
{
    public static PlayerDataCarrier Instance;

    public PlayerData LoadedPlayerData;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}