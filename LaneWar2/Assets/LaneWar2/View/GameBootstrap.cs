using LaneWar2.Core.Data;
using LaneWar2.Core.Simulation;
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

        private void Start()
        {
            sim = new Simulation();

            sim.AddSystem(new SpawnSystem(footmanDefinition, ownerId: 0, spawnIntervalTicks, spawnPosX: -laneHalfLength, spawnPosY: 0f));
            sim.AddSystem(new SpawnSystem(footmanDefinition, ownerId: 1, spawnIntervalTicks, spawnPosX: laneHalfLength, spawnPosY: 0f));
            sim.AddSystem(new MovementSystem());
            sim.AddSystem(new CombatSystem());
            sim.AddSystem(new ResourceSystem());
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
