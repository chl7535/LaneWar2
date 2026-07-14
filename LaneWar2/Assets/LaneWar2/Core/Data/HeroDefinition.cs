using System.Collections.Generic;
using UnityEngine;

namespace LaneWar2.Core.Data
{
    [CreateAssetMenu(menuName = "LaneWar2/Hero Definition", fileName = "HeroDefinition")]
    public class HeroDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private List<HeroTierData> tiers = new List<HeroTierData>();
        [SerializeField] private HeroRole role;
        [SerializeField] private int spawnCost; // 이 영웅 1기를 생산하는 비용(골드). 등급 해금 비용(HeroTierData.UnlockCost)과 별개.

        public string Id => id;
        public string DisplayName => displayName;
        public IReadOnlyList<HeroTierData> Tiers => tiers;
        public HeroRole Role => role;
        public int SpawnCost => spawnCost;
    }
}
