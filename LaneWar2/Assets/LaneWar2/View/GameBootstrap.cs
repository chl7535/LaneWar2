using LaneWar2.Core.Data;
using LaneWar2.Core.Simulation;
using LaneWar2.Core.Tech;
using UnityEngine;

namespace LaneWar2.View
{
    /// <summary>
    /// Unity 진입점. Simulation을 구성하고 Time.deltaTime을 결정론 틱 루프로 흘려보내는
    /// 유일한 지점이다(Simulation 내부는 UnityEngine.Time에 의존하지 않는다).
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private UnitDefinition footmanDefinition;
        [SerializeField] private int spawnIntervalTicks = 60; // 20틱/초 기준 3초마다 스폰
        [SerializeField] private float laneHalfLength = 10f;

        private Simulation sim;
        private int lastLoggedTick;

        // SimulationRenderer 등 View 측 컴포넌트가 시뮬 상태를 읽기 전용으로 참조하기 위한 통로.
        public Simulation Sim => sim;

        private void Start()
        {
            sim = new Simulation();

            sim.AddSystem(new SpawnSystem(footmanDefinition, ownerId: 0, spawnIntervalTicks, spawnPosX: -laneHalfLength, spawnPosY: 0f));
            sim.AddSystem(new SpawnSystem(footmanDefinition, ownerId: 1, spawnIntervalTicks, spawnPosX: laneHalfLength, spawnPosY: 0f));
            sim.AddSystem(new MovementSystem());
            sim.AddSystem(new CombatSystem());
            sim.AddSystem(new ResourceSystem());

            // TODO(M4/UI): 지금은 UI가 없어 라인 A 업그레이드 확인용으로 P0가 400틱(20초) 이후
            // 골드가 모이는 첫 틱에 자동 구매하도록 예약해둔다(TechSystem이 골드 부족 시 매 틱 재시도).
            sim.AddSystem(new TechSystem(new (int tick, int ownerId)[] { (400, 0) }));
        }

        private void Update()
        {
            sim.Advance(Time.deltaTime);

            if (sim.CurrentTick - lastLoggedTick >= Simulation.TickRate)
            {
                lastLoggedTick = sim.CurrentTick;
                LogSummary();
            }
        }

        private void LogSummary()
        {
            SimulationContext ctx = sim.Context;
            int goldP0 = ctx.GetGold(0);
            int goldP1 = ctx.GetGold(1);

            Debug.Log($"[Tick {sim.CurrentTick}] 유닛수={ctx.Units.Count}, P0골드={goldP0}, P1골드={goldP1}");
        }
    }
}
