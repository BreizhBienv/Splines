using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BézierSpline : MonoBehaviour
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
        if (index % 3 == 0)
        {
            Vector3 delta = point - m_Points[index];
            if (index > 0)
            {
                m_Points[index - 1] += delta;
            }
            if (index + 1 < m_Points.Count)
            {
                m_Points[index + 1] += delta;
            }
        }

        m_Points[index] = point;
        EnforceAligned(index);
    }

    private void Update()
    {
        if (m_Points.Count < 4)
            return;

        m_MovingGO.transform.position = GetBézierPoint();

        m_Tvalue += Time.deltaTime;

        if (m_Tvalue >= 1f)
        {
            m_Tvalue %= 1f;
            m_CurveCounter += 1;
        }
    }

    private Vector3 GetBézierPoint()
    {
        int bézierCounter = (m_CurveCounter * 3) % (ControlPointCount - 1);

        Vector3[] curve =
        {
            m_Points[bézierCounter],
            m_Points[bézierCounter + 1],
            m_Points[bézierCounter + 2],
            m_Points[bézierCounter + 3],
        };

        Vector3 pos = SplinesHelper.ComputeBézierCurve(m_Tvalue, curve);

        return pos;
    }

    public void AddCurve()
    {
        if (m_Points.Count == 0)
        {
            m_Points.Add(new(0, 0, 0));
            m_Points.Add(new(0, 0, 10));
            m_Points.Add(new(10, 0, 10));
            m_Points.Add(new(10, 0, 0));
            return;
        }

        Vector3 ctr = m_Points.Last();
        Vector3 tan = m_Points[ControlPointCount - 2];
        Vector3 tanDir = (tan - ctr) * -1;
        tan = ctr + tanDir;

        //add last ctr point a tan
        m_Points.Add(tan);

        ctr += (Vector3.right * 10);
        tan = ctr + tanDir;

        //add new tan & new ctr
        m_Points.Add(tan);
        m_Points.Add(ctr);
    }

    public void RemoveLastCurve()
    {
        if (ControlPointCount == 4)
        {
            m_Points.Clear();
            return;
        }

        m_Points.RemoveRange(ControlPointCount - 3, 3);
    }

    private void EnforceAligned(int index)
    {
        int mod = index % 3;

        //return if controle point or if at the start/end spline (no opposite tan)
        if (mod == 0 || index - 2 < 0 || index + 2 > ControlPointCount - 1)
            return;

        int fixedIndex = index;

        int middleIndex = 0, enforcedIndex = 0;
        if (mod == 1)
        {
            middleIndex = index - 1;
            enforcedIndex = index - 2;
        }
        else if (mod == 2)
        {
            middleIndex = index + 1;
            enforcedIndex = index + 2;
        }

        Vector3 middle = m_Points[middleIndex];
        Vector3 enforcedTangent = middle - m_Points[fixedIndex];
        enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, m_Points[fixedIndex]);
        m_Points[enforcedIndex] = middle + enforcedTangent;
    }
}
