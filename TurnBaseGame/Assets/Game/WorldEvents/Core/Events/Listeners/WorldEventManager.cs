using UnityEngine;
using Game.WorldEvents.Core;
using Game.WorldEvents.Events;

namespace Game.WorldEvents.Listeners
{
    public class WorldEventManager : MonoBehaviour
    {
        private System.IDisposable _bossSub;

        private void OnEnable()
        {
            _bossSub = EventBus.Subscribe<BossDefeated>(OnBossDefeated);
        }

        private void OnDisable()
        {
            _bossSub?.Dispose(); _bossSub = null;
        }

        private void OnBossDefeated(BossDefeated e)
        {
            Debug.Log($"[World] Boss '{e.BossId}' down at {e.Position} (PLv {e.PlayerLevel}).");
        }

    }
}