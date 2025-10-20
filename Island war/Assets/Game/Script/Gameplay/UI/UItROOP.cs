using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UITroop : UICanvas
{
    [Header("UI References")]
    [Tooltip("Nút để MỞ kho lính (ví dụ: 'Xem kho')")]
    [SerializeField] private Button viewInventoryButton;
     [SerializeField] private Button viewRecruitButton;
    
    // --- THÊM MỚI ---
    [Tooltip("Nút để ĐÓNG kho lính (ví dụ: nút 'X' hoặc 'Đóng')")]
    [SerializeField] private Button closeInventoryButton;
    // --- KẾT THÚC THÊM MỚI ---

    [Tooltip("Object cha chứa ScrollView (để bật/tắt)")]
    [SerializeField] private GameObject scrollViewContainer;
    
    [Tooltip("Prefab của ô lính (SoldierInventorySlot)")]
    [SerializeField] private GameObject soldierSlotPrefab;
    
    [Tooltip("Đối tượng 'Content' bên trong ScrollView")]
    [SerializeField] private RectTransform contentPanel;

    // Bỏ biến 'isInventoryOpen' vì 2 nút đã tự quản lý trạng thái

    void Start()
    {
        if (viewInventoryButton != null)
        {
            // Nút MỞ sẽ gọi hàm Mở
            viewInventoryButton.onClick.AddListener(OpenInventory);
        }

        // --- THÊM MỚI ---
        if (closeInventoryButton != null)
        {
            // Nút ĐÓNG sẽ gọi hàm Đóng
            closeInventoryButton.onClick.AddListener(CloseInventory);
        }
        // --- KẾT THÚC THÊM MỚI ---

        // Thiết lập trạng thái ban đầu
        if (scrollViewContainer != null)
        {
            scrollViewContainer.SetActive(false);
        }
        viewInventoryButton.gameObject.SetActive(true);
        viewRecruitButton.gameObject.SetActive(true);
        closeInventoryButton.gameObject.SetActive(false);
    }
     public void Back()
    {
        UIManager.Instance.CloseUI<UITroop>(0f);
        UIManager.Instance.OpenUI<UIHome>();
    }

    /// <summary>
    /// HÀM MỚI: Được gọi khi bấm nút 'viewInventoryButton'
    /// </summary>
    private void OpenInventory()
    {
        if (scrollViewContainer != null)
        {
            scrollViewContainer.SetActive(true);
        }

        // Ẩn nút Mở, Hiện nút Đóng
        viewInventoryButton.gameObject.SetActive(false);
        viewRecruitButton.gameObject.SetActive(false);
        closeInventoryButton.gameObject.SetActive(true);

        // Tải dữ liệu lính
        PopulateInventory();
    }

    /// <summary>
    /// HÀM SỬA ĐỔI: (public)
    /// Được gọi khi bấm nút 'closeInventoryButton' HOẶC khi chọn 1 lính
    /// </summary>
    public void CloseInventory()
    {
        if (scrollViewContainer != null)
        {
            scrollViewContainer.SetActive(false);
        }

        // Ẩn nút Đóng, Hiện nút Mở
        viewInventoryButton.gameObject.SetActive(true);
        viewRecruitButton.gameObject.SetActive(true);
        closeInventoryButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Tải dữ liệu từ GameController và tạo các ô lính
    /// (Giữ nguyên logic từ phiên bản trước)
    /// </summary>
    private void PopulateInventory()
    {
        if (contentPanel == null || soldierSlotPrefab == null)
        {
            Debug.LogError("Chưa gán Content Panel hoặc Soldier Slot Prefab cho UITroop!");
            return;
        }

        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        Dictionary<int, int> inventory = GameController.Instance.GetSoldierInventory();
        SoldierData soldierData = GameController.Instance.soldierData;
        if(soldierData == null)
        {
            Debug.LogError("GameController.Instance.soldierData bị null!");
            return;
        }

        foreach (var entry in inventory)
        {
            int soldierId = entry.Key;
            int soldierCount = entry.Value;

            if (soldierCount > 0)
            {
                Sprite icon = soldierData.GetSpriteById(soldierId);
                
                GameObject slotGO = Instantiate(soldierSlotPrefab, contentPanel);
                
                SoldierInventorySlot slotScript = slotGO.GetComponent<SoldierInventorySlot>();
                if(slotScript != null)
                {
                    // Truyền tham chiếu 'this' (UITroop) vào prefab
                    // để prefab có thể gọi CloseInventory() khi được bấm
                    slotScript.Setup(soldierId, icon, soldierCount, this); 
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (viewInventoryButton != null)
        {
            viewInventoryButton.onClick.RemoveAllListeners();
        }
        
        // --- THÊM MỚI ---
        if (closeInventoryButton != null)
        {
            closeInventoryButton.onClick.RemoveAllListeners();
        }
        // --- KẾT THÚC THÊM MỚI ---
    }
}