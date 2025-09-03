// UISquircle – selectable corners, anchor/pivot aware (fixed bottom-left)
// Credit: Soprachev Andrei (base) + patch

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Primitives/Squircle Fix")]
    public class UISquircleFix : UIPrimitiveBase
    {
        public enum ShapeType { Classic, Scaled }

        [System.Flags]
        public enum CornerMask
        {
            None        = 0,
            TopLeft     = 1 << 0,
            TopRight    = 1 << 1,
            BottomRight = 1 << 2,
            BottomLeft  = 1 << 3,
        }

        [Header("Shape")]
        public ShapeType shapeType = ShapeType.Scaled;
        [Range(1f, 40f)] public float n = 4f;     // curvature
        [Min(0.5f)]      public float delta = 6f; // sampling (px)
        [Min(0f)]        public float radius = 64f;
        public CornerMask corners = CornerMask.TopLeft | CornerMask.TopRight; // 기본: 윗면만

        readonly List<Vector2> edge = new();

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            edge.Clear();

            Rect r = rectTransform.rect;               // pivot/anchor 반영
            float x0 = r.xMin, x1 = r.xMax;
            float y0 = r.yMin, y1 = r.yMax;
            float w  = r.width, h  = r.height;

            float rx = Mathf.Min(radius, w * 0.5f);
            float ry = Mathf.Min(radius, h * 0.5f);
            if (shapeType == ShapeType.Scaled)
            {
                float rr = Mathf.Min(rx, ry);
                rx = ry = rr;
            }

            int steps = Mathf.Max(2, Mathf.CeilToInt(Mathf.Max(rx, ry) / Mathf.Max(1f, delta)));

            // --- 경계(시계방향) 구성: 시작점을 BL 또는 BL+rx 로 잡아 폐곡선 생성
            // bottom: start
            edge.Add(new Vector2( Has(corners, CornerMask.BottomLeft) ? x0 + rx : x0, y0));
            // bottom edge to BR start
            edge.Add(new Vector2( Has(corners, CornerMask.BottomRight) ? x1 - rx : x1, y0));
            // BR arc
            if (Has(corners, CornerMask.BottomRight) && rx > 0 && ry > 0)
                AddArc(edge, new Vector2(x1 - rx, y0 + ry), rx, ry, 270f, 360f, steps, n);

            // right edge to TR start
            edge.Add(new Vector2(x1, Has(corners, CornerMask.TopRight) ? y1 - ry : y1));
            // TR arc
            if (Has(corners, CornerMask.TopRight) && rx > 0 && ry > 0)
                AddArc(edge, new Vector2(x1 - rx, y1 - ry), rx, ry, 0f, 90f, steps, n);

            // top edge to TL start
            edge.Add(new Vector2( Has(corners, CornerMask.TopLeft) ? x0 + rx : x0, y1));
            // TL arc
            if (Has(corners, CornerMask.TopLeft) && rx > 0 && ry > 0)
                AddArc(edge, new Vector2(x0 + rx, y1 - ry), rx, ry, 90f, 180f, steps, n);

            // left edge to BL start
            edge.Add(new Vector2(x0, Has(corners, CornerMask.BottomLeft) ? y0 + ry : y0));
            // BL arc (이게 이전 버전에서 안 보이던 부분)
            if (Has(corners, CornerMask.BottomLeft) && rx > 0 && ry > 0)
                AddArc(edge, new Vector2(x0 + rx, y0 + ry), rx, ry, 180f, 270f, steps, n);
            // 이제 마지막 점이 시작점(rounded면 x0+rx,y0 / 아니면 x0,y0)과 일치함 → 폐곡선 완성

            // --- 중앙 팬으로 채우기 (기준점을 BL가 아니라 '센터'로)
            Vector2 center = new((x0 + x1) * 0.5f, (y0 + y1) * 0.5f);
            int centerIdx = 0;
            vh.AddVert(center, color, Vector2.zero);

            // 경계 버텍스 추가
            for (int i = 0; i < edge.Count; i++)
                vh.AddVert(edge[i], color, Vector2.zero);

            // 삼각형(센터, i, i+1) – 마지막은 처음과 연결
            for (int i = 0; i < edge.Count; i++)
            {
                int a = centerIdx;
                int b = 1 + i;
                int c = 1 + ((i + 1) % edge.Count);
                vh.AddTriangle(a, b, c);
            }
        }

        static void AddArc(List<Vector2> list, Vector2 center, float a, float b,
                           float degStart, float degEnd, int steps, float n)
        {
            for (int i = 0; i <= steps; i++)
            {
                float t  = Mathf.Lerp(degStart, degEnd, i / (float)steps) * Mathf.Deg2Rad;
                float cs = Mathf.Cos(t), sn = Mathf.Sin(t);
                float sx = Mathf.Sign(cs) * Mathf.Pow(Mathf.Abs(cs), 2f / n);
                float sy = Mathf.Sign(sn) * Mathf.Pow(Mathf.Abs(sn), 2f / n);
                list.Add(new Vector2(center.x + sx * a, center.y + sy * b));
            }
        }

        static bool Has(CornerMask m, CornerMask f) => (m & f) != 0;

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetVerticesDirty();
        }

    #if UNITY_EDITOR
        [CustomEditor(typeof(UISquircle))]
        class UISquircleEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                EditorGUILayout.HelpBox("센터 팬으로 그리므로 BL 라운딩도 정상 동작.\n" +
                                        "Corners는 4개 토글만 제공. Nothing/Everything은 Unity 기본.", MessageType.None);
            }
        }
    #endif
    }
}
