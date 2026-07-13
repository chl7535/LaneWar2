using LaneWar2.Core.Data;

namespace LaneWar2.Core.Units
{
    /// <summary>
    /// 전장에 존재하는 살아있는 유닛 한 기. 데이터 컨테이너이며 전투/이동 로직은 갖지 않는다.
    /// </summary>
    public class Unit
    {
        public readonly int Id;
        public readonly int OwnerId;
        public readonly UnitDefinition Def;

        public int CurrentHp;
        public float PosX;
        public float PosY;
        public float AttackCooldown;

        public bool IsAlive => CurrentHp > 0;

        public Unit(int id, int ownerId, UnitDefinition def, float posX, float posY)
        {
            Id = id;
            OwnerId = ownerId;
            Def = def;
            CurrentHp = def.MaxHp;
            PosX = posX;
            PosY = posY;
        }
    }
}
