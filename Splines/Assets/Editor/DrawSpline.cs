using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spline))]
public class DrawSpline : Editor
{
    void OnSceneGUI()
    {
        Spline be = target as Spline;

        be.m_SplinePoints[0] = Handles.PositionHandle(be.m_SplinePoints[0], Quaternion.identity);
        be.m_SplinePoints[1] = Handles.PositionHandle(be.m_SplinePoints[1], Quaternion.identity);
        be.m_SplinePoints[2] = Handles.PositionHandle(be.m_SplinePoints[2], Quaternion.identity);
        be.m_SplinePoints[3] = Handles.PositionHandle(be.m_SplinePoints[3], Quaternion.identity);

        // Visualize the tangent lines
        Handles.DrawDottedLine(be.m_SplinePoints[0], be.m_SplinePoints[1], 5);
        Handles.DrawDottedLine(be.m_SplinePoints[3], be.m_SplinePoints[2], 5);

        Handles.DrawBezier(
            be.m_SplinePoints[0], be.m_SplinePoints[3],
            be.m_SplinePoints[1], be.m_SplinePoints[2],
            Color.red, null, 5f);
    }
}
