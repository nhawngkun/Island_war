using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class BuildingInstanceInfo
{
    public BuildingItem itemData;
    public int currentLevel; 
}

public class BuidingManager : MonoBehaviour
{
    // Tham chiếu đến BuildingData sẽ được lấy từ GameManager
    private BuildingData buildingData;

    [Title("References")]
    [SerializeField, Required] private Camera mainCamera;

    [Title("Layer Masks")]
    [SerializeField] private LayerMask houseMask;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private LayerMask buildingMask;       // Building thường - chỉ di chuyển/swap
    [SerializeField] private LayerMask buildingUpLvMask;   // Building có thể nâng cấp - di chuyển/swap/merge

    [Title("Building Settings")]
    [SerializeField] private float liftHeight = 5f;

    [Title("Runtime Info (Read Only)")]
    [ReadOnly, ShowInInspector] private bool isBuildMode = false;
    [ReadOnly, ShowInInspector] private BuildingItem selectedItem;
    [ReadOnly, ShowInInspector] private BuildingLayer selectedLayer;

    [Title("Move Mode Info (Read Only)")]
    [ReadOnly, ShowInInspector] private bool isMovingBuilding = false;
    private GameObject buildingToMove;
    private BuildingTile originalTile;
    private BuildingInstanceInfo movingInstanceInfo;
    private bool canMergeThisBuilding;

    // Dictionary lưu thông tin của tất cả building đã đặt
    private Dictionary<GameObject, BuildingInstanceInfo> placedBuildings = new Dictionary<GameObject, BuildingInstanceInfo>();

    private BuildingTile hoveredTile;
    private BuildingTile lastHoveredTile;

    /// <summary>
    /// Khởi tạo manager, lấy dữ liệu trực tiếp từ GameManager singleton.
    /// </summary>
    public void Initialize()
    {
        // Lấy BuildingData trực tiếp từ GameManager.Instance
        if (GameManager.IsInstanceValid())
        {
            this.buildingData = GameManager.Instance.BuildingData;
            Debug.Log("BuildingManager đã được khởi tạo thành công.");
        }
        else
        {
            Debug.LogError("Không tìm thấy GameManager trong scene!");
        }
    }

    void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    void Update()
    {
        // Ưu tiên xử lý chế độ di chuyển building
        if (isMovingBuilding)
        {
            HandleBuildingMovement();
            return;
        }

        // Xử lý chế độ xây dựng
        if (isBuildMode && selectedItem != null)
        {
            HandleMouseHover();

            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceBuilding();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelBuildMode();
            }
            return;
        }

