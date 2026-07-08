namespace LaneWar2.Core.Simulation
{
    public class SimulationContext
    {
        public int CurrentTick;
        public float DeltaTime; // 한 틱의 고정 시간(FixedDeltaTime)

        // TODO: 이후 유닛 목록, 플레이어 상태 등 시뮬레이션 전역 상태가 여기 추가될 예정
    }
}
