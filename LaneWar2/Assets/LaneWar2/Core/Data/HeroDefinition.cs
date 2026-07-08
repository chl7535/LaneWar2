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

        public string Id => id;
        public string DisplayName => displayName;
        public IReadOnlyList<HeroTierData> Tiers => tiers;
    }
}
