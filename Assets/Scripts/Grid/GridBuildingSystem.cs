using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GridBuildingSystem : MonoBehaviour
{
    [SerializeField] private PrefabsSO prefabs;
    [SerializeField] private Transform BuildingHolder;
    PlacedObjectTypeSO placedObjectTypeSO;
    public static GridBuildingSystem Instance { get; private set; }
    [HideInInspector] public Barracks ChoosenBarraks;
    [Space]
    [SerializeField] private GameObject BarracksGhost;
    [SerializeField] private GameObject PowerPlantGhost;
    private GameObject Ghost;
    private Pathfinding pathfinding;

    private Grid<GridObject> grid;
    private void Awake()
    {
        Ghost = BarracksGhost;

        int gridWidth = 100;
        int gridHeight = 100;
        float cellSize = 1f;

        Instance = this;
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, Vector3.zero, (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));
        pathfinding = new Pathfinding(gridWidth, gridHeight, cellSize);
    }

    public void Build()
    {
        if (placedObjectTypeSO != null)
        {
            grid.GetXY(UtilsClass.GetMouseWorldPosition(), out int x, out int z);

            GridObject gridObject = grid.GetGridObject(x, z);
            List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(new Vector2Int(x, z));

            bool canBuild = true;
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                if (!grid.isValid(gridPosition.x, gridPosition.y) || !grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                {
                    canBuild = false;
                    break;
                }
            }
            if (canBuild)
            {
                Transform buildTransform = Instantiate(placedObjectTypeSO.prefab, grid.GetWorldPosition(x, z), Quaternion.identity, BuildingHolder);
                buildTransform.GetComponent<HealthSystem>().SetGridList(gridPositionList);
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    pathfinding.GetNode(gridPosition.x, gridPosition.y).SetIsWalkable(!pathfinding.GetNode(gridPosition.x, gridPosition.y).isWalkable);
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetTransform(buildTransform);
                }
                CancelBuilding();
            }
            else
            {
                Spawner.Instance.CreateWorldTextPopup("Cannot build here!", UtilsClass.GetMouseWorldPosition());
            }
        }
    }

    public bool isWalkable(int _x, int _y)
    {
        return pathfinding.GetNode(_x, _y).isWalkable;
    }

    public void ClearArea(List<Vector2Int> _gridPositionList)
    {
        foreach (Vector2Int gridPosition in _gridPositionList)
        {
            pathfinding.GetNode(gridPosition.x, gridPosition.y).SetIsWalkable(true);
            grid.GetGridObject(gridPosition.x, gridPosition.y).ClearTransform();
        }

    }

    public void CancelBuilding()
    {
        Ghost.SetActive(false);
        placedObjectTypeSO = null;
        UIManager.Instance.UnSelectBuildingType();
    }

    public void ShowGhost()
    {
        if (placedObjectTypeSO != null)
        {
            grid.GetXY(UtilsClass.GetMouseWorldPosition(), out int x, out int z);
            Ghost.transform.position = grid.GetWorldPosition(x, z);
        }
    }

    public void SelectToBuild(BuildingType buildingType)
    {
        Ghost.SetActive(false);
        switch (buildingType)
        {
            case BuildingType.Barracks:
                placedObjectTypeSO = prefabs.Barracks;
                Ghost = BarracksGhost;
                break;
            case BuildingType.PowerPlant:
                placedObjectTypeSO = prefabs.PowerPlant;
                Ghost = PowerPlantGhost;
                break;
        }
        Ghost.SetActive(true);
    }

    public void SelectBuilding(PowerPlant powerPlant)
    {
        UIManager.Instance.SelectPowerPlant();
    }

    public void SelectBuilding(Barracks barracks)
    {
        UIManager.Instance.SelectBarracks();
        ChoosenBarraks = barracks;
    }

    public void HideFlag()
    {
        if (ChoosenBarraks != null)
        {
            ChoosenBarraks.ShowFlag(false);
            ChoosenBarraks = null;
        }
    }

    public void PlaceFlag(Vector2 pos)
    {
        if (ChoosenBarraks != null && isWalkable((int)pos.x, (int)pos.y))
        {
            ChoosenBarraks.PlaceFlag(pos);
        }
    }

    public class GridObject
    {
        private Grid<GridObject> grid;
        private int x;
        private int y;
        private Transform transform;

        public GridObject(Grid<GridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetTransform(Transform transform)
        {
            this.transform = transform;
            grid.TriggerGridObjectChanged(x, y);
        }

        public void ClearTransform()
        {
            transform = null;
            grid.TriggerGridObjectChanged(x, y);
        }

        public bool CanBuild()
        {
            return transform == null;
        }

        public override string ToString()
        {
            return x + ", " + y + "\n" + transform;
        }
    }
}

