using System;
using System.Linq;
using UnityEngine;

public enum ECurveType
{
    Hermite,
    Bézier,
    B_Spline,
    Catmull_Rom,
}

public class SplinesHelper
{
    #region Hermite
    static float[,] M_Hermite =
    {
        {   2,  -2, 1,  1   },
        {   -3, 3,  -2, -1  },
        {   0,  0,  1,  0   },
        {   1,  0,  0,  0   }
    };

    public static Vector3 ComputeHermite(float pT, GameObject[] pSplinePoints)
    {
        float[,] T = MatrixT(pT, pSplinePoints);
        float[,] M = M_Hermite;

        float[,] matrix_TM = 
            MultiplyMatrices(T, M);
        float[,] matrix_TMG = 
            MultiplyMatrices(matrix_TM, HermiteControlPoints(pSplinePoints));

        return new(matrix_TMG[0, 0], matrix_TMG[0, 1], matrix_TMG[0, 2]);
    }

    static float[,] HermiteControlPoints(GameObject[] pSplinePoints)
    {
        Vector3 entry = pSplinePoints.First().transform.position;
        Vector3 exit = pSplinePoints.Last().transform.position;

        Vector3 entryTangent = pSplinePoints[1].transform.position;
        Vector3 exitTangent = pSplinePoints[pSplinePoints.Length - 2].transform.position;

        Vector3 R1 = entryTangent - entry;
        Vector3 R2 = exitTangent - exit;

        float[,] controlPoints =
        {
            { entry.x, entry.y, entry.z },
            { exit.x, exit.y, exit.z },
            { R1.x, R1.y, R1.z },
            { R2.x, R2.y, R2.z },
        };

        return controlPoints;
    }
    #endregion

    #region Bézier
    static float[,] M_Bézier =
    {
        {   -1,  3, -3, 1   },
        {   3, -6,  3,  0   },
        {   -3,  3,  0, 0   },
        {   1,  0,  0,  0   }
    };

    public static Vector3 ComputeBézierCurve(float pT, GameObject[] pSplinePoints)
    {
        float[,] T = MatrixT(pT, pSplinePoints);
        float[,] M = M_Bézier;

        float[,] matrix_TM = 
            MultiplyMatrices(T, M);
        float[,] matrix_TMG = 
            MultiplyMatrices(matrix_TM, BézierControlPoints(pSplinePoints));

        return new(matrix_TMG[0, 0], matrix_TMG[0, 1], matrix_TMG[0, 2]);
    }

    static float[,] BézierControlPoints(GameObject[] pSplinePoints)
    {
        float[,] controlPoints = new float[pSplinePoints.Length, 3];

        for (int i = 0; i < pSplinePoints.Length; i++)
        {
            Vector3 pos = pSplinePoints[i].transform.position;

            controlPoints[i, 0] = pos.x;
            controlPoints[i, 1] = pos.y;
            controlPoints[i, 2] = pos.z;
        }

        return controlPoints;
    }
    #endregion

    #region B_Spline
    static float[,] M_B_Spline =
    {
        {   -1,  3, -3, 1   },
        {   3, -6,  3,  0   },
        {   -3,  0, 3,  0   },
        {   1,  4,  1,  0   }
    };

    public static Vector3 ComputeBSplineCurve(float pT, GameObject[] pSplinePoints)
    {
        float[,] T = MatrixT(pT, pSplinePoints);
        float[,] M = M_B_Spline;

        float[,] matrix_TM =
            MultiplyMatrices(T, M);
        float[,] matrix_TMG =
            MultiplyMatrices(matrix_TM, BSplineControlPoints(pSplinePoints));

        Vector3 result = new(matrix_TMG[0, 0], matrix_TMG[0, 1], matrix_TMG[0, 2]);

        return result / 6;
    }

    static float[,] BSplineControlPoints(GameObject[] pSplinePoints)
    {
        float[,] controlPoints = new float[pSplinePoints.Length, 3];

        for (int i = 0; i < pSplinePoints.Length; i++)
        {
            Vector3 pos = pSplinePoints[i].transform.position;

            controlPoints[i, 0] = pos.x;
            controlPoints[i, 1] = pos.y;
            controlPoints[i, 2] = pos.z;
        }

        return controlPoints;
    }
    #endregion

    #region Catmull_Rom
    static float[,] M_Catmull_Rom =
    {
        {   -1,  3, -3, 1   },
        {   2, -5,  4,  -1  },
        {   -1,  0, 1,  0   },
        {   0,  2,  0,  0   }
    };

    public static Vector3 ComputeCatmullRomCurve(float pT, GameObject[] pSplinePoints)
    {
        float[,] T = MatrixT(pT, pSplinePoints);
        float[,] M = M_Catmull_Rom;

        float[,] matrix_TM =
            MultiplyMatrices(T, M);
        float[,] matrix_TMG =
            MultiplyMatrices(matrix_TM, CatmullRomControlPoints(pSplinePoints));

        Vector3 result = new(matrix_TMG[0, 0], matrix_TMG[0, 1], matrix_TMG[0, 2]);

        return result / 2;
    }

    static float[,] CatmullRomControlPoints(GameObject[] pSplinePoints)
    {
        float[,] controlPoints = new float[pSplinePoints.Length, 3];

        for (int i = 0; i < pSplinePoints.Length; i++)
        {
            Vector3 pos = pSplinePoints[i].transform.position;

            controlPoints[i, 0] = pos.x;
            controlPoints[i, 1] = pos.y;
            controlPoints[i, 2] = pos.z;
        }

        return controlPoints;
    }
    #endregion

    static float[,] MatrixT(float pT, GameObject[] pSplinePoints)
    {
        int splineDegree = pSplinePoints.Length - 1;

        float[,] matrixT = new float[1, pSplinePoints.Length];
        for (int i = 0; i < splineDegree; ++i)
        {
            int expo = splineDegree - i;
            matrixT[0, i] = Mathf.Pow(pT, expo);
        }

        matrixT[0, pSplinePoints.Length - 1] = 1;
        return matrixT;
    }

    static float[,] MultiplyMatrices(float[,] pM1, float[,] pM2)
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
