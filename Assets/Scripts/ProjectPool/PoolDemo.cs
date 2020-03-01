using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolDemo : MonoBehaviour
{
    GameObject obj;
    float creatTime;


    void Update()
    {
        creatTime = 0;
        obj = PoolManager.Instance.GetGameObject();
        obj.transform.SetParent(transform);
        obj.SetActive(true);
        if (obj.GetComponent<ItemCtrl>() == null)
        {
            obj.AddComponent<ItemCtrl>();
        }
        obj = null;


    }
}
