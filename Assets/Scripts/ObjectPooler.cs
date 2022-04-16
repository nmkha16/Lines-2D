using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ObjectPoolItem{
    [SerializeField] public GameObject objectToPool;
    [SerializeField] public int amountToPool;
    public bool expanded;

}

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;
    // Start is called before the first frame update

    [SerializeField] public List<ObjectPoolItem> itemsToPool;
    public List<GameObject> pooledObjects;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        pooledObjects = new List<GameObject>();
        foreach(ObjectPoolItem item in itemsToPool)
        {
            for (int i=0; i<item.amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item.objectToPool);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }

    public GameObject getPooledObject(string tag)
    {
        for(int i = 0;i<pooledObjects.Count;i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag)
            {
                return pooledObjects[i];
            }
        }
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.tag == tag)
            {
                if (item.expanded)
                {
                    GameObject obj = (GameObject)Instantiate(item.objectToPool);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                    return obj;
                }
            }
        }
        return null;
    }


    // need manual call
    public void DestroyOnExplosion(GameObject _gameObject)
    {
        _gameObject.SetActive(false);
    }
}
