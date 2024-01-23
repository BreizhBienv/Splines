using System;
using UnityEngine;

public class Spline : MonoBehaviour
{
    [SerializeField] private GameObject[] m_SplinePoints;
    [SerializeField] private ECurveType m_CurveType;

    [SerializeField] private GameObject m_MovingGO;
    
    private float m_Tvalue;

    private void Update()
    {
        m_Tvalue = (m_Tvalue + Time.deltaTime) % 1f;
        Vector3 newPos = Vector3.zero;

        switch (m_CurveType)
        {
            default:
            case ECurveType.Bézier:
                newPos = SplinesHelper.ComputeBézierCurve(m_Tvalue, m_SplinePoints);
                break;

            case ECurveType.Hermite:
                newPos = SplinesHelper.ComputeHermite(m_Tvalue, m_SplinePoints);
                break;

            case ECurveType.B_Spline:
                newPos = SplinesHelper.ComputeBSplineCurve(m_Tvalue, m_SplinePoints);
                break;

            case ECurveType.Catmull_Rom:
                newPos = SplinesHelper.ComputeCatmullRomCurve(m_Tvalue, m_SplinePoints);
                break;
        }

        m_MovingGO.transform.position = newPos;
    }
}
