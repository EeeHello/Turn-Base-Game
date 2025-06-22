using UnityEngine;

public class EnemyDataCarrier : MonoBehaviour
{
    public static EnemyDataCarrier Instance;

    public Stats LoadedEnemyStats;

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
