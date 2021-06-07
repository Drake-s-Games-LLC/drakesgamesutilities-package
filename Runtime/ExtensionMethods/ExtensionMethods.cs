using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public static class ExtensionMethods
{
    #region Array

    public static T[] CopyAndResize<T>(this T[] arr, int addCount, int startIndex = 0)
    {
        var result = new T[arr.Length + addCount];
        Array.Copy(arr, 0, result, startIndex, arr.Length);
        return result;
    }

    public static T[] CopyAndRemoveFirst<T>(this T[] arr, int removeCount)
    {
        var result = new T[arr.Length - removeCount];
        Array.Copy(arr, removeCount, result, 0, arr.Length - removeCount);
        return result;
    }

    #endregion

    #region Dictionary

    public static V GetValueOrDefault<K, V>(this IDictionary<K, V> dictionary, K key)
    {
        V result;
        dictionary.TryGetValue(key, out result);
        return result;
    }

    public static V GetValueOrDefault<K, V>(this IDictionary<K, V> dictionary, K key, V defaultValue)
    {
        V result;
        if (dictionary.TryGetValue(key, out result) == false)
        {
            result = defaultValue;
        }
        return result;
    }

    #endregion

    #region MonoBehaviour

    public static void StartCoroutine(this MonoBehaviour mb, IEnumerable coroutine)
    {
        mb.StartCoroutine(coroutine.GetEnumerator());
    }

    public static void ExecuteDelayedByFrames(this MonoBehaviour mb, int framesDelay, Action action)
    {
        mb.StartCoroutine(ExecuteDelayedByFramesCoroutine(framesDelay, action));
    }

    public static void ExecuteDelayedByTime(this MonoBehaviour mb, Action action, float time)
    {
        mb.StartCoroutine(ExecuteDelayedByTimeCoroutine(time, action));
    }

    private static IEnumerator ExecuteDelayedByFramesCoroutine(int framesDelay, Action action)
    {
        for (int i = 0; i < framesDelay; i++)
        {
            yield return null;
        }
        action();
    }

    private static IEnumerator ExecuteDelayedByTimeCoroutine(float timeDelay, Action action)
    {
        yield return new WaitForSeconds(timeDelay);
        action();
    }

    #endregion

    #region GameObject

    public static T GetOrAddComponent<T>(this GameObject go, bool searchChildren = false) where T : Component
    {
        var component = searchChildren == true ? go.GetComponentInChildren<T>() : go.GetComponent<T>();
        if (component == null)
        {
            component = go.AddComponent<T>();
        }
        return component;
    }

    public static void SetLayerRecursively(this GameObject go, int layerNumber)
    {
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    public static void SetLayerRecursively(this GameObject go, string layerName)
    {
        ExtensionMethods.SetLayerRecursively(go, LayerMask.NameToLayer(layerName));
    }

    #endregion

    #region Transform

    public static void DestroyChildren(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public static Vector3 TransformPointTo(this Transform from, Transform to, Vector3 point)
    {
        var world = from.InverseTransformPoint(point);
        var localInTo = to.TransformPoint(world);
        return localInTo;
    }

    #endregion

    #region RectTransform

    private static Vector3[] cornerCoords = new Vector3[4];

    public static Vector3 GetWorldCenter(this RectTransform tr)
    {
        tr.GetWorldCorners(cornerCoords);
        var width = cornerCoords[3].x - cornerCoords[0].x;
        var height = cornerCoords[1].y - cornerCoords[0].y;
        var result = cornerCoords[0] + new Vector3(width / 2f, height / 2f);
        return result;
    }

    public static Vector2 GetLocalCenter(this RectTransform tr)
    {
        var delta = new Vector2((0.5f - tr.pivot.x) * tr.sizeDelta.x, (0.5f - tr.pivot.y) * tr.sizeDelta.y);
        return tr.localPosition + (Vector3)delta;
    }

    public static void ChangePivot(this RectTransform tr, Vector2 newPivot)
    {
        var delta = newPivot - tr.pivot;
        var distance = new Vector2(delta.x * tr.sizeDelta.x, delta.y * tr.sizeDelta.y);
        tr.localPosition += (Vector3)distance;

        tr.pivot = newPivot;
    }

    #endregion

    #region Strings

    public static string ToUpperFirst(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }
        return char.ToUpper(text[0]) + text.Substring(1).ToLower();
    }

    public static bool ContainsSplit(this string container, string value)
    {
        if (value.Length > container.Length)
        {
            return false;
        }

        for (int i = 0, j = 0; i < container.Length; i++)
        {
            if (container[i] == value[j])
            {
                j++;
                if (j == value.Length)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool ContainsSplit(this string container, string content, out int splits)
    {
        if (content.Length > container.Length)
        {
            splits = -1;
            return false;
        }

        splits = 0;
        bool lastMatched = false;
        for (int i = 0, j = 0; i < container.Length; i++)
        {
            if (container[i] == content[j])
            {
                j++;
                lastMatched = true;
                if (j == content.Length)
                {
                    return true;
                }
            }
            else
            {
                if (lastMatched == true)
                {
                    splits++;
                }
                lastMatched = false;
            }
        }

        splits = -1;
        return false;
    }

    #endregion

    #region Bool

    public static int AsMultiplier(this bool value)
    {
        return value == true ? 1 : -1;
    }

    #endregion

    #region Streams

    public static void WriteInt(this Stream stream, int data)
    {
        stream.WriteByte((byte)((data >> 24) & 255));
        stream.WriteByte((byte)((data >> 16) & 255));
        stream.WriteByte((byte)((data >> 8) & 255));
        stream.WriteByte((byte)((data >> 0) & 255));
    }

    #endregion

    #region Color

    public static string ToRgbaHex(this Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");
        return hex;
    }

    public static uint ToRgbaUint(this Color32 color)
    {
        uint hex = (uint)color.r << 24 | (uint)color.g << 16 | (uint)color.b << 8 | (uint)color.a << 0;
        return hex;
    }

    public static string ToArgbHex(this Color32 color)
    {
        string hex = color.a.ToString("X2") + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }

    public static uint ToArgbUint(this Color32 color)
    {
        uint hex = (uint)color.a << 24 | (uint)color.r << 16 | (uint)color.g << 8 | (uint)color.b << 0;
        return hex;
    }

    public static string ToRgbHex(this Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }

    public static uint ToRgbUint(this Color32 color)
    {
        uint hex = (uint)color.r << 16 | (uint)color.g << 8 | (uint)color.b << 0;
        return hex;
    }

    public static string ToRgbaHex(this Color color)
    {
        return ((Color32)color).ToRgbaHex();
    }

    public static uint ToRgbaUint(this Color color)
    {
        return ((Color32)color).ToRgbaUint();
    }

    public static string ToArgbHex(this Color color)
    {
        return ((Color32)color).ToArgbHex();
    }

    public static uint ToArgbUint(this Color color)
    {
        return ((Color32)color).ToArgbUint();
    }

    public static string ToRgbHex(this Color color)
    {
        return ((Color32)color).ToRgbHex();
    }

    public static uint ToRgbUint(this Color color)
    {
        return ((Color32)color).ToRgbUint();
    }

    public static Color WithAlpha(this Color color, float alpha)
    {
        var result = color;
        result.a = alpha;
        return result;
    }

    public static Color32 WithAlpha(this Color32 color, byte alpha)
    {
        var result = color;
        result.a = alpha;
        return result;
    }

    #endregion

    #region IEnumerable

    public static T Random<T>(this IEnumerable<T> elements, IRandomGenerator r)
    {
        return GetRandomElement(elements, r, default(T), false);
    }

    public static T RandomOrDefault<T>(this IEnumerable<T> elements, IRandomGenerator r, T defaultValue = default(T))
    {
        return GetRandomElement(elements, r, defaultValue, false);
    }

    private static T GetRandomElement<T>(IEnumerable<T> elements, IRandomGenerator r, T defaultValue, bool exceptionIfNoElement)
    {
        var collection = elements as ICollection<T>;
        if (elements == null || (collection != null && collection.Count == 0) || (collection == null && elements.Any() == false))
        {
            if (exceptionIfNoElement == true)
            {
                throw new InvalidOperationException("The enumerable contains no elements.");
            }
            else
            {
                return defaultValue;
            }
        }

        var count = elements.Count();

        int index = r.Next(count);

        var result = elements.ElementAt(index);

        return result;
    }

    public static T Random<T>(this IEnumerable<T> elements, IRandomGenerator r, Func<T, float> chance)
    {
        return GetRandomElement(elements, r, default(T), false);
    }

    public static T RandomOrDefault<T>(this IEnumerable<T> elements, IRandomGenerator r, Func<T, float> chance, T defaultValue = default(T))
    {
        return GetRandomElement(elements, r, defaultValue, false);
    }

    private static T GetRandomElement<T>(IEnumerable<T> elements, IRandomGenerator r, Func<T, float> chance, T defaultValue, bool exceptionIfNoElement)
    {
        var collection = elements as ICollection<T>;
        if (elements == null || (collection != null && collection.Count == 0) || (collection == null && elements.Any() == false))
        {
            if (exceptionIfNoElement == true)
            {
                throw new InvalidOperationException("The enumerable contains no elements.");
            }
            else
            {
                return defaultValue;
            }
        }

        if (chance == null)
        {
            throw new ArgumentException("The given chance function is null.");
        }

        float totalChance = elements.Sum(chance);

        if (totalChance == 0.0)
        {
            if (exceptionIfNoElement == true)
            {
                throw new InvalidOperationException("No element with a chance other than 0.");
            }
            else
            {
                return defaultValue;
            }
        }

        float selection = r.NextFloat(totalChance);

        float currentSum = 0f;
        T result = default(T);
        foreach (var element in elements)
        {
            result = element;
            currentSum += chance(element);
            if (currentSum > selection)
            {
                break;
            }
        }

        return result;
    }

    #endregion

    #region List

    public static IEnumerable<T> EnumerateAndClean<T>(this List<T> list) where T : UnityEngine.Object
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null)
            {
                yield return list[i];
            }
            else
            {
                list.RemoveAt(i);
                i--;
            }
        }
    }

    public static void Shuffle<T>(this IList<T> list, IRandomGenerator r)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = r.Next(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static List<T> Cleared<T>(this List<T> list)
    {
        list.Clear();
        return list;
    }

    public static void AddRangeNonAlloc<T>(this List<T> list, List<T> otherList)
    {
        for (int i = 0; i < otherList.Count; i++)
        {
            list.Add(otherList[i]);
        }
    }

    public static void AddRangeNonAlloc<T>(this List<T> list, T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            list.Add(array[i]);
        }
    }

    /// <summary>
    /// Removes the element at given index and moves the last element to that position to make it fast.
    /// </summary>
    /// <param name="list">The list from which an element will be removed</param>
    /// <param name="index">The index of the element to remove</param>
    public static void FastRemoveAt<T>(this List<T> list, int index)
    {
        var lastIndex = list.Count - 1;
        var element = list[lastIndex];
        list[index] = element;
        list.RemoveAt(lastIndex);
    }

    /// <summary>
    /// Removes the given element and moves the last element to its position to make it fast.
    /// </summary>
    public static void FastRemove<T>(this List<T> list, T item)
    {
        var index = list.IndexOf(item);
        if (index >= 0)
        {
            list.FastRemoveAt(index);
        }
    }

    /// <summary>
    /// Replaces the element at the given index with a "null" value.
    /// </summary>
    public static void NullifyAt<T>(this List<T> list, int index, T nullValue = default(T))
    {
        list[index] = default(T);
    }

    /// <summary>
    /// Replaces an item with a "null" value, if the item exists.
    /// </summary>
    public static void Nullify<T>(this List<T> list, T item, T nullValue = default(T))
    {
        var index = list.IndexOf(item);
        if (index >= 0)
        {
            list[index] = nullValue;
        }
    }

    #endregion

    #region NavMeshPath
    public static float Length(this NavMeshPath path)
    {
        if (path.corners.Length < 2)
        {
            return 0;
        }

        float distance = 0;
        for (int i = 1; i < path.corners.Length; i++)
        {
            distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }
        return distance;
    }

    #endregion

    #region Vectors

    public static Vector2 XY(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.y);
    }

    public static Vector2 XZ(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }

    public static Vector2 YZ(this Vector3 vec)
    {
        return new Vector2(vec.y, vec.z);
    }

    public static Vector3 WithX(this Vector3 vec, float x)
    {
        return new Vector3(x, vec.y, vec.z);
    }

    public static Vector3 WithY(this Vector3 vec, float y)
    {
        return new Vector3(vec.x, y, vec.z);
    }

    public static Vector3 WithZ(this Vector3 vec, float z)
    {
        return new Vector3(vec.x, vec.y, z);
    }

    public static Vector2 WithX(this Vector2 vec, float x)
    {
        return new Vector2(x, vec.y);
    }

    public static Vector2 WithY(this Vector2 vec, float y)
    {
        return new Vector2(vec.x, y);
    }

    public static Vector3 ToV3XY(this Vector2 vec)
    {
        return new Vector3(vec.x, vec.y, 0f);
    }

    public static Vector3 ToV3XZ(this Vector2 vec)
    {
        return new Vector3(vec.x, 0f, vec.y);
    }

    public static Vector3 ToV3YZ(this Vector2 vec)
    {
        return new Vector3(0f, vec.x, vec.y);
    }

    public static Vector2 Rotate90Cw(this Vector2 vec)
    {
        return new Vector2(vec.y, -vec.x);
    }

    public static Vector2 Rotate90Ccw(this Vector2 vec)
    {
        return new Vector2(-vec.y, vec.x);
    }

    public static Vector2 Rotate(this Vector2 vec, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = vec.x;
        float ty = vec.y;
        vec.x = (cos * tx) - (sin * ty);
        vec.y = (sin * tx) + (cos * ty);
        return vec;
    }

    public static Vector3 RotateAroundY(this Vector3 v, float degrees)
    {
        if (degrees == 0)
        {
            return v;
        }

        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float tz = v.z;
        v.x = (cos * tx) - (sin * tz);
        v.z = (sin * tx) + (cos * tz);
        return v;
    }

    public static Vector3 GetVector3FromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
    }

    public static Quaternion RotationFromVector(Vector3 dir)
    {
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(new Vector3(0, 0, angle));
    }
    #endregion

    #region Rect

    public static Vector2 ClampPoint(this Rect rect, Vector2 point)
    {
        if (point.x < rect.xMin)
        {
            point.x = rect.xMin;
        }

        if (point.x > rect.xMax)
        {
            point.x = rect.xMax;
        }

        if (point.y < rect.yMin)
        {
            point.y = rect.yMin;
        }

        if (point.y > rect.yMax)
        {
            point.y = rect.yMax;
        }

        return point;
    }

    public static Vector3 ClampPointXY(this Rect rect, Vector3 point)
    {
        if (point.x < rect.xMin)
        {
            point.x = rect.xMin;
        }

        if (point.x > rect.xMax)
        {
            point.x = rect.xMax;
        }

        if (point.y < rect.yMin)
        {
            point.y = rect.yMin;
        }

        if (point.y > rect.yMax)
        {
            point.y = rect.yMax;
        }

        return point;
    }

    public static Vector3 ClampPointXZ(this Rect rect, Vector3 point)
    {
        if (point.x < rect.xMin)
        {
            point.x = rect.xMin;
        }

        if (point.x > rect.xMax)
        {
            point.x = rect.xMax;
        }

        if (point.z < rect.yMin)
        {
            point.z = rect.yMin;
        }

        if (point.z > rect.yMax)
        {
            point.z = rect.yMax;
        }

        return point;
    }

    public static Vector3 ClampPointYZ(this Rect rect, Vector3 point)
    {
        if (point.y < rect.xMin)
        {
            point.y = rect.xMin;
        }

        if (point.y > rect.xMax)
        {
            point.y = rect.xMax;
        }

        if (point.z < rect.yMin)
        {
            point.z = rect.yMin;
        }

        if (point.z > rect.yMax)
        {
            point.z = rect.yMax;
        }

        return point;
    }

    #endregion

    public static class EnumerableHelper<E>
    {
        private static System.Random r;

        static EnumerableHelper()
        {
            r = new System.Random();
        }

        public static T Random<T>(IEnumerable<T> input)
        {
            return input.ElementAt(r.Next(input.Count()));
        }

    }

    public static T Random<T>(this IEnumerable<T> input)
    {
        return EnumerableHelper<T>.Random(input);
    }
}
