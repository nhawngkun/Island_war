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
    Soldier = 2 // Layer cho lính
}

public enum BuildingType
{
    House,
    Soldier,
    WoodenWall,
    MainHouse // ADDED: Thêm nhà chính
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
    
    [TabGroup("Soldier Layer", "Soldiers")]
    [TableList(ShowIndexLabels = true, AlwaysExpanded = true)]
    public List<BuildingItem> soldierLayerItems = new List<BuildingItem>();

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
    
    [Tooltip("Chi phí để xây dựng ban đầu")]
    public int cost;

    // ADDED: Danh sách chi phí để nâng cấp. Index 0 = chi phí từ lv1->2, index 1 = chi phí từ lv2->3...
    [Tooltip("Chi phí để nâng cấp lên các cấp độ tiếp theo (ví dụ: index 0 là chi phí để lên level 2)")]
    public List<int> upgradeCosts = new List<int>();

    [Tooltip("Danh sách các prefab tương ứng với các cấp độ (ví dụ: index 0 là level 1, index 1 là level 2,...)")]
    public List<GameObject> levelPrefabs = new List<GameObject>();
}