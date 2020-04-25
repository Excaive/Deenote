using System.Collections.Generic;
using UnityEngine;

namespace Deenote
{
    public sealed class ObjectPool<T> where T : Object
    {
        public delegate void Callback(T item);

        private readonly List<T> _objects = new List<T>();
        private readonly List<bool> _available = new List<bool>();
        private readonly T _prefab;
        private readonly Transform _parent;
        public event Callback InitCallback;
        public event Callback GetCallback;
        public event Callback ReturnCallback;

        public ObjectPool(T prefab, Transform parent = null)
        {
            this._prefab = prefab;
            this._parent = parent;
        }

        public int Size
        {
            get => _objects.Count;
            set
            {
                for (int i = _objects.Count; i < value; i++)
                {
                    InstantiateNew();
                    _available.Add(true);
                }
            }
        }

        private T InstantiateNew()
        {
            T newObject = !(_parent is null)
                ? Object.Instantiate(_prefab, _parent)
                : Object.Instantiate(_prefab);
            InitCallback?.Invoke(newObject);
            _objects.Add(newObject);
            return newObject;
        }

        public T GetObject()
        {
            for (int i = 0; i < _available.Count; i++)
                if (_available[i])
                {
                    _available[i] = false;
                    GetCallback?.Invoke(_objects[i]);
                    return _objects[i];
                }
            T newObject = InstantiateNew();
            _available.Add(false);
            GetCallback?.Invoke(newObject);
            return newObject;
        }

        public void ReturnObject(T returnedObject)
        {
            for (int i = 0; i < _objects.Count; i++)
                if (_objects[i] == returnedObject)
                {
                    _available[i] = true;
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
