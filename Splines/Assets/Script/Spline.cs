using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Spline : MonoBehaviour
{
    [SerializeField] public List<Vector3> m_SplinePoints;
    [SerializeField] public ECurveType m_CurveType;

    [SerializeField] private GameObject m_MovingGO;

    private float m_Tvalue;
    private int m_CurveCounter = 0;

    private void Update()
    {
        m_MovingGO.transform.position = GetPointBh();

        m_Tvalue += Time.deltaTime;

        if (m_Tvalue >= 1f)
        {
            m_Tvalue %= 1f;
            m_CurveCounter += 1;
        }
    }

    private Vector3 GetPointBh()
    {
        Vector3 newPos = Vector3.zero;
        switch (m_CurveType)
        {
            default:
            case ECurveType.Bézier:
                newPos = GetBézierPoint();
                break;

            case ECurveType.Hermite:
                //newPos = SplinesHelper.ComputeHermite(t, pPoints);
                break;

            case ECurveType.B_Spline:
                //newPos = SplinesHelper.ComputeBSplineCurve(t, pPoints);
                break;

            case ECurveType.Catmull_Rom:
                //newPos = SplinesHelper.ComputeCatmullRomCurve(t, pPoints);
                break;
        }

        return newPos;
    }

    private Vector3 GetBézierPoint()
    {
        int bézierCounter = (m_CurveCounter * 3) % (m_SplinePoints.Count - 1);

        Vector3[] curve =
        {
            m_SplinePoints[bézierCounter],
            m_SplinePoints[bézierCounter + 1],
            m_SplinePoints[bézierCounter + 2],
            m_SplinePoints[bézierCounter + 3],
        };

        Vector3 pos = SplinesHelper.ComputeBézierCurve(m_Tvalue, curve);

        return pos;
    }

    private Vector3 GetBSplinePoint()
    {
        return Vector3.back;
    }

    public Vector3 GetPointEditor(float pT, Vector3[] pPoints)
    {
        Vector3 newPos = Vector3.zero;
        float t = Mathf.Clamp01(pT);

        switch (m_CurveType)
        {
            default:
            case ECurveType.Bézier:
                newPos = SplinesHelper.ComputeBézierCurve(t, pPoints);
                break;

            case ECurveType.Hermite:
                newPos = SplinesHelper.ComputeHermite(t, pPoints);
                break;

            case ECurveType.B_Spline:
                newPos = SplinesHelper.ComputeBSplineCurve(t, pPoints);
                break;

            case ECurveType.Catmull_Rom:
                newPos = SplinesHelper.ComputeCatmullRomCurve(t, pPoints);
                break;
        }

        return newPos;
    }

    public void AddCurve()
    {
        Vector3 lastTan = m_SplinePoints[m_SplinePoints.Count - 2];
        Vector3 lastControl = m_SplinePoints.Last();

        Vector3 entryTan = lastTan - lastControl;

        m_SplinePoints.Add(lastControl + entryTan * -1);

        m_SplinePoints.Add(lastControl + Vector3.forward);
        m_SplinePoints.Add(lastControl + Vector3.forward + Vector3.up);
    }
}
