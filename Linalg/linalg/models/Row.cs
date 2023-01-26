namespace MathTools;

public class Row
{
    //TODO: Documentation
    public Row(params double[] data)
    {
        this.data = data;
    }

    public double[] data { get; set; }
    public int Length => data.Length;

    public double this[int i]
    {
        get => data[i];
        set => data[i] = value;
    }

    public static Row operator *(Row fst, double alph)
    {
        var tmp = new double[fst.Length];
        for (var i = 0; i < fst.Length; i++) tmp[i] = fst[i] * alph;
        return new Row(tmp);
    }


    public static Row operator /(double alph, Row fst)
    {
        return 1 / alph * fst;
    }


    public static Row operator /(Row fst, double alph)
    {
        return 1 / alph * fst;
    }

    public static Row operator *(double alph, Row fst)
    {
        return fst * alph;
    }

    public static Row operator -(Row fst, Row snd)
    {
        return fst + snd * -1;
    }

    public static Row operator +(Row fst, Row snd)
    {
        if (fst.Length != snd.Length) throw new InvalidOperationException("Rows of different size cannot be added");
        var tmp = new double[fst.Length];
        for (var i = 0; i < fst.Length; i++) tmp[i] = fst[i] + snd[i];

        return new Row(tmp);
    }
}