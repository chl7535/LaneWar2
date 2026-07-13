using System.Collections.Generic;
using LaneWar2.Core.Units;

namespace LaneWar2.Core.Simulation
{
    public class SimulationContext
    {
        public int CurrentTick;
        public float DeltaTime; // 한 틱의 고정 시간(FixedDeltaTime)

        public List<Unit> Units = new List<Unit>();
        public int NextUnitId;

        // ownerId -> 보유 골드. 미등록 ownerId는 0으로 취급(지연 초기화).
        public Dictionary<int, int> Gold = new Dictionary<int, int>();

        // CombatSystem이 이번 틱에 발생한 처치를 기록하면 ResourceSystem이 같은 틱 안에서 읽고 비운다.
        public List<KillEvent> PendingKills = new List<KillEvent>();

        public int GetGold(int ownerId)
        {
            return Gold.TryGetValue(ownerId, out int amount) ? amount : 0;
        }

        public void AddGold(int ownerId, int amount)
        {
            Gold[ownerId] = GetGold(ownerId) + amount;
        }
    }

    public readonly struct KillEvent
    {
        public readonly int KillerOwnerId;
        public readonly int VictimId;

        public KillEvent(int killerOwnerId, int victimId)
        {
            KillerOwnerId = killerOwnerId;
            VictimId = victimId;
        }
    }
}
