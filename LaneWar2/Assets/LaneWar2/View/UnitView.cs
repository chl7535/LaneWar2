using UnityEngine;

namespace LaneWar2.View
{
    /// <summary>
    /// 시뮬 유닛 하나에 대응하는 화면상의 큐브. Core 상태를 읽어 자기 자신(transform, color)만 갱신한다.
    /// </summary>
    public class UnitView : MonoBehaviour
    {
        private static readonly Color[] OwnerColors = { Color.blue, Color.red };

        public int UnitId { get; private set; }

        private Renderer cachedRenderer;

        public void Init(int unitId, int ownerId)
        {
            UnitId = unitId;
            cachedRenderer = GetComponent<Renderer>();
            SetOwnerColor(ownerId);
        }

        public void SetOwnerColor(int ownerId)
        {
            Color color = ownerId >= 0 && ownerId < OwnerColors.Length ? OwnerColors[ownerId] : Color.gray;
            cachedRenderer.material.color = color;
        }

        public void SyncPosition(float posX, float posY)
        {
            transform.position = new Vector3(posX, 0.5f, posY);
        }
    }
}
