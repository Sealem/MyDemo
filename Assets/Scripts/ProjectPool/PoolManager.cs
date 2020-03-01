using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private List<GameObject> gameObjects;
    public static PoolManager Instance;
    public int PoolMax = 1;

    public GameObject prefab;
    GameObject localObj;
    void Awake()
    {
        Instance = this;
        gameObjects = new List<GameObject>();
    }

    //取出对象
    public GameObject GetGameObject()
    {
        if (gameObjects.Count <= 0)
        {
            return Instantiate(prefab);
        }

        localObj = gameObjects[0];
        gameObjects.RemoveAt(0);
        return localObj;
    }

    //存入对象
    public void SetGameObject(GameObject obj)
    {
        obj.SetActive(false);
        if (gameObjects.Count >= PoolMax)
        {
            Destroy(obj);
            return;
        }
        gameObjects.Add(obj);
    }

}
