using LaneWar2.Core.Data;
using LaneWar2.Core.Simulation;
using LaneWar2.Core.Tech;
using UnityEngine;
using UnityEngine.InputSystem;

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

        [SerializeField] private HeroDefinition heroDefinition; // 임시 키 입력 생산 테스트용
        [SerializeField] private UnitDefinition heroUnitDefinition; // 임시 키 입력 생산 테스트용(이동속도/사거리/공격주기 전용, 체력/공격력은 HeroDefinition 티어에서 옴)
        [SerializeField] private HeroGrade testProductionGrade = HeroGrade.Low; // 임시 키 입력으로 생산할 등급(UI 없어 인스펙터로 지정)

        private Simulation sim;
        private HeroProductionSystem heroProduction;
        private int lastLoggedTick;

        // SimulationRenderer 등 View 측 컴포넌트가 시뮬 상태를 읽기 전용으로 참조하기 위한 통로.
        public Simulation Sim => sim;

        private void Start()
        {
            sim = new Simulation();
            heroProduction = new HeroProductionSystem();

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
            HandleHeroProductionInput();

            if (sim.CurrentTick - lastLoggedTick >= Simulation.TickRate)
            {
                lastLoggedTick = sim.CurrentTick;
                LogSummary();
            }
        }

        // 임시 테스트 입력: UI가 아직 없어 숫자 키로 영웅 생산을 트리거한다.
        // 입력은 여기(View)에서만 읽고, Core에는 TryProduceHero 호출만 전달한다(Core는 UnityEngine.Input에 의존하지 않음).
        private void HandleHeroProductionInput()
        {
            if (Keyboard.current == null)
            {
                return;
            }

            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                RequestHeroProduction(ownerId: 0, spawnPosX: -laneHalfLength);
            }

            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                RequestHeroProduction(ownerId: 1, spawnPosX: laneHalfLength);
            }
        }

        private void RequestHeroProduction(int ownerId, float spawnPosX)
        {
            bool produced = heroProduction.TryProduceHero(sim.Context, ownerId, heroDefinition, heroUnitDefinition,
                testProductionGrade, spawnPosX, spawnPosY: 0f);

            if (!produced)
            {
                Debug.LogWarning($"P{ownerId} 영웅 생산 실패(골드 부족) - 보유 {sim.Context.GetGold(ownerId)}, 필요 {heroDefinition.SpawnCost}");
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
