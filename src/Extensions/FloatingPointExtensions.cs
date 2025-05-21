namespace ScrubJay.Extensions;

[PublicAPI]
public static class FloatingPointExtensions
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="f64"></param>
    /// <param name="other"></param>
    /// <param name="tolerance">
    /// A value in [1..0) where 1.0 indicates that any two values are equal and 0 indicates none are
    /// thus a very small value like 1e-10
    /// </param>
    /// <returns></returns>
    /// <seealso href="https://roundwide.com/equality-comparison-of-floating-point-numbers-in-csharp/"/>
    public static bool NearlyEquals(this double f64, double other, double tolerance = 1e-10)
    {
        var diff = Math.Abs(f64 - other);
        return diff <= tolerance ||
            diff <= Math.Max(Math.Abs(f64), Math.Abs(other)) * tolerance;
    }

    public static bool NearlyEquals(this float f32, float other, float tolerance = 1e-10f)
    {
        var diff = Math.Abs(f32 - other);
        return diff <= tolerance ||
            diff <= Math.Max(Math.Abs(f32), Math.Abs(other)) * tolerance;
    }


//    /// <summary>
//    ///
//    /// </summary>
//    /// <param name="f32"></param>
//    /// <param name="other"></param>
//    /// <param name="max"></param>
//    /// <returns></returns>
//    /// <seealso href="https://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/"/>
//    public static bool NearlyEqual(this float A, float B, int maxUlpsDiff)
//    {
//        // Different signs means they do not match.
//        if (float.IsNegative(A) != float.IsNegative(B))
//        {
//            // Check for equality to ensure +0 == -0
//            // ReSharper disable once CompareOfFloatsByEqualityOperator
//            return A == B;
//        }
//
//        // Find the difference in ULPs.
//        var aBits = BitConverter.SingleToInt32Bits(A);
//        var bBits = BitConverter.SingleToInt32Bits(B);
//
//        int ulpsDiff = Math.Abs(aBits - bBits);
//        return ulpsDiff <= maxUlpsDiff;
//
//        /*
//
//        Float_t uA(A);
//        Float_t uB(B);
//
//        // Different signs means they do not match.
//        if (uA.Negative() != uB.Negative())
//        {
//            // Check for equality to make sure +0==-0
//            if (A == B)
//                return true;
//            return false;
//        }
//
//        // Find the difference in ULPs.
//        int ulpsDiff = abs(uA.i - uB.i);
//        if (ulpsDiff <= maxUlpsDiff)
//            return true;
//        return false;
//        */
//    }
}