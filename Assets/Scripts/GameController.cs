using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    private Vector3 startPosition;
    private List<SoldierUnit> selectedUnitList = new List<SoldierUnit>();
    [SerializeField] private Transform selectionAreaTransform;
    private void Awake()
    {
        Instance = this;
        selectionAreaTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            BuildingManager.Instance.Build();
            startPosition = UtilsClass.GetMouseWorldPosition();
            selectionAreaTransform.gameObject.SetActive(true);
        }
        if (Input.GetMouseButton(0))
        {
            SelectArea();
        }
        if (Input.GetMouseButtonUp(0))
        {
            SelectUnits();
        }
        if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            BuildingManager.Instance.SetFlag();
            ControlUnits();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.UnSelectBuildingType();
        }
        BuildingManager.Instance.ShowGhost();
    }

    private void ControlUnits()
    {
        Vector3 moveToPosition = UtilsClass.GetMouseWorldPosition();
        List<Vector3> targetPositionList = GetPositionListAround(moveToPosition, new float[] { 1f, 2f, 3f }, new int[] { 5, 10, 20 });
        int targetPositionListIndex = 0;

        foreach (SoldierUnit soldierUnit in selectedUnitList)
        {
            soldierUnit.MoveTo(targetPositionList[targetPositionListIndex]);
            targetPositionListIndex = (targetPositionListIndex + 1) % targetPositionList.Count;
        }
    }

    private void SelectArea()
    {
        Vector3 currentMousePosition = UtilsClass.GetMouseWorldPosition();
        Vector3 lowerLeft = new Vector3(
            Mathf.Min(startPosition.x, currentMousePosition.x),
            Mathf.Min(startPosition.y, currentMousePosition.y)
        );
        Vector3 upperRight = new Vector3(
            Mathf.Max(startPosition.x, currentMousePosition.x),
            Mathf.Max(startPosition.y, currentMousePosition.y)
        );
        selectionAreaTransform.position = lowerLeft;
        selectionAreaTransform.localScale = upperRight - lowerLeft;
    }

    private List<Vector3> GetPositionListAround(Vector3 startPosition, float[] ringDistanceArray, int[] ringPositionCountArray)
    {
        List<Vector3> positionList = new List<Vector3>();
        positionList.Add(startPosition);
        for (int i = 0; i < ringDistanceArray.Length; i++)
        {
            positionList.AddRange(GetPositionListAround(startPosition, ringDistanceArray[i], ringPositionCountArray[i]));
        }
        return positionList;
    }

    private List<Vector3> GetPositionListAround(Vector3 startPosition, float distance, int positionCount)
    {
        List<Vector3> positionList = new List<Vector3>();
        for (int i = 0; i < positionCount; i++)
        {
            float angle = i * (360f / positionCount);
            Vector3 dir = ApplyRotationToVector(new Vector3(1, 0), angle);
            Vector3 position = startPosition + dir * distance;
            positionList.Add(position);
        }
        return positionList;
    }

    private Vector3 ApplyRotationToVector(Vector3 vec, float angle)
    {
        return Quaternion.Euler(0, 0, angle) * vec;
    }


    private void SelectUnits()
    {
        selectionAreaTransform.gameObject.SetActive(false);
        Collider2D[] collider2DArray = Physics2D.OverlapAreaAll(startPosition, UtilsClass.GetMouseWorldPosition());
        foreach (SoldierUnit soldierUnit in selectedUnitList)
        {
            soldierUnit.SetSelectedVisible(false);
        }
        selectedUnitList.Clear();
        foreach (Collider2D collider2D in collider2DArray)
        {
            SoldierUnit soldierUnit = collider2D.GetComponent<SoldierUnit>();
            if (soldierUnit != null)
            {
                soldierUnit.SetSelectedVisible(true);
                selectedUnitList.Add(soldierUnit);
            }
        }
    }

}