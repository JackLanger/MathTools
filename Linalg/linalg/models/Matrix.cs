using MathTools.linalg.models.comparers;

namespace MathTools;

public class Matrix
{
    private readonly MatrixComparer _comparer = new();

    private double? _det;

    // backing fields
    private Matrix? _transpose;

    private Matrix(double[,] data, Matrix trans)
    {
        Data = data;
        _transpose = trans;
    }

    public Matrix(double[,] data)
    {
        Data = data;
    }

    public Matrix(int x, int y)
    {
        Data = new double[x, y];
    }

    /// <summary>
    ///     Return the determining of the Matrix if it exist. Will Throw if matrix is not
    ///     Square.
    ///     <exception cref="InvalidMatrixOperation"> if Matrix not square</exception>
    /// </summary>
    public double D => _det ?? CalcDetermining();

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
    ///     Calculates the determining of a square matrix.
    /// </summary>
    /// <returns>The determining of the Matrix</returns>
    /// <exception cref="NotImplementedException"></exception>
    private double CalcDetermining()
    {
        if (Rows != Cols)
            throw new InvalidMatrixOperation(
                $"Determinants are only available for square matrices, but a matrix of size {Rows}x{Cols} was provided");

        if (Rows == 2)
            return SmallDetermining();
        double det = 0;
        for (var r = 0; r <= Rows - 3; r++)
        for (var c = 0; c <= Cols - 3; c++)
            det += Data[(0 + r) % Rows, (0 + c) % Cols]
                   * Data[(1 + r) % Rows, (1 + c) % Cols]
                   * Data[(2 + r) % Rows, (2 + c) % Cols]
                   // middle
                   + Data[(1 + r) % Rows, (0 + c) % Cols]
                   * Data[(2 + r) % Rows, (1 + c) % Cols]
                   * Data[(0 + r) % Rows, (2 + c) % Cols]
                   // right
                   + Data[(2 + r) % Rows, (0 + c) % Cols]
                   * Data[(0 + r) % Rows, (1 + c) % Cols]
                   * Data[(1 + r) % Rows, (2 + c) % Cols]
                   // subtracts
                   - Data[(2 + r) % Rows, (0 + c) % Cols]
                   * Data[(1 + r) % Rows, (1 + c) % Cols]
                   * Data[(0 + r) % Rows, (2 + c) % Cols]
                   // middle
                   - Data[(2 + r) % Rows, (1 + c) % Cols]
                   * Data[(1 + r) % Rows, (2 + c) % Cols]
                   * Data[(0 + r) % Rows, (0 + c) % Cols]
                   // right
                   - Data[(2 + r) % Rows, (2 + c) % Cols]
                   * Data[(1 + r) % Rows, (0 + c) % Cols]
                   * Data[(0 + r) % Rows, (1 + c) % Cols];
        _det = det; // assign value so  calculation has to be run only once
        return det;
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