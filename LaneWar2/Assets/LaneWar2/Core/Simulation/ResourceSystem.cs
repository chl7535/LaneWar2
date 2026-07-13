namespace LaneWar2.Core.Simulation
{
    /// <summary>
    /// CombatSystem이 이번 틱에 기록한 처치(PendingKills)를 정산해 처치자 소유주에게 골드를 지급한다.
    /// 시스템 등록 순서상 Combat 바로 다음에 실행되어야 같은 틱 안에서 이벤트를 소비하고 비울 수 있다.
    /// </summary>
    public class ResourceSystem : ISimulationSystem
    {
        public const int GoldPerKill = 10;

        public void Tick(SimulationContext ctx)
        {
            for (int i = 0; i < ctx.PendingKills.Count; i++)
            {
                KillEvent kill = ctx.PendingKills[i];
                ctx.AddGold(kill.KillerOwnerId, GoldPerKill);
            }

            ctx.PendingKills.Clear();
        }
    }
}
