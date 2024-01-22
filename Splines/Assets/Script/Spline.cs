using System;
using UnityEngine;

public class Spline : MonoBehaviour
{
    [SerializeField] private GameObject[] m_SplinePoints;
    [SerializeField] private GameObject m_MovingGO;
    
    private int m_SplineDegree;
    private float m_Tvalue;

    private void Start()
    {
        m_SplineDegree = m_SplinePoints.Length - 1;
    }

    private void Update()
    {
        m_Tvalue = (m_Tvalue + Time.deltaTime) % 1f;
        m_MovingGO.transform.position = ComputeBézierCurve(m_Tvalue);
    }

    Vector3 ComputeBézierCurve(float pT)
    {
        float[,] T = MatrixT(pT);
        float[,] M = SplinesGenerics.M_Bézier;

        float[,] matrix_TM = MultiplyMatrices(T, M);
        float[,] matrix_TMG = MultiplyMatrices(matrix_TM, BézierControlPoints());

        return new(matrix_TMG[0, 0], matrix_TMG[0, 1], matrix_TMG[0, 2]);
    }

    float[,] BézierControlPoints()
    {
        float[,] controlPoints = new float[m_SplinePoints.Length, 3]; 

        for (int i = 0; i < m_SplinePoints.Length; i++)
        {
            Vector3 pos = m_SplinePoints[i].transform.position;

            controlPoints[i, 0] = pos.x;
            controlPoints[i, 1] = pos.y;
            controlPoints[i, 2] = pos.z;
        }

        return controlPoints;
    }

    float[,] MatrixT(float pT)
    {
        float[,] matrixT = new float[1, m_SplinePoints.Length];
        for (int i = 0; i < m_SplineDegree; ++i)
        {
            int expo = m_SplineDegree - i;
            matrixT[0, i] = Mathf.Pow(pT, expo);
        }

        matrixT[0, m_SplinePoints.Length - 1] = 1;
        return matrixT;
    }

    float[,] MultiplyMatrices(float[,] pM1, float[,] pM2)
    {
        // getting matrix lengths for better performance  
        var m1Rows = pM1.GetLength(0);
        var m1Cols = pM1.GetLength(1);
        var m2Rows = pM2.GetLength(0);
        var m2Cols = pM2.GetLength(1);

        // checking if product is defined  
        if (m1Cols != m2Rows)
            throw new InvalidOperationException
              ("Product is undefined. n columns of first matrix must equal to n rows of second matrix");

        // creating the final product matrix  
        float[,] product = new float[m1Rows, m2Cols];

        // looping through matrix 1 rows  
        for (int i = 0; i < m1Rows; i++)
        {
            // loop through matrix 2 columns  
            for (int j = 0; j < m2Cols; j++)
            {
                // loop through matrix 1 columns or matrix 2 rows to calculate the dot product  
                for (int k = 0; k < m1Cols; k++)
                {
                    product[i, j] +=
                      pM1[i, k] *
                      pM2[k, j];
                }
            }
        }

        return product;
    }


}
