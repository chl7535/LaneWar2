using LaneWar2.Core.Data;
using UnityEngine;

namespace LaneWar2.View
{
    /// <summary>
    /// HeroDefinition(체력/공격력/역할)과 그 영웅 전용 UnitDefinition(이동속도/사거리/공격주기) 한 쌍.
    /// HeroDefinition 자체는 UnitDefinition을 참조하지 않으므로(Core는 둘을 독립 데이터로 다룸),
    /// 이 페어링은 View(GameBootstrap)에서만 관리한다.
    /// </summary>
    [System.Serializable]
    public class HeroLoadout
    {
        [SerializeField] private HeroDefinition heroDefinition;
        [SerializeField] private UnitDefinition unitDefinition;

        public HeroDefinition HeroDefinition => heroDefinition;
        public UnitDefinition UnitDefinition => unitDefinition;
    }
}
