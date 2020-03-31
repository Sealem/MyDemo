using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CreatImg : MonoBehaviour
{
    public RawImage image;
    public Button StartBtn;
    public Button ResetBtn;
    public Button EndBtn;
    public Button EscBtn;

    public Text time;
    private float TimeText;    //已用时间
    public Text Step;
    private int StepNum;     //已用步数

    public Sprite[] images;
    private int[] CellArray;
    public Image Cell;
    private int[] randomArr;

    public GameObject Window;
    public Button windowBtn;

    void Start()
    {
        StartBtn.onClick.AddListener(OnStartBtnClick);
        ResetBtn.onClick.AddListener(OnResetBtnClick);
        EndBtn.onClick.AddListener(OnEndBtnClick);
        EscBtn.onClick.AddListener(OnEscBtnClick);
        windowBtn.onClick.AddListener(OnWindowBtnClick);
        OnReset();
    }

    /// <summary>
    /// 初始�?生成默认的图像结�?
    /// </summary>
    private void OnReset()
    {
        Window.SetActive(false);
        Debug.Log("初始化");
        gameEnd = false;
        isStart = false;
        RemoveAllChild(transform);
        isStart = false;
        StepNum = 0;
        TimeText = 0;
        Step.text = "步数:" + StepNum.ToString();
        time.text = "已用时间:" + TimeSpan.FromSeconds(TimeText).ToString().Substring(3, 5);
        CellArray = new int[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            Image obj = Instantiate(Cell);
            obj.gameObject.SetActive(true);
            obj.name = i.ToString();
            CellArray[i] = i;
            obj.sprite = images[i];
            obj.transform.SetParent(transform);
            obj.GetComponent<RectTransform>().localScale = Vector3.one;
            if (i == images.Length - 1)
            {
                obj.color = new Color(0, 0, 0, 0);
            }

            obj.GetComponent<Button>().onClick.AddListener(()=> OnBtnClick(obj.gameObject));
            //if (!obj.gameObject.GetComponent<InputImg>())
            //{
            //    obj.gameObject.AddComponent<InputImg>();
            //}



        }
    }

    void RemoveAllChild(Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    bool isStart;
    bool gameEnd;
    private void Update()
    {
        if (isStart && !gameEnd)
        {
            TimeText += Time.deltaTime;
            time.text = "已用时间:"+TimeSpan.FromSeconds(TimeText).ToString().Substring(3, 5);
            Step.text = "步数:" + StepNum.ToString();
        }
        if (gameEnd)
        {
            isStart = false;
        }

    }

    void OnStartBtnClick()
    {
        Debug.Log("开始");
        if (isStart) return;
        isStart = true;
        //判断是否无解,如果无解将一直随�?直到结果有意�?
        randomArr = PinTuManager.Instance.ArrayRandom(CellArray);

        while (PinTuManager.Instance.Result(randomArr) %2 != 0)
        {
            PinTuManager.Instance.ArrayRandom(randomArr);
        }

        foreach (var item in randomArr)
        {
            print(item);
        }

        //将碎图排列成随机数组的结�?
        for (int i = 0; i < randomArr.Length; i++)
        {
            for (int j = 0; j < randomArr.Length; j++)
            {
                if (int.Parse(transform.GetChild(j).name) == randomArr[i])
                {
                    transform.GetChild(j).SetAsLastSibling();
                }
            }
        }
    }
    void OnResetBtnClick()
    {
        OnReset();

        OnStartBtnClick();
    }

    void OnEndBtnClick()
    {
        OnReset();
    }
    void OnEscBtnClick()
    {

    }

    int count;  //当前碎图数量
    int EndImgIndex;
    void OnBtnClick(GameObject obj)
    {
        if (!isStart) return;
        count = CellArray.Length;
        for (int i = 0; i < count; i++)
        {
            if (transform.GetChild(i).name == (count - 1).ToString())
            {
                EndImgIndex = i;
            }
        }

        //获取当前对象下标
        int index = obj.transform.GetSiblingIndex();
        //如果当前对象为隐藏的图片,返回
        if (EndImgIndex == index)
        {
            return;
        }
        //寻找周围隐藏图片
        if (index + 1 == EndImgIndex || index + Mathf.Sqrt(count) == EndImgIndex || index - 1 == EndImgIndex || index - Mathf.Sqrt(count) == EndImgIndex)
        {
            Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            StepNum++;
            GameObject tmpObj = transform.GetChild(EndImgIndex).gameObject;
            obj.transform.SetSiblingIndex(EndImgIndex);
            //tmpObj.transform.SetSiblingIndex(index);

            int tmp = randomArr[EndImgIndex];
            randomArr[EndImgIndex] = randomArr[index];
            randomArr[index] = tmp;
            foreach (var item in randomArr)
            {
                print(item);
            }
            if(PinTuManager.Instance.Result(randomArr) == 0)
            {
                gameEnd = true;
                Window.SetActive(true);
                
            }
        }

    }

    void OnWindowBtnClick()
    {
        Window.SetActive(false);
        OnReset();
    }

}
