using LaneWar2.Core.Data;
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

            if (ticksSinceLastSpawn < spawnIntervalTicks)
            {
                return;
            }

            ticksSinceLastSpawn = 0;

            int id = ctx.NextUnitId;
            ctx.NextUnitId++;

            var unit = new Unit(id, ownerId, footmanDef, spawnPosX, spawnPosY);
            ctx.Units.Add(unit);

            Debug.Log($"[Tick {ctx.CurrentTick}] Player {ownerId} 풋맨 스폰 (id={id}), 현재 유닛 수={ctx.Units.Count}");
        }
    }
}
