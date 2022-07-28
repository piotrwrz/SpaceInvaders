using System;
using System.Collections.Concurrent;
using UnityEngine;

public class ObjectPool<T>
{
    private readonly ConcurrentBag<T> objects;
    private readonly Func<T> objectGenerator;

    public ObjectPool(Func<T> generator)
    {
        objectGenerator = generator ?? throw new ArgumentNullException(nameof(generator));
        objects = new ConcurrentBag<T>();
    }

    public T Get() => objects.TryTake(out T item) ? item : objectGenerator();

    public void Return(T item) => objects.Add(item);
}