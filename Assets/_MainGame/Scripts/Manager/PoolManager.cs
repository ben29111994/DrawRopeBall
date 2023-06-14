using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PoolManager : MonoBehaviour
{
    private static PoolManager instance;
    public static PoolManager Instance { get { return instance; } }

    [Header("Pool Manager")]
    public List<ObjectPool> ObjectPools = new List<ObjectPool>();
    
    public enum NameObject
    {
        Effect_Splash,
        Effect_CubeBreak,
        Cube,
        Dot,
        Bullet
    }

    [System.Serializable]
    public class ObjectPool
    {
        [HideInInspector] public Transform parent;
        public Object objectPrefab;
        public NameObject nameObject;

        [HideInInspector]
        public List<Object> listObject = new List<Object>();
    }

    private void Awake()
    {
        instance = (instance == null) ? this : instance;
    }

    private void Start()
    {
        GenerateObjectPool();
    }

    public void GenerateObjectPool()
    {
        int count = ObjectPools.Count;

        for (int i = 0; i < count; i++)
        {
            int amount = 10;
            Object prefab = ObjectPools[i].objectPrefab;

            GameObject newParent = new GameObject();
            newParent.gameObject.name = ObjectPools[i].nameObject.ToString();
            newParent.transform.SetParent(this.transform);
            ObjectPools[i].parent = newParent.transform;
            Transform parent = ObjectPools[i].parent;

            for (int j = 0; j < amount; j++)
            {
                GameObject objectClone = Instantiate(prefab, parent) as GameObject;
                objectClone.SetActive(false);
                ObjectPools[i].listObject.Add(objectClone);
            }
        }
    }

    public Object GetObject(NameObject name)
    {
        int count = ObjectPools.Count;
        ObjectPool objectPool = null;

        for (int i = 0; i < count; i++)
        {
            if (ObjectPools[i].nameObject == name)
            {
                objectPool = ObjectPools[i];
            }
        }

        if (objectPool == null) return null;

        int childCount = objectPool.listObject.Count;

        for (int i = 0; i < childCount; i++)
        {
            Object childObject = objectPool.listObject[i];
            GameObject _go = childObject as GameObject;
            if (_go.activeInHierarchy == false)
            {
                return childObject;
            }
        }

        Object objectClone = Instantiate(objectPool.objectPrefab, objectPool.parent) as Object;
        GameObject _go2 = objectClone as GameObject;
        _go2.SetActive(false);
        objectPool.listObject.Add(objectClone);
        return objectClone;
    }

    public void RefreshItem(NameObject name)
    {
        for (int i = 0; i < ObjectPools.Count; i++)
        {
            if (ObjectPools[i].nameObject == name)
            {
                for (int k = 0; k < ObjectPools[i].listObject.Count; k++)
                {
                    GameObject _go = ObjectPools[i].listObject[k] as GameObject;
                    _go.transform.SetParent(ObjectPools[i].parent);
                    _go.SetActive(false);
                }
            }
        }
    }

    public void RefreshAll()
    {
        int count = 1 + NameObject.GetValues(typeof(PoolManager.NameObject)).Cast<int>().Max();
        for (int i = 0; i < count; i++)
        {
            NameObject _nameObject = (NameObject)i;
            RefreshItem(_nameObject);
        }
    }
}
