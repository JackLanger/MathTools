namespace MathTools_TestLib.utils;

public static class ArrayUtils
{
    public static bool ArrayAreEqual(double[] actual, double[] expected)
    {
        for (var i = 0; i < actual.Length; i++)
            if (!NumberIsEqual(actual[i], expected[i]))
                return false;

        return true;
    }

    /// <summary>
    ///     Checks if two numbers are equal, by defining a certain accuracy.
    /// </summary>
    /// <param name="actual">actual</param>
    /// <param name="expected">the expected Value</param>
    /// <param name="accuracy">Range of accuracy defaults to 0.00001 due to floating point errors</param>
    /// <returns>true if actual equal +- accuracy</returns>
    public static bool NumberIsEqual(double actual, double expected, double accuracy = 0.00001)
    {
        return Math.Abs(actual - expected) < accuracy;
    }
}