using System;
using System.Collections.Generic;
using UnityEngine;

namespace LaneWar2.Core.Data
{
    /// <summary>
    /// 공격자 역할이 방어자 역할을 상대할 때의 데미지 배율 규칙 한 줄.
    /// 1.0=보통, 1.0보다 크면 강함, 작으면 약함.
    /// </summary>
    [Serializable]
    public class HeroMatchupRule
    {
        [SerializeField] private HeroRole attackerRole;
        [SerializeField] private HeroRole defenderRole;
        [SerializeField] private float damageMultiplier = 1f;

        public HeroRole AttackerRole => attackerRole;
        public HeroRole DefenderRole => defenderRole;
        public float DamageMultiplier => damageMultiplier;

        public HeroMatchupRule(HeroRole attackerRole, HeroRole defenderRole, float damageMultiplier)
        {
            this.attackerRole = attackerRole;
            this.defenderRole = defenderRole;
            this.damageMultiplier = damageMultiplier;
        }
    }

    /// <summary>
    /// 역할 간 상성표. 명시된 (공격자, 방어자) 조합만 배율을 가지며, 나머지는 기본값 1.0(보통)이다.
    /// 6x6 행렬 대신 규칙 리스트 형태라 인스펙터에서 필요한 상성만 추가/조정하면 된다.
    /// 지금은 데이터 정의만 하며, 전투 로직에 실제로 반영하는 것은 다음 단계에서 다룬다.
    /// </summary>
    [CreateAssetMenu(menuName = "LaneWar2/Hero Matchup Table", fileName = "HeroMatchupTable")]
    public class HeroMatchupTable : ScriptableObject
    {
        [SerializeField]
        private List<HeroMatchupRule> rules = new List<HeroMatchupRule>
        {
            new HeroMatchupRule(HeroRole.Assassin, HeroRole.Ranged, 1.3f), // 암살자 > 원딜
            new HeroMatchupRule(HeroRole.Tank, HeroRole.Melee, 1.2f),      // 탱커 > 근딜
            new HeroMatchupRule(HeroRole.Ranged, HeroRole.Tank, 1.2f),     // 원딜 > 탱커
            new HeroMatchupRule(HeroRole.Melee, HeroRole.Assassin, 1.2f),  // 근딜 > 암살자
        };

        public IReadOnlyList<HeroMatchupRule> Rules => rules;

        public float GetMultiplier(HeroRole attackerRole, HeroRole defenderRole)
        {
            for (int i = 0; i < rules.Count; i++)
            {
                HeroMatchupRule rule = rules[i];
                if (rule.AttackerRole == attackerRole && rule.DefenderRole == defenderRole)
                {
                    return rule.DamageMultiplier;
                }
            }

            return 1f;
        }
    }
}
