
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

// ============ ENUMS ============
public enum BuildingLayer
{
    House = 0,  // Layer cho nhà
    Wall = 1,   // Layer cho hàng rào
    Soldier = 2 // ADDED: Layer cho lính
}

public enum BuildingType
{
    House,
    Soldier,    // ADDED
    WoodenWall,
}

// ============ SCRIPTABLE OBJECT DATA ============
[CreateAssetMenu(fileName = "BuildingData", menuName = "CityBuilder/Building Data")]
public class BuildingData : ScriptableObject
{
    [TabGroup("House Layer", "Houses & Units")]
    [TableList(ShowIndexLabels = true, AlwaysExpanded = true)]
    public List<BuildingItem> houseLayerItems = new List<BuildingItem>();

    [TabGroup("Wall Layer", "Walls & Defenses")]
    [TableList(ShowIndexLabels = true, AlwaysExpanded = true)]
    public List<BuildingItem> wallLayerItems = new List<BuildingItem>();

    // ADDED: Tab và List mới cho Soldier
    [TabGroup("Soldier Layer", "Soldiers")]
    [TableList(ShowIndexLabels = true, AlwaysExpanded = true)]
    public List<BuildingItem> soldierLayerItems = new List<BuildingItem>();

    // CHANGED: Cập nhật hàm để tìm kiếm trong cả 3 layer
    public BuildingItem GetItemById(int id, BuildingLayer layer)
    {
        List<BuildingItem> targetList;
        switch (layer)
        {
            case BuildingLayer.House:
                targetList = houseLayerItems;
                break;
            case BuildingLayer.Wall:
                targetList = wallLayerItems;
                break;
            case BuildingLayer.Soldier:
                targetList = soldierLayerItems;
                break;
            default:
                return null;
        }
        return targetList.Find(item => item.id == id);
    }
}

[System.Serializable]
public class BuildingItem
{
    public int id;
    public string itemName;
    public BuildingType buildingType;
    
    [Tooltip("Prefab mặc định hoặc prefab cho Level 1")]
    public GameObject prefab;
    
    public int cost;

    // ADDED: List prefab cho các cấp độ, bạn có thể thêm prefab level 2, 3... vào đây
    [Tooltip("Danh sách các prefab tương ứng với các cấp độ (ví dụ: index 0 là level 1, index 1 là level 2,...)")]
    public List<GameObject> levelPrefabs = new List<GameObject>();
}