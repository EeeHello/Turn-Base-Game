using UnityEngine;
using Game.WorldEvents.Core;
using Game.WorldEvents.Events;
using UnityEngine.InputSystem;

namespace Game.WorldEvents.Debugging
{
    /// <summary>
    /// Debug purposes
    /// Right now only tested on Boss zombie
    /// </summary>
    public class WorldEventDebugKeys : MonoBehaviour
    {
        [SerializeField] private string bossId = "AlphaZombie001";

        [Header("Input settings")]
        public InputAction bossKillingKey;

        private void OnEnable()
        {
            bossKillingKey.Enable();
            bossKillingKey.performed += OnBossKilled;
        }

        private void OnDisable()
        {
            bossKillingKey.performed -= OnBossKilled;
            bossKillingKey.Disable();
        }

        private void OnBossKilled(InputAction.CallbackContext context)
        {
            Debug.Log($"[DEBUG] Boss {bossId} killed via debug key!");

            // Use static Publish instead of .Instance
            WorldEventManager.Publish(new BossKilledEvent(bossId));
        }
    }
}
