using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(BSpline))]
public class DrawBSpline : Editor
{
    private BSpline Spline;
    Transform HandleTransform;
    Quaternion HandleRot;

    private const int lineSteps = 100;

    private const float HandleSize = 0.04f;
    private const float PickSize = 0.06f;

    private int SelectedIndex = -1;

    void OnSceneGUI()
    {
        Spline = target as BSpline;
        if (Spline == null)
            return;

        HandleTransform = Spline.transform;
        HandleRot = Spline.transform.rotation;

        if (Spline.ControlPointCount < 4)
            return;

        DisplayPoints();
        DiplayBSpline();
    }

    private void DisplayPoints()
    {
        Vector3 p0 = ShowPoint(0);
        for (int i = 1; i <= Spline.ControlPointCount - 1; ++i)
        {
            int ctrId = i;

            Vector3 p1 = ShowPoint(ctrId);

            Handles.color = Color.gray;
            Handles.DrawDottedLine(p0, p1, 5);
            p0 = p1;
        }
    }

    private Vector3 ShowPoint(int index)
    {
        Vector3 point = HandleTransform.TransformPoint(Spline.GetControlPoint(index));

        float size = HandleUtility.GetHandleSize(point);
        Handles.color = Color.white;
        if (Handles.Button(point, HandleRot, size * HandleSize, size * PickSize, Handles.DotHandleCap))
        {
            SelectedIndex = index;
            Repaint();
        }

        if (SelectedIndex != index)
            return point;

        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, HandleRot);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(Spline, "Move Point");
            EditorUtility.SetDirty(Spline);
            Spline.SetControlPoint(index, HandleTransform.InverseTransformPoint(point));
        }

        return point;
    }

    private void DiplayBSpline()
    {
        Handles.color = Color.red;
        for (int i = 0; i < Spline.ControlPointCount - 3; ++i)
        {
            Vector3[] curve =
            {
                   HandleTransform.TransformPoint(Spline.GetControlPoint(i)),
                   HandleTransform.TransformPoint(Spline.GetControlPoint(i + 1)),
                   HandleTransform.TransformPoint(Spline.GetControlPoint(i + 2)),
                   HandleTransform.TransformPoint(Spline.GetControlPoint(i + 3)),
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
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BSpline spl = target as BSpline;

        if (SelectedIndex >= 0 && SelectedIndex < Spline.ControlPointCount)
            DrawSelectedPointInspector();


        //Add button
        if (GUILayout.Button("Add Curve"))
        {
            Undo.RecordObject(spl, "Add Curve");
            spl.AddCurve();
            EditorUtility.SetDirty(spl);
        }

        //Remove button
        if (GUILayout.Button("Remove Last Curve"))
        {
            Undo.RecordObject(spl, "Remove Last Curve");
            spl.RemoveLastCurve();
            EditorUtility.SetDirty(spl);
        }
    }

    private void DrawSelectedPointInspector()
    {
        GUILayout.Label("Selected Point");
        EditorGUI.BeginChangeCheck();
        Vector3 point = EditorGUILayout.Vector3Field("Position", Spline.GetControlPoint(SelectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(Spline, "Move Point");
            EditorUtility.SetDirty(Spline);
            Spline.SetControlPoint(SelectedIndex, point);
        }
    }
}
