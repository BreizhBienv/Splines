using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatmullRomSpline : MonoBehaviour
{
    [SerializeField] private GameObject m_MovingGO;

    [SerializeField] private List<Vector3> m_Points;

    private float m_Tvalue;
    private int m_CurveCounter = 0;

    public int ControlPointCount
    {
        get
        {
            return m_Points.Count;
        }
    }

    public Vector3 GetControlPoint(int index)
    {
        return m_Points[index];
    }

    public void SetControlPoint(int index, Vector3 point)
    {
        m_Points[index] = point;
    }

    private void Update()
    {
        if (m_Points.Count < 4)
            return;

        m_MovingGO.transform.position = GetCatmullRomPoint();

        m_Tvalue += Time.deltaTime;

        if (m_Tvalue >= 1f)
        {
            m_Tvalue %= 1f;
            m_CurveCounter += 1;
        }
    }

    private Vector3 GetCatmullRomPoint()
    {
        int bSplineCounter = m_CurveCounter % (ControlPointCount - 3);

        Vector3[] curve =
        {
            m_Points[bSplineCounter],
            m_Points[bSplineCounter + 1],
            m_Points[bSplineCounter + 2],
            m_Points[bSplineCounter + 3],
        };

        Vector3 pos = SplinesHelper.ComputeCatmullRomCurve(m_Tvalue, curve);

        return pos;
    }

    public void AddCurve()
    {
        if (ControlPointCount == 0)
        {
            m_Points.Add(new(0, 0, 0));
            m_Points.Add(new(10, 0, 10));
            m_Points.Add(new(20, 0, 0));
            m_Points.Add(new(30, 0, 10));
            return;
        }

        Vector3 ctr2 = m_Points[ControlPointCount - 2];
        Vector3 lastCtr = m_Points[ControlPointCount - 1];

        ctr2 += (Vector3.right * 20);
        lastCtr += (Vector3.right * 20);

        m_Points.Add(ctr2);
        m_Points.Add(lastCtr);
    }

    public void RemoveLastCurve()
    {
        if (ControlPointCount == 0)
            return;

        if (ControlPointCount == 4)
        {
            m_Points.Clear();
            return;
        }

        m_Points.RemoveAt(m_Points.Count - 1);
        m_Points.RemoveAt(m_Points.Count - 1);
    }
}
