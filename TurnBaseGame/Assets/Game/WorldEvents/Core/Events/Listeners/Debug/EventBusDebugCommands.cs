using Game.WorldEvents.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.WorldEvents.Debugging
{
    public class EventBusDebugInputHandler : MonoBehaviour
    {
        public InputAction _toggleDebugAction;
        public InputAction _printStatsAction;
        public InputAction _clearSubscriptionsAction;

        private void Start()
        {
            SetupInputActions();
        }

        private void SetupInputActions()
        {

            if (_toggleDebugAction != null)
            {
                _toggleDebugAction.performed += OnToggleDebug;
                _toggleDebugAction.Enable();
            }

            if (_printStatsAction != null)
            {
                _printStatsAction.performed += OnPrintStats;
                _printStatsAction.Enable();
            }

            if (_clearSubscriptionsAction != null)
            {
                _clearSubscriptionsAction.performed += OnClearSubscriptions;
                _clearSubscriptionsAction.Enable();
            }
        }

        private void OnDisable()
        {
            // Clean up input actions
            if (_toggleDebugAction != null)
            {
                _toggleDebugAction.performed -= OnToggleDebug;
                _toggleDebugAction.Disable();
            }

            if (_printStatsAction != null)
            {
                _printStatsAction.performed -= OnPrintStats;
                _printStatsAction.Disable();
            }

            if (_clearSubscriptionsAction != null)
            {
                _clearSubscriptionsAction.performed -= OnClearSubscriptions;
                _clearSubscriptionsAction.Disable();
            }
        }

        private void OnToggleDebug(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
#if UNITY_EDITOR
                EventBusDebug.Enabled = !EventBusDebug.Enabled;
                Debug.Log($"[EventBus] Debug {(EventBusDebug.Enabled ? "ENABLED" : "DISABLED")}");
#endif
            }
        }

        private void OnPrintStats(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
#if UNITY_EDITOR
                EventBusDebug.PrintSubscriberStats();
#endif
            }
        }

        private void OnClearSubscriptions(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                EventBus.Clear();
                Debug.Log("[EventBus] All subscriptions cleared");
            }
        }
    }
}