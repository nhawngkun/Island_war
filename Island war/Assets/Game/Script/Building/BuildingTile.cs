
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

public class BuildingTile : MonoBehaviour
{
    [Title("Tile Settings")]
    [SerializeField] private BuildingLayer layerType;
    
    [Title("Highlight Visuals")]
    [SerializeField] private GameObject highlightPrefab;

    [Title("Runtime Info (Read Only)")]
    [ReadOnly, ShowInInspector] private bool isOccupied = false;
    [ReadOnly, ShowInInspector] private GameObject currentBuilding;

    private Vector3 centerPosition;
    private GameObject highlightInstance;

    public BuildingLayer LayerType => layerType;
    public bool IsOccupied => isOccupied;
    public Vector3 CenterPosition => centerPosition;
    public GameObject CurrentBuilding => currentBuilding;

    void Awake()
    {
        centerPosition = transform.position;
        gameObject.layer = LayerMask.NameToLayer(layerType == BuildingLayer.House ? "House" : "Wall");

        if (highlightPrefab != null)
        {
            highlightInstance = Instantiate(highlightPrefab, transform);
            SetHighlight(false);
        }
    }

    public void SetHighlight(bool isActive)
    {
        if (highlightInstance == null) return;
        highlightInstance.SetActive(isActive);
        float yPos = isActive ? 0.1f : -0.22f; 
        highlightInstance.transform.localPosition = new Vector3(0f, yPos, 0f);
    }

    // CHANGED: Cho phép đặt lính lên layer House
    public bool CanBuild(BuildingLayer requiredLayer)
    {
        // Điều kiện: Chưa bị chiếm VÀ (layer của ô đất khớp với layer yêu cầu HOẶC layer yêu cầu là lính và layer ô đất là nhà)
        return !isOccupied && (layerType == requiredLayer || (requiredLayer == BuildingLayer.Soldier && layerType == BuildingLayer.House));
    }
    
    public bool PlaceBuilding(GameObject buildingPrefab)
    {
        if (isOccupied) return false;
        GameObject newInstance = Instantiate(buildingPrefab, centerPosition, Quaternion.identity);
        return PlaceExistingBuilding(newInstance, buildingPrefab);
    }

    public bool PlaceExistingBuilding(GameObject buildingInstance, GameObject originalPrefab)
    {
        if (isOccupied) return false;

        currentBuilding = buildingInstance;
        currentBuilding.transform.SetParent(transform);
        currentBuilding.transform.localPosition = new Vector3(0f, 0.1f, 0f);
        currentBuilding.transform.localRotation = Quaternion.identity;

        Vector3 parentScale = transform.lossyScale;
        Vector3 originalPrefabScale = originalPrefab.transform.localScale;
        currentBuilding.transform.localScale = new Vector3(
            originalPrefabScale.x / parentScale.x,
            originalPrefabScale.y / parentScale.y,
            originalPrefabScale.z / parentScale.z
        );

        isOccupied = true;
        return true;
    }

    public GameObject ReleaseBuilding()
    {
        if (currentBuilding == null) return null;
        GameObject buildingToMove = currentBuilding;
        currentBuilding = null;
        isOccupied = false;
        buildingToMove.transform.SetParent(null);
        return buildingToMove;
    }

    public void RemoveBuilding()
    {
        if (currentBuilding != null)
        {
            Destroy(currentBuilding);
            currentBuilding = null;
            isOccupied = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = layerType == BuildingLayer.House ? Color.blue : Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale * 0.9f);
    }
}