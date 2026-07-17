using LaneWar2.Core.Units;
using UnityEngine;

namespace LaneWar2.Core.Simulation
{
    /// <summary>
    /// 매틱 두 플레이어의 기지 생존 여부를 확인해 게임 종료를 판정한다.
    /// 기지 Unit 참조를 생성 시점에 직접 들고 있는다 — CombatSystem이 죽은 유닛을 같은 틱 안에
    /// ctx.Units에서 즉시 제거하므로, 리스트를 다시 검색하는 방식으로는 죽은 기지를 찾을 수 없다.
    /// </summary>
    public class VictorySystem : ISimulationSystem
    {
        private readonly Unit baseP0;
        private readonly Unit baseP1;

        public VictorySystem(Unit baseP0, Unit baseP1)
        {
            this.baseP0 = baseP0;
            this.baseP1 = baseP1;
        }

        public void Tick(SimulationContext ctx)
        {
            bool p0Dead = !baseP0.IsAlive;
            bool p1Dead = !baseP1.IsAlive;

            if (!p0Dead && !p1Dead)
            {
                return;
            }

            ctx.IsGameOver = true;

            if (p0Dead && p1Dead)
            {
                ctx.WinnerId = -1;
                Debug.Log($"[Tick {ctx.CurrentTick}] 게임 종료! 무승부 (양측 기지 동시 파괴)");
            }
            else if (p0Dead)
            {
                ctx.WinnerId = 1;
                Debug.Log($"[Tick {ctx.CurrentTick}] 게임 종료! P1 승리 (P0 기지 파괴)");
            }
            else
            {
                ctx.WinnerId = 0;
                Debug.Log($"[Tick {ctx.CurrentTick}] 게임 종료! P0 승리 (P1 기지 파괴)");
            }
        }
    }
}
