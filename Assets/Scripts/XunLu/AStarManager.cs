using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStart
{
    public class AStarManager
    {
        private static AStarManager instance;
        private Vector2 cellSize;
        private Vector2 gridSize;
        private Vector3 mapPos;
        private Vector3 startPos;

        public static AStarManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AStarManager();
                }
                return instance;
            }
        }

        public Dictionary<Vector2, Cell> allCells = new Dictionary<Vector2, Cell>();
        List<Vector3> resultPos = new List<Vector3>();
        List<Cell> openList = new List<Cell>();
        List<Cell> closeList = new List<Cell>();

        /// <summary>
        /// 创建Grid
        /// </summary>
        public void CreateGrid(GameObject mapObj, Vector2 cellSize)
        {
            Renderer mapRenderer = mapObj.GetComponent<Renderer>();
            Vector3 mapSize = mapRenderer.bounds.size;
            mapPos = mapObj.transform.position;

            Vector2 gridSize = new Vector2(mapSize.x, mapSize.z);
            Vector2 offSet = -(gridSize - cellSize) / 2;

            this.gridSize = gridSize;
            this.cellSize = cellSize;

            for (int i = 0; i < (gridSize.y / cellSize.y); i++)
            {
                for (int j = 0; j < (gridSize.x / cellSize.x); j++)
                {
                    Vector2 cellLocalPos = new Vector2(j, i) * cellSize + offSet;
                    Vector3 cellPosition = new Vector3(mapPos.x + cellLocalPos.x, mapPos.y, mapPos.z + cellLocalPos.y);

                    Cell cell = new Cell(j, i, false);
                    cell.pos = cellPosition;
                    allCells.Add(new Vector2(j, i), cell);
                    if (i == 0 && j == 0) startPos = cellPosition;
                }
            }
        }

        /// <summary>
        /// 通过世界坐标获取Cell
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Cell GetCell(Vector3 pos)
        {


#if false
        //公式：(int)(pos.x - map.x-tmpC%2*cellSize.x/2)/cellSize.x*cellSize.x+map.x+cellSize.x
        //理解：（点击点-地图点）/单格尺寸 * 单个尺寸+地图点
        //记录生成cell的两轴数量
        int gridCountX = (int)(gridSize.x / cellSize.x);
        int gridCountY = (int)(gridSize.y / cellSize.y);

        float tmpX = pos.x - startPos.x;
        float tmpY = pos.z - startPos.z;

        if (tmpX < 0) tmpX -= cellSize.x;
        if (tmpY < 0) tmpY -= cellSize.y;
        Debug.Log("tmpX/Y:"+ tmpX+"/" +tmpY);
        float aX = tmpX - ((gridCountX % 2) * cellSize.x/2);
        float aY = tmpY - ((gridCountY % 2) * cellSize.y/2);

        Debug.Log("aX/Y:" + aX + "/" + aY);

        int bX = (int)(aX / cellSize.x);
        int bY = (int)(aY / cellSize.y);
        Debug.Log("bX/Y:" + bX + "/" + bY);

        float cX = bX * cellSize.x;
        float cY = bY * cellSize.y;
        Debug.Log("cX/Y:" + cX + "/" + cY);


        float dX = cX + startPos.x;
        float dY = cY + startPos.z;
        Debug.Log("dX/Y:" + dX + "/" + dY);


        float eX = dX + (cellSize.x / 2);
        float eY = dY + (cellSize.y / 2);
        Debug.Log("eX/Y:" + eX + "/" + eY);


        float x = (int)((tmpX - (gridCountX % 2) * cellSize.x) / cellSize.x) * cellSize.x + startPos.x;
        float y = (int)((tmpY - (gridCountY % 2) * cellSize.y) / cellSize.y) * cellSize.y + startPos.z;

        Vector3 resultPos = new Vector3(x, startPos.y, y);
        Vector3 resultPos2 = new Vector3(dX, startPos.y, dY);
        Debug.Log("获取结果："+resultPos2+"/   "+ resultPos);
        Cell tmpCell;
        allCells.TryGetValue(resultPos, out tmpCell);


        Debug.Log(tmpCell);

#else
            //记录生成cell的两轴数量
            int gridCountX = (int)(gridSize.x / cellSize.x);
            int gridCountY = (int)(gridSize.y / cellSize.y);
            //获取坐标差值
            float tmpX = pos.x - startPos.x + cellSize.x / 2;
            float tmpY = pos.z - startPos.z + cellSize.y / 2;
            //计算下标
            float IndexX = (int)(tmpX / cellSize.x);
            float IndexY = (int)(tmpY / cellSize.y);
            Vector2 resultPos = new Vector3(IndexX, IndexY);

            allCells.TryGetValue(resultPos, out Cell tmpCell);
#endif
            return tmpCell;
        }

        /// <summary>
        /// 寻找路径
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
        {
            Debug.Log("开始坐标" + startPos + "结束坐标" + endPos);
            openList.Clear();
            closeList.Clear();
            resultPos.Clear();
            Cell startCell = GetCell(startPos);
            Cell endCell = GetCell(endPos);
            startCell.parent = null;

            openList.Add(startCell);

            while (openList.Count > 0)
            {
                Cell curCell = openList[0];
                for (int i = 0; i < openList.Count; i++)
                {
                    if (openList[i].cost < curCell.cost)
                    {
                        curCell = openList[i];
                    }
                }

                openList.Remove(curCell);
                closeList.Add(curCell);

                if (curCell.x == endCell.x && curCell.y == endCell.y)
                {
                    Cell cell = endCell;
                    //找到终点，寻路结束
                    while (cell != null)
                    {
                        resultPos.Add(cell.pos);
                        Cell tmpCell = cell;
                        cell = cell.parent;
                    }
                    return resultPos;
                }

                List<Cell> aroundCells = GetAroundCell(curCell);

                foreach (var cell in aroundCells)
                {
                    //如果cell是障碍物或非检测点，跳过
                    if (cell.isWall || closeList.Contains(cell)) continue;
                    //获取当前cell和检测cell之间的cost,记录下当前cell到起点的距离
                    int cost = curCell.startCost + GetDistanceCost(curCell, cell);

                    if (cost < cell.startCost || !openList.Contains(cell))
                    {
                        cell.startCost = cost;
                        cell.endCost = GetDistanceCost(cell, endCell);
                        cell.parent = curCell;

                        if (!openList.Contains(cell))
                        {
                            openList.Add(cell);
                        }
                    }
                }
            }
            Debug.Log("寻路失败");
            return null;
        }

        /// <summary>
        /// 获取两个cell之间的估价距离
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public int GetDistanceCost(Cell a, Cell b)
        {
            int x = Mathf.Abs(a.x - b.x);
            int y = Mathf.Abs(a.y - b.y);

            if (x > y) return 14 * y + 10 * (x - y);
            else return 14 * x + 10 * (y - x);
        }

        /// <summary>
        /// 获取周围Cell
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public List<Cell> GetAroundCell(Cell cell)
        {
            List<Cell> cells = new List<Cell>();

            Cell tmpCell;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    //是自身跳过
                    if (i == 0 && j == 0) continue;
                    Vector2 result = new Vector2(cell.x, cell.y) + new Vector2(j, i);
                    allCells.TryGetValue(result, out tmpCell);
                    if (tmpCell != null)
                    {
                        cells.Add(tmpCell);
                    }

                }
            }
            return cells;
        }


        public void AddObstacle(Vector3 pos, GameObject obs)
        {
            Cell obstacle = GetCell(pos);
            if (obstacle.isWall) return;
            GameObject obj = Object.Instantiate(obs);
            obj.transform.position = obstacle.pos;
            obstacle.isWall = true;
            allCells[new Vector2(obstacle.x, obstacle.y)].isWall = true;
        }
    }

    public class Cell
    {
        //所有cell的相对坐标，记录生成cell的位置（从0，0开始）
        public int x = 0;
        public int y = 0;

        //cell的世界坐标(key值)
        public Vector3 pos = Vector3.zero;

        //是否是障碍物
        public bool isWall = false;
        //距起点距离
        public int startCost = 0;
        //据终点距离
        public int endCost = 0;
        //总距离
        public int cost
        {
            get { return startCost + endCost; }
        }
        //父对象
        public Cell parent = null;
        //构造
        public Cell(int x, int y, bool isWall)
        {
            this.x = x;
            this.y = y;
            this.isWall = isWall;
        }
    }

}