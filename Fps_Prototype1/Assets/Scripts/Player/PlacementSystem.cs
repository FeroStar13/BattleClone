using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlacementSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Grid _grid;
    [SerializeField] GameObject _playerObject;
     Player _player;

    [Header("PlcementIndicators")]
     GameObject _cellIndicator;
    [SerializeField] GameObject _placementPos;
    [SerializeField] GameObject[] _blockIndicator;

    [Header("BuildingBlocks")]
    [SerializeField] GameObject _floor;
    [SerializeField] GameObject _wall;
    [SerializeField] GameObject _ramp;

    public BuildingBlocksEnum _buildingBlocks;

   public enum BuildingBlocksEnum
    {
        Wall,
        Floor,
        Ramp,
        Null //if add another element need to be placed before null
    }

    public GameObject CellIndicator { get => _cellIndicator; set => _cellIndicator = value; }

    private void Start()
    {
        _player = _playerObject.GetComponent<Player>();
    }
    private void Update()
    {
        if(CellIndicator  != null && _player.IsBuildingMode == true)
        {
            // Vector3 mousePos = _inputManager.GetSelectedMapPosition();
            //  Vector3 mousePos = new Vector3(0, 0, Camera.main.transform.position.z + 2.5f);
            Vector3 mousePos = _placementPos.transform.position;

          
            GetXYZ(mousePos, out int x, out int y, out int z);

        Vector3Int gridPosition = _grid.WorldToCell(GetCellCenter(x, y, z));
        CellIndicator.transform.position = _grid.WorldToCell(gridPosition);
         PlacementRotation();
        }

    }

    public void PlacementRotation()
    {
        Quaternion newRotation = Quaternion.identity; // Default rotation
        Quaternion currentRotation = _playerObject.transform.rotation;

        // Determine the new rotation based on the player's rotation
        if (currentRotation.eulerAngles.y >= 45f && currentRotation.eulerAngles.y < 135f)
        {
            newRotation = Quaternion.Euler(0, 90, 0); // Rotate 90 degrees
            //Right
        }
        else if (currentRotation.eulerAngles.y >= 315 && currentRotation.eulerAngles.y < 45f)
        {
            newRotation = Quaternion.Euler(0, 0, 0); // No rotation
            //Front
        }
        else if (currentRotation.eulerAngles.y >= 225 && currentRotation.eulerAngles.y < 315)
        {
            newRotation = Quaternion.Euler(0, -90, 0); // Rotate -90 degrees
            //Left
        }
        else if (currentRotation.eulerAngles.y >= 135f && currentRotation.eulerAngles.y < 225)
        {
            newRotation = Quaternion.Euler(0, 180, 0); // Rotate 180 degrees
            //Back
        }
        //Can Smoothly interpolate the cell indicator's rotation towards the new rotation but with that hight value does it immediately
        CellIndicator.transform.rotation = Quaternion.Lerp(CellIndicator.transform.rotation, newRotation, Time.deltaTime * 10000);

    }

    Vector3 GetCellCenter(int x, int y, int z)
    {
       return new Vector3 (x, y, z) * 4 + Vector3.one * 2;
    }

    void GetXYZ(Vector3 worldPosition, out int x, out int y, out int z)
    {
        //Math that calculates the new cell size for the grid
        x = Mathf.FloorToInt(worldPosition.x / 4);
        y = Mathf.FloorToInt(worldPosition.y / 4);
        z = Mathf.FloorToInt(worldPosition.z / 4);

        if(y <= 0)
        {
          y = 0;
        }
    }

   public void Build()
    {
        GameObject newBuild;

        switch (_buildingBlocks)
        {
            case BuildingBlocksEnum.Wall:
             newBuild = Instantiate(_wall, _blockIndicator[(int)BuildingBlocksEnum.Wall].transform.position, _blockIndicator[(int)BuildingBlocksEnum.Wall].transform.rotation);
             break;

            case BuildingBlocksEnum.Floor:
             newBuild = Instantiate(_floor, _blockIndicator[(int)BuildingBlocksEnum.Floor].transform.position, _blockIndicator[(int)BuildingBlocksEnum.Floor].transform.rotation);
             break;

            case BuildingBlocksEnum.Ramp:
             newBuild = Instantiate(_ramp, _blockIndicator[(int)BuildingBlocksEnum.Ramp].transform.position, _blockIndicator[(int)BuildingBlocksEnum.Ramp].transform.rotation);
             break;

        }
    }

    public void ChangePlacementBlock()
    {
        if(_cellIndicator != null)
        {
        _cellIndicator.SetActive(true);
        }

        for (int i = 0; i < _blockIndicator.Length; i++)
        {
            _blockIndicator[i].SetActive(false);
        }

        if(_buildingBlocks == BuildingBlocksEnum.Wall)
        {
            _blockIndicator[(int)BuildingBlocksEnum.Wall].SetActive(true);
            _cellIndicator = _blockIndicator[(int)BuildingBlocksEnum.Wall];
        }
        else if(_buildingBlocks == BuildingBlocksEnum.Floor)
        {
            _blockIndicator[(int)BuildingBlocksEnum.Floor].SetActive(true);
            _cellIndicator = _blockIndicator[(int)BuildingBlocksEnum.Floor];
        }
        else if (_buildingBlocks == BuildingBlocksEnum.Ramp)
        {
            _blockIndicator[(int)BuildingBlocksEnum.Ramp].SetActive(true);
            _cellIndicator = _blockIndicator[(int)BuildingBlocksEnum.Ramp];
        }
    }
}
