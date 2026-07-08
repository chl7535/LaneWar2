using UnityEngine;

namespace LaneWar2.Core.Data
{
    [CreateAssetMenu(menuName = "LaneWar2/Tech Node", fileName = "TechNodeDefinition")]
    public class TechNodeDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private int level;
        [SerializeField] private int cost;
        [SerializeField] private string description;
        [SerializeField] private float attackBonus;
        [SerializeField] private float hpBonus;
        [SerializeField] private float spawnRateBonus;

        public string Id => id;
        public string DisplayName => displayName;
        public int Level => level;
        public int Cost => cost;
        public string Description => description;
        public float AttackBonus => attackBonus;
        public float HpBonus => hpBonus;
        public float SpawnRateBonus => spawnRateBonus;
    }
}
