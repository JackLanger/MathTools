using MathTools;
using MathTools_TestLib.utils;
using NUnit.Framework;

namespace MathTools_TestLib.LinAlg;

public class LinalgTest
{
    [Test]
    public void Test_Solve_LGS()
    {
        var m = new Matrix(new double[,] {{5, 2, 8}, {1, 5, 9}, {7, 5, 3}});
        double[] solv = {8, 6, 1};
// Expected calculated with Wolfram Alpha
        double[] expected = {29 / 135.0, -197 / 270.0, 283 / 270.0};
        var actual = MathTools.LinAlg.Solve(m, solv);
        Console.WriteLine($"actual:[{string.Join(" | ", actual)}],\nexpected:[{string.Join(" | ", expected)}]");
        Assert.True(ArrayUtils.ArrayAreEqual(actual, expected));
    }

    [Test]
    public void Test_Inverse()
    {
        var m = new Matrix(new double[,] {{5, 2, 8}, {1, 5, 9}, {7, 5, 3}});
// Expected calculated with Wolfram Alpha
        var expected = new Matrix(new[,]
        {
            {1 / 9.0, -17.0 / 135, 11.0 / 135}, {-2.0 / 9, 41.0 / 270, 37.0 / 270}, {1.0 / 9, 11.0 / 270, -23.0 / 270}
        });
        var inverse = m.Inverse();
        for (var i = 0; i < 3; i++)
            Console.WriteLine(
                $"actual:{string.Join(" | ", inverse[i].data)},\texpected:{string.Join(" | ", expected[i].data)}");
        Assert.True(inverse.Equals(expected));
    }

    [Test]
    public void Test_LGS_With_Zero_Value_Rows()
    {
        //todo(jack): implement.
    }
}