using System.Collections.Generic;
using UnityEngine;

public class EnemyDataCarrier : MonoBehaviour
{
    public static EnemyDataCarrier Instance;

    public List<Stats> LoadedEnemyStatsList = new List<Stats>();

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
