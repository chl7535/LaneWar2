using System.Collections.Generic;
using LaneWar2.Core.Data;
using LaneWar2.Core.Units;
using UnityEngine;

namespace LaneWar2.Core.Simulation
{
    /// <summary>
    /// 영웅 수동 생산 API. 등급 해금(테크트리)과는 별개로, 요청 시점에 골드를 확인/차감하고
    /// 영웅 1기를 전장에 생성한다. 자동으로 매틱 실행되지 않으며(ISimulationSystem 아님),
    /// 호출자(View의 입력 처리 등)가 필요할 때 TryProduceHero를 호출하는 방식이다.
    /// </summary>
    public class HeroProductionSystem
    {
        /// <summary>
        /// 골드가 충분하고 해당 등급의 티어 데이터가 있으면 영웅 1기를 생산한다. 실패 시 아무 것도 바뀌지 않는다.
        /// </summary>
        public bool TryProduceHero(SimulationContext ctx, int ownerId, HeroDefinition heroDef, UnitDefinition heroUnitDef,
            HeroGrade grade, float spawnPosX, float spawnPosY)
        {
            HeroTierData tier = FindTier(heroDef, grade);

            if (tier == null)
            {
                Debug.LogWarning($"[Tick {ctx.CurrentTick}] HeroDefinition '{heroDef.Id}'에 {grade} 등급 티어가 없어 영웅 생산을 건너뜀");
                return false;
            }

            int cost = heroDef.SpawnCost;

            if (ctx.GetGold(ownerId) < cost)
            {
                return false;
            }

            ctx.AddGold(ownerId, -cost);

            int id = ctx.NextUnitId;
            ctx.NextUnitId++;

            var hero = new Unit(id, ownerId, heroUnitDef, spawnPosX, spawnPosY, tier.MaxHp, tier.AttackDamage,
                isHero: true, heroGrade: grade, heroId: heroDef.Id);
            ctx.Units.Add(hero);

            Debug.Log($"[Tick {ctx.CurrentTick}] P{ownerId} 영웅 '{heroDef.DisplayName}' ({heroDef.Role}, {GetGradeLabel(grade)}) 생산! 코스트 {cost}, 남은 골드 {ctx.GetGold(ownerId)}");

            return true;
        }

        private static HeroTierData FindTier(HeroDefinition heroDef, HeroGrade grade)
        {
            IReadOnlyList<HeroTierData> tiers = heroDef.Tiers;

            for (int i = 0; i < tiers.Count; i++)
            {
                if (tiers[i].Grade == grade)
                {
                    return tiers[i];
                }
            }

            return null;
        }

        private static string GetGradeLabel(HeroGrade grade)
        {
            switch (grade)
            {
                case HeroGrade.Low: return "하급";
                case HeroGrade.Mid: return "중급";
                case HeroGrade.High: return "상급";
                case HeroGrade.Legendary: return "전설";
                case HeroGrade.Epic: return "에픽";
                default: return grade.ToString();
            }
        }
    }
}
