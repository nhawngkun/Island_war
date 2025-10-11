
using UnityEngine;
using Sirenix.OdinInspector;

public class GameManager : Singleton<GameManager> 
{
    [Title("Game Data Assets")]
    [SerializeField, Required] private BuildingData buildingData;

  
    public BuildingData BuildingData => buildingData;

  
    public override void Awake()
    {
       
        base.Awake(); 
        
        
    }

   
    public void SaveData()
    {
        Debug.Log("Đang lưu dữ liệu game...");
       
    }

    public void LoadData()
    {
        Debug.Log("Đang tải dữ liệu game...");
       
    }

    // Bạn có thể thêm các phương thức ở đây để ghi nhận sự thay đổi dữ liệu
    public void RecordBuildingChange(BuildingTile tile, BuildingItem item)
    {
        Debug.Log($"Đã ghi nhận thay đổi: {item.itemName} được đặt tại {tile.name}");
        // Tại đây bạn sẽ cập nhật mô hình dữ liệu lưu trữ của mình
    }
}