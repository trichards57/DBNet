using static Microsoft.VisualBasic.Conversion;
using static System.Math;

internal static class Bitwise
{
    public static DoubleWord BitAND(DoubleWord bitsA, DoubleWord bitsB)
    {
        DoubleWord BitAND = null;

        for (var counter = 0; counter < 31; counter++)
        {
            BitAND.bit[counter] = bitsA.bit[counter] && bitsB.bit[counter];
        }
        return BitAND;
    }

    public static DoubleWord BitOR(DoubleWord bitsA, DoubleWord bitsB)
    {
        DoubleWord BitOR = null;

        for (var counter = 0; counter < 31; counter++)
        {
            BitOR.bit[counter] = bitsA.bit[counter] || bitsB.bit[counter];
        }
        return BitOR;
    }

    public static void BitShiftLeft(DoubleWord bits)
    {
        for (var counter = 31; counter > 1; counter--)
        {
            bits.bit[counter] = bits.bit[counter - 1];
        }

        bits.bit[0] = false;
    }

    public static void BitShiftRight(DoubleWord bits)
    {
        var lastbit = bits.bit[31];

        for (var counter = 0; counter < 30; counter++)
        {
            bits.bit[counter] = bits.bit[counter + 1];
        }

        bits.bit[31] = lastbit;
    }

    public static int BitToNumber(DoubleWord bits)
    {
        var BitToNumber = 0;
        var negative = false;

        if (bits.bit[31] == true)
        {
            //negative value, so subtract 1 then invert bits
            //to get magnitude
            negative = true;
            DecBits(bits);
            InvertBits(bits);
        }

        for (var counter = 0; counter < 30; counter++)
        { //31st bit is always zero at this point
            BitToNumber += (int)Pow(2, counter) * (bits.bit[counter] ? 1 : 0);
        }

        if (negative)
        {
            BitToNumber = -BitToNumber;
        }
        return BitToNumber;
    }

    public static DoubleWord BitXOR(DoubleWord bitsA, DoubleWord bitsB)
    {
        DoubleWord BitXOR = null;

        for (var counter = 0; counter < 31; counter++)
        {
            BitXOR.bit[counter] = bitsA.bit[counter] ^ bitsB.bit[counter];
        }
        return BitXOR;
    }

    public static void DecBits(DoubleWord bits)
    {
        for (var counter = 0; counter < 31; counter++)
        {
            if (bits.bit[counter] == true)
            {
                bits.bit[counter] = false;
                return;//we're done
            }
            else
            {
                //we have to borrow bits
                bits.bit[counter] = true;
            }
        }
    }

    public static void IncBits(DoubleWord bits)
    {
        for (var counter = 0; counter < 31; counter++)
        {
            if (bits.bit[counter] == false)
            {
                bits.bit[counter] = true;
                return;//we're done
            }
            else
            {
                //we have to carry bits
                bits.bit[counter] = false;
            }
        }
    }

    public static void InvertBits(DoubleWord bits)
    {
        for (var counter = 0; counter < 31; counter++)
        {
            bits.bit[counter] = !bits.bit[counter];
        }
    }

    public static DoubleWord NumberToBit(int value)
    {
        DoubleWord NumberToBit = null;
        var negative = false;

        if (value < 0)
        {
            negative = true;
            value = -value;
        }

        for (var counter = 30; counter > 0; counter--)
        {
            if (value / Int(2 ^ counter) == 1)
            {
                //bit is true
                NumberToBit.bit[counter] = true;
                value = value - 2 ^ counter;
            }
            else
            {
                NumberToBit.bit[counter] = false;
            }
        }

        if (negative)
        {
            //invert bits then add 1
            InvertBits(NumberToBit);
            IncBits(NumberToBit);
        }
        return NumberToBit;
    }

    public class DoubleWord
    {
        public bool[] bit = new bool[31];
    }
}
