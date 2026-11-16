using System;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Grand nombre pour l'économie : valeur = mantisse × 1000^exposant.
/// L'exposant correspond à l'index du suffixe (0 = rien, 1 = K, 2 = M...).
/// Toute la normalisation (passer de 999 999 à 999,9K, etc.) est faite ici, une seule fois.
/// </summary>
[Serializable]
public struct BigNumber : IComparable<BigNumber>, IEquatable<BigNumber>
{
    private const double Base = 1000d;

    // Au-delà de cet écart d'exposant, le plus petit nombre est négligeable (< 1e-18 du plus grand).
    private const int MaxExponentGap = 6;

    // Les anciens champs double2 s'appelaient x / y : FormerlySerializedAs garde les valeurs
    // déjà enregistrées dans les assets et la scène.
    [FormerlySerializedAs("x")] [SerializeField] private double mantissa;
    [FormerlySerializedAs("y")] [SerializeField] private int exponent;

    public static readonly BigNumber Zero = default;
    public static readonly BigNumber One = new BigNumber(1d);

    public double Mantissa => mantissa;
    public int Exponent => exponent;
    public bool IsZero => mantissa == 0d;

    public BigNumber(double mantissa, int exponent = 0)
    {
        this.mantissa = mantissa;
        this.exponent = exponent;
        Normalize(ref this.mantissa, ref this.exponent);
    }

    private static void Normalize(ref double mantissa, ref int exponent)
    {
        if (mantissa == 0d || double.IsNaN(mantissa) || double.IsInfinity(mantissa))
        {
            mantissa = 0d;
            exponent = 0;
            return;
        }

        while (Math.Abs(mantissa) >= Base)
        {
            mantissa /= Base;
            exponent++;
        }

        while (Math.Abs(mantissa) < 1d && exponent > 0)
        {
            mantissa *= Base;
            exponent--;
        }

        if (exponent < 0)
        {
            mantissa *= Math.Pow(Base, exponent);
            exponent = 0;
        }
    }

    /// <summary>Valeur approchée en double (déborde vers l'infini pour les nombres énormes).</summary>
    public double ToDouble() => mantissa * Math.Pow(Base, exponent);

    public BigNumber Round(int digits) => new BigNumber(Math.Round(mantissa, digits), exponent);

    public static implicit operator BigNumber(double value) => new BigNumber(value);

    public static BigNumber operator +(BigNumber a, BigNumber b)
    {
        if (a.IsZero) return b;
        if (b.IsZero) return a;
        if (a.exponent < b.exponent) (a, b) = (b, a);

        int gap = a.exponent - b.exponent;
        if (gap > MaxExponentGap) return a;
        return new BigNumber(a.mantissa + b.mantissa / Math.Pow(Base, gap), a.exponent);
    }

    public static BigNumber operator -(BigNumber a) => new BigNumber(-a.mantissa, a.exponent);
    public static BigNumber operator -(BigNumber a, BigNumber b) => a + (-b);
    public static BigNumber operator *(BigNumber a, double factor) => new BigNumber(a.mantissa * factor, a.exponent);
    public static BigNumber operator /(BigNumber a, double divisor) => new BigNumber(a.mantissa / divisor, a.exponent);

    public int CompareTo(BigNumber other)
    {
        int sign = Math.Sign(mantissa);
        int otherSign = Math.Sign(other.mantissa);
        if (sign != otherSign) return sign.CompareTo(otherSign);
        if (sign == 0) return 0;
        if (exponent != other.exponent)
        {
            return sign > 0 ? exponent.CompareTo(other.exponent) : other.exponent.CompareTo(exponent);
        }
        return mantissa.CompareTo(other.mantissa);
    }

    public static bool operator <(BigNumber a, BigNumber b) => a.CompareTo(b) < 0;
    public static bool operator >(BigNumber a, BigNumber b) => a.CompareTo(b) > 0;
    public static bool operator <=(BigNumber a, BigNumber b) => a.CompareTo(b) <= 0;
    public static bool operator >=(BigNumber a, BigNumber b) => a.CompareTo(b) >= 0;
    public static bool operator ==(BigNumber a, BigNumber b) => a.Equals(b);
    public static bool operator !=(BigNumber a, BigNumber b) => !a.Equals(b);

    public bool Equals(BigNumber other) => mantissa.Equals(other.mantissa) && exponent == other.exponent;
    public override bool Equals(object obj) => obj is BigNumber other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(mantissa, exponent);
    public override string ToString() => MoneyFormatter.Format(this);
}
