using LaneWar2.Core.Data;
using LaneWar2.Core.Simulation;
using LaneWar2.Core.Tech;
using LaneWar2.Core.Units;
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
        [SerializeField] private UnitDefinition baseDefinition;
        [SerializeField] private int spawnIntervalTicks = 60; // 20틱/초 기준 3초마다 스폰
        [SerializeField] private float laneHalfLength = 10f;
        [SerializeField] private float baseBehindDistance = 3f; // 스폰 지점보다 더 후방에 기지를 둔다

        // 임시 키 입력 생산 테스트용. 인덱스 0~3 = 탱커/원딜/근딜/암살자, 숫자키 1~4에 대응.
        [SerializeField] private HeroLoadout[] heroRoster = new HeroLoadout[4];

        private const HeroGrade TestProductionGrade = HeroGrade.Low; // UI 없어 하급으로 고정 생산

        private Simulation sim;
        private HeroProductionSystem heroProduction;
        private int lastLoggedTick;
        private bool gameOverLogged;

        // SimulationRenderer 등 View 측 컴포넌트가 시뮬 상태를 읽기 전용으로 참조하기 위한 통로.
        public Simulation Sim => sim;

        private void Start()
        {
            sim = new Simulation();
            heroProduction = new HeroProductionSystem();

            float baseX = laneHalfLength + baseBehindDistance;
            Unit baseP0 = BaseSpawner.SpawnBase(sim.Context, baseDefinition, ownerId: 0, posX: -baseX, posY: 0f);
            Unit baseP1 = BaseSpawner.SpawnBase(sim.Context, baseDefinition, ownerId: 1, posX: baseX, posY: 0f);

            sim.AddSystem(new SpawnSystem(footmanDefinition, ownerId: 0, spawnIntervalTicks, spawnPosX: -laneHalfLength, spawnPosY: 0f));
            sim.AddSystem(new SpawnSystem(footmanDefinition, ownerId: 1, spawnIntervalTicks, spawnPosX: laneHalfLength, spawnPosY: 0f));

            sim.AddSystem(new MovementSystem());
            sim.AddSystem(new CombatSystem());
            sim.AddSystem(new ResourceSystem());
            sim.AddSystem(new VictorySystem(baseP0, baseP1));

            // TODO(M4/UI): 지금은 UI가 없어 라인 A 업그레이드 확인용으로 P0가 400틱(20초) 이후
            // 골드가 모이는 첫 틱에 자동 구매하도록 예약해둔다(TechSystem이 골드 부족 시 매 틱 재시도).
            sim.AddSystem(new TechSystem(new (int tick, int ownerId)[] { (400, 0) }));
        }

        private void Update()
        {
            sim.Advance(Time.deltaTime);

            if (sim.Context.IsGameOver)
            {
                LogGameOverOnce();
                return;
            }

            HandleHeroProductionInput();

            if (sim.CurrentTick - lastLoggedTick >= Simulation.TickRate)
            {
                lastLoggedTick = sim.CurrentTick;
                LogSummary();
            }
        }

        private void LogGameOverOnce()
        {
            if (gameOverLogged)
            {
                return;
            }

            gameOverLogged = true;

            SimulationContext ctx = sim.Context;
            string result = ctx.WinnerId < 0 ? "무승부" : $"P{ctx.WinnerId} 승리";
            Debug.Log($"[Tick {ctx.CurrentTick}] === 게임 종료: {result} === 더 이상 진행되지 않습니다.");
        }

        // 임시 테스트 입력: UI가 아직 없어 숫자 키로 영웅 생산을 트리거한다.
        // 1/2/3/4 = P0가 탱커/원딜/근딜/암살자 생산, Shift+1/2/3/4 = P1이 동일 역할 생산.
        // 입력은 여기(View)에서만 읽고, Core에는 TryProduceHero 호출만 전달한다(Core는 UnityEngine.Input에 의존하지 않음).
        private void HandleHeroProductionInput()
        {
            if (Keyboard.current == null)
            {
                return;
            }

            bool isPlayer1 = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;

            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                RequestHeroProduction(rosterIndex: 0, isPlayer1);
            }

            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                RequestHeroProduction(rosterIndex: 1, isPlayer1);
            }

            if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                RequestHeroProduction(rosterIndex: 2, isPlayer1);
            }

            if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                RequestHeroProduction(rosterIndex: 3, isPlayer1);
            }
        }

        private void RequestHeroProduction(int rosterIndex, bool isPlayer1)
        {
            HeroLoadout loadout = rosterIndex < heroRoster.Length ? heroRoster[rosterIndex] : null;

            if (loadout == null || loadout.HeroDefinition == null || loadout.UnitDefinition == null)
            {
                Debug.LogWarning($"heroRoster[{rosterIndex}]가 인스펙터에 설정되지 않아 영웅 생산을 건너뜀");
                return;
            }

            int ownerId = isPlayer1 ? 1 : 0;
            float spawnPosX = isPlayer1 ? laneHalfLength : -laneHalfLength;

            bool produced = heroProduction.TryProduceHero(sim.Context, ownerId, loadout.HeroDefinition, loadout.UnitDefinition,
                TestProductionGrade, spawnPosX, spawnPosY: 0f);

            if (!produced)
            {
                Debug.LogWarning($"P{ownerId} 영웅 '{loadout.HeroDefinition.DisplayName}' 생산 실패(골드 부족) - 보유 {sim.Context.GetGold(ownerId)}, 필요 {loadout.HeroDefinition.SpawnCost}");
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
