using LaneWar2.Core.Data;
using LaneWar2.Core.Units;
using UnityEngine;

namespace LaneWar2.Core.Simulation
{
    /// <summary>
    /// 게임 시작 시 1회 각 플레이어의 기지 Unit을 생성하는 팩토리. 매틱 도는 시스템이 아니라
    /// 진입점(GameBootstrap)이 시작 시 한 번 호출한다.
    /// </summary>
    public static class BaseSpawner
    {
        public static Unit SpawnBase(SimulationContext ctx, UnitDefinition baseDef, int ownerId, float posX, float posY)
        {
            int id = ctx.NextUnitId;
            ctx.NextUnitId++;

            var baseUnit = new Unit(id, ownerId, baseDef, posX, posY, baseDef.MaxHp, baseDef.AttackDamage, isBase: true);
            ctx.Units.Add(baseUnit);

            Debug.Log($"[Tick {ctx.CurrentTick}] P{ownerId} 기지 생성 (id={id}), HP={baseDef.MaxHp}, 위치=({posX}, {posY})");

            return baseUnit;
        }
    }
}
