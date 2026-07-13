using UnityEngine;

namespace LaneWar2.View
{
    /// <summary>
    /// 시뮬 유닛 하나에 대응하는 화면상의 큐브. Core 상태를 읽어 자기 자신(transform, color)만 갱신한다.
    /// </summary>
    public class UnitView : MonoBehaviour
    {
        private static readonly Color[] OwnerColors = { Color.blue, Color.red };
        private const float HeroBrightenAmount = 0.35f; // 영웅은 오너 색을 흰색 쪽으로 밝게 블렌드해 구분

        public int UnitId { get; private set; }
        public bool IsHero { get; private set; }

        private Renderer cachedRenderer;

        public void Init(int unitId, int ownerId, bool isHero)
        {
            UnitId = unitId;
            IsHero = isHero;
            cachedRenderer = GetComponent<Renderer>();
            SetOwnerColor(ownerId);
        }

        public void SetOwnerColor(int ownerId)
        {
            Color baseColor = ownerId >= 0 && ownerId < OwnerColors.Length ? OwnerColors[ownerId] : Color.gray;
            cachedRenderer.material.color = IsHero ? Color.Lerp(baseColor, Color.white, HeroBrightenAmount) : baseColor;
        }

        public void SyncPosition(float posX, float posY)
        {
            // 큐브 바닥이 y=0에 닿도록 스케일의 절반을 y로 사용(스케일이 다른 영웅/풋맨 모두 대응).
            transform.position = new Vector3(posX, transform.localScale.y * 0.5f, posY);
        }
    }
}