        // Xử lý click để bắt đầu di chuyển building
        if (Input.GetMouseButtonDown(0))
        {
            TryStartMoveBuilding();
        }
    }

    private void TryStartMoveBuilding()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, buildingMask | buildingUpLvMask))
        {
            BuildingTile sourceTile = hit.collider.GetComponentInParent<BuildingTile>();

            if (sourceTile != null && sourceTile.IsOccupied)
            {
                if (placedBuildings.TryGetValue(sourceTile.CurrentBuilding, out BuildingInstanceInfo instanceInfo))
                {
                    StartMovingBuilding(sourceTile, instanceInfo);
                }
            }
        }
    }

    private void StartMovingBuilding(BuildingTile sourceTile, BuildingInstanceInfo instanceInfo)
    {
        isMovingBuilding = true;
        originalTile = sourceTile;
        movingInstanceInfo = instanceInfo;
        buildingToMove = originalTile.ReleaseBuilding();
        canMergeThisBuilding = (buildingUpLvMask.value & (1 << buildingToMove.layer)) != 0;

        Vector3 currentPos = buildingToMove.transform.position;
        buildingToMove.transform.position = new Vector3(currentPos.x, currentPos.y + liftHeight, currentPos.z);
        
        string buildingType = canMergeThisBuilding ? "(Có thể merge)" : "(Chỉ di chuyển)";
        Debug.Log($"→ Đang di chuyển: {movingInstanceInfo.itemData.itemName} (Level {movingInstanceInfo.currentLevel + 1}) {buildingType}");
    }

    private void HandleBuildingMovement()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        LayerMask combinedMask = houseMask | wallMask;
        BuildingTile currentFrameTile = null;

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, combinedMask))
        {
            Vector3 targetPos = hit.point;
            buildingToMove.transform.position = new Vector3(targetPos.x, targetPos.y + liftHeight, targetPos.z);
            BuildingTile hitTile = hit.collider.GetComponent<BuildingTile>();

            if (hitTile != null)
            {
                if (hitTile.LayerType == originalTile.LayerType)
                {
                    currentFrameTile = hitTile;
                }
            }
        }

        UpdateHighlight(currentFrameTile);
        hoveredTile = currentFrameTile;

        if (Input.GetMouseButtonUp(0))
        {
            StopMovingBuilding();
        }
    }

    private void StopMovingBuilding()
    {
        bool actionTaken = false;

        if (canMergeThisBuilding && CanMerge())
        {
            PerformMerge(originalTile, hoveredTile);
            actionTaken = true;
        }
        else if (hoveredTile != null && hoveredTile.LayerType == originalTile.LayerType)
        {
            if (hoveredTile.IsOccupied)
            {
                SwapBuildings(originalTile, hoveredTile);
                actionTaken = true;
            }
            else
            {
                GameObject originalPrefab = GetPrefabForLevel(movingInstanceInfo);
                hoveredTile.PlaceExistingBuilding(buildingToMove, originalPrefab);
                placedBuildings[buildingToMove] = movingInstanceInfo;
                Debug.Log($"✓ Đã di chuyển {movingInstanceInfo.itemData.itemName} đến vị trí mới.");
                actionTaken = true;
            }
        }

        if (!actionTaken)
        {
            GameObject originalPrefab = GetPrefabForLevel(movingInstanceInfo);
            originalTile.PlaceExistingBuilding(buildingToMove, originalPrefab);
            placedBuildings[buildingToMove] = movingInstanceInfo;
            Debug.LogWarning("✗ Vị trí không hợp lệ! Trả về vị trí cũ.");
        }

        UpdateHighlight(null);

        isMovingBuilding = false;
        buildingToMove = null;
        originalTile = null;
        movingInstanceInfo = null;
        hoveredTile = null;
        canMergeThisBuilding = false;
    }

    private GameObject GetPrefabForLevel(BuildingInstanceInfo info)
    {
        if (info.itemData.levelPrefabs != null && 
            info.itemData.levelPrefabs.Count > info.currentLevel &&
            info.itemData.levelPrefabs[info.currentLevel] != null)
        {
            return info.itemData.levelPrefabs[info.currentLevel];
        }
        
        return info.itemData.prefab;
    }

    private bool CanMerge()
    {
        if (hoveredTile == null || !hoveredTile.IsOccupied) return false;

        GameObject targetBuilding = hoveredTile.CurrentBuilding;
        if ((buildingUpLvMask.value & (1 << targetBuilding.layer)) == 0) return false;

        if (!placedBuildings.TryGetValue(targetBuilding, out BuildingInstanceInfo targetInfo)) return false;

        bool isSameType = movingInstanceInfo.itemData.id == targetInfo.itemData.id;
        bool isSameLevel = movingInstanceInfo.currentLevel == targetInfo.currentLevel;
        int nextLevel = movingInstanceInfo.currentLevel + 1;
        bool hasNextLevelPrefab = movingInstanceInfo.itemData.levelPrefabs.Count > nextLevel;

        return isSameType && isSameLevel && hasNextLevelPrefab;
    }

    private void PerformMerge(BuildingTile fromTile, BuildingTile onTile)
    {
        GameObject targetBuilding = onTile.CurrentBuilding;
        BuildingInstanceInfo targetInfo = placedBuildings[targetBuilding];

        Destroy(buildingToMove);
        placedBuildings.Remove(buildingToMove);

        targetInfo.currentLevel++;
        int newLevelIndex = targetInfo.currentLevel;
        GameObject newPrefab = targetInfo.itemData.levelPrefabs[newLevelIndex];

        placedBuildings.Remove(targetBuilding);
        onTile.RemoveBuilding();
        onTile.PlaceBuilding(newPrefab);
        placedBuildings.Add(onTile.CurrentBuilding, targetInfo);

        Debug.Log($"✨ Đã hợp nhất và nâng cấp {targetInfo.itemData.itemName} lên Level {targetInfo.currentLevel + 1}!");
    }

    private void SwapBuildings(BuildingTile tileA, BuildingTile tileB)
    {
        GameObject buildingB = tileB.CurrentBuilding;
        BuildingInstanceInfo infoB = placedBuildings[buildingB];

        tileB.ReleaseBuilding();

        GameObject prefabForMoving = GetPrefabForLevel(movingInstanceInfo);
        tileB.PlaceExistingBuilding(buildingToMove, prefabForMoving);
        placedBuildings[buildingToMove] = movingInstanceInfo;

        GameObject prefabForB = GetPrefabForLevel(infoB);
        tileA.PlaceExistingBuilding(buildingB, prefabForB);

        Debug.Log($"🔄 Đã hoán đổi: {movingInstanceInfo.itemData.itemName} ↔ {infoB.itemData.itemName}");
    }

    private void HandleMouseHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        LayerMask currentMask = (selectedLayer == BuildingLayer.House || selectedLayer == BuildingLayer.Soldier) ? houseMask : wallMask;
        hoveredTile = null;

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, currentMask))
        {
            BuildingTile tile = hit.collider.GetComponent<BuildingTile>();
            if (tile != null)
            {
                hoveredTile = tile;
            }
        }

        UpdateHighlight(hoveredTile);
    }

    private void TryPlaceBuilding()
    {
        if (hoveredTile == null) return;

        if (hoveredTile.CanBuild(selectedLayer))
        {
            GameObject prefabToPlace = (selectedItem.levelPrefabs.Count > 0) ? selectedItem.levelPrefabs[0] : selectedItem.prefab;
            bool success = hoveredTile.PlaceBuilding(prefabToPlace);
            if (success)
            {
                BuildingInstanceInfo newInstanceInfo = new BuildingInstanceInfo
                {
                    itemData = selectedItem,
                    currentLevel = 0
                };
                placedBuildings.Add(hoveredTile.CurrentBuilding, newInstanceInfo);
                Debug.Log($"✓ Đã đặt {selectedItem.itemName} tại {hoveredTile.CenterPosition}");

                // Ví dụ về việc thông báo cho GameManager
                // GameManager.Instance.RecordBuildingChange(hoveredTile, selectedItem);
            }
        }
        else
        {
            Debug.LogWarning("✗ Không thể xây dựng tại vị trí này!");
        }
    }

    public void SelectBuilding(int buildingId, BuildingLayer layer)
    {
        if (buildingData == null)
        {
            Debug.LogError("BuildingManager chưa được khởi tạo! Hãy chắc chắn rằng nó đã được kết nối với GameController.");
            return;
        }

        selectedItem = buildingData.GetItemById(buildingId, layer);
        if (selectedItem != null)
        {
            selectedLayer = layer;
            isBuildMode = true;
            Debug.Log($"→ Đã chọn: {selectedItem.itemName} (Layer: {layer})");
        }
        else
        {
            Debug.LogError($"Không tìm thấy building với ID {buildingId} trong layer {layer}");
        }
    }

    public void CancelBuildMode()
    {
        UpdateHighlight(null);
        hoveredTile = null;
        isBuildMode = false;
        selectedItem = null;
        Debug.Log("✗ Đã hủy chế độ xây dựng");
    }

    private void UpdateHighlight(BuildingTile currentTile)
    {
        if (currentTile == lastHoveredTile) return;

        if (lastHoveredTile != null)
        {
            lastHoveredTile.SetHighlight(false);
        }

        if (currentTile != null)
        {
            currentTile.SetHighlight(true);
        }

        lastHoveredTile = currentTile;
    }
}