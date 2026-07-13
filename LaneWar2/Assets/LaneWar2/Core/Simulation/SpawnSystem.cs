using LaneWar2.Core.Data;
using LaneWar2.Core.Tech;
using LaneWar2.Core.Units;
using UnityEngine;

namespace LaneWar2.Core.Simulation
{
    /// <summary>
    /// 기지가 일정 주기(spawnIntervalTicks)마다 풋맨을 자동 생성한다.
    /// </summary>
    public class SpawnSystem : ISimulationSystem
    {
        private readonly UnitDefinition footmanDef;
        private readonly int ownerId;
        private readonly int spawnIntervalTicks;
        private readonly float spawnPosX;
        private readonly float spawnPosY;

        private int ticksSinceLastSpawn;

        public SpawnSystem(UnitDefinition footmanDef, int ownerId, int spawnIntervalTicks, float spawnPosX = 0f, float spawnPosY = 0f)
        {
            this.footmanDef = footmanDef;
            this.ownerId = ownerId;
            this.spawnIntervalTicks = spawnIntervalTicks;
            this.spawnPosX = spawnPosX;
            this.spawnPosY = spawnPosY;
        }

        public void Tick(SimulationContext ctx)
        {
            ticksSinceLastSpawn++;

            PlayerTechState techState = ctx.GetTechState(ownerId);
            int effectiveIntervalTicks = Mathf.Max(1, Mathf.RoundToInt(spawnIntervalTicks * techState.SpawnIntervalMultiplier));

            if (ticksSinceLastSpawn < effectiveIntervalTicks)
            {
                return;
            }

            ticksSinceLastSpawn = 0;

            int id = ctx.NextUnitId;
            ctx.NextUnitId++;

            // 원본 def는 읽기만 하고, 강화 보너스를 합산한 값을 스폰 시점에 유닛에 고정한다.
            int effectiveMaxHp = footmanDef.MaxHp + techState.BonusHp;
            int effectiveAttackDamage = footmanDef.AttackDamage + techState.BonusAttack;

            var unit = new Unit(id, ownerId, footmanDef, spawnPosX, spawnPosY, effectiveMaxHp, effectiveAttackDamage);
            ctx.Units.Add(unit);

            Debug.Log($"[Tick {ctx.CurrentTick}] Player {ownerId} 풋맨 스폰 (id={id}), 현재 유닛 수={ctx.Units.Count}");
        }
    }
}
