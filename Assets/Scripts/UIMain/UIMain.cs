using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoBehaviour
{
    //列表对象
    public GameObject panel;
    //列表按钮
    [SerializeField]
    public MainButton[] MainButtons = new MainButton[0];
    //返回按钮
    public Button BackBtn;
    //新打开的界面
    GameObject OpenPanel;
    void Start()
    {
        BackBtn.onClick.AddListener(OnBackBtnClick);
        BackBtn.gameObject.SetActive(false);
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
        
        btn.ButtonName.onClick.AddListener(()=>
        {
            OpenPanel = Instantiate(btn.value); 
            panel.SetActive(false);
            BackBtn.gameObject.SetActive(true);
        });

    }

    void OnBackBtnClick()
    {
        panel.SetActive(true);
        Destroy(OpenPanel);

        BackBtn.gameObject.SetActive(false);
    }

}

[Serializable]
public class MainButton
{
    public Button ButtonName;
    public GameObject value;
}