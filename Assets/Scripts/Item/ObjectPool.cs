using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private T prefab;
    private Transform parent;
    private int initialSize = 10;
    private Queue<T> pool = new Queue<T>();

    public ObjectPool(T prefab, int initialSize = 10, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;
        pool = new Queue<T>();
        InitPool(initialSize);
    }

    /// <summary>
    /// Initialize pool with given size.
    /// </summary>
    private void InitPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            T obj = Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }


    /// <summary>
    /// Get object from pool. Expands if empty.
    /// </summary>
    public T Get(Transform newParent = null)
    {
        if (pool.Count == 0)
        {
            ExpandPool(5);
        }

        T obj = pool.Dequeue();
        obj.gameObject.SetActive(true);
        obj.transform.SetParent(newParent ?? parent);
        return obj;
    }

    /// <summary>
    /// Return object back to pool.
    /// </summary>
    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(parent);
        pool.Enqueue(obj);
    }

    /// <summary>
    /// Expand pool with more objects.
    /// </summary>
    public void ExpandPool(int count)
    {
        InitPool(count);
    }
}