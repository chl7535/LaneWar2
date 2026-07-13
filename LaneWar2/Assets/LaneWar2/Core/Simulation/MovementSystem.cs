using LaneWar2.Core.Units;
using UnityEngine;

namespace LaneWar2.Core.Simulation
{
    /// <summary>
    /// 각 유닛을 가장 가까운 적을 향해 이동시킨다. 사거리 안이면 이동하지 않는다(전투는 CombatSystem 담당).
    /// </summary>
    public class MovementSystem : ISimulationSystem
    {
        public void Tick(SimulationContext ctx)
        {
            for (int i = 0; i < ctx.Units.Count; i++)
            {
                Unit unit = ctx.Units[i];

                if (!unit.IsAlive)
                {
                    continue;
                }

                Unit enemy = TargetingUtil.FindNearestEnemy(ctx, unit);

                if (enemy == null)
                {
                    continue;
                }

                float dx = enemy.PosX - unit.PosX;
                float dy = enemy.PosY - unit.PosY;
                float distSq = dx * dx + dy * dy;
                float rangeSq = unit.Def.AttackRange * unit.Def.AttackRange;

                if (distSq <= rangeSq)
                {
                    continue;
                }

                float dist = Mathf.Sqrt(distSq);

                if (dist <= 0f)
                {
                    continue;
                }

                float moveDist = unit.Def.MoveSpeed * ctx.DeltaTime;
                unit.PosX += (dx / dist) * moveDist;
                unit.PosY += (dy / dist) * moveDist;
            }
        }
    }
}
