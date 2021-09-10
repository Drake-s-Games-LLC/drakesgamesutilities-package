using System;
using System.Collections.Generic;
using UnityEngine;

public interface IRandomGenerator
{
    int Next();
    float NextFloat();
}

public static class RandomGenerator
{
    #region Random

    public static int Next(this IRandomGenerator r, int max)
    {
        return (int)(r.NextFloat() * max);
    }

    public static int Next(this IRandomGenerator r, int min, int max)
    {
        return min + (int)(r.NextFloat() * (max - min));
    }

    public static bool NextBool(this IRandomGenerator r)
    {
        return r.Next(2) == 0;
    }

    public static int NextSign(this IRandomGenerator r)
    {
        return (r.NextBool() == true) ? 1 : -1;
    }

    public static float NextFloat(this IRandomGenerator r, float max)
    {
        return r.NextFloat() * max;
    }

    public static float NextFloat(this IRandomGenerator r, float min, float max)
    {
        return min + r.NextFloat() * (max - min);
    }

    public static float NextFloatDual(this IRandomGenerator r, float min, float max)
    {
        var delta = max - min;
        var value = r.NextFloat(-delta, delta);
        float result;
        if (value < 0)
        {
            result = -min + value;
        }
        else
        {
            result = min + value;
        }
        return result;
    }

    public static Vector2 NextInsideUnitCircle(this IRandomGenerator r)
    {
        var angle = r.NextFloat() * Mathf.PI * 2;
        var radius = Mathf.Sqrt(r.NextFloat());
        var x = radius * Mathf.Cos(angle);
        var y = radius * Mathf.Sin(angle);
        return new Vector2(x, y);
    }

    public static Vector3 NextInsideUnitCircleXZ(this IRandomGenerator r)
    {
        var angle = r.NextFloat() * Mathf.PI * 2;
        var radius = Mathf.Sqrt(r.NextFloat());
        var x = radius * Mathf.Cos(angle);
        var z = radius * Mathf.Sin(angle);
        return new Vector3(x, 0f, z);
    }

    public static T NextOf2<T>(this IRandomGenerator r, T element1, T element2)
    {
        return (r.NextFloat() < 0.5f) ? element1 : element2;
    }

    public static T NextOf2<T>(this IRandomGenerator r, T element1, float chance1, T element2, float chance2)
    {
        return (r.NextFloat(chance1 + chance2) < chance1) ? element1 : element2;
    }

    public static T NextOf3<T>(this IRandomGenerator r, T element1, T element2, T element3)
    {
        var num = r.Next(3);
        switch (num)
        {
            case 0:
                return element1;
            case 1:
                return element2;
            case 2:
                return element3;
        }
        throw new Exception("Unreachable code exception!");
    }

    public static T NextOf3<T>(this IRandomGenerator r, T element1, float chance1, T element2, float chance2, T element3, float chance3)
    {
        var num = r.NextFloat(chance1 + chance2 + chance3);
        if (num < chance1)
        {
            return element1;
        }
        else if (num < chance1 + chance2)
        {
            return element2;
        }
        else
        {
            return element3;
        }
    }

    public static void NextBytes(this IRandomGenerator r, byte[] buffer)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = (byte)r.Next();
        }
    }

    public static void NextSplit(this IRandomGenerator r, int buckets, List<float> result, float numberToSplit = 1f)
    {
        result.Clear();
        // Add split points
        for (int i = 1; i < buckets; i++)
        {
            result.Add(r.NextFloat());
        }

        // Sort split points
        result.Sort();

        // Add last split point as 1, to use the same calculation for it
        result.Add(1f);

        // Calculate the actual values (distance between split points multiplied by the value that we're splitting)
        for (int i = buckets - 1; i > 0; i--)
        {
            result[i] = (result[i] - result[i - 1]) * numberToSplit;
        }
        result[0] *= numberToSplit;
    }

    public static Vector2 NextDirection2D(this IRandomGenerator r)
    {
        var angle = r.NextFloat(360f) * Mathf.Deg2Rad;
        var result = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        return result;
    }

    public static Vector3 NextDirection3D(this IRandomGenerator r)
    {
        var angle = r.NextFloat(360f) * Mathf.Deg2Rad;
        var z = r.NextFloat(-1f, 1f);
        var mp = Mathf.Sqrt(1 - z * z);
        var x = mp * Mathf.Cos(angle);
        var y = mp * Mathf.Sin(angle);
        return new Vector3(x, y, z);
    }

    #endregion
}

public class StandardRandomGenerator : IRandomGenerator
{
    private System.Random random;

    public StandardRandomGenerator(System.Random random)
    {
        this.random = random;
    }

    public int Next()
    {
        return this.random.Next();
    }

    public float NextFloat()
    {
        return (float)this.random.NextDouble();
    }
}

// The implementation may not be perfect, but it only exists for completeness.
public class UnityRandomGenerator : IRandomGenerator
{
    public int Next()
    {
        var part1 = UnityEngine.Random.Range(0, 1 << 16);
        var part2 = UnityEngine.Random.Range(0, 1 << 16);
        var result = part1 << 16 + part2;
        return result;
    }

    public float NextFloat()
    {
        return UnityEngine.Random.value;
    }
}