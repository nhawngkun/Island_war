using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;       // Thư viện DOTween cho animation
using System.Collections;
using System.Linq;      // Thư viện LINQ để truy vấn dữ liệu

public class UIRecruit : UICanvas
{
    [Header("UI References")]
    [Tooltip("Nút bấm để bắt đầu quay tướng")]
    [SerializeField] private Button recruitButton;

    [Tooltip("Image sẽ thực hiện animation xoay")]
    [SerializeField] private Image spinningImage;

    [Tooltip("Image hiển thị thẻ tướng kết quả (nằm đè lên trên spinningImage)")]
    [SerializeField] private Image soldierCardImage;
    [SerializeField] private Image soldierCardImageBg;

    [Header("Animation Settings")]
    [Tooltip("Thời gian animation xoay (giây)")]
    [SerializeField] private float spinDuration = 10f;

    [Tooltip("Độ phóng to của ảnh sau khi xoay xong")]
    [SerializeField] private float postSpinScale = 1.3f;

    [Tooltip("Thời gian cho animation phóng to và thu nhỏ")]
    [SerializeField] private float scaleDuration = 0.25f;

    // Cờ để ngăn người dùng bấm nút liên tục khi đang quay
    private bool isRecruiting = false;

    void Start()
    {
        if (recruitButton != null)
        {
            recruitButton.onClick.AddListener(OnRecruitButtonClicked);
        }
        
        if (soldierCardImage != null)
        {
            soldierCardImage.gameObject.SetActive(false);
            soldierCardImageBg.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Hàm được gọi khi người dùng bấm nút quay tướng.
    /// </summary>
    private void OnRecruitButtonClicked()
    {
        if (isRecruiting) return;
        
        StartCoroutine(RecruitSequence());
    }

    /// <summary>
    /// Coroutine xử lý toàn bộ quá trình: animation -> chọn lính -> hiển thị kết quả.
    /// </summary>
    /// <summary>
/// Coroutine xử lý toàn bộ quá trình: animation -> chọn lính -> hiển thị kết quả.
/// </summary>
private IEnumerator RecruitSequence()
{
    isRecruiting = true;
    recruitButton.interactable = false;

    // --- BƯỚC 1: CHUẨN BỊ ---
    // Ẩn thẻ kết quả và thẻ animation trước
    soldierCardImage.gameObject.SetActive(false);
    soldierCardImageBg.gameObject.SetActive(false);
    spinningImage.gameObject.SetActive(true);

    // Đặt "thẻ animation" (tái sử dụng spinningImage) ở vị trí xuất phát
    // Ví dụ: ở giữa màn hình nhưng vô hình và rất nhỏ
    spinningImage.transform.localPosition = Vector3.zero;
    spinningImage.transform.localScale = Vector3.zero;
    spinningImage.color = new Color(1, 1, 1, 0.5f); // Làm cho nó mờ ảo

    // Lấy thông tin lính ngẫu nhiên TRƯỚC khi bắt đầu animation
    var availableSoldierIds = GameController.Instance.BuildingData.soldierLayerItems.Select(s => s.id).ToList();
    if (availableSoldierIds.Count == 0)
    {
        Debug.LogError("Không có lính nào trong BuildingData! Không thể quay.");
        isRecruiting = false;
        recruitButton.interactable = true;
        yield break;
    }
    int randomSoldierId = availableSoldierIds[Random.Range(0, availableSoldierIds.Count)];
    Sprite resultSprite = GameController.Instance.soldierData.GetSpriteById(randomSoldierId);

    if (resultSprite == null)
    {
        Debug.LogError($"Không tìm thấy Sprite cho lính có ID: {randomSoldierId} trong SoldierData!");
        isRecruiting = false;
        recruitButton.interactable = true;
        yield break;
    }

    // --- BƯỚC 2: TẠO SEQUENCE ANIMATION MỚI ---
    Sequence mySequence = DOTween.Sequence();
    float animationDuration = 0.7f; // Tổng thời gian hiệu ứng phóng lên

    // GIAI ĐOẠN 1: Phóng to thẻ animation (giống như năng lượng tụ lại)
    mySequence.Append(spinningImage.transform.DOScale(postSpinScale * 1.2f, animationDuration).SetEase(Ease.OutExpo));
    mySequence.Join(spinningImage.DOFade(1, animationDuration / 2)); // Hiện rõ dần

    // GIAI ĐOẠN 2: Lóe sáng và HIỆN KẾT QUẢ
    // Dùng InsertCallback để thực hiện hành động này ngay trước khi giai đoạn 1 kết thúc
    mySequence.InsertCallback(animationDuration - 0.1f, () => {
        // 1. Ẩn thẻ animation
        spinningImage.gameObject.SetActive(false);

        // 2. Chuẩn bị và hiển thị thẻ kết quả
        soldierCardImage.sprite = resultSprite;
        soldierCardImage.transform.localScale = Vector3.one * postSpinScale * 1.2f; // Bắt đầu ở kích thước lớn
        soldierCardImage.gameObject.SetActive(true);
        soldierCardImageBg.gameObject.SetActive(true);
        
        // 3. (QUAN TRỌNG) Hiệu ứng flash lóe sáng từ background
       
    });

    // GIAI ĐOẠN 3: Thẻ kết quả thu nhỏ lại và ổn định vị trí
    mySequence.Append(soldierCardImage.transform.DOScale(1f, 0.5f).SetEase(Ease.OutQuad));

    // Chờ sequence chính hoàn thành
    yield return mySequence.Play().WaitForCompletion();

    // Thêm lính vào kho của người chơi
    GameController.Instance.AddSoldier(randomSoldierId);

    // GIAI ĐOẠN 4: Hiệu ứng "nhấn mạnh" cuối cùng để tạo sự thỏa mãn
    yield return soldierCardImage.transform.DOScale(1.1f, 0.15f).SetEase(Ease.OutQuad).WaitForCompletion();
    yield return soldierCardImage.transform.DOScale(1.0f, 0.2f).SetEase(Ease.InQuad).WaitForCompletion();

    // --- BƯỚC 3: RESET TRẠNG THÁI ---
    isRecruiting = false;
    recruitButton.interactable = true;
}
    private void OnDestroy()
    {
        if (recruitButton != null)
        {
            recruitButton.onClick.RemoveAllListeners();
        }
        if (spinningImage != null)
        {
            spinningImage.transform.DOKill();
        }
    }
}