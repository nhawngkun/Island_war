using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class GameController : Singleton<GameController>
{
    [Title("Manager References")]
    [SerializeField, Required] private BuidingManager buidingManager;
    
    public BuidingManager BuidingManager => buidingManager;

    [Title("Data References")]
    [Tooltip("Kéo file ScriptableObject BuildingData của bạn vào đây")]
    [SerializeField, Required] public BuildingData BuildingData; 
    [Tooltip("Kéo file ScriptableObject SoldierData bạn vừa tạo vào đây")]
    [SerializeField, Required] public SoldierData soldierData;   

    [Title("Soldier Inventory")]
    [ShowInInspector, ReadOnly]
    // Dictionary để lưu số lượng lính: Key = ID lính, Value = Số lượng
    private Dictionary<int, int> soldierInventory = new Dictionary<int, int>();

    public override void Awake()
    {
        base.Awake();
        InitializeSoldierInventory();
    }

    private void Start()
    {
        buidingManager.Initialize();
    }
    
    /// <summary>
    /// Khởi tạo kho lính với số lượng mặc định là 0 cho tất cả các loại lính.
    /// </summary>
    private void InitializeSoldierInventory()
    {
        if (BuildingData == null)
        {
            Debug.LogError("BuildingData chưa được gán trong GameController!");
            return;
        }

        foreach (var soldierItem in BuildingData.soldierLayerItems)
        {
            if (!soldierInventory.ContainsKey(soldierItem.id))
            {
                soldierInventory.Add(soldierItem.id, 0);
            }
        }
        Debug.Log("✅ Kho lính đã được khởi tạo với số lượng mặc định là 0.");
    }

    /// <summary>
    /// Thêm một số lượng lính vào kho.
    /// </summary>
    public void AddSoldier(int soldierId, int amount = 1)
    {
        if (soldierInventory.ContainsKey(soldierId))
        {
            soldierInventory[soldierId] += amount;
            Debug.Log($"👍 Đã cộng {amount} lính (ID: {soldierId}). Tổng số: {soldierInventory[soldierId]}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Không tìm thấy lính với ID {soldierId} trong kho!");
        }
    }

    /// <summary>
    /// Tiêu thụ (trừ đi) một đơn vị lính từ kho. Dùng sau khi xây nhà lính.
    /// </summary>
    /// <param name="soldierId">ID của lính cần tiêu thụ.</param>
    public void UseSoldier(int soldierId)
    {
        if (soldierInventory.ContainsKey(soldierId) && soldierInventory[soldierId] > 0)
        {
            soldierInventory[soldierId]--;
            Debug.Log($"✔️ Đã sử dụng 1 lính (ID: {soldierId}). Còn lại: {soldierInventory[soldierId]}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Không thể sử dụng lính ID {soldierId} vì không có trong kho!");
        }
    }

    /// <summary>
    /// Lấy số lượng của một loại lính cụ thể.
    /// </summary>
    public int GetSoldierCount(int soldierId)
    {
        soldierInventory.TryGetValue(soldierId, out int count);
        return count;
    }
}