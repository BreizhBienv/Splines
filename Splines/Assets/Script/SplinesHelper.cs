using System;
using System.Collections.Generic;
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

    public static Vector3 ComputeHermite(float pT, Vector3[] pPoints)
    {
        int degree = pPoints.Length - 1;
        float[,] T = MatrixT(pT, degree);
        float[,] M = M_Hermite;

        float[,] matrix_TM = 
            MultiplyMatrices(T, M);
        float[,] matrix_TMG = 
            MultiplyMatrices(matrix_TM, HermiteControlPoints(pPoints));

        return new(matrix_TMG[0, 0], matrix_TMG[0, 1], matrix_TMG[0, 2]);
    }

    static float[,] HermiteControlPoints(Vector3[] pPoints)
    {
        Vector3 entry = pPoints.First();
        Vector3 exit = pPoints.Last();

        Vector3 entryTangent = pPoints[1];
        Vector3 exitTangent = pPoints[pPoints.Length - 2];

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

    public static Vector3 ComputeBézierCurve(float pT, Vector3[] pPoints)
    {
        int degree = pPoints.Length - 1;
        float[,] T = MatrixT(pT, degree);
        float[,] M = M_Bézier;

        float[,] matrix_TM = 
            MultiplyMatrices(T, M);
        float[,] matrix_TMG = 
            MultiplyMatrices(matrix_TM, BézierControlPoints(pPoints));

        return new(matrix_TMG[0, 0], matrix_TMG[0, 1], matrix_TMG[0, 2]);
    }

    static float[,] BézierControlPoints(Vector3[] pPoints)
    {
        float[,] controlPoints = new float[pPoints.Length, 3];

        for (int i = 0; i < pPoints.Length; i++)
        {
            Vector3 pos = pPoints[i];

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
        {   -1, 3,  -3, 1   },
        {   3,  -6, 3,  0   },
        {   -3, 0,  3,  0   },
        {   1,  4,  1,  0   }
    };

    public static Vector3 ComputeBSplineCurve(float pT, Vector3[] pPoints)
    {
        int degree = pPoints.Length - 1;
        float[,] T = MatrixT(pT, degree);
        float[,] M = M_B_Spline;

        float[,] matrix_TM =
            MultiplyMatrices(T, M);
        float[,] matrix_TMG =
            MultiplyMatrices(matrix_TM, BSplineControlPoints(pPoints));

        Vector3 result = new(matrix_TMG[0, 0], matrix_TMG[0, 1], matrix_TMG[0, 2]);

        return result / 6f;
    }

    static float[,] BSplineControlPoints(Vector3[] pPoints)
    {
        float[,] controlPoints = new float[pPoints.Length, 3];

        for (int i = 0; i < pPoints.Length; i++)
        {
            Vector3 pos = pPoints[i];

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

    public static Vector3 ComputeCatmullRomCurve(float pT, Vector3[] pPoints)
    {
        int degree = pPoints.Length - 1;
        float[,] T = MatrixT(pT, degree);
        float[,] M = M_Catmull_Rom;

        float[,] matrix_TM =
            MultiplyMatrices(T, M);
        float[,] matrix_TMG =
            MultiplyMatrices(matrix_TM, CatmullRomControlPoints(pPoints));

        Vector3 result = new(matrix_TMG[0, 0], matrix_TMG[0, 1], matrix_TMG[0, 2]);

        return result / 2f;
    }

    static float[,] CatmullRomControlPoints(Vector3[] pPoints)
    {
        float[,] controlPoints = new float[pPoints.Length, 3];

        for (int i = 0; i < pPoints.Length; i++)
        {
            Vector3 pos = pPoints[i];

            controlPoints[i, 0] = pos.x;
            controlPoints[i, 1] = pos.y;
            controlPoints[i, 2] = pos.z;
        }

        return controlPoints;
    }
    #endregion

    static float[,] MatrixT(float pT, int pDegree)
    {
        float[,] matrixT = new float[1, pDegree + 1];
        for (int i = 0; i < pDegree; ++i)
        {
            int expo = pDegree - i;
            matrixT[0, i] = Mathf.Pow(pT, expo);
        }

        matrixT[0, pDegree] = 1;
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
