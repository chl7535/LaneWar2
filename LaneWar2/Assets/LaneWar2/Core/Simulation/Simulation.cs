using System.Collections.Generic;

namespace LaneWar2.Core.Simulation
{
    /// <summary>
    /// 결정론적 고정 timestep 시뮬레이션 루프.
    /// UnityEngine.Time/Update에 의존하지 않는다 — 같은 입력이면 어떤 기기에서도 같은 결과가 나와야 한다.
    /// </summary>
    public class Simulation
    {
        public const int TickRate = 20; // 초당 20틱
        public const float FixedDeltaTime = 1f / TickRate;

        private readonly List<ISimulationSystem> systems = new List<ISimulationSystem>();
        private readonly SimulationContext context = new SimulationContext();
        private float accumulator;

        // TODO: 결정론적 시드 기반 RNG는 여기 추가 예정 (System.Random 대신 자체 구현으로 플랫폼 간 일관성 보장)

        public int CurrentTick => context.CurrentTick;

        public void AddSystem(ISimulationSystem system)
        {
            systems.Add(system);
        }

        /// <summary>
        /// 한 틱을 진행한다. 등록된 시스템을 등록 순서대로 실행하며,
        /// 이 순서 보장이 결정론(같은 입력 → 같은 결과)에 핵심적이다.
        /// </summary>
        public void Tick()
        {
            context.CurrentTick++;
            context.DeltaTime = FixedDeltaTime;

            for (int i = 0; i < systems.Count; i++)
            {
                systems[i].Tick(context);
            }
        }

        /// <summary>
        /// 실제 경과 시간을 누적하고, FixedDeltaTime만큼 쌓일 때마다 Tick()을 호출한다.
        /// 이 accumulator 패턴이 프레임률과 무관하게 고정된 틱 진행을 보장한다.
        /// 남는 시간(FixedDeltaTime 미만)은 다음 Advance 호출까지 유지된다.
        /// </summary>
        public void Advance(float realDeltaTime)
        {
            accumulator += realDeltaTime;

            while (accumulator >= FixedDeltaTime)
            {
                Tick();
                accumulator -= FixedDeltaTime;
            }
        }
    }
}
