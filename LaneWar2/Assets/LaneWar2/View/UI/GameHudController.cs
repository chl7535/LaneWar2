using LaneWar2.Core.Data;
using LaneWar2.Core.Simulation;
using LaneWar2.Core.Tech;
using UnityEngine;
using UnityEngine.UI;

namespace LaneWar2.View.UI
{
    /// <summary>
    /// P0(플레이어) 전용 HUD. GameBootstrap.Sim을 읽기만 해서 화면을 갱신하고,
    /// 버튼 클릭은 GameBootstrap의 공개 API(RequestTechUpgrade/RequestHeroProduction)로만 전달한다.
    /// 골드 차감/유닛 생성 같은 실제 규칙은 전혀 갖지 않는다 — Core가 전담.
    /// </summary>
    public class GameHudController : MonoBehaviour
    {
        private const int OwnerId = 0; // 이 HUD는 항상 P0(사람 플레이어)를 조작한다.

        [SerializeField] private GameBootstrap gameBootstrap;

        [Header("상단 정보")]
        [SerializeField] private Text goldText;
        [SerializeField] private Text debugText;

        [Header("병영 업그레이드")]
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Text upgradeLabel;

        [Header("영웅 생산 (roster 인덱스 0~3 = 탱커/원딜/근딜/암살자)")]
        [SerializeField] private Button tankButton;
        [SerializeField] private Text tankLabel;
        [SerializeField] private Button rangedButton;
        [SerializeField] private Text rangedLabel;
        [SerializeField] private Button meleeButton;
        [SerializeField] private Text meleeLabel;
        [SerializeField] private Button assassinButton;
        [SerializeField] private Text assassinLabel;

        [Header("게임 종료 표시")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Text resultText;

        private void Start()
        {
            upgradeButton.onClick.AddListener(() => gameBootstrap.RequestTechUpgrade(OwnerId));
            tankButton.onClick.AddListener(() => gameBootstrap.RequestHeroProduction(0, OwnerId));
            rangedButton.onClick.AddListener(() => gameBootstrap.RequestHeroProduction(1, OwnerId));
            meleeButton.onClick.AddListener(() => gameBootstrap.RequestHeroProduction(2, OwnerId));
            assassinButton.onClick.AddListener(() => gameBootstrap.RequestHeroProduction(3, OwnerId));
        }

        private void Update()
        {
            if (gameBootstrap == null || gameBootstrap.Sim == null)
            {
                return;
            }

            SimulationContext ctx = gameBootstrap.Sim.Context;
            int gold = ctx.GetGold(OwnerId);

            goldText.text = $"골드: {gold}";
            debugText.text = $"상대 골드: {ctx.GetGold(1)} | 틱: {ctx.CurrentTick} | 유닛: {ctx.Units.Count}";

            UpdateUpgradeButton(ctx, gold);
            UpdateHeroButton(tankButton, tankLabel, 0, ctx, gold);
            UpdateHeroButton(rangedButton, rangedLabel, 1, ctx, gold);
            UpdateHeroButton(meleeButton, meleeLabel, 2, ctx, gold);
            UpdateHeroButton(assassinButton, assassinLabel, 3, ctx, gold);

            gameOverPanel.SetActive(ctx.IsGameOver);

            if (ctx.IsGameOver)
            {
                resultText.text = ctx.WinnerId == OwnerId ? "승리!" : ctx.WinnerId < 0 ? "무승부" : "패배";
                SetAllButtonsInteractable(false);
            }
        }

        private void UpdateUpgradeButton(SimulationContext ctx, int gold)
        {
            PlayerTechState state = ctx.GetTechState(OwnerId);

            if (state.BaseLevel >= TechSystem.MaxLevel)
            {
                upgradeLabel.text = "병영 최대 레벨";
                upgradeButton.interactable = false;
                return;
            }

            int cost = TechSystem.GetUpgradeCost(state.BaseLevel);
            upgradeLabel.text = $"병영 업그레이드 (비용 {cost})";
            upgradeButton.interactable = !ctx.IsGameOver && gold >= cost;
        }

        private void UpdateHeroButton(Button button, Text label, int rosterIndex, SimulationContext ctx, int gold)
        {
            HeroLoadout loadout = rosterIndex < gameBootstrap.HeroRoster.Length ? gameBootstrap.HeroRoster[rosterIndex] : null;
            HeroDefinition def = loadout != null ? loadout.HeroDefinition : null;

            if (def == null)
            {
                label.text = "(미설정)";
                button.interactable = false;
                return;
            }

            label.text = $"{def.DisplayName} 생산 ({def.SpawnCost})";
            button.interactable = !ctx.IsGameOver && gold >= def.SpawnCost;
        }

        private void SetAllButtonsInteractable(bool interactable)
        {
            upgradeButton.interactable = interactable;
            tankButton.interactable = interactable;
            rangedButton.interactable = interactable;
            meleeButton.interactable = interactable;
            assassinButton.interactable = interactable;
        }
    }
}
