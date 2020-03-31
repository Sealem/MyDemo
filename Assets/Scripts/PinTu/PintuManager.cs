using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pintu
{
    public class PintuManager
    {

        //单例
        private static PintuManager instance;
        public static PintuManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PintuManager();
                }
                return instance;
            }

        }

        //游戏阶数
        public int order = 0;

        private Cell[,] allCells;

        /// <summary>
        /// 初始化游戏
        /// </summary>
        public void Init(List<Cell> mList, List<Vector2> allPos)
        {
            //通过总长度开根，获取阶数
            order = (int)Mathf.Sqrt(mList.Count);
            allCells = new Cell[order, order];
            if (order == 0) return;

            //乱序
            mList = RandomList(mList);
            //判断是否可还原
            while (isTrue(mList)%2 != 0)
            {
                mList = RandomList(mList);
            }
            One2Two(mList, allPos);

        }

        /// <summary>
        /// 获取逆序数，判断是否可还原
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        public int isTrue(List<Cell> cells)
        {
            //计算逆序数
            int count = 0;
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].id == cells.Count - 1)
                {
                    //计算空格还原到对应位置的差值
                    int tmpcount = allCells.Length - i - 1;
                    count += tmpcount;
                }

                for (int j = i+1; j < cells.Count; j++)
                {
                    if (cells[i].id > cells[j].id)
                    {
                        count += 1;
                    }

                }
            }

            return count;
            //如果逆序数为奇数，则不可还原，反之可还原

        }

        /// <summary>
        /// 乱序
        /// </summary>
        /// <param name="mList"></param>
        /// <returns></returns>
        public List<Cell> RandomList(List<Cell> mList)
        {
            for (int i = 0; i < mList.Count; i++)
            {
                int random = Random.Range(0, mList.Count - 1);
                Cell tmp = mList[i];
                mList[i] = mList[random];
                mList[random] = tmp;
            }
            return mList;

            //Cell tmp = mList[8];
            //mList[8] = mList[7];
            //mList[7] = tmp;

            //tmp = mList[7];
            //mList[7] = mList[6];
            //mList[6] = tmp;

            //tmp = mList[6];
            //mList[6] = mList[3];
            //mList[3] = tmp;

            //tmp = mList[3];
            //mList[3] = mList[0];
            //mList[0] = tmp;

            //tmp = mList[0];
            //mList[0] = mList[1];
            //mList[1] = tmp;



            //Cell tmp0 = mList[0];
            //Cell tmp1 = mList[1];
            //Cell tmp2 = mList[2];
            //Cell tmp3 = mList[3];
            //Cell tmp4 = mList[4];
            //Cell tmp5 = mList[5];
            //Cell tmp6 = mList[6];
            //Cell tmp7 = mList[7];
            //Cell tmp8 = mList[8];


            //mList[0] = tmp0;
            //mList[1] = tmp1;
            //mList[2] = tmp2;
            //mList[3] = tmp3;
            //mList[4] = tmp5;
            //mList[5] = tmp8;
            //mList[6] = tmp6;
            //mList[7] = tmp4;
            //mList[8] = tmp7;




            return mList;
        }

        /// <summary>
        /// 更改位置
        /// </summary>
        public void ChangePos(Cell curCell)
        {
            //debug(allCells);
            //查找空格
            Cell endCell = CheckAroundCell(curCell);
            if (endCell == null) return;
            //将当前图片可空格交换
            Cell tmpCell = new Cell(-1,null);
            //tmpCell = endCell;
            tmpCell.pos = endCell.pos;
            tmpCell.x = endCell.x;
            tmpCell.y = endCell.y;

            //endCell = curCell;
            endCell.pos = curCell.pos;
            endCell.x = curCell.x;
            endCell.y = curCell.y;
            UpdatePos(endCell);

            //curCell = tmpCell;
            curCell.pos = tmpCell.pos;
            curCell.x = tmpCell.x;
            curCell.y = tmpCell.y;
            UpdatePos(curCell);

            int resurt = isTrue(Two2One(allCells));
            if(resurt == 0)
            {
                Debug.Log("游戏结束");
            }
            else
            {
                Debug.Log("未结束");
            }
            Debug.Log("AAAAAAAAAAAAAA");


        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="cell"></param>
        public void UpdatePos(Cell cell)
        {
            allCells[cell.x, cell.y] = cell;
            cell.obj.transform.position = cell.pos;
        }

        /// <summary>
        /// 检测当前点击方块周围的空格方块，返回空白方块的下标
        /// </summary>
        /// <param name="curCell"></param>
        /// <returns></returns>
        public Cell CheckAroundCell(Cell curCell)
        {
            int tmpX = curCell.x;
            int tmpY = curCell.y;

            Cell tmpCell;
            //检测右侧
            tmpCell = GetItemRight(curCell);

            if (tmpCell != null && tmpCell.id == allCells.Length - 1) return tmpCell;
            //检测左侧
            tmpCell = GetItemLeft(curCell);
            if (tmpCell != null && tmpCell.id == allCells.Length - 1) return tmpCell;

            //检测下侧
            tmpCell = GetItemDown(curCell);
            if (tmpCell != null && tmpCell.id == allCells.Length - 1) return tmpCell;

            //检测上侧
            tmpCell = GetCellUp(curCell);
            if (tmpCell != null && tmpCell.id == allCells.Length - 1) return tmpCell;

            return null;

        }

        #region 获取周围Cell
        /// <summary>
        /// 获取上方Item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Cell GetCellUp(Cell item)
        {
            int row = item.x;
            int col = item.y - 1;
            if (CheckLegality(row, col))
            {
                return allCells[row, col];
            }
            return null;
        }

        /// <summary>
        /// 获取下方Item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Cell GetItemDown(Cell item)
        {
            int row = item.x;
            int col = item.y + 1;
            if (CheckLegality(row, col))
            {
                return allCells[row, col];
            }
            return null;
        }

        /// <summary>
        /// 获取左方Item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Cell GetItemLeft(Cell item)
        {
            int row = item.x - 1;
            int col = item.y;
            if (CheckLegality(row, col))
            {
                return allCells[row, col];
            }
            return null;
        }

        /// <summary>
        /// 获取右方Item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Cell GetItemRight(Cell item)
        {
            int row = item.x + 1;
            int col = item.y;
            if (CheckLegality(row, col))
            {
                return allCells[row, col];
            }
            return null;
        }

        /// <summary>
        /// 检测是否越界
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool CheckLegality(int x, int y)
        {
            if (x < 0 || x >= order || y < 0 || y >= order) return false;
            return true;
        }

        #endregion


        /// <summary>
        /// 一维转二维
        /// </summary>
        public void One2Two(List<Cell> mList, List<Vector2> allPos)
        {
            //转为二维数组
            for (int i = 0; i < mList.Count; i++)
            {
                allCells[i % order, i / order] = mList[i];
                mList[i].x = i % order;
                mList[i].y = i / order;
                mList[i].pos = allPos[i];
                UpdatePos(mList[i]);
            }
        }

        /// <summary>
        /// 二维转一维
        /// </summary>
        public List<Cell> Two2One(Cell[,] tmpArray)
        {
            List<Cell> cellList = new List<Cell>();
            for (int i = 0; i < order; i++)
            {
                for (int j = 0; j < order; j++)
                {
                    cellList.Add(tmpArray[j, i]);
                }
            }
            return cellList;
        }


        public void debug(Cell[,] cells)
        {
            for (int i = 0; i < order; i++)
            {
                for (int j = 0; j < order; j++)
                {
                    Debug.Log(cells[j,i].id);
                }
            }
        }

        /***********************************************************/
        //自动寻路方法

        public List<CellsTwoDim> openList = new List<CellsTwoDim>();
        public List<CellsTwoDim> closeList = new List<CellsTwoDim>();

#if true
        public List<Cell> FindPath()

        {
            openList.Clear();
            closeList.Clear();

            //获取初始状态
            CellsTwoDim startList = new CellsTwoDim(allCells,0, GetManhattan(allCells));
            //将初始状态添加到openList中
            openList.Add(startList);
            int a = 0;
            while (openList.Count>0)
            //while(a<100)
            {
                a++;
                CellsTwoDim curList = openList[0];
                //循环检测openlist中的所有二维数组
                for (int i = 0; i < openList.Count; i++)
                {
                    if (openList[i].f < curList.f)
                    {
                        curList = openList[i];

                    }
                }
                Debug.Log("-------默认-------");
                debug(curList.cells);

                openList.Remove(curList);
                closeList.Add(curList);

                //找到终点
                if (isTrue(Two2One(curList.cells)) == 0)
                {
                    List<Cell> result = new List<Cell>();
                    int count = 0;
                    Debug.Log("找到终点，寻路结束");
                    CellsTwoDim tmp = curList;
                    while (tmp != null)
                    {
                        for (int i = 0; i < order; i++)
                        {
                            for (int j = 0; j < order; j++)
                            {
                                if(tmp.cells[i,j].id == tmp.cells.Length - 1)
                                {
                                    Cell tmp1 = new Cell(-1,null);
                                    tmp1.x = i;
                                    tmp1.y = j;
                                    result.Add(tmp1);
                                }
                            }
                        }
                        tmp = tmp.patent;

                        count++;
                    }

                    Debug.Log("总步数："+count);
                    return result;
                }


                //没有找到终点，则寻找当前状态可衍生出的其他状态（最多4种，最少两种）
                //openList.Clear();
                var allDerArray = FindDerivationArray(curList.cells);
                for (int i = 0; i < allDerArray.Count; i++)
                {
                    //debug(allDerArray[i].cells);
                    //if(GetManhattan(allDerArray[i]) > GetManhattan(allDerArray[i + 1]))
                    //{

                    //}
                    bool aaaa = false;
                    foreach (var item in closeList)
                    {
                        if(panduanxiangdeng(Two2One(allDerArray[i].cells), Two2One(item.cells)))
                        {
                            aaaa = true;
                            break;
                        }
                        else
                        {
                            aaaa = false;
                        }

                    }
                    //bool aa = Enumerable.SequenceEqual(Two2One(a), Two2One(b));
                    if (aaaa) continue;
                    openList.Add(allDerArray[i]);
                    allDerArray[i].patent = curList;
                }



            }

            Debug.Log("寻路失败");
            return null;

        }

#endif

        /// <summary>
        /// 获取曼哈顿距离
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        public int GetManhattan(Cell[,] cells)
        {
            /*
             * 计算方式：平移的总步数/阶数+平移总步数%阶数
             * 以3阶为例，平移3步，则为纵向平移1步，所以每3步可以简化为1步，纵向移动取商值，横向移动取余值
             */

            int tmpManhattan = 0;
            List<Cell> tmpCells =  Two2One(cells);
            for (int i = 0; i < tmpCells.Count; i++)
            {
                if (tmpCells[i].id == tmpCells.Count - 1) continue;
                int count = Mathf.Abs(tmpCells[i].id - i);
                int num = (count / order) + (count % order);
                tmpManhattan += num;
            }
            return tmpManhattan * 100;
        }

        List<CellsTwoDim> DerCells = new List<CellsTwoDim>();
        int depth = 0;
        /// <summary>
        /// 寻找衍生状态
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        public List<CellsTwoDim> FindDerivationArray(Cell[,] cells)
        {
            DerCells.Clear();
            int x = -1;
            int y = -1;
            //寻找空白方块，找到后记录坐标
            foreach (var item in cells)
            {
                if(item.id == cells.Length - 1)
                {
                    x = item.x;
                    y = item.y;
                }
            }


            //Debug.Log("-------默认-------"+x+"/"+y);
            //debug(cells);
            //寻找右侧
            if (CheckLegality(x + 1, y))
            {
                //缓存当前二维数组（以下对缓存数组进行操作）
                Cell[,] tmpCells = CloneCells(cells);

                Cell tmpCell = new Cell(-1, null);//临时Cell
                

                Cell startCell = tmpCells[x, y];
                //Debug.Log(startCell.GetHashCode() + "/" + cells[x, y].GetHashCode());
                Cell endCell = tmpCells[x + 1, y];

                //tmpCell = endCell;
                tmpCell.x = endCell.x;
                tmpCell.y = endCell.y;
                tmpCell.pos = endCell.pos;

                //endCell = startCell;
                endCell.x = startCell.x;
                endCell.y = startCell.y;
                endCell.pos = startCell.pos;

                //startCell = tmpCell;
                startCell.x = tmpCell.x;
                startCell.y = tmpCell.y;
                startCell.pos = tmpCell.pos;

                tmpCells[startCell.x, startCell.y] = startCell;
                tmpCells[endCell.x, endCell.y] = endCell;

                //Debug.Log("-------右-------");
                //debug(tmpCells);

                DerCells.Add(new CellsTwoDim(tmpCells, depth, GetManhattan(tmpCells)));
            }
            //寻找左侧
            if (CheckLegality(x -1, y))
            {
                //缓存当前二维数组（以下对缓存数组进行操作）
                Cell[,] tmpCells = CloneCells(cells);

                Cell tmpCell = new Cell(-1, null);//临时Cell
                //Debug.Log(tmpCell.GetHashCode() + "/" + cells.GetHashCode());

                Cell startCell = tmpCells[x, y];
                Cell endCell = tmpCells[x - 1, y];

                //tmpCell = cells[x + i, y + j];
                tmpCell.x = endCell.x;
                tmpCell.y = endCell.y;
                tmpCell.pos = endCell.pos;


                //cells[x + i, y + j] = cells[x, y];
                endCell.x = startCell.x;
                endCell.y = startCell.y;
                endCell.pos = startCell.pos;

                //cells[x, y] = tmpCell;
                startCell.x = tmpCell.x;
                startCell.y = tmpCell.y;
                startCell.pos = tmpCell.pos;

                tmpCells[startCell.x, startCell.y] = startCell;
                tmpCells[endCell.x, endCell.y] = endCell;

                //debug(cells);
                //Debug.Log("-------左-------");
                //debug(tmpCells);

                DerCells.Add(new CellsTwoDim(tmpCells, depth, GetManhattan(tmpCells)));
            }



            //寻找上侧
            if (CheckLegality(x, y-1))
            {
                //缓存当前二维数组（以下对缓存数组进行操作）
                Cell[,] tmpCells = CloneCells(cells);
                //Debug.Log(tmpCells.GetHashCode() + "/" + cells.GetHashCode());

                Cell tmpCell = new Cell(-1, null);//临时Cell
                Cell startCell = tmpCells[x, y];
                Cell endCell = tmpCells[x, y - 1];

                //tmpCell = cells[x + i, y + j];
                tmpCell.x = endCell.x;
                tmpCell.y = endCell.y;
                tmpCell.pos = endCell.pos;

                //cells[x + i, y + j] = cells[x, y];
                endCell.x = startCell.x;
                endCell.y = startCell.y;
                endCell.pos = startCell.pos;

                //cells[x, y] = tmpCell;
                startCell.x = tmpCell.x;
                startCell.y = tmpCell.y;
                startCell.pos = tmpCell.pos;

                tmpCells[startCell.x, startCell.y] = startCell;
                tmpCells[endCell.x, endCell.y] = endCell;

                //debug(cells);
                //Debug.Log("-------上-------");
                //debug(tmpCells);

                DerCells.Add(new CellsTwoDim(tmpCells, depth, GetManhattan(tmpCells)));
            }

            //寻找下侧
            if (CheckLegality(x, y+1))
            {
                //缓存当前二维数组（以下对缓存数组进行操作）
                Cell[,] tmpCells = CloneCells(cells);
                //Debug.Log(tmpCells.GetHashCode() + "/" + cells.GetHashCode());

                Cell tmpCell = new Cell(-1, null);//临时Cell
                Cell startCell = tmpCells[x, y];
                Cell endCell = tmpCells[x, y + 1];

                //tmpCell = cells[x + i, y + j];
                tmpCell.x = endCell.x;
                tmpCell.y = endCell.y;
                tmpCell.pos = endCell.pos;

                //cells[x + i, y + j] = cells[x, y];
                endCell.x = startCell.x;
                endCell.y = startCell.y;
                endCell.pos = startCell.pos;

                //cells[x, y] = tmpCell;
                startCell.x = tmpCell.x;
                startCell.y = tmpCell.y;
                startCell.pos = tmpCell.pos;

                tmpCells[startCell.x, startCell.y] = startCell;
                tmpCells[endCell.x, endCell.y] = endCell;

                //debug(cells);
                //Debug.Log("-------下-------");
                //debug(tmpCells);

                DerCells.Add(new CellsTwoDim(tmpCells, depth, GetManhattan(tmpCells)));
            }

            depth += 1;


            //Debug.Log(DerCells.Count+"//////");
            return DerCells;
        }

        public Cell[,] CloneCells(Cell[,] cells)
        {
            Cell[,] tmpCell = new Cell[order, order];
            for (int i = 0; i < order; i++)
            {
                for (int j = 0; j < order; j++)
                {
                    tmpCell[i, j] = new Cell(-1,null);
                    tmpCell[i, j].id = cells[i, j].id;
                    tmpCell[i, j].obj = cells[i, j].obj;
                    tmpCell[i, j].x = cells[i, j].x;
                    tmpCell[i, j].y = cells[i, j].y;
                    tmpCell[i, j].pos = cells[i, j].pos;
                }
            }
            return tmpCell;
        }


        public bool panduanxiangdeng(List<Cell> cells1, List<Cell> cells2)
        {
            bool a = false;
            //Debug.Log("_____________________________________");
            for (int i = 0; i < cells1.Count; i++)
            {
                //Debug.Log(cells1[i].id + "-" + cells2[i].id);
                if(cells1[i].id != cells2[i].id)
                {
                    //Debug.Log("fasle");
                    return false;
                }
            }
            //Debug.Log("true");

            return true;
        }


        public IEnumerator  Auto(List<Cell> aaa)
        {
            aaa.Reverse();


            for (int i = 0; i < aaa.Count; i++)
            {
                int x = aaa[i].x;
                int y = aaa[i].y;

                ChangePos(allCells[x,y]);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    public class Cell
    {
        public int id = -1;
        public GameObject obj;

        public int x;
        public int y;
        public Vector2 pos;
        public Cell(int id, GameObject obj)
        {
            this.id = id;
            this.obj = obj;
        }

    }

    /// <summary>
    /// 二维数组类型
    /// </summary>
    public class CellsTwoDim
    {
        int g = 0;
        int h = 0;
        public int f
        {
            get
            {
                return g + h;
            }
        }
        public Cell[,] cells;
        public CellsTwoDim patent = null;

        public CellsTwoDim(Cell[,] cells,int g,int h)
        {
            this.cells = cells;
            this.g = g;
            this.h = h;
        }
    }
}

