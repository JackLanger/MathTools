using MathTools.linalg.models.comparers;

namespace MathTools;

/// <summary>
///     Matrix Class. This is a wrapper around a  multidimensional double array of size NxM. The Matrix can be used and
///     accessed
///     much as a multidimensional array and extends the functionality to basic operations known to matrices. These
///     operations include but do not limit to arithmetical operations.
/// </summary>
public class Matrix
{
    //Todo: extend to generic type if possible.
    // backing fields
    private readonly MatrixComparer _comparer = new();

    private double? _det;

    /// <summary>
    ///     Backing field for the transpose of the matrix. The transpose is initialized as null and is set with the first call.
    ///     The Transpose has a reference to the Matrix it self to enable fast conversion between matrices and there transpose.
    /// </summary>
    private Matrix? _transpose;

    /// <summary>
    ///     Create a new matrix object with the data provided and a inverse. This is not Accessible from outside the Matrix but
    ///     is used internally to create the inverse and transpose of the matrix while assigning the Matrix as its inverse or
    ///     transpose.
    /// </summary>
    /// <param name="data">The data to assign</param>
    /// <param name="trans">The transpose or inverse</param>
    private Matrix(double[,] data, Matrix trans)
    {
        Data = data;
        _transpose = trans;
    }

    /// <summary>
    ///     Create a new Matrix and assign the data to the backing field.
    /// </summary>
    /// <param name="_data">The data to assign</param>
    public Matrix(double[,] _data)
    {
        Data = _data;
    }


    /// <summary>
    ///     Creates a square matrix of size n by n. All values default to 0.
    /// </summary>
    /// <param name="n">Number of rows and columns</param>
    public Matrix(int n) : this(n, n)
    {
    }

    /// <summary>
    ///     Creates a Matrix with the Dimensions n,m. All Values are set to 0 by default.
    /// </summary>
    /// <param name="n">number of rows</param>
    /// <param name="m">Number of Columns</param>
    public Matrix(int n, int m)
    {
        Data = new double[n, m];
    }

    /// <summary>
    ///     Create a Matrix from a Serialized Matrix by providing the dimensions of the Matrix.
    /// </summary>
    /// <param name="data">data to use to populate the matrix</param>
    /// <param name="n">Number of rows</param>
    /// <param name="m">number of Columns</param>
    public Matrix(double[] data, int n, int m) : this(n, m)
    {
        for (var i = 0; i < n; i++)
        for (var j = 0; j < m; j++)
            Data[i, j] = data[i * j + j];
    }

    /// <summary>
    ///     Returns the Determinant of a Matrix. The Determinant is an important indicator if an
    ///     Linear Equation System (LES) can be solved. If the Determinant of a Matrix equals 0 (det(A)=0),
    ///     linear dependencies within the matrix exist and there is not a single solution to the LES.
    ///     The LES could still present an infinite number of solutions or none at all.
    ///     <exception cref="InvalidMatrixOperation"> if Matrix not square</exception>
    /// </summary>
    public double D => _det ??= GetDet();

    /// <summary>
    ///     Data container for the matrix. Multidimensional array of double.
    /// </summary>
    private double[,] Data { get; }

    // Getter and setter
    public int Rows => Data.GetLength(0);
    public int Cols => Data.GetLength(1);
    public int Rank => Data.Rank;

    /// <summary>
    ///     Returns a Transpose of the Matrix. If transpose is null this function will generate a new Transpose and assign it
    ///     to the backing field.
    /// </summary>
    public Matrix T => _transpose ?? TransposeMatrix();


    /// <summary>
    ///     Index a value within the Data.
    /// </summary>
    /// <param name="row">Index of the row</param>
    /// <param name="col">Index of the column</param>
    public double this[int row, int col]
    {
        get => Data[row, col];
        set => Data[row, col] = value;
    }

    /// <summary>
    ///     Index a whole row within the matrix.
    /// </summary>
    /// <param name="row">index of the row</param>
    public Row this[int row]
    {
        get => GetRow(row);
        set => SetRow(value, row);
    }


    private Matrix TransposeMatrix()
    {
        var transData = new double[Cols, Rows];
        for (var i = 0; i < Rows; i++)
        for (var j = 0; j < Cols; j++)
            transData[j, i] = this[i, j];
        _transpose = new Matrix(transData, this);
        return _transpose;
    }

