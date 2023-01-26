using System.Runtime.CompilerServices;

namespace MathTools;

public class InvalidMatrixOperation : Exception
{
    public InvalidMatrixOperation(Matrix lft, Matrix rgt, [CallerMemberName] string name = null) : base(
        $"The Matrix Operation {name} between matrices of {lft.Rows}x{lft.Cols} and {rgt.Rows}x{rgt.Cols} is invalid.")
    {
    }

    public InvalidMatrixOperation(string msg) : base(msg)
    {
    }

    public InvalidMatrixOperation(int row, Matrix m, [CallerMemberName] string name = null) : base(
        $"The Operation {name} caused an error because the index: [{row}] was unreachable in the Matrix m=[{m.Rows}x{m.Cols}]")
    {
    }
}