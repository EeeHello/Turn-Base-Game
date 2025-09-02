using Game.WorldEvents.Core;
using Game.WorldEvents.Events;
using System;
using UnityEngine;

public class WorldEventListenerTest : EventListenerBehaviour<BossKilledEvent>
{
    protected override void OnEvent(BossKilledEvent evt)
    {
        Debug.Log($"[QUEST] Boss {evt.BossId} was killed! Quest updated.");
    }

    // Optional: Additional manual subscription example
    private IDisposable _manualSubscription;

    private void Start()
    {
        // Example of using the SubscribeOnce extension
        _manualSubscription = this.SubscribeOnce<BossKilledEvent>(OnOneTimeBossKill);
    }

    private void OnOneTimeBossKill(BossKilledEvent evt)
    {
        Debug.Log($"[ONE-TIME] Boss {evt.BossId} killed! This will only trigger once.");
    }

    private void OnDestroy()
    {
        _manualSubscription?.Dispose();
    }
}