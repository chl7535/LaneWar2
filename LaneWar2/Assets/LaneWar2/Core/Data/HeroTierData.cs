using System;
using UnityEngine;

namespace LaneWar2.Core.Data
{
    [Serializable]
    public class HeroTierData
    {
        [SerializeField] private HeroGrade grade;
        [SerializeField] private int maxHp;
        [SerializeField] private int attackDamage;
        [SerializeField] private int unlockCost; // 이 "등급"을 테크트리에서 해금하는 비용(등급별). 영웅 1기 생산 비용은 HeroDefinition.SpawnCost(영웅별)와 별개.
        [SerializeField] private string unlockedSkillId;

        public HeroGrade Grade => grade;
        public int MaxHp => maxHp;
        public int AttackDamage => attackDamage;
        public int UnlockCost => unlockCost;
        public string UnlockedSkillId => unlockedSkillId;
    }
}
