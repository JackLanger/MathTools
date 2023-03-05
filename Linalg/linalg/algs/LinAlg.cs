namespace MathTools;

/// <summary>
///     Utility Class for solving matrices. This Class Provides an API for solving matrix equations.
/// </summary>
public static class LinAlg
{
    /// <summary>
    ///     Solve a Matrix using the solving vector and return the result of the operation as a double
    ///     array.
    /// </summary>
    /// <param name="matrix">the Matrix to solve</param>
    /// <param name="solv">The Solving vector used for solving the Matrix</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="InvalidMatrixOperation">
    ///     if non square matrix is provided or solving vector dimensions do not match
    ///     Matrix Rows/Rank
    /// </exception>
    public static double[] Solve(Matrix matrix, double[] solv)
    {
        // pre computation checks.
        IsSquareCheck(matrix);
        RankCheck(matrix, solv);
        // create deep copies
        Matrix m;
        var s = new double [solv.Length];
        DeepCopy(ref matrix, out m);
        solv.CopyTo(s, 0);

        // solve.
        // iterate down
        for (var i = 1; i < m.Rows; i++)
        for (var j = i; j < m.Rows; j++)
        {
            if (m[i, i] == 0) // the main diagonal index is 0 we need to swap
            {
                m.PivotRows(i, i + 1);
                (s[i], s[i + 1]) = (s[i + 1], s[i]);
            }

            var a = -1 * m[j, i - 1] / m[i - 1, i - 1];
            m[j] += m[i - 1] * a;
            s[j] += a * s[i - 1];
        }

        if (isNullRow(m[m.Rows], solv[m.Rows]))
            throw new InvalidMatrixOperation($"LES has an infinite number of solutions {m},{solv}");
        // iterate back up
        for (var i = m.Rows - 2; i >= 0; i--)
        for (var j = i; j >= 0; j--)
        {
            // take last row right index and sequentially iterate up 
            var a = -1 * m[j, i + 1] / m[i + 1, i + 1];
            m[j] += m[i + 1] * a;
            s[j] += a * s[i + 1];
        }

        // normalize the data
        for (var i = 0; i < m.Rows; i++)
        {
            var alpha = m[i, i];
            m[i] /= alpha;
            s[i] /= alpha;
        }

        return s;
    }

    private static bool isNullRow(Row row, double d)
    {
        for (var i = 0; i < row.Length; i++)
            if (row[i] != 0)
                return false;
        // todo: return matrix information!
        if (d != 0) throw new InvalidMatrixOperation("LES is unsolvable!");

        return true;
    }


    private static void DeepCopy(ref Matrix from, out Matrix to)
    {
        to = new Matrix(from.Rows, from.Cols);

        for (var i = 0; i < to.Rows; i++)
        for (var j = 0; j < to.Cols; j++)
            to[i, j] = from[i, j];
    }

    private static void RankCheck(Matrix matrix, double[] solv)
    {
        if (solv.Length != matrix.Rows)
        {
            if (matrix.Rank == solv.Length)
                throw new NotImplementedException();
            throw new InvalidMatrixOperation(
                "Can't solve the LES, the solving Vector has to match the Matrix Rows.");
        }
    }

    private static void IsSquareCheck(Matrix matrix)
    {
        if (matrix.Cols != matrix.Rows)
        {
            if (matrix.Rank == matrix.Rows)
                throw new NotImplementedException();
            throw new InvalidMatrixOperation(
                $"Can't Solve the a non Square Matrix with dimensions {matrix.Rows}x{matrix.Cols}. The Feature is not yet implemented");
        }
    }

    /// <summary>
    ///     Calculate the Inverse of a Matrix. The Process is similiar to solving a linear equation system.
    ///     Instead of a Vector
    ///     to solve against a unified matrix is used. The Inverse is the result of the Solving the
    ///     equations and applying the
    ///     steps to the unification matrix.
    /// </summary>
    /// <param name="matrix">matrix to Invert</param>
    /// <returns>The inverse of the matrix.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="InvalidMatrixOperation">if non square Matrix is provided</exception>
    public static Matrix Inverse(this Matrix matrix)
    {
        // pre computation checks.
        IsSquareCheck(matrix);
        // create deep copies
        Matrix m;
        DeepCopy(ref matrix, out m);

        // solve.
        var solv = new Matrix(m.Rows, m.Cols);
        for (var i = 0; i < m.Rows; i++) solv[i, i] = 1.0; // create unification matrix

        for (var i = 1; i < m.Rows; i++)
        for (var j = i; j < m.Rows; j++)
        {
            if (m[i, i] == 0)
            {
                m.PivotRows(i, i + 1);
                solv.PivotRows(i, i + 1);
            }

            var a = -1 * m[j, i - 1] / m[i - 1, i - 1];
            m[j] += m[i - 1] * a;
            solv[j] += a * solv[i - 1];
        }

        for (var i = m.Rows - 2; i >= 0; i--)
        for (var j = i; j >= 0; j--)
        {
            // take last row right index and sequentially iterate up 
            var a = -1 * m[j, i + 1] / m[i + 1, i + 1];
            m[j] += m[i + 1] * a;
            solv[j] += a * solv[i + 1];
        }

        // normalize the data
        for (var i = 0; i < m.Rows; i++) solv[i] /= m[i, i];

        return solv;
    }
}