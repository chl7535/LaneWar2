using LaneWar2.Core.Units;

namespace LaneWar2.Core.Simulation
{
    /// <summary>
    /// 결정론적 타겟 선택 로직. MovementSystem과 CombatSystem이 공유한다.
    /// </summary>
    public static class TargetingUtil
    {
        /// <summary>
        /// self와 다른 ownerId를 가진 살아있는 적 중 가장 가까운 적을 찾는다.
        /// 거리가 같으면 id가 작은 쪽을 선택한다.
        /// </summary>
        public static Unit FindNearestEnemy(SimulationContext ctx, Unit self)
        {
            Unit best = null;
            float bestDistSq = 0f;

            for (int i = 0; i < ctx.Units.Count; i++)
            {
                Unit candidate = ctx.Units[i];

                if (candidate == self || candidate.OwnerId == self.OwnerId || !candidate.IsAlive)
                {
                    continue;
                }

                float dx = candidate.PosX - self.PosX;
                float dy = candidate.PosY - self.PosY;
                float distSq = dx * dx + dy * dy;

                if (best == null || distSq < bestDistSq || (distSq == bestDistSq && candidate.Id < best.Id))
                {
                    best = candidate;
                    bestDistSq = distSq;
                }
            }

            return best;
        }
    }
}
