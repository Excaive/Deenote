using System.Collections.Generic;
using UnityEngine;

namespace Deenote
{
    public sealed class ObjectPool<T> where T : Object
    {
        public delegate void Callback(T item);

        private readonly List<T> objects = new List<T>();
        private readonly List<bool> available = new List<bool>();
        private readonly T prefab;
        private readonly Transform parent;
        public event Callback InitCallback;
        public event Callback GetCallback;
        public event Callback ReturnCallback;

        public ObjectPool(T prefab, Transform parent = null, int startAmount = 20)
        {
            this.prefab = prefab;
            this.parent = parent;
            for (int i = 0; i < startAmount; i++)
            {
                InstantiateNew();
                available.Add(true);
            }
        }

        private T InstantiateNew()
        {
            T newObject = !(parent is null)
                ? Object.Instantiate(prefab, parent)
                : Object.Instantiate(prefab);
            InitCallback?.Invoke(newObject);
            objects.Add(newObject);
            return newObject;
        }

        public T GetObject()
        {
            for (int i = 0; i < available.Count; i++)
                if (available[i])
                {
                    available[i] = false;
                    GetCallback?.Invoke(objects[i]);
                    return objects[i];
                }
            T newObject = InstantiateNew();
            available.Add(false);
            GetCallback?.Invoke(newObject);
            return newObject;
        }

        public void ReturnObject(T returnedObject)
        {
            for (int i = 0; i < objects.Count; i++)
                if (objects[i] == returnedObject)
                {
                    available[i] = true;
                    ReturnCallback?.Invoke(returnedObject);
                    return;
                }
            Object.Destroy(returnedObject);
#if DEBUG
            Debug.LogError("Error: Trying to return an object that's not from this pool " +
                           $"(of type {typeof(T)}), object destroyed");
#endif
        }
    }
}
