using System.Diagnostics;
using MathTools;
using NUnit.Framework;

namespace MathTools_TestLib;

public class InvalidMatrixOperationTest
{
    private void ThrowInvalidMatrixException()
    {
        throw new InvalidMatrixOperation(new Matrix(0, 0), new Matrix(0, 0));
    }

    [Test]
    public void CallerNameTest()
    {
        try
        {
            ThrowInvalidMatrixException();
        }
        catch (InvalidMatrixOperation e)
        {
            Debug.Assert(e.Message.Contains("ThrowInvalidMatrixException"));
        }
    }
}