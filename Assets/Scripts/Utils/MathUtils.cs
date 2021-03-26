using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtils
{
    public const float FLOAT_ZERO = 0.000001f;

    /// <summary>
    /// Safe float comparison with optional margin.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="margin"></param>
    /// <returns></returns>
    public static bool FloatIsEqual(float a, float b, float margin = FLOAT_ZERO)
    {
        return Mathf.Abs(a - b) < margin;
    }
}
