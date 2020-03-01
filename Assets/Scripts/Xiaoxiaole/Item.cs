using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class Item : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int itemRow;
    public int itemCol;

    public Sprite curSprite;
    public Image curImage;

    public GameCtrl gameCtrl;

    public bool hasCheck;

    private void Awake()
    {
        //curImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        gameCtrl = GameCtrl.Instance;
    }

    Vector2 downPos;
    Vector2 upPos;
    /// <summary>
    /// 鼠标按下
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        downPos = eventData.position;
    }
    /// <summary>
    /// 鼠标抬起
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        //判断是否正在操作
        if (gameCtrl.isOperation) return;
        gameCtrl.isOperation = true;
        upPos = eventData.position;
        //获取方向
        Vector2 dir = GetDirection();
        //点击异常处理
        if (dir.magnitude != 1)
        {
            gameCtrl.isOperation = false;
            return;
        }
        //开启协程交换
        StartCoroutine(ItemExchange(dir));
    }

    /// <summary>
	/// Item交换
	/// </summary>
	/// <returns>The exchange.</returns>
	/// <param name="dir">Dir.</param>
	IEnumerator ItemExchange(Vector2 dir)
    {
        //获取目标行列
        int targetRow = itemRow + Convert.ToInt32(dir.y);
        int targetCol = itemCol + Convert.ToInt32(dir.x);
        //检测对象是否存在
        bool isLagal = gameCtrl.CheckLegality(targetRow, targetCol);
        if (!isLagal)
        {
            gameCtrl.isOperation = false;
            //不存在跳出
            yield break;
        }
        //获取目标
        Item target = gameCtrl.allItems[targetRow, targetCol];
        //从全局列表中获取当前item，查看是否已经被消除，被消除后不能再交换
        Item myItem = gameCtrl.allItems[itemRow, itemCol];
        if (!target || !myItem)
        {
            gameCtrl.isOperation = false;
            //Item已经被消除
            yield break;
        }
        //相互移动
        target.ItemMove(itemRow, itemCol, transform.position);
        ItemMove(targetRow, targetCol, target.transform.position);
        //还原标志位
        bool reduction = false;
        //消除处理
        gameCtrl.CheckAroundBoom(this);
        if (gameCtrl.boomList.Count == 0)
        {
            reduction = true;
        }
        gameCtrl.CheckAroundBoom(target);
        if (gameCtrl.boomList.Count != 0)
        {
            reduction = false;
        }
        //还原
        if (reduction)
        {
            //延迟
            yield return new WaitForSeconds(0.2f);
            //临时行列
            int tempRow, tempCol;
            tempRow = myItem.itemRow;
            tempCol = myItem.itemCol;
            //移动
            myItem.ItemMove(target.itemRow,target.itemCol, target.transform.position);
            target.ItemMove(tempRow,tempCol, myItem.transform.position);
            //延迟
            yield return new WaitForSeconds(0.2f);
            //操作完毕
            gameCtrl.isOperation = false;
        }
    }

    /// <summary>
	/// Item的移动
	/// </summary>
	public void ItemMove(int targetRow, int targetCol, Vector3 pos)
    {
        //改行列
        itemRow = targetRow;
        itemCol = targetCol;
        //改全局列表
        gameCtrl.allItems[targetRow, targetCol] = this;
        //移动
        transform.DOMove(pos, 0.2f);
    }

    /// <summary>
    /// 下落
    /// </summary>
    /// <param name="pos">Position.</param>
    public void ItemDrop(Vector3 pos)
    {
        //下落
        transform.DOMove(pos, 0.2f);
    }



    /// <summary>
    /// 获取方向
    /// </summary>
    /// <returns></returns>
    public Vector2 GetDirection()
    {
        //方向向量
        Vector2 dir = upPos - downPos;
        //如果是横向滑动
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            //返回横向坐标
            return new Vector2(dir.x / Mathf.Abs(dir.x), 0);
        }
        else
        {
            //返回纵向坐标
            return new Vector2(0, dir.y / Mathf.Abs(dir.y));
        }
    }
}
