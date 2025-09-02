using UnityEngine;

namespace Game.WorldEvents.Events
{
    [System.Serializable]
    public class BossDefeated
    {
        public string BossId;
        public Vector3 Position;
        public int PlayerLevel;

        public BossDefeated(string bossId, Vector3 position, int playerLevel)
        {
            BossId = bossId; Position = position; PlayerLevel = playerLevel;
        }
    }
}