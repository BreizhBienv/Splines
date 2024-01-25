using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BézierSpline))]
public class DrawBézierSpline : Editor
{
    private BézierSpline Spline;
    Transform HandleTransform;
    Quaternion HandleRot;

    private const int lineSteps = 100;

    private const float HandleSize = 0.04f;
    private const float PickSize = 0.06f;

    private int SelectedIndex = -1;

    void OnSceneGUI()
    {
        Spline = target as BézierSpline;
        if (Spline == null)
            return;

        HandleTransform = Spline.transform;
        HandleRot = Spline.transform.rotation;

        DisplayPoints();

        if (Spline.ControlPointCount < 4)
            return;

        DisplayBézier();
    }

    private void DisplayPoints()
    {
        for (int i = 0; i <= Spline.ControlPointCount - 1; i += 3)
        {
            int ctrId = i;
            int prevTanID = i - 1;
            int nextTanId = i + 1;

            Vector3 p0 = ShowPoint(ctrId);

            if (prevTanID >= 0)
            {
                Vector3 t0 = ShowPoint(prevTanID);
                Handles.color = Color.gray;
                Handles.DrawDottedLine(p0, t0, 5);
            }

            if (nextTanId <= Spline.ControlPointCount - 1)
            {
                Vector3 t1 = ShowPoint(nextTanId);
                Handles.color = Color.gray;
                Handles.DrawDottedLine(p0, t1, 5);
            }
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

    private void DisplayBézier()
    {
        Handles.color = Color.red;
        for (int i = 0; i < Spline.ControlPointCount - 3; i += 3)
        {
            Vector3[] curve =
            {
                    Spline.GetControlPoint(i),
                    Spline.GetControlPoint(i + 1),
                    Spline.GetControlPoint(i + 2),
                    Spline.GetControlPoint(i + 3),
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
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BézierSpline spl = target as BézierSpline;

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
