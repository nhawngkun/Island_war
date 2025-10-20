using UnityEngine;
using Sirenix.OdinInspector;

public class GameManager : Singleton<GameManager>
{
    [Title("Game Data Assets")]
    [SerializeField, Required] private BuildingData buildingData;
    

    // ADDED: Quản lý tài nguyên và cấp độ nhà chính
    [Title("Game State")]
    [SerializeField] private int playerMoney = 1000;
    [SerializeField] private int mainHouseLevel = 1;

    public BuildingData BuildingData => buildingData;
    public int MainHouseLevel => mainHouseLevel;
    
    public override void Awake()
    {
        base.Awake();
    }
    
    // ADDED: Phương thức kiểm tra và tiêu tiền
    public bool CanAfford(int amount)
    {
        return playerMoney >= amount;
    }

    public void SpendMoney(int amount)
    {
        if (CanAfford(amount))
        {
            playerMoney -= amount;
            Debug.Log($"Đã tiêu {amount}. Tiền còn lại: {playerMoney}");
            // Cập nhật UI tiền tệ ở đây
        }
    }

    // ADDED: Phương thức để cập nhật cấp độ nhà chính khi nó được nâng cấp
    public void SetMainHouseLevel(int newLevel)
    {
        mainHouseLevel = newLevel;
        Debug.Log($"Nhà chính đã được nâng cấp lên Level {newLevel}!");
    }

    public void SaveData()
    {
        Debug.Log("Đang lưu dữ liệu game...");
    }

    public void LoadData()
    {
        Debug.Log("Đang tải dữ liệu game...");
    }

    public void RecordBuildingChange(BuildingTile tile, BuildingItem item)
    {
        Debug.Log($"Đã ghi nhận thay đổi: {item.itemName} được đặt tại {tile.name}");
    }
}