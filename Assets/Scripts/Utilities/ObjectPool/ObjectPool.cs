// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="ObjectPool.cs" author="Lars-Kristian Svenoy" company="Cardlike">
// //  Copyright @ 2018 Cardlike. All rights reserved.
// // </copyright>
// // <summary>
// //   TODO - Insert file description
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace Utilities.ObjectPool
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;

    [Serializable]
    public class ObjectPool
    {
        public int Amount;

        public GameObject Prefab;

        public bool CanExpand = true;

        private readonly List<IPoolable> objects = new List<IPoolable>();

        public Type PoolType { get; private set; }

        public T GetPooled<T>()
            where T : class, IPoolable
        {
            foreach (var obj in this.objects)
                if (!obj.IsEnabled)
                {
                    obj.Enable();
                    return (T)obj;
                }

            if (this.CanExpand)
            {
                var newObj = GameObject.Instantiate(this.Prefab).GetComponent<IPoolable>();
                newObj.Disable();
                this.objects.Add(newObj);

                newObj.Enable();
                return (T)newObj;
            }

            return null;
        }

        public void Initialize()
        {
            var poolable = this.Prefab.GetComponent<IPoolable>();
            if (poolable == null)
                throw new InvalidOperationException($"ObjectPool Prefab {this.Prefab.name} is not IPoolable");

            this.PoolType = poolable.GetType();

            for (var i = 0; i < this.Amount; i++)
            {
                var newObj = GameObject.Instantiate(this.Prefab);
                newObj.name = Prefab.name;

                var poolObj = newObj.GetComponent<IPoolable>();
                poolObj.Disable();
                this.objects.Add(poolObj);
            }
        }
    }
}