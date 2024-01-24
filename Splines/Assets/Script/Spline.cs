using System;
using UnityEngine;

public class Spline : MonoBehaviour
{
    [SerializeField] public Vector3[] m_SplinePoints;
    [SerializeField] public ECurveType m_CurveType;

    [SerializeField] private GameObject m_MovingGO;

    private float m_Tvalue;

    private void Update()
    {
        m_MovingGO.transform.position = GetPoint(m_Tvalue);

        m_Tvalue = (m_Tvalue + Time.deltaTime) % 1f;
    }

    public Vector3 GetPoint(float pT)
    {
        Vector3 newPos = Vector3.zero;
        float t = Mathf.Clamp01(pT);

        switch (m_CurveType)
        {
            default:
            case ECurveType.Bézier:
                newPos = SplinesHelper.ComputeBézierCurve(t, m_SplinePoints);
                break;

            case ECurveType.Hermite:
                newPos = SplinesHelper.ComputeHermite(t, m_SplinePoints);
                break;

            case ECurveType.B_Spline:
                newPos = SplinesHelper.ComputeBSplineCurve(t, m_SplinePoints);
                break;

            case ECurveType.Catmull_Rom:
                newPos = SplinesHelper.ComputeCatmullRomCurve(t, m_SplinePoints);
                break;
        }

        return newPos;
    }
}
