//                       _oo0oo_
//                      o8888888o
//                      88" . "88
//                      (| -_- |)
//                      0\  =  /0
//                    ___/`---'\___
//                  .' \\|     |// '.
//                 / \\|||  :  |||// \
//                / _||||| -:- |||||- \
//               |   | \\\  -  /// |   |
//               | \_|  ''\---/''  |_/ |
//               \  .-\__  '-'  ___/-. /
//             ___'. .'  /--.--\  `. .'___
//          ."" '<  `.___\_<|>_/___.' >' "".
//         | | :  `- \`.;`\ _ /`;.`/ - ` : | |
//         \  \ `_.   \_ __\ /__ _/   .-` /  /
//     =====`-.____`.___ \_____/___.-`___.-'=====
//                       `=---='
//
//     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//            Code không BUG đời không nể
//     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace ExtentionUnity
{
    public static class ExtentionUnity 
    {
        private static readonly Dictionary<GameObject, List<GameObject>> ObjectPools = new Dictionary<GameObject, List<GameObject>>();
        /// <summary>
        /// Initializes an object pool for the specified prefab.
        /// </summary>
        public static GameObject CreatePool(this GameObject prefab)
        {
            GameObject obj = UnityEngine.Object.Instantiate(prefab);
            obj.SetActive(false);
            ObjectPools[prefab].Add(obj);
            obj.transform.position = prefab.transform.position;
            return obj;
        }/// <summary>
        /// Creates a pool of objects for the specified prefab with a specified size.
        /// </summary>
        /// <param name="poolSize">The desired size of the object pool.</param>
        public static void CreatePool(this GameObject prefab, int poolSize)
        {
            for(int i = 0; i < poolSize; i++)
            {
                prefab.CreatePool();
            }
        }
        /// <summary>
        /// The method clones the original object and returns the clone using the Object Pool design pattern.
        /// </summary>
        /// <returns>The spawned or newly created GameObject.</returns>
        public static GameObject Spawn(this GameObject prefab)
        {
            if (!ObjectPools.ContainsKey(prefab))
            {
                ObjectPools[prefab] = new List<GameObject>();
            }

            foreach (var item in ObjectPools[prefab])
            {
                if(!item.activeInHierarchy)
                {
                    item.SetActive(true);
                    item.transform.position = prefab.transform.position;
                    return item;
                }
            }
            GameObject obj = prefab.CreatePool();
            obj.SetActive(true);
            return obj;
        }
        /// <summary>
        /// The method clones the original object and returns the clone using the Object Pool design pattern.
        /// </summary>
        /// <param name="parent">The parent transform to which the new object will be assigned.</param>
        /// <returns>The spawned or newly created GameObject.</returns>
        public static GameObject Spawn(this GameObject prefab, Transform parent)
        {
            GameObject obj = prefab.Spawn();
            obj.transform.SetParent(parent);
            return obj;
        }
        /// <summary>
        /// The method clones the original object and returns the clone using the Object Pool design pattern.
        /// </summary>
        /// <param name="position">The position at which the spawned object will be placed.</param>
        /// <returns>The spawned or newly created GameObject.</returns>
        public static GameObject Spawn(this GameObject prefab, Vector3 position)
        {
            GameObject obj = prefab.Spawn();
            obj.transform.position = position;
            return obj;
        }
        /// <summary>
        /// The method clones the original object and returns the clone using the Object Pool design pattern.
        /// </summary>
        /// <param name="position">The position at which the spawned object will be placed.</param>
        /// <param name="parent">The parent transform to which the new object will be assigned.</param>
        /// <returns>The spawned or newly created GameObject.</returns>
        public static GameObject Spawn(this GameObject prefab, Vector3 position, Transform parent)
        {
            GameObject obj = prefab.Spawn(position);
            obj.transform.SetParent(parent);
            return obj;
        }
        /// <summary>
        /// The method clones the original object and returns the clone using the Object Pool design pattern.
        /// </summary>
        /// <param name="position">The position at which the spawned object will be placed.</param>
        /// <param name="rotation">The rotation that will be applied to the spawned object.</param>
        /// <returns>The spawned or newly created GameObject.</returns>
        public static GameObject Spawn(this GameObject prefab, Vector3 position, Quaternion rotation)
        {
            GameObject obj = prefab.Spawn(position);
            obj.transform.rotation = rotation;
            return obj;
        }
        /// <summary>
        /// The method clones the original object and returns the clone using the Object Pool design pattern.
        /// </summary>
        /// <param name="position">The position at which the spawned object will be placed.</param>
        /// <param name="rotation">The rotation that will be applied to the spawned object.</param>
        /// <param name="parent">The parent transform to which the new object will be assigned.</param>
        /// <returns>The spawned or newly created GameObject.</returns>
        public static GameObject Spawn(this GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject obj = prefab.Spawn(position, rotation);
            obj.transform.SetParent(parent);
            return obj;
        }
        /// <summary>
        /// Return the number of pools created.
        /// </summary>
        public static int CountPooled(this GameObject prefab) => ObjectPools.ContainsKey(prefab) ? ObjectPools[prefab].Count : 0;
        /// <summary>
        /// Return the number of pools currently in use.
        /// </summary>
        public static int CountSpawned(this GameObject prefab)
        {
            int count = 0;
            foreach (GameObject item in ObjectPools[prefab])
            {
                if(item.activeInHierarchy)
                {
                    count++;
                }
            }
            return count;
        }
        /// <summary>
        /// "Destroy and remove all pools from memory."
        /// </summary>
        public static void DestroyAll(this GameObject prefab)
        {
            if(!ObjectPools.ContainsKey(prefab)) return;
            foreach (GameObject item in ObjectPools[prefab])
            {
                UnityEngine.Object.Destroy(item);
            }
            ObjectPools[prefab].Clear();
        }
        /// <summary>
        /// Deactivate the object pool and put it into memory cache.
        /// </summary>
        public static void Recycle(this GameObject prefab)
        {
            prefab.SetActive(false);
            prefab.transform.position = new Vector3(0f, -99999f, 0f);
        }
        /// <summary>
        /// Deactivate all object pools and put them into memory cache.
        /// </summary>
        public static void RecycleAll(this GameObject prefab)
        {
            foreach (GameObject item in ObjectPools[prefab])
            {
                item.Recycle();
            }
        }
        /// <summary>
        /// Perform actions after a period of time.
        /// </summary>
        /// <param name="timeDelay">Delay time for executing an action.</param>
        /// <param name="action">The action is performed.</param>
        public static Coroutine ActionWaitTime(this MonoBehaviour mono, float timeDelay, Action action)
        {
            Coroutine newCoroutine = mono.StartCoroutine(NewAction(timeDelay, action));
            return newCoroutine;
        }
        private static IEnumerator NewAction(float timeDelay, Action action)
        {
            yield return new WaitForSeconds(timeDelay);
            action();
        }
    }
}