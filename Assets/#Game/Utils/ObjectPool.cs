namespace _Game.Utils
{using System.Collections.Generic;
    using UnityEngine;

    public class ObjectPool<T> where T : MonoBehaviour
    {
        private readonly T prefab;
        private readonly Queue<T> pool = new Queue<T>();
        private Transform parent;

        public ObjectPool(T prefab, int initialSize, Transform parent = null)
        {
            this.prefab = prefab;
            this.parent = parent;

            for (int i = 0; i < initialSize; i++)
            {
                T newObj = Object.Instantiate(prefab, parent);
                newObj.gameObject.SetActive(false);
                pool.Enqueue(newObj);
            }
        }

        public T GetObject()
        {
            T obj = pool.Count > 0 ? pool.Dequeue() : Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void ReturnObject(T obj)
        {
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

}