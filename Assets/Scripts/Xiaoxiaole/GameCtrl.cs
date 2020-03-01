using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCtrl : MonoBehaviour
{
    public GameObject ItemCell;

    public static GameCtrl Instance;
    public Sprite[] Sprites;

    public int Row;
    public int Col;

    public Vector2 offset;

    public Item[,] allItems;

    public Vector2[,] allPos;
    //相同Item列表
    public List<Item> sameItemList;
    //要消除的Item列表
    public List<Item> boomList;
    //正在操作
    public bool isOperation = false;
    //正在执行消除
    public bool allBoom = false;
    //Item边长
    public float itemSize;


    private void Awake()
    {
        Instance = this;
        allItems = new Item[Row, Col];
        allPos = new Vector2[Row, Col];
        sameItemList = new List<Item>();
        boomList = new List<Item>();
    }

    private void Start()
    {
        
        Init();
        AllBoom();
    }

    /// <summary>
    /// 初始化游戏，生成Item并记录
    /// </summary>
    private void Init()
    {
        //获取Item边长
        itemSize = ItemCell.GetComponent<RectTransform>().rect.width;
        offset.x = (Col - 1) / 2 * itemSize;
        offset.y = (Row - 1) / 2 * itemSize;
        //生成Item
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Col; j++)
            {
                //获取实例化对象
                GameObject curObj = Instantiate(ItemCell, transform);
                //设置对象坐标
                curObj.transform.localPosition = new Vector2(j * itemSize, i * itemSize) - offset;
                //创建随机图案
                int randomSpr = Random.Range(0, Sprites.Length);
                //获取当前对象的Item
                Item curItem = curObj.GetComponent<Item>();
                //记录Item的坐标
                curItem.itemRow = i;
                curItem.itemCol = j;
                //设置Item的图案
                curItem.curSprite = Sprites[randomSpr];
                curItem.curImage.sprite = curItem.curSprite;

                //保存
                allItems[i, j] = curItem;
                allPos[i, j] = curObj.transform.position;

            }
        }
    }


    /// <summary>
    /// 检测清除（入口）
    /// </summary>
    private void AllBoom()
    {
        bool hasBoom = false;
        foreach (var item in allItems)
        {
            if(item && !item.hasCheck)
            {
                //检测当前Item周围的消除
                CheckAroundBoom(item);
                //开始消除
                if (boomList.Count > 0)
                {
                    hasBoom = true;
                    isOperation = true;
                }
            }
        }
        if (!hasBoom) isOperation = false;
    }

    /// <summary>
    /// 检测周围消除
    /// </summary>
    public void CheckAroundBoom(Item item)
    {
        sameItemList.Clear();
        boomList.Clear();
        FillSameItemsList(item);
        FillBoomList(item);
    }

    /// <summary>
    /// 填充相同Item列表
    /// </summary>
    /// <param name="item"></param>
    private void FillSameItemsList(Item item)
    {
        //如果当前相同列表中存在当前对象，返回不执行
        if (sameItemList.Contains(item)) return;
        //将当前对象放入相同列表
        sameItemList.Add(item);
        Item[] tmpitems = new Item[]
        {
            GetItemUp(item),
            GetItemDown(item),
            GetItemLeft(item),
            GetItemRight(item)
        };

        for (int i = 0; i < tmpitems.Length; i++)
        {
            if (tmpitems[i] == null) continue;
            if (tmpitems[i].curSprite == item.curSprite)
                FillSameItemsList(tmpitems[i]);
        }

    }

    /// <summary>
    /// 根据相同列表填充消除列表
    /// </summary>
    /// <param name="item"></param>
    private void FillBoomList(Item curItem)
    {
        int tmpRow = 0;
        int tmpCol = 0;
        //临时列表
        List<Item> tmpRowList = new List<Item>();
        List<Item> tmpColList = new List<Item>();
        foreach (var item in sameItemList)
        {
            //在同一行
            if(item.itemRow == curItem.itemRow)
            {
                //判断中间是否有间隙
                bool rowCanBoom = true;

                if (rowCanBoom)
                {
                    tmpRow++;
                    tmpRowList.Add(item);
                }
            }
            //在同一列
            if(item.itemCol == curItem.itemCol)
            {
                //判断中间是否有间隙
                bool colCanBoom = true;

                if (colCanBoom)
                {
                    tmpCol++;
                    tmpColList.Add(item);
                }
            }
        }

        //是否可以横向消除
        bool horizontalBoom = false;
        //横向消除数量在3个以上
        if (tmpRow >= 3)
        {
            //将临时列表中的Item全部放入BoomList
            boomList.AddRange(tmpRowList);
            horizontalBoom = true;
        }
        //纵向消除在3个以上
        if (tmpCol >= 3)
        {
            //先判断是否存在横向消除，存在则将自身剔除（存在横向时，自身已经添加到横向消除列表中）
            if (horizontalBoom)
            {
                boomList.Remove(curItem);
            }
            boomList.AddRange(tmpColList);
        }

        //如果不存在消除对象
        if (boomList.Count <= 0) return;
        //创建协程消除列表
        List<Item> tmpBoomList = new List<Item>();
        tmpBoomList.AddRange(boomList);
        StartCoroutine(StartBoom(tmpBoomList));
    }

    /// <summary>
    /// 协程消除（过程）
    /// </summary>
    /// <returns></returns>
    IEnumerator StartBoom(List<Item> tmpBoomList)
    {
        foreach (var item in tmpBoomList)
        {
            item.hasCheck = true;
            //清除动画

            //将清除对象从列表移除
            allItems[item.itemRow, item.itemCol] = null;
        }

        //检测是否已经执行了清除动画
        //while (true)
        //{
        //    yield return 0;
        //}

        //延迟0.2s后开始下落
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(ItemDrop());
        //延迟0.3s后开始回收对象
        yield return new WaitForSeconds(0.3f);
        foreach (var item in tmpBoomList)
        {
            Destroy(item.gameObject);
        }
    }

    /// <summary>
    /// Item下落
    /// </summary>
    /// <returns></returns>
    IEnumerator ItemDrop()
    {
        isOperation = true;
        //逐列检测
        for (int i = 0; i < Col; i++)
        {
            //计数器
            int count = 0;
            //下落队列
            Queue<Item> dropQueue = new Queue<Item>();
            //逐行检测
            for (int j = 0; j < Row; j++)
            {
                if(allItems[j,i] != null)
                {
                    count++;
                    dropQueue.Enqueue(allItems[j, i]);
                }
            }

            //下落
            for (int k = 0; k < count; k++)
            {
                //获取要下落的Item
                Item curItem = dropQueue.Dequeue();
                //修改全局数组
                allItems[curItem.itemRow, curItem.itemCol] = null;
                //修改Item行数
                curItem.itemRow = k;
                allItems[curItem.itemRow, curItem.itemCol] = curItem;
                //下落
                curItem.ItemDrop(allPos[curItem.itemRow, curItem.itemCol]);
            }
        }
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(CreateNewItem());
        yield return new WaitForSeconds(0.2f);
        AllBoom();
    }

    /// <summary>
    /// 生成新的Item
    /// </summary>
    /// <returns></returns>
    IEnumerator CreateNewItem()
    {
        isOperation = true;
        for (int i = 0; i < Col; i++)
        {
            int count = 0;
            Queue<GameObject> newItemQueue = new Queue<GameObject>();
            for (int j = 0; j < Row; j++)
            {
                if(allItems[j,i] == null)
                {
                    //生成Item
                    GameObject curObj = Instantiate(ItemCell);
                    curObj.transform.parent = transform;
                    curObj.transform.position = allPos[Row - 1, i];
                    newItemQueue.Enqueue(curObj);
                    count++;
                }
            }

            for (int k = 0; k < count; k++)
            {
                Item curItem = newItemQueue.Dequeue().GetComponent<Item>();

                //创建随机图案
                int randomSpr = Random.Range(0, Sprites.Length);
                //设置Item的图案
                curItem.curSprite = Sprites[randomSpr];
                curItem.curImage.sprite = curItem.curSprite;
                //获取要移动的行数
                int step = Row - count + k;
                //移动
                curItem.ItemMove(step, i, allPos[step, i]);
            }
        }
        yield break;
    }


    /// <summary>
    /// 获取上方Item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private Item GetItemUp(Item item)
    {
        int row = item.itemRow;
        int col = item.itemCol + 1;
        if (CheckLegality(row, col))
        {
            return allItems[row, col];
        }
        return null;
    }

    /// <summary>
    /// 获取下方Item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private Item GetItemDown(Item item)
    {
        int row = item.itemRow;
        int col = item.itemCol - 1;
        if (CheckLegality(row, col))
        {
            return allItems[row, col];
        }
        return null;
    }

    /// <summary>
    /// 获取左方Item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private Item GetItemLeft(Item item)
    {
        int row = item.itemRow - 1;
        int col = item.itemCol;
        if (CheckLegality(row, col))
        {
            return allItems[row, col];
        }
        return null;
    }

    /// <summary>
    /// 获取右方Item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private Item GetItemRight(Item item)
    {
        int row = item.itemRow + 1;
        int col = item.itemCol;
        if (CheckLegality(row, col))
        {
            return allItems[row, col];
        }
        return null;
    }

    /// <summary>
    /// 判断对象是否存在
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public bool CheckLegality(int row, int col)
    {
        if (row < 0 || row >= Row || col < 0 || col >= Col) return false;
        return true;
    }
}
