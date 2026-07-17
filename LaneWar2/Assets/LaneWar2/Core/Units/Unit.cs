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

        // 스폰 시점에 원본 def + 강화 보너스를 합산해 확정한 값. def 자체는 건드리지 않는다.
        public readonly int EffectiveMaxHp;
        public readonly int EffectiveAttackDamage;

        // 영웅 표식. 풋맨은 IsHero=false로 기존과 동일하게 동작한다.
        public readonly bool IsHero;
        public readonly HeroGrade HeroGrade;
        public readonly string HeroId;

        // 기지 표식. 이동/공격 정지는 UnitDefinition(moveSpeed=0, attackDamage=0)으로 처리하고,
        // 이 플래그는 승패 판정/렌더링 구분용 식별자로만 쓴다.
        public readonly bool IsBase;

        public int CurrentHp;
        public float PosX;
        public float PosY;
        public float AttackCooldown;

        public bool IsAlive => CurrentHp > 0;

        public Unit(int id, int ownerId, UnitDefinition def, float posX, float posY, int effectiveMaxHp, int effectiveAttackDamage,
            bool isHero = false, HeroGrade heroGrade = HeroGrade.Low, string heroId = null, bool isBase = false)
        {
            Id = id;
            OwnerId = ownerId;
            Def = def;
            EffectiveMaxHp = effectiveMaxHp;
            EffectiveAttackDamage = effectiveAttackDamage;
            IsHero = isHero;
            HeroGrade = heroGrade;
            HeroId = heroId;
            IsBase = isBase;
            CurrentHp = effectiveMaxHp;
            PosX = posX;
            PosY = posY;
        }
    }
}
