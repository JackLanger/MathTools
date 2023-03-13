namespace MathTools;

/// <summary>
///     A vector is a quantity having direction as well as magnitude, especially as determining the position of one point
///     in space relative to another.
/// </summary>
public class Vector
{
    private readonly double[] _data;
    private Matrix? _transpose;

    /// <summary>
    ///     Creates a new Vector of n dimensions.
    /// </summary>
    /// <param name="n">number of dimensions</param>
    public Vector(int n) : this(new double[n])
    {
    }

    /// <summary>
    ///     Creates a new Vector from the data provided.
    /// </summary>
    /// <param name="data">one dimensional array containing vector data</param>
    public Vector(double[] data)
    {
        _data = data;
    }

    public int Length => _data.Length;

    public double this[int i]
    {
        get => _data[i];
        set => _data[i] = value;
    }

    #region Operators

    /// <summary>
    ///     Multiply a Vector from left with a matrix, that is the transpose of a Vector to the right. Result of a successful
    ///     Operation is Matrix of the dimensions of Length of the left vector and Columns of the right matrix.
    /// </summary>
    /// <param name="v">Vector v</param>
    /// <param name="m">Matrix m which is the transpose of a vector</param>
    /// <returns>a Matrix of dimensions according to the length of the vector and the columns of the matrix</returns>
    /// <exception cref="InvalidMatrixOperation">if dimensions don't match up</exception>
    public static Matrix operator *(Vector v, Matrix m)
    {
        if (m.Rows > 1)
            throw new InvalidMatrixOperation(
                $"Unable to multiply a vector from left to a matrix of size {m.Rows}x{m.Cols}");

        var tmp = new Matrix(v.Length, m.Cols);
        for (var i = 0; i < v.Length; i++)
        for (var j = 0; j < m.Cols; j++)
            tmp[i, j] = v[i] * m[0, j];

        return tmp;
    }

    public static Vector operator -(Vector fst, Vector snd)
    {
        return fst + -1 * snd;
    }

    /// <summary>
    ///     Subtracts a value n from each entry in the vector.
    /// </summary>
    /// <param name="v">the vector to apply this action on</param>
    /// <param name="n">the value to subtract</param>
    /// <returns>new Vector with the updated values.</returns>
    public static Vector operator -(Vector v, double n)
    {
        var tmp = new Vector(v.Length);
        for (var i = 0; i < v.Length; i++) tmp[i] = v[i] - n;

        return tmp;
    }

    /// <summary>
    ///     subtracts a vector from a given value n. The operation n - x is applied for each entry i for the Vector v. This in
    ///     turn means that a new Vector v is returned where the value of each entry is equal to n-xi.
    /// </summary>
    /// <param name="n">value to be subtracted from</param>
    /// <param name="v">the vector used for the operation</param>
    /// <returns>a new Vector with the modified values.</returns>
    public static Vector operator -(double n, Vector v)
    {
        var tmp = new Vector(v.Length);
        for (var i = 0; i < v.Length; i++) tmp[i] = n - v[i];

        return tmp;
    }

    public static Vector operator +(Vector fst, Vector snd)
    {
        if (fst.Length != snd.Length)
            throw new InvalidVectorOperation(
                "In order to perform this operation, both vectors need to be of the same size.");
        var tmp = new Vector(fst.Length);

        for (var i = 0; i < fst.Length; i++) tmp[i] = fst[i] + snd[i];
        return tmp;
    }

    public static Vector operator /(double alpha, Vector v)
    {
        return v * (1 / alpha);
    }

    public static Vector operator /(Vector v, double alpha)
    {
        return v * (1 / alpha);
    }

    public static Vector operator *(double alpha, Vector v)
    {
        return v * alpha;
    }

    public static Vector operator *(Vector v, double alpha)
    {
        var tmp = new Vector(v.Length);
        for (var i = 0; i < v.Length; i++) tmp[i] = v[i] * alpha;

        return tmp;
    }

    public static Vector operator *(Vector a, Vector b)
    {
        if (a.Length != b.Length)
            throw new InvalidVectorOperation("Operation invalid on vertices of different dimensions");

        var tmp = new Vector(a.Length);
        for (var i = 0; i < a.Length; i++) tmp[i] = a[i] * b[i];

        return tmp;
    }


    /// <summary>
    ///     Multiply a Matrix m from left with a vector to the right. This Operation will result in a new Vector of the size of
    ///     the Rows in the matrix m.
    /// </summary>
    /// <param name="m">the matrix</param>
    /// <param name="v">the vector</param>
    /// <returns>new Vector of size N, where N is the number of Rows in the matrix.</returns>
    /// <exception cref="InvalidMatrixOperation">
    ///     if matrix and vector are incompatible due to a difference in the size. More
    ///     specific the number of columns in the matrix has to match the dimensions in the vector.
    /// </exception>
    public static Vector operator *(Matrix m, Vector v)
    {
        if (m.Cols != v.Length)
            throw new InvalidMatrixOperation(
                $"Can not multiply matrix of size {m.Rows}x{m.Cols} with a Vector of size {v.Length}");
        var tmp = new Vector(m.Rows);
        for (var i = 0; i < m.Rows; i++)
        for (var j = 0; j < m.Cols; j++)
            tmp[i] += m[i, j] * v[j];

        return tmp;
    }

    #endregion

    #region Vector Functions

    /// <summary>
    ///     Returns the actual length of the vector.
    /// </summary>
    /// <returns></returns>
    public double Abs()
    {
        double tmp = 0;
        for (var i = 0; i < Length; i++) tmp += _data[i] * _data[i];

        return Math.Sqrt(tmp);
    }


    /// <summary>
    ///     Returns the transpose of the Vector. due to the nature of the Transpose it is no longer defined as a Vector and
    ///     therefore a Matrix of the size 1xN is returned.
    /// </summary>
    /// <returns>A Matrix of size 1xN</returns>
    public Matrix T()
    {
        if (_transpose is not null) return _transpose;
        _transpose = new Matrix(1, Length);

        for (var i = 0; i < Length; i++) _transpose[0, i] = _data[i];

        return _transpose;
    }

    /// <summary>
    ///     Returns a new Vector that is normalized, which means that the vector has an absolute length/ magnitude of 1.
    /// </summary>
    /// <returns>new Vector with the magnitude of 1</returns>
    public Vector Normalize()
    {
        return this / Abs();
    }

    #endregion
}