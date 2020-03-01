using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCtrl : MonoBehaviour
{
    float Destroytime; //销毁时间
    void Start()
    {
        Destroytime = 0;
        gameObject.AddComponent<Rigidbody>();
        gameObject.GetComponent<Renderer>().material.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1);
        transform.position = transform.parent.transform.position;
    }

    void Update()
    {
        Destroytime += Time.deltaTime;
        if (Destroytime >= 3)
        {
            Destroy(gameObject.GetComponent<Rigidbody>());
            Destroy(gameObject.GetComponent<ItemCtrl>());
            PoolManager.Instance.SetGameObject(gameObject);
        }
    }

}
