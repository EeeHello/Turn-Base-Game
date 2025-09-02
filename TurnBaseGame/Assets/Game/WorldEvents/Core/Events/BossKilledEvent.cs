using UnityEngine;

namespace Game.WorldEvents.Events
{
    public struct BossKilledEvent
    {
        public string BossId { get; }

        public BossKilledEvent(string bossId)
        {
            BossId = bossId;
        }
    }
}
