using System.Collections.Generic;
using UnityEngine;

public class HermitianSpline : MonoBehaviour
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
        if (index % 2 == 0)
        {
            Vector3 delta = point - m_Points[index];
            m_Points[index + 1] += delta;
        }

        m_Points[index] = point;
    }

    private void Update()
    {
        if (m_Points.Count < 4)
            return;

        m_MovingGO.transform.position = GetHermitePoint();

        m_Tvalue += Time.deltaTime;

        if (m_Tvalue >= 1f)
        {
            m_Tvalue %= 1f;
            m_CurveCounter += 1;
        }
    }

    private Vector3 GetHermitePoint()
    {
        int counter = (m_CurveCounter * 2) % (m_Points.Count - 2);

        Vector3[] curve =
        {
            transform.TransformPoint(m_Points[counter]),
            transform.TransformPoint(m_Points[counter + 1]),
            transform.TransformPoint(m_Points[counter + 2]),
            transform.TransformPoint(m_Points[counter + 3]),
        };

        Vector3 pos = SplinesHelper.ComputeHermite(m_Tvalue, curve);

        return pos;
    }

    public void AddCurve()
    {
        if (m_Points.Count == 0)
        {
            m_Points.Add(new());
            m_Points.Add(new(0, 0, 10));
            return;
        }

        Vector3 ctr = m_Points[m_Points.Count - 2];
        Vector3 tan = m_Points[m_Points.Count - 1];
        Vector3 tanDir = (tan - ctr) * -1;

        ctr += (Vector3.right * 10);
        tan = ctr + tanDir;

        m_Points.Add(ctr);
        m_Points.Add(tan);
    }

    public void RemoveLastCurve()
    {
        m_Points.RemoveAt(m_Points.Count - 1);
        m_Points.RemoveAt(m_Points.Count - 1);
    }
}
