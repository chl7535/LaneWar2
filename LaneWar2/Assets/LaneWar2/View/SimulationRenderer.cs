using System.Collections.Generic;
using LaneWar2.Core.Simulation;
using UnityEngine;

namespace LaneWar2.View
{
    /// <summary>
    /// Core 시뮬 상태(Simulation.Context.Units)를 읽기만 해서 화면 큐브와 동기화한다.
    /// Core를 절대 수정하지 않는다 — GameBootstrap.Sim을 통해 읽기 전용으로 접근.
    /// </summary>
    public class SimulationRenderer : MonoBehaviour
    {
        [SerializeField] private GameBootstrap gameBootstrap;
        [SerializeField] private float cubeScale = 0.8f;
        [SerializeField] private float heroCubeScaleMultiplier = 1.5f;

        private readonly Dictionary<int, UnitView> views = new Dictionary<int, UnitView>();
        private readonly HashSet<int> aliveIdsThisFrame = new HashSet<int>();
        private readonly List<int> staleIdsBuffer = new List<int>();

        private void LateUpdate()
        {
            Simulation sim = gameBootstrap != null ? gameBootstrap.Sim : null;

            if (sim == null)
            {
                return;
            }

            SimulationContext ctx = sim.Context;
            aliveIdsThisFrame.Clear();

            for (int i = 0; i < ctx.Units.Count; i++)
            {
                var unit = ctx.Units[i];

                if (!unit.IsAlive)
                {
                    continue;
                }

                aliveIdsThisFrame.Add(unit.Id);

                if (!views.TryGetValue(unit.Id, out UnitView view))
                {
                    view = CreateUnitView(unit.Id, unit.OwnerId, unit.IsHero);
                    views[unit.Id] = view;
                }

                view.SyncPosition(unit.PosX, unit.PosY);
            }

            RemoveStaleViews();
        }

        private UnitView CreateUnitView(int unitId, int ownerId, bool isHero)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = $"UnitView_{unitId}";
            cube.transform.SetParent(transform);
            cube.transform.localScale = Vector3.one * cubeScale * (isHero ? heroCubeScaleMultiplier : 1f);

            UnitView view = cube.AddComponent<UnitView>();
            view.Init(unitId, ownerId, isHero);
            return view;
        }

        private void RemoveStaleViews()
        {
            staleIdsBuffer.Clear();

            foreach (KeyValuePair<int, UnitView> kvp in views)
            {
                if (!aliveIdsThisFrame.Contains(kvp.Key))
                {
                    staleIdsBuffer.Add(kvp.Key);
                }
            }

            for (int i = 0; i < staleIdsBuffer.Count; i++)
            {
                int staleId = staleIdsBuffer[i];
                Destroy(views[staleId].gameObject);
                views.Remove(staleId);
            }
        }
    }
}
