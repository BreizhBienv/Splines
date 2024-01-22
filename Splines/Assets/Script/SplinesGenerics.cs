using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplinesGenerics
{
    public enum ECurveType
    {
        Hermitienne,
        Bézier,
        B_Spline,
        Catmull_Rom,
    }

    public static float[,] M_Hermitienne =
    {
        {   2,  -2, 1,  1   },
        {   -3, 3,  -2, -1  },
        {   0,  0,  1,  0   },
        {   1,  0,  0,  0   }
    };

    public static float[,] M_Bézier =
    {
        {   -1,  3, -3, 1   },
        {   3, -6,  3,  0   },
        {   -3,  3,  0, 0   },
        {   1,  0,  0,  0   }
    };

    public static float[,] M_B_Spline =
    {
        {   -1,  3, -3, 1   },
        {   3, -6,  3,  0   },
        {   -3,  0, 3,  0   },
        {   1,  4,  1,  0   }
    };

    public static float[,] M_Catmull_Rom =
    {
        {   -1,  3, -3, 1   },
        {   2, -5,  4,  -1  },
        {   -1,  0, 1,  0   },
        {   0,  2,  0,  0   }
    };
}
