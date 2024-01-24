using System;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HermitianSpline))]
public class DrawHermitianSpline : Editor
{
    private HermitianSpline HS;
    Transform HandleTransform;
    Quaternion HandleRot;

    private const int lineSteps = 100;

    private const float HandleSize = 0.04f;
    private const float PickSize = 0.06f;

    private int SelectedIndex = -1;

    void OnSceneGUI()
    {
        HS = target as HermitianSpline;
        if (HS == null)
            return;

        HandleTransform = HS.transform;
        HandleRot       = HS.transform.rotation;

        DisplayPoints();

        if (HS.ControlPointCount < 4)
            return;

        DisplayHermite();
    }

    private void DisplayPoints()
    {
        for (int i = 0; i < HS.ControlPointCount - 1; i += 2)
        {
            int ctrId = i;
            int tanId = i + 1;

            Vector3 p0 = ShowPoint(ctrId);
            Vector3 p1 = ShowPoint(tanId);

            Handles.color = Color.gray;
            Handles.DrawDottedLine(p0, p1, 5);
        }
    }

    private Vector3 ShowPoint(int index)
    {
        Vector3 point = HandleTransform.TransformPoint(HS.GetControlPoint(index));

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
            Undo.RecordObject(HS, "Move Point");
            EditorUtility.SetDirty(HS);
            HS.SetControlPoint(index, HandleTransform.InverseTransformPoint(point));
        }

        return point;
    }

    private void DisplayHermite()
    {
        Handles.color = Color.red;
        for (int i = 0; i < HS.ControlPointCount - 3; i += 2)
        {
            Vector3[] curve =
            {
                HandleTransform.TransformPoint(HS.GetControlPoint(i)),
                HandleTransform.TransformPoint(HS.GetControlPoint(i + 1)),
                HandleTransform.TransformPoint(HS.GetControlPoint(i + 2)),
                HandleTransform.TransformPoint(HS.GetControlPoint(i + 3)),
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

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HermitianSpline spl = target as HermitianSpline;
        
        if (SelectedIndex >= 0 && SelectedIndex < HS.ControlPointCount)
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
        Vector3 point = EditorGUILayout.Vector3Field("Position", HS.GetControlPoint(SelectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(HS, "Move Point");
            EditorUtility.SetDirty(HS);
            HS.SetControlPoint(SelectedIndex, point);
        }
    }
}
