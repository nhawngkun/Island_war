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
    private BuildingData buildingData;

    [Title("References")]
    [SerializeField, Required] private Camera mainCamera;
    [SerializeField, Required] private GameObject upgradeIconPrefab;

    [Title("Layer Masks")]
    [SerializeField] private LayerMask houseMask;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private LayerMask buildingMask;
    [SerializeField] private LayerMask buildingUpLvMask;
    [SerializeField] private LayerMask upgradeIconLayerMask;

    [Title("Building Settings")]
    [SerializeField] private float liftHeight = 5f;
    [Tooltip("Điều chỉnh vị trí của icon nâng cấp so với tâm của công trình (X, Y, Z)")]
    [SerializeField] private Vector3 upgradeIconOffset = new Vector3(0, 0, -1.5f);

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

    private Dictionary<GameObject, BuildingInstanceInfo> placedBuildings = new Dictionary<GameObject, BuildingInstanceInfo>();
    private BuildingTile hoveredTile;
    private BuildingTile lastHoveredTile;

    private GameObject currentUpgradeIcon;
    private BuildingTile tileWithActiveIcon;

    public void Initialize()
    {
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
        if (isMovingBuilding)
        {
            HandleBuildingMovement();
            return;
        }

        if (isBuildMode && selectedItem != null)
        {
            HandleMouseHover();
            if (Input.GetMouseButtonDown(0)) TryPlaceBuilding();
            if (Input.GetKeyDown(KeyCode.Escape)) CancelBuildMode();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitIcon, 1000f, upgradeIconLayerMask))
        {
            RequestUpgradeByClick(hitIcon.collider.GetComponent<UpgradeIcon>().TargetTile);
            return;
        }

        if (Physics.Raycast(ray, out RaycastHit hitBuilding, 1000f, buildingMask | buildingUpLvMask))
        {
            BuildingTile clickedTile = hitBuilding.collider.GetComponentInParent<BuildingTile>();
            if (clickedTile == null || !clickedTile.IsOccupied) return;

            placedBuildings.TryGetValue(clickedTile.CurrentBuilding, out BuildingInstanceInfo instanceInfo);
            if (instanceInfo == null) return;

            if (clickedTile == tileWithActiveIcon)
            {
                CloseUpgradeIcon();
                return;
            }

            CloseUpgradeIcon();

            bool shouldShowIcon = instanceInfo.itemData.buildingType == BuildingType.House ||
                                  instanceInfo.itemData.buildingType == BuildingType.MainHouse ||
                                  instanceInfo.itemData.buildingType == BuildingType.WoodenWall;

            if (shouldShowIcon)
            {
                ShowUpgradeIcon(clickedTile);
            }

            StartMovingBuilding(clickedTile, instanceInfo);
        }
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
            
            if (currentUpgradeIcon != null)
            {
                Vector3 buildingBasePosition = new Vector3(buildingToMove.transform.position.x, targetPos.y, buildingToMove.transform.position.z);
                currentUpgradeIcon.transform.position = buildingBasePosition + upgradeIconOffset;
            }

            BuildingTile hitTile = hit.collider.GetComponent<BuildingTile>();
            if (hitTile != null && hitTile.LayerType == originalTile.LayerType)
            {
                currentFrameTile = hitTile;
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
            CloseUpgradeIcon();
        }
        else if (hoveredTile != null && hoveredTile.LayerType == originalTile.LayerType)
        {
            if (hoveredTile.IsOccupied)
            {
                SwapBuildings(originalTile, hoveredTile);
                tileWithActiveIcon = hoveredTile;
                actionTaken = true;
            }
            else
            {
                GameObject originalPrefab = GetPrefabForLevel(movingInstanceInfo);
                hoveredTile.PlaceExistingBuilding(buildingToMove, originalPrefab);
                placedBuildings.Add(buildingToMove, movingInstanceInfo);
                tileWithActiveIcon = hoveredTile;
                Debug.Log($"✓ Đã di chuyển {movingInstanceInfo.itemData.itemName} đến vị trí mới.");
                actionTaken = true;
            }
        }

        if (!actionTaken)
        {
            GameObject originalPrefab = GetPrefabForLevel(movingInstanceInfo);
            originalTile.PlaceExistingBuilding(buildingToMove, originalPrefab);
            placedBuildings.Add(buildingToMove, movingInstanceInfo);
            tileWithActiveIcon = originalTile;
            Debug.LogWarning("✗ Vị trí không hợp lệ! Trả về vị trí cũ.");
        }

        UpdateHighlight(null);
        isMovingBuilding = false;
        buildingToMove = null;
        originalTile = null;
        movingInstanceInfo = null;
        hoveredTile = null;
        canMergeThisBuilding = false;

        if (tileWithActiveIcon != null && currentUpgradeIcon != null)
        {
             currentUpgradeIcon.transform.position = tileWithActiveIcon.CenterPosition + upgradeIconOffset;
        }
    }
    
    #region Các hàm không thay đổi
    private void ShowUpgradeIcon(BuildingTile targetTile)
    {
        if (upgradeIconPrefab == null) return;
        Vector3 iconPosition = targetTile.CenterPosition + upgradeIconOffset;
        currentUpgradeIcon = Instantiate(upgradeIconPrefab, iconPosition, Quaternion.identity);
        currentUpgradeIcon.GetComponent<UpgradeIcon>().Initialize(targetTile);
        tileWithActiveIcon = targetTile;
    }

    public void CloseUpgradeIcon()
    {
        if (currentUpgradeIcon != null)
        {
            Destroy(currentUpgradeIcon);
            currentUpgradeIcon = null;
        }
        tileWithActiveIcon = null;
    }
    
    public void RequestUpgradeByClick(BuildingTile tileToUpgrade)
    {
        if (tileToUpgrade == null) return;
        if (!placedBuildings.TryGetValue(tileToUpgrade.CurrentBuilding, out BuildingInstanceInfo info)) return;

        int currentLevelIndex = info.currentLevel;
        int nextLevelIndex = currentLevelIndex + 1;

        if (info.itemData.levelPrefabs.Count <= nextLevelIndex) { Debug.LogWarning("Đã đạt cấp độ tối đa!"); return; }
        if (info.itemData.upgradeCosts.Count <= currentLevelIndex) { Debug.LogError($"Không có chi phí nâng cấp cho lv {nextLevelIndex + 1}"); return; }

        int upgradeCost = info.itemData.upgradeCosts[currentLevelIndex];
        if (!GameManager.Instance.CanAfford(upgradeCost)) { Debug.LogWarning("Không đủ tiền!"); return; }

        if (info.itemData.buildingType == BuildingType.House)
        {
            if (nextLevelIndex >= GameManager.Instance.MainHouseLevel)
            {
                Debug.LogWarning($"Cần nâng cấp Nhà Chính lên Level {nextLevelIndex + 1} trước!");
                return;
            }
        }

        GameManager.Instance.SpendMoney(upgradeCost);
        info.currentLevel++;
        GameObject newPrefab = info.itemData.levelPrefabs[info.currentLevel];

        placedBuildings.Remove(tileToUpgrade.CurrentBuilding);
        tileToUpgrade.RemoveBuilding();
        tileToUpgrade.PlaceBuilding(newPrefab);
        placedBuildings.Add(tileToUpgrade.CurrentBuilding, info);

        if (info.itemData.buildingType == BuildingType.MainHouse)
        {
            GameManager.Instance.SetMainHouseLevel(info.currentLevel + 1);
        }

        Debug.Log($"✨ Đã nâng cấp {info.itemData.itemName} lên Level {info.currentLevel + 1}!");
        CloseUpgradeIcon();
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

        targetInfo.currentLevel++;
        int newLevelIndex = targetInfo.currentLevel;
        GameObject newPrefab = targetInfo.itemData.levelPrefabs[newLevelIndex];

        placedBuildings.Remove(targetBuilding);
        onTile.RemoveBuilding();
        onTile.PlaceBuilding(newPrefab);
        placedBuildings.Add(onTile.CurrentBuilding, targetInfo);

        Debug.Log($"✨ Đã hợp nhất và nâng cấp {targetInfo.itemData.itemName} lên Level {targetInfo.currentLevel + 1}!");
    }

    private void StartMovingBuilding(BuildingTile sourceTile, BuildingInstanceInfo instanceInfo)
    {
        isMovingBuilding = true;
        originalTile = sourceTile;
        movingInstanceInfo = instanceInfo;
        buildingToMove = sourceTile.ReleaseBuilding();
        placedBuildings.Remove(buildingToMove);

        canMergeThisBuilding = (buildingUpLvMask.value & (1 << buildingToMove.layer)) != 0;
        Vector3 currentPos = buildingToMove.transform.position;
        buildingToMove.transform.position = new Vector3(currentPos.x, currentPos.y + liftHeight, currentPos.z);
        Debug.Log($"→ Đang di chuyển: {movingInstanceInfo.itemData.itemName} (Level {movingInstanceInfo.currentLevel + 1})");
    }
    
    private GameObject GetPrefabForLevel(BuildingInstanceInfo info)
    {
        if (info.itemData.levelPrefabs != null && info.itemData.levelPrefabs.Count > info.currentLevel && info.itemData.levelPrefabs[info.currentLevel] != null)
        {
            return info.itemData.levelPrefabs[info.currentLevel];
        }
        return info.itemData.prefab;
    }

    private void SwapBuildings(BuildingTile tileA, BuildingTile tileB)
    {
        GameObject buildingB = tileB.CurrentBuilding;
        BuildingInstanceInfo infoB = placedBuildings[buildingB];

        tileB.ReleaseBuilding();
        placedBuildings.Remove(buildingB);

        GameObject prefabForMoving = GetPrefabForLevel(movingInstanceInfo);
        tileB.PlaceExistingBuilding(buildingToMove, prefabForMoving);
        placedBuildings.Add(buildingToMove, movingInstanceInfo);

        GameObject prefabForB = GetPrefabForLevel(infoB);
        tileA.PlaceExistingBuilding(buildingB, prefabForB);
        placedBuildings.Add(buildingB, infoB);

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

    // *** UPDATED: Kiểm tra và tiêu thụ lính từ kho ***
    private void TryPlaceBuilding()
    {
        if (hoveredTile == null) return;
        
        // Kiểm tra nếu đang đặt lính (BuildingLayer.Soldier)
        if (selectedLayer == BuildingLayer.Soldier)
        {
            // Kiểm tra xem có đủ lính trong kho không
            int soldierCount = GameController.Instance.GetSoldierCount(selectedItem.id);
            
            if (soldierCount <= 0)
            {
                Debug.LogWarning($"⚠️ Không đủ lính {selectedItem.itemName} trong kho! Cần quay thêm lính.");
                return;
            }
        }
        
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
                
                // *** UPDATED: Tiêu thụ 1 lính từ kho sau khi đặt thành công ***
                if (selectedLayer == BuildingLayer.Soldier)
                {
                    GameController.Instance.UseSoldier(selectedItem.id);
                }
                
                Debug.Log($"✓ Đã đặt {selectedItem.itemName} tại {hoveredTile.CenterPosition}");
            }
        }
        else
        {
            Debug.LogWarning("✗ Không thể xây dựng tại vị trí này!");
        }
    }

    public void SelectBuilding(int buildingId, BuildingLayer layer)
    {
        if (buildingData == null) { Debug.LogError("BuildingManager chưa được khởi tạo!"); return; }
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
    #endregion
}