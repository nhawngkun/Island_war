using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoldierInventorySlot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image soldierIcon;
    [SerializeField] private TextMeshProUGUI soldierCountText;
    [SerializeField] private Button button;

    // Biến nội bộ
    private int soldierId;
    private int soldierCount;
    private UITroop troopUI; // Tham chiếu đến UI cha (để đóng)

    void Start()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
        button.onClick.AddListener(OnSlotClicked);
    }

    /// <summary>
    /// HÀM SỬA ĐỔI: Thêm tham số 'parentUI' và lưu 'count'
    /// </summary>
    public void Setup(int id, Sprite icon, int count, UITroop parentUI)
    {
        this.soldierId = id;
        this.soldierCount = count; // Lưu lại số lượng
        this.troopUI = parentUI;   // Lưu lại UI cha

        if (soldierIcon != null)
        {
            soldierIcon.sprite = icon;
            soldierIcon.gameObject.SetActive(icon != null); 
        }
        
        if (soldierCountText != null)
        {
            soldierCountText.text = $"x{soldierCount}";
        }
    }

    /// <summary>
    /// HÀM SỬA ĐỔI: Logic chính cho YÊU CẦU 2
    /// </summary>
    private void OnSlotClicked()
    {
        // 1. Kiểm tra xem còn lính không
        // (GameController.UseSoldier cũng kiểm tra, nhưng kiểm tra ở đây sẽ tốt cho UX hơn)
        if (soldierCount <= 0)
        {
            Debug.LogWarning($"Không còn lính (ID: {soldierId}) để xây!");
            // (Bạn có thể thêm âm thanh "lỗi" ở đây)
            return;
        }

        // 2. Kích hoạt BuidingManager
        if (GameController.IsInstanceValid())
        {
            // Logic này sao chép từ BuildingButton.cs
            // BuildingLayer.Soldier là layer lính (theo file BuildingData.cs)
            GameController.Instance.BuidingManager.SelectBuilding(soldierId, BuildingLayer.Soldier);
            
            // 3. (Quan trọng) Đóng kho lính để người dùng thấy map
            if (troopUI != null)
            {
                troopUI.CloseInventory();
            }
        }
        else
        {
            Debug.LogError("GameController is not found in the scene!");
        }
    }

    private void OnDestroy()
    {
        if(button != null)
            button.onClick.RemoveAllListeners();
    }
}