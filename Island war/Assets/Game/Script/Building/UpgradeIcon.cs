// UpgradeIcon.cs

using UnityEngine;

public class UpgradeIcon : MonoBehaviour
{
    private Camera mainCamera;
    private BuildingTile targetTile;

    // Property để BuidingManager có thể lấy thông tin
    public BuildingTile TargetTile => targetTile; 

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        // Luôn xoay UI để đối mặt với camera (billboarding)
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                             mainCamera.transform.rotation * Vector3.up);
        }
    }
    
    /// <summary>
    /// Nhận thông tin về ô đất (tile) mà nó thuộc về.
    /// </summary>
    public void Initialize(BuildingTile tile)
    {
        targetTile = tile;
    }
}