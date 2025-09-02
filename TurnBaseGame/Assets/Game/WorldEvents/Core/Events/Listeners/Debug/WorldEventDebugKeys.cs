using UnityEngine;
using Game.WorldEvents.Core;
using Game.WorldEvents.Events;

namespace Game.WorldEvents.Debugging
{
    public class WorldEventDebugKeys : MonoBehaviour
    {
        [SerializeField] private string bossId = "AlphaOgre";

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                var pos = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                EventBus.Publish(new BossDefeated(bossId, pos, playerLevel: 7));
            }
        }
    }
}
