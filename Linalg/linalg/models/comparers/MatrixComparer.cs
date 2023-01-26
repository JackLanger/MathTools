namespace MathTools.linalg.models.comparers;

public class MatrixComparer : IEqualityComparer<Matrix>
{
    public bool Equals(Matrix? x, Matrix? y)
    {
        if (x is null || y is null) return false;

        // compare size
        if (x.Rows != y.Rows || x.Cols != y.Cols)
            return false;

        // check all values in the backing data.
        for (var i = 0; i < x.Rows; i++)
        for (var j = 0; j < x.Cols; j++)
            if (Math.Abs(x[i, j] - y[i, j]) > .000001)
                return false;
        // return true if all tests pass
        return true;
    }


    public int GetHashCode(Matrix obj)
    {
        return obj.GetHashCode();
    }
}