    /// <summary>
    ///     Adds tow matrices of equal size.
    /// </summary>
    /// <param name="lft">left side matrix</param>
    /// <param name="rgt">right hand side matrix</param>
    /// <returns>The Result of the Operation</returns>
    /// <exception cref="InvalidMatrixOperation"></exception>
    public static Matrix operator +(Matrix lft, Matrix rgt)
    {
        if (lft.Rows != rgt.Rows && lft.Cols != rgt.Cols)
            throw new InvalidMatrixOperation(lft, rgt);

        Matrix tmp = new(lft.Rows, lft.Cols);
        for (var i = 0; i < lft.Rows; i++)
        for (var j = 0; j < lft.Cols; j++)
            tmp[i, j] = lft[i, j] + rgt[i, j];

        return tmp;
    }

    /// <summary>
    ///     Divide a Matrix by a given Value.
    /// </summary>
    /// <param name="m">The matrix</param>
    /// <param name="alpha">The value to divide by</param>
    /// <returns>Matrix with reduced values</returns>
    public static Matrix operator /(Matrix m, double alpha)
    {
        return m * (1 / alpha);
    }


    /// <summary>
    ///     Divide a Matrix by a given Value.
    /// </summary>
    /// <param name="m">The matrix</param>
    /// <param name="alpha">The value to divide by</param>
    /// <returns>Matrix with reduced values</returns>
    public static Matrix operator /(double alpha, Matrix m)
    {
        return 1 / alpha * m;
    }

    /// <summary>
    ///     Naive Matrix multiplication
    /// </summary>
    /// <param name="lft">left side matrix</param>
    /// <param name="rgt">right side matrix</param>
    /// <returns>a matrix that, which is the result of the operation</returns>
    /// <exception cref="InvalidMatrixOperation">if matrix is not defined</exception>
    public static Matrix operator *(Matrix lft, Matrix rgt)
    {
        if (lft.Cols != rgt.Rows)
            throw new InvalidMatrixOperation(lft, rgt);
        Matrix tmp = new(lft.Rows, rgt.Cols);

        for (var i = 0; i < lft.Rows; i++)
        for (var j = 0; j < rgt.Cols; j++)
        for (var k = 0; k < rgt.Rows; k++)
            tmp[i, j] += lft[i, k] * rgt[k, j];

        return tmp;
    }

    /// <summary>
    ///     Multiply a matrix with a alpha.
    /// </summary>
    /// <param name="matrix">Matrix </param>
    /// <param name="alpha">Alpha</param>
    /// <returns>Matrix that is the result of the Operation</returns>
    public static Matrix operator *(Matrix matrix, double alpha)
    {
        Matrix tmp = new(matrix.Rows, matrix.Cols);
        for (var i = 0; i < matrix.Rows; i++)
        for (var j = 0; j < matrix.Cols; j++)
            tmp[i, j] = matrix[i, j] * alpha;

        return tmp;
    }

    /// <summary>
    ///     Multiply a matrix with a alpha.
    /// </summary>
    /// <param name="matrix">Matrix </param>
    /// <param name="alpha">Alpha</param>
    /// <returns>Matrix that is the result of the Operation</returns>
    public static Matrix operator *(double alpha, Matrix matrix)
    {
        return matrix * alpha;
    }

    /// <summary>
    ///     Subtracts the right matrix from the left matrix. Matrices are required to be of the same size. If the condition is
    ///     not met an InvalidMatrixOperation is thrown.
    /// </summary>
    /// <param name="lft">left side matrix</param>
    /// <param name="rgt">right hand side matrix</param>
    /// <returns>The Result of the Operation</returns>
    /// <exception cref="InvalidMatrixOperation">If condition is not met</exception>
    public static Matrix operator -(Matrix lft, Matrix rgt)
    {
        return lft + rgt * -1;
    }

    /// <summary>
    ///     Retrieve a column as a one dimensional array.
    /// </summary>
    /// <param name="col">Index of the column</param>
    /// <returns>Array of the column</returns>
    public double[] GetColumn(int col)
    {
        return Enumerable.Range(0, Data.GetLength(0))
            .Select(x => Data[x, col])
            .ToArray();
    }

    /// <summary>
    ///     Return the indexed row as one dimensional array.
    /// </summary>
    /// <param name="row">Index of the column</param>
    /// <returns>Array of the row fetched from the matrix</returns>
    private Row GetRow(int row)
    {
        return new Row(Enumerable.Range(0, Data.GetLength(1))
            .Select(x => Data[row, x])
            .ToArray());
    }

