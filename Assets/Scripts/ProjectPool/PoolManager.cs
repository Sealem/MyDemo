using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private List<GameObject> gameObjects;
    public static PoolManager Instance;
    public int PoolMax = 10;

    public GameObject prefab;

    void Awake()
    {
        Instance = this;
        gameObjects = new List<GameObject>();
    }

    public GameObject GetGameObject()
    {
        if (gameObjects.Count <= 0)
        {
            return Instantiate(prefab);
        }
        GameObject localObj = gameObjects[0];
        gameObjects.RemoveAt(0);
        return localObj;
    }

    public void SetGameObject(GameObject obj)
    {
        if (gameObjects.Count >= PoolMax)
        {
            
        }
    }

}
