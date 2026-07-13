using LaneWar2.Core.Units;
using UnityEngine;

namespace LaneWar2.Core.Simulation
{
    /// <summary>
    /// 각 유닛이 가장 가까운 적이 사거리 안에 있으면 공격한다. 틱이 끝나면 죽은 유닛을 정리한다.
    /// </summary>
    public class CombatSystem : ISimulationSystem
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

                if (distSq > rangeSq)
                {
                    continue;
                }

                if (unit.AttackCooldown <= 0f)
                {
                    enemy.CurrentHp -= unit.EffectiveAttackDamage;
                    unit.AttackCooldown = unit.Def.AttackInterval;

                    if (!enemy.IsAlive)
                    {
                        ctx.PendingKills.Add(new KillEvent(unit.OwnerId, enemy.Id));
                        Debug.Log($"[Tick {ctx.CurrentTick}] Unit {unit.Id}(P{unit.OwnerId})가 Unit {enemy.Id} 처치");
                    }
                }
                else
                {
                    unit.AttackCooldown -= ctx.DeltaTime;
                }
            }

            ctx.Units.RemoveAll(u => !u.IsAlive);
        }
    }
}
