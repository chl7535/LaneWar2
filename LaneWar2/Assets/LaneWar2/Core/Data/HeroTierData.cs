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
        [SerializeField] private int unlockCost;
        [SerializeField] private string unlockedSkillId;

        public HeroGrade Grade => grade;
        public int MaxHp => maxHp;
        public int AttackDamage => attackDamage;
        public int UnlockCost => unlockCost;
        public string UnlockedSkillId => unlockedSkillId;
    }
}