    private void SetRow(Row rowdata, int row)
    {
        for (var i = 0; i < Cols; i++) Data[row, i] = rowdata[i];
    }

    public void SetCol(double[] colData, int col)
    {
        for (var i = 0; i < Cols; i++) Data[i, col] = colData[i];
    }

    /// <summary>
    ///     Shorthand method to swap two rows. Will throw out of bounds exception if row is not within Bounds.
    /// </summary>
    /// <param name="n">Index of the first row</param>
    /// <param name="m">Index of the second row</param>
    /// <exception cref="InvalidMatrixOperation">If row is out of bounds of the data</exception>
    public void PivotRows(int n, int m)
    {
        try
        {
            (this[n], this[m]) = (this[m], this[n]);
        }
        catch (IndexOutOfRangeException e)
        {
            Console.WriteLine(e);
            if (n > Rows)
                throw new InvalidMatrixOperation(n, this);
            throw new InvalidMatrixOperation(m, this);
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is Matrix other)
            return Equals(other);
        return false;
    }

    private bool Equals(Matrix other)
    {
        return _comparer.Equals(this, other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Data);
    }

    /// <summary>
    ///     Calculate and return the Determinant.
    /// </summary>
    /// <returns>the determinant</returns>
    /// <exception cref="InvalidMatrixOperation">If no data is present</exception>
    private double GetDet()
    {
        if (Rows != Cols)
            throw new InvalidMatrixOperation("The can not be calculate the determinant of a non square matrix.");

        var n = Data.Length;

        if (n < 3)
        {
            if (n == 1) return Data[0, 0];
            if (n == 2) return Data[0, 0] * Data[1, 1] - Data[1, 0] * Data[0, 1];

            throw new InvalidMatrixOperation("Matrix has size of 0");
        }

        if (n == 3) return DetSarrus();

        double det = 0;
        for (var i = 0; i < n; i++)
        for (var j = 0; j < n; j++)
        {
            var fact = (int)Math.Pow(-1, i + j);
            det += fact * Data[i, j] * getSubA(i, j).GetDet();
        }

        return det;
    }

    /// <summary>
    ///     Creates a Matrix of size n-1 and copies all values from the backing field to the new matrix.
    ///     All fields but fields with the row number i and column number j.
    /// </summary>
    /// <param name="i">row index</param>
    /// <param name="j">column index</param>
    /// <returns>A Sub Matrix of the current matrix without the provided column and row</returns>
    /// <exception cref="Exception"> if the current matrix is smaller than 2x2</exception>
    private Matrix getSubA(int i, int j)
    {
        var n = Data.Length - 1;
        if (n < 2)
            throw new Exception("Invalid exception Sub Matrix of size 1 cannot be created as it is a single value");
        var tmp = new Matrix(n);

        for (var k = 0; k < n; k++)
        for (var l = 0; l < n; l++)
            if (k > i && l > j)
                tmp[k - 1, l - 1] = Data[k, l];
            else if (k > n)
                tmp[k - 1, l] = Data[k, l];
            else if (l > n)
                tmp[k, l - 1] = Data[k, l];
            else
                tmp[k, l] = Data[k, l];

        return tmp;
    }


    /// <summary>
    ///     Return the determinant of the 3x3 matrix calculated as of the Sarrus.
    /// </summary>
    /// <returns>Determinant of the matrix</returns>
    private double DetSarrus()
    {
        return Data[0, 0]
               * Data[1, 1]
               * Data[2, 2]
               // middle
               + Data[1, 0]
               * Data[2, 1]
               * Data[0, 2]
               // right
               + Data[2, 0]
               * Data[0, 1]
               * Data[1, 2]
               // subtracts
               - Data[2, 0]
               * Data[1, 1]
               * Data[0, 2]
               // middle
               - Data[2, 1]
               * Data[1, 2]
               * Data[0, 0]
               // right
               - Data[2, 2]
               * Data[1, 0]
               * Data[0, 1];
    }

    private double SmallDetermining()
    {
        return Data[0, 0] * Data[1, 1]
               + Data[0, 1] * Data[1, 0]
               - Data[1, 0] * Data[0, 1]
               - Data[0, 1] * Data[0, 0]
            ;
    }
}