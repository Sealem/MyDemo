using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class PlanePos : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IDragHandler
{
    public Camera cameraa;
    public GameObject Cell;
    public GameObject Map;
    public Button AddObstacleBtn;
    public Button FindPathBtn;

    private GameObject startObj;

    private Vector2 cellSize;
    private Vector2 gridSize;
    private Vector3 mapPos;

    private bool AddObstacleMode;   //添加障碍物
    private bool AddStartPosMode;   //添加起点
    private bool AddEndPosMode;     //添加终点
    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        Ray ray = cameraa.ScreenPointToRay(eventData.position);
        RaycastHit hit;
        bool res = Physics.Raycast(ray, out hit);
        if (!res) return;



        ///添加障碍物
        if (AddObstacleMode)
        {
            AStarManager.Instance.AddObstacle(hit.point, Cell);
            return;
        }
        ///寻路
        if (startObj == null)
        {
            //清除所有路径
            for (int i = 0; i < Map.transform.childCount; i++)
            {
                Destroy(Map.transform.GetChild(i).gameObject);
            }
            startObj = Instantiate(Cell);
            startObj.transform.SetParent(Map.transform);
            startObj.name = "startObj";
            startObj.transform.position = hit.point;
            return;
        }

        List<Vector3> result = AStarManager.Instance.FindPath(startObj.transform.position, hit.point);
        if (result == null) return;

        //清除所有路径
        for (int i = 0; i < Map.transform.childCount; i++)
        {
            Destroy(Map.transform.GetChild(i).gameObject);
        }

        foreach (var item in result)
        {

            var obj = Instantiate(Cell);
            obj.transform.SetParent(Map.transform);
            obj.transform.position = item;
            
        }


        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }


    private void Start()
    {
        mapPos = Map.transform.position;
        Renderer mapRenderer = Map.GetComponent<Renderer>();
        Vector3 mapSize = mapRenderer.bounds.size;
        gridSize = new Vector2(mapSize.x, mapSize.z);
        OnFindBtnClick();
        AddObstacleBtn.onClick.AddListener(OnObstacleBtnClick);
        FindPathBtn.onClick.AddListener(OnFindBtnClick);
        AStarManager.Instance.CreateGrid(Map, new Vector2(1f, 1f));
    }

    private void OnObstacleBtnClick()
    {
        AddObstacleMode = true;

    }

    private void OnFindBtnClick()
    {
        AddObstacleMode = false;
    }


}
