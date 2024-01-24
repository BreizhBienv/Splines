using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spline))]
public class DrawSpline : Editor
{
    private const int lineSteps = 100;

    void OnSceneGUI()
    {
        Spline be = target as Spline;

        be.m_SplinePoints[0] = Handles.PositionHandle(be.m_SplinePoints[0], Quaternion.identity);
        be.m_SplinePoints[1] = Handles.PositionHandle(be.m_SplinePoints[1], Quaternion.identity);
        be.m_SplinePoints[2] = Handles.PositionHandle(be.m_SplinePoints[2], Quaternion.identity);
        be.m_SplinePoints[3] = Handles.PositionHandle(be.m_SplinePoints[3], Quaternion.identity);

        Handles.color = Color.red;
        Handles.SphereHandleCap(0, be.m_SplinePoints[0], Quaternion.identity, 1, EventType.Repaint);
        Handles.color = Color.yellow;
        Handles.SphereHandleCap(0, be.m_SplinePoints[1], Quaternion.identity, 1, EventType.Repaint);
        Handles.SphereHandleCap(0, be.m_SplinePoints[2], Quaternion.identity, 1, EventType.Repaint);
        Handles.color = Color.green;
        Handles.SphereHandleCap(0, be.m_SplinePoints[3], Quaternion.identity, 1, EventType.Repaint);

        // Visualize the tangent lines
        Handles.color = Color.gray;
        Handles.DrawDottedLine(be.m_SplinePoints[0], be.m_SplinePoints[1], 5);
        Handles.DrawDottedLine(be.m_SplinePoints[3], be.m_SplinePoints[2], 5);

        Handles.color = Color.red;
        Vector3 lineStart = be.GetPoint(0f);
        Vector3 lineFinish = be.GetPoint(1f);
        for (int i = 1; i <= lineSteps; i++)
        {
            Vector3 lineEnd = be.GetPoint(i / (float)lineSteps);
            Handles.DrawLine(lineStart, lineEnd, 2f);
            lineStart = lineEnd;
        }

        //Handles.DrawBezier(
        //    be.m_SplinePoints[0], be.m_SplinePoints[3],
        //    be.m_SplinePoints[1], be.m_SplinePoints[2],
        //    Color.red, null, 5f);
    }
}
