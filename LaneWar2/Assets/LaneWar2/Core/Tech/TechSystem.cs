using System.Collections.Generic;
using LaneWar2.Core.Simulation;
using UnityEngine;

namespace LaneWar2.Core.Tech
{
    /// <summary>
    /// 라인 A(병영 강화) 업그레이드 규칙. TryUpgrade가 실제 처리 API이며,
    /// 생성자로 받은 예약 목록은 UI가 붙기 전까지 Console로 동작을 확인하기 위한 테스트용 자동 발동이다.
    /// </summary>
    public class TechSystem : ISimulationSystem
    {
        public const int AttackBonusPerLevel = 2;
        public const int HpBonusPerLevel = 20;
        public const float SpawnIntervalMultiplierPerLevel = 0.9f; // 레벨업마다 스폰 간격 10% 단축
        public const int MaxLevel = 5;

        /// <summary>
        /// 테스트용 자동 발동 예약 하나. 골드가 부족하면 조용히 실패하므로,
        /// FromTick 이후 매 틱 재시도하다가 한 번 성공하면 더 이상 시도하지 않는다.
        /// </summary>
        private class ScheduledTestUpgrade
        {
            public int FromTick;
            public int OwnerId;
            public bool Fired;
        }

        private readonly List<ScheduledTestUpgrade> scheduledTestUpgrades;

        public TechSystem(IEnumerable<(int tick, int ownerId)> scheduledTestUpgrades = null)
        {
            this.scheduledTestUpgrades = new List<ScheduledTestUpgrade>();

            if (scheduledTestUpgrades != null)
            {
                foreach ((int tick, int ownerId) in scheduledTestUpgrades)
                {
                    this.scheduledTestUpgrades.Add(new ScheduledTestUpgrade { FromTick = tick, OwnerId = ownerId });
                }
            }
        }

        public static int GetUpgradeCost(int currentLevel)
        {
            return currentLevel * 50;
        }

        public void Tick(SimulationContext ctx)
        {
            for (int i = 0; i < scheduledTestUpgrades.Count; i++)
            {
                ScheduledTestUpgrade schedule = scheduledTestUpgrades[i];

                if (schedule.Fired || ctx.CurrentTick < schedule.FromTick)
                {
                    continue;
                }

                schedule.Fired = TryUpgrade(ctx, schedule.OwnerId);
            }
        }

        /// <summary>
        /// 골드가 충분하고 최대 레벨 미만이면 업그레이드를 적용한다. 실패 시 아무 것도 바뀌지 않는다.
        /// </summary>
        public bool TryUpgrade(SimulationContext ctx, int ownerId)
        {
            PlayerTechState state = ctx.GetTechState(ownerId);

            if (state.BaseLevel >= MaxLevel)
            {
                return false;
            }

            int cost = GetUpgradeCost(state.BaseLevel);

            if (ctx.GetGold(ownerId) < cost)
            {
                return false;
            }

            ctx.AddGold(ownerId, -cost);

            float newMultiplier = state.SpawnIntervalMultiplier * SpawnIntervalMultiplierPerLevel;
            state.ApplyUpgrade(AttackBonusPerLevel, HpBonusPerLevel, newMultiplier);

            Debug.Log($"[Tick {ctx.CurrentTick}] P{ownerId} 병영 Lv{state.BaseLevel} 업그레이드! (공격+{AttackBonusPerLevel}, 체력+{HpBonusPerLevel}), 남은 골드={ctx.GetGold(ownerId)}");
            return true;
        }
    }
}
