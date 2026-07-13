namespace LaneWar2.Core.Tech
{
    /// <summary>
    /// 플레이어 1인의 라인 A(병영 강화) 상태. 순수 데이터 홀더이며 비용 계산이나 규칙은 TechSystem이 담당한다.
    /// </summary>
    public class PlayerTechState
    {
        public int BaseLevel = 1;
        public int BonusAttack;
        public int BonusHp;
        public float SpawnIntervalMultiplier = 1f;

        public void ApplyUpgrade(int attackDelta, int hpDelta, float newSpawnIntervalMultiplier)
        {
            BaseLevel++;
            BonusAttack += attackDelta;
            BonusHp += hpDelta;
            SpawnIntervalMultiplier = newSpawnIntervalMultiplier;
        }
    }
}
