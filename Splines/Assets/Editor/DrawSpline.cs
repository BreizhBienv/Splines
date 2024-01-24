using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spline))]
public class DrawSpline : Editor
{
    private const int lineSteps = 100;

    private Spline be;

    void OnSceneGUI()
    {
        be = target as Spline;

        switch (be.m_CurveType)
        {
            default:
            case ECurveType.Bézier:
                DisplayBézier();
                break;

            case ECurveType.Hermite:
                DisplayHermite();
                break;

            case ECurveType.B_Spline:
                DiplayBSpline();
                break;

            case ECurveType.Catmull_Rom:
                DisplayCatmullRom();
                break;
        }
    }

    #region DiplaySpline
    private void DisplayHermite()
    {
        DisplayPointsAndTan();

        Handles.color = Color.red;
        for (int i = 0; i < be.m_SplinePoints.Count - 1; i += 3)
        {
            Vector3[] curve =
            {
                    be.m_SplinePoints[i],
                    be.m_SplinePoints[i + 1],
                    be.m_SplinePoints[i + 2],
                    be.m_SplinePoints[i + 3],
            };

            Vector3 lineStart = SplinesHelper.ComputeHermite(0f, curve);
            for (int j = 1; j <= lineSteps; ++j)
            {
                Vector3 lineEnd = SplinesHelper.ComputeHermite(j / (float)lineSteps, curve);
                Handles.DrawLine(lineStart, lineEnd, 2f);
                lineStart = lineEnd;
            }
        }
    }

    private void DisplayBézier()
    {
        DisplayPointsAndTan();

        Handles.color = Color.red;
        for (int i = 0; i < be.m_SplinePoints.Count - 1; i += 3)
        {
            Vector3[] curve =
            {
                    be.m_SplinePoints[i],
                    be.m_SplinePoints[i + 1],
                    be.m_SplinePoints[i + 2],
                    be.m_SplinePoints[i + 3],
            };

            Vector3 lineStart = SplinesHelper.ComputeBézierCurve(0f, curve);
            for (int j = 1; j <= lineSteps; ++j)
            {
                Vector3 lineEnd = SplinesHelper.ComputeBézierCurve(j / (float)lineSteps, curve);
                Handles.DrawLine(lineStart, lineEnd, 2f);
                lineStart = lineEnd;
            }
        }
    }

    private void DiplayBSpline()
    {
        Handles.color = Color.red;
        for (int i = 0; i < be.m_SplinePoints.Count - 3; ++i)
        {
            Vector3[] curve =
            {
                    be.m_SplinePoints[i],
                    be.m_SplinePoints[i + 1],
                    be.m_SplinePoints[i + 2],
                    be.m_SplinePoints[i + 3],
            };

            Vector3 lineStart = SplinesHelper.ComputeBSplineCurve(0f, curve);
            for (int j = 1; j <= lineSteps; ++j)
            {
                Vector3 lineEnd = SplinesHelper.ComputeBSplineCurve(j / (float)lineSteps, curve);
                Handles.DrawLine(lineStart, lineEnd, 2f);
                lineStart = lineEnd;
            }
        }
    }

    private void DisplayCatmullRom()
    {
        Handles.color = Color.red;
        for (int i = 0; i < be.m_SplinePoints.Count - 3; ++i)
        {
            Vector3[] curve =
            {
                    be.m_SplinePoints[i],
                    be.m_SplinePoints[i + 1],
                    be.m_SplinePoints[i + 2],
                    be.m_SplinePoints[i + 3],
            };

            Vector3 lineStart = SplinesHelper.ComputeCatmullRomCurve(0f, curve);
            for (int j = 1; j <= lineSteps; ++j)
            {
                Vector3 lineEnd = SplinesHelper.ComputeCatmullRomCurve(j / (float)lineSteps, curve);
                Handles.DrawLine(lineStart, lineEnd, 2f);
                lineStart = lineEnd;
            }
        }
    }
    #endregion

    #region DisplayHandlePoints&Tan
    private void DisplayPointsAndTan()
    {
        for (int i = 0; i < be.m_SplinePoints.Count; i += 3)
        {
            int prevTanId = i - 1;
            int ctrId = i;
            int nextTanId = i + 1;

            prevTanId = Mathf.Clamp(prevTanId, 0, be.m_SplinePoints.Count - 1);
            nextTanId = Mathf.Clamp(nextTanId, 0, be.m_SplinePoints.Count - 1);

            Handles.color = Color.green;
            be.m_SplinePoints[ctrId] = Handles.PositionHandle(be.m_SplinePoints[ctrId], Quaternion.identity);
            Handles.SphereHandleCap(0, be.m_SplinePoints[ctrId], Quaternion.identity, 1, EventType.Repaint);

            if (prevTanId != ctrId)
            {
                be.m_SplinePoints[prevTanId] =
                    Handles.PositionHandle(be.m_SplinePoints[prevTanId], Quaternion.identity);

                Handles.color = Color.yellow;
                Handles.SphereHandleCap(0, be.m_SplinePoints[prevTanId], Quaternion.identity, 1, EventType.Repaint);

                Handles.color = Color.gray;
                Handles.DrawDottedLine(be.m_SplinePoints[ctrId], be.m_SplinePoints[prevTanId], 5);
            }

            if (nextTanId != ctrId)
            {
                be.m_SplinePoints[nextTanId] =
                    Handles.PositionHandle(be.m_SplinePoints[nextTanId], Quaternion.identity);

                Handles.color = Color.yellow;
                Handles.SphereHandleCap(0, be.m_SplinePoints[nextTanId], Quaternion.identity, 1, EventType.Repaint);

                Handles.color = Color.gray;
                Handles.DrawDottedLine(be.m_SplinePoints[ctrId], be.m_SplinePoints[nextTanId], 5);
            }
        }
    }
    #endregion

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Spline spl = target as Spline;
        if (GUILayout.Button("Add Curve"))
        {
            Undo.RecordObject(spl, "Add Curve");
            spl.AddCurve();
            EditorUtility.SetDirty(spl);
        }
    }
}
