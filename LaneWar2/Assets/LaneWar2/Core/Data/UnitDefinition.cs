using UnityEngine;

namespace LaneWar2.Core.Data
{
    [CreateAssetMenu(menuName = "LaneWar2/Unit Definition", fileName = "UnitDefinition")]
    public class UnitDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private int maxHp;
        [SerializeField] private int attackDamage;
        [SerializeField] private float attackInterval;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float attackRange;

        public string Id => id;
        public string DisplayName => displayName;
        public int MaxHp => maxHp;
        public int AttackDamage => attackDamage;
        public float AttackInterval => attackInterval;
        public float MoveSpeed => moveSpeed;
        public float AttackRange => attackRange;
    }
}
