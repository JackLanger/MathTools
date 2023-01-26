using System.Diagnostics;
using MathTools;
using NUnit.Framework;

namespace MathTools_TestLib.models;

public class MatrixTest
{
    [Test]
    public void Test_Pivoting()
    {
        var actual = new Matrix(new double[,] {{1, 2, 3}, {4, 5, 6}});
        var expected = new Matrix(new double[,] {{4, 5, 6}, {1, 2, 3}});
        (actual[0], actual[1]) = (actual[1], actual[0]);
        Debug.Assert(actual.Equals(expected));
        actual.PivotRows(1, 0);

        Debug.Assert(!actual.Equals(expected));
    }

    [Test]
    public void Test_Pivoting_Out_Of_Bounds()
    {
        var actual = new Matrix(new double[,] {{1, 2, 3}, {4, 5, 6}});
        Assert.Throws(typeof(InvalidMatrixOperation), () => actual.PivotRows(2, 0));
    }

    [Test]
    public void Test_Multiplication_With_SQR_Matrix()
    {
        var first = new Matrix(new double[,] {{1, 2, 3}, {4, 5, 6}, {7, 8, 9}});
        var second = new Matrix(new double[,] {{7, 3, 2}, {2, 2, 2}, {4, 7, 1}});
        var fromLeft = new Matrix(new double[,] {{23, 28, 9}, {62, 64, 24}, {101, 100, 39}});
        var fromRight = new Matrix(new double[,] {{33, 45, 57}, {24, 30, 36}, {39, 51, 63}});

        Debug.Assert((first * second).Equals(fromLeft));
        Debug.Assert((second * first).Equals(fromRight));
    }

    [Test]
    public void Test_Multiplication_With_ASYNC_Matrix()
    {
        var first = new Matrix(new double[,] {{1, 2, 3}, {4, 5, 6}, {7, 8, 9}});
        var second = new Matrix(new double[,] {{7}, {5}, {8}});
        var expected = new Matrix(new double[,] {{41}, {101}, {161}});

        Debug.Assert((first * second).Equals(expected));

        Assert.Throws<InvalidMatrixOperation>(() =>
        {
            var a = second * first;
        });
    }

    [Test]
    public void Test_Multiplication_With_Alpha()
    {
        var first = new Matrix(new double[,] {{1, 2, 3}, {4, 5, 6}, {7, 8, 9}});
        var expected = new Matrix(new double[,] {{2, 4, 6}, {8, 10, 12}, {14, 16, 18}});

        Debug.Assert((first * 2).Equals(expected));
    }

    [Test]
    public void Test_Subtract_Matrix()
    {
        var first = new Matrix(new double[,] {{1, 2, 3}, {4, 5, 6}, {7, 8, 9}});
        var second = new Matrix(new double[,] {{1, 2, 3}, {4, 5, 6}, {7, 8, 9}});

        var expected = new Matrix(new double[,] {{0, 0, 0}, {0, 0, 0}, {0, 0, 0}});

        Debug.Assert((first - second).Equals(expected));
    }

    [Test]
    public void Test_Addition()
    {
        var first = new Matrix(new double[,] {{1, 2, 3}, {4, 5, 6}, {7, 8, 9}});
        var second = new Matrix(new double[,] {{1, 2, 3}, {4, 5, 6}, {7, 8, 9}});
        var expected = new Matrix(new double[,] {{2, 4, 6}, {8, 10, 12}, {14, 16, 18}});

        Debug.Assert((first + second).Equals(expected));
    }

    [Test]
    public void Test_Transpose()
    {
        var actual = new Matrix(new double[,] {{1, 2, 3}, {4, 5, 6}, {7, 8, 9}});
        var expected = new Matrix(new double[,] {{1, 4, 7}, {2, 5, 8}, {3, 6, 9}});

        Debug.Assert(actual.T.Equals(expected));
        Debug.Assert(actual.T.T.Equals(actual));
    }

    private Matrix RandomMatrix(int n, int m)
    {
        var rand = new Random();
        var data = new double[n, m];
        for (var i = 0; i < n; i++)
        for (var j = 0; j < m; j++)
            data[i, j] = rand.NextDouble();

        return new Matrix(data);
    }

    [Test]
    public void Test_Determining()
    {
        double expected = 0;
        var first = new Matrix(new double[,] {{5, 2, 3}, {5, 2, 3}, {5, 2, 3}});
        Assert.That(expected, Is.EqualTo(first.D));

        var snd = new Matrix(new double[,] {{5, 2}, {5, 2}});
        Assert.That(expected, Is.EqualTo(snd.D));

        var third = new Matrix(new double[,]
        {
            {1, 2, 3, 4, 5, 6, 7, 8, 9}, {1, 2, 3, 4, 5, 6, 7, 8, 9}, {1, 2, 3, 4, 5, 6, 7, 8, 9},
            {1, 2, 3, 4, 5, 6, 7, 8, 9}, {1, 2, 3, 4, 5, 6, 7, 8, 9}, {1, 2, 3, 4, 5, 6, 7, 8, 9},
            {1, 2, 3, 4, 5, 6, 7, 8, 9}, {1, 2, 3, 4, 5, 6, 7, 8, 9}, {1, 2, 3, 4, 5, 6, 7, 8, 9}
        });
        Assert.That(expected, Is.EqualTo(third.D));
    }

    [Test]
    public void Benchmark_Multiplication()
    {
        var N = 100;
        var _watch = new Stopwatch();
        var times = new TimeSpan[N];
        _watch.Start();
        var first = RandomMatrix(10, 10);
        var second = RandomMatrix(10, 10);
        for (var i = 0; i < N; i++)
        {
            _ = first * second;
            times[i] = _watch.Elapsed;
            _watch.Restart();
        }

        _watch.Stop();
        for (var i = 0; i < N; i++) Console.WriteLine($"{i:0000}:\t{times[i]:c}");
    }
}