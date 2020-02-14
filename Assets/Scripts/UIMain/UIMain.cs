using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoBehaviour
{
    [SerializeField]
    public MainButton[] MainButtons = new MainButton[0];

    void Start()
    {
        for (int i = 0; i < MainButtons.Length; i++)
        {
            OnButtonClick(MainButtons[i]);
        }
    }

    void Update()
    {
        
    }

    void OnButtonClick(MainButton btn)
    {
        
        btn.ButtonName.onClick.AddListener(()=>{ Instantiate(btn.value); });
    }

}

[Serializable]
public class MainButton
{
    public Button ButtonName;
    public GameObject value;
    public int a;
}