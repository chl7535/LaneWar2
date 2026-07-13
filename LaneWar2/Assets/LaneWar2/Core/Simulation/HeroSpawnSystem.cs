using System.Collections.Generic;
using LaneWar2.Core.Data;
using LaneWar2.Core.Units;
using UnityEngine;

namespace LaneWar2.Core.Simulation
{
    /// <summary>
    /// 플레이어당 영웅 1기를 게임 시작 시 소환한다. 이동/사거리/공격주기는 heroUnitDef(일반 UnitDefinition)에서,
    /// 체력/공격력은 heroDef의 startingGrade 티어 수치에서 가져와 스폰 시점에 확정한다.
    /// 등급 해금/성장(구매로 등급 올리기)은 다음 단계에서 다룬다 — 여기서는 소환까지만.
    /// </summary>
    public class HeroSpawnSystem : ISimulationSystem
    {
        private readonly HeroDefinition heroDef;
        private readonly UnitDefinition heroUnitDef;
        private readonly HeroGrade startingGrade;
        private readonly int ownerId;
        private readonly float spawnPosX;
        private readonly float spawnPosY;
        private readonly int spawnTick;

        private bool hasSpawned;

        public HeroSpawnSystem(HeroDefinition heroDef, UnitDefinition heroUnitDef, HeroGrade startingGrade, int ownerId,
            float spawnPosX = 0f, float spawnPosY = 0f, int spawnTick = 1)
        {
            this.heroDef = heroDef;
            this.heroUnitDef = heroUnitDef;
            this.startingGrade = startingGrade;
            this.ownerId = ownerId;
            this.spawnPosX = spawnPosX;
            this.spawnPosY = spawnPosY;
            this.spawnTick = spawnTick;
        }

        public void Tick(SimulationContext ctx)
        {
            if (hasSpawned || ctx.CurrentTick < spawnTick)
            {
                return;
            }

            hasSpawned = true;

            HeroTierData tier = FindTier(startingGrade);

            if (tier == null)
            {
                Debug.LogWarning($"[Tick {ctx.CurrentTick}] HeroDefinition '{heroDef.Id}'에 {startingGrade} 등급 티어가 없어 영웅 소환을 건너뜀");
                return;
            }

            int id = ctx.NextUnitId;
            ctx.NextUnitId++;

            var hero = new Unit(id, ownerId, heroUnitDef, spawnPosX, spawnPosY, tier.MaxHp, tier.AttackDamage,
                isHero: true, heroGrade: startingGrade, heroId: heroDef.Id);
            ctx.Units.Add(hero);

            Debug.Log($"[Tick {ctx.CurrentTick}] P{ownerId} 영웅 '{heroDef.DisplayName}' ({GetGradeLabel(startingGrade)}) 소환 - HP {tier.MaxHp}, ATK {tier.AttackDamage}");
        }

        private HeroTierData FindTier(HeroGrade grade)
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
