using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pintu;

public class PintuGame : MonoBehaviour
{
    PintuManager instance = PintuManager.Instance;

    public GameObject parent;

    public Button startBtn;

    //乱序状态
    private List<Cell> allCells = new List<Cell>();
    //记录位置
    private List<Vector2> allPos = new List<Vector2>();
    void Start()
    {
        startBtn.onClick.AddListener(AutoBtn);
        //填充初始数组
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            Cell tmpCell = new Cell(i, parent.transform.GetChild(i).gameObject);
            allPos.Add(tmpCell.obj.transform.position);
            allCells.Add(tmpCell);

            //添加点击事件
            tmpCell.obj.GetComponent<Button>().onClick.AddListener(() => { OnClick(tmpCell); }) ;
        }

        //初始化游戏，转为二维数组
        instance.Init(allCells, allPos);



        //int[] a = new int[] { 1,2,3};
        //int[] b = new int[] { 1,2,3};
        //int[] c = a;


        //List<int[]> d = new List<int[]>();
        //d.Add(a);

        //bool aa = d.Contains(b);
        //bool bb = d.Contains(c);

        //Debug.Log(aa+"//"+bb);



    }

    void AutoBtn()
    {
        if (instance.FindPath() == null) return;
        StartCoroutine(instance.Auto(instance.FindPath()));
    }

    void OnClick(Cell cell)
    {
        instance.ChangePos(cell);
    }

}
