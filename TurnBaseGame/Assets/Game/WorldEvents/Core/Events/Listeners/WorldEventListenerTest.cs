using UnityEngine;
using Game.WorldEvents.Core;
using Game.WorldEvents.Events;

public class WorldEventListenerTest : MonoBehaviour
{
    private void OnEnable()
    {
        WorldEventManager.Subscribe<BossKilledEvent>(OnBossKilled);
    }

    private void OnDisable()
    {
        WorldEventManager.Unsubscribe<BossKilledEvent>(OnBossKilled);
    }

    private void OnBossKilled(BossKilledEvent evt)
    {
        Debug.Log($"[QUEST] Boss {evt.BossId} was killed! Quest updated.");
    }
}
