// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Helpers
{
    #region Optimization
    // Camera.main used to be slow but now it's fine. So this method is redundant.
    // public static Camera _camera;
    // public static Camera Camera
    // {
    //     get
    //     {
    //         if (_camera == null) _camera = Camera.main;
    //         return _camera;
    //     }
    // }

    // WaitForSeconds cache
    static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();
    public static WaitForSeconds GetWait(float time)
    {
        if (WaitDictionary.TryGetValue(time, out var wait)) return wait;
        WaitDictionary[time] = new WaitForSeconds(time);
        return WaitDictionary[time];
    }
    #endregion

    #region UI
    // If cursor/finger click is over UI
    static PointerEventData _eventDataCurrentPosition;
    static List<RaycastResult> _results;
    public static bool IsOverUI()
    {
        _eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        _results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
        return _results.Count > 0;
    }

    // Get world position of canvas element (use with camera canvas mode)
    public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, Camera.main, out var result);
        return result;
    }
    #endregion

    #region Enumerator Extensions
    // Similar to Children on the transform
    public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> enumerator)
    {
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }
    #endregion

    #region Transform
    // Delete all children, the other one is better since it uses direct indexing and is more safer since it iterates backwards
    // public static void DeleteChildren(this Transform t)
    // {
    //     foreach (Transform child in t) Object.Destroy(child.gameObject);
    // }

    public static IEnumerable<Transform> Children(this Transform parent)
    {
        foreach (Transform child in parent)
        {
            yield return child;
        }
    }

    static void PerformActionOnChildren(this Transform parent, System.Action<Transform> action)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            action(parent.GetChild(i));
        }
    }

    public static void DestroyChildren(this Transform parent)
    {
        parent.PerformActionOnChildren(child => Object.Destroy(child.gameObject));
    }

    public static void EnableChildren(this Transform parent)
    {
        parent.PerformActionOnChildren(child => child.gameObject.SetActive(true));
    }

    public static void DisableChildren(this Transform parent)
    {
        parent.PerformActionOnChildren(child => child.gameObject.SetActive(false));
    }

    #endregion

    #region Vector3
    // Quickly change say y of a vector
    public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
    }

    // Quickly add to a vector
    public static Vector3 Add(this Vector3 vector, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(vector.x + (x ?? 0), vector.y + (y ?? 0), vector.z + (z ?? 0));
    }
    #endregion

    #region GameObject
    // AddComponent if doesn't exist
    public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
            component = gameObject.AddComponent<T>();
        return component;
    }

    // C# null, used for null coalesing and propagation where Unity null fails
    public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;

    // Destroy children
    public static void DestroyChildren(this GameObject gameObject)
    {
        gameObject.transform.DestroyChildren();
    }
    #endregion
}
