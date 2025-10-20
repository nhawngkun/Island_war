using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Linq;

// Đổi tên class cho đúng chuẩn (viết hoa chữ C)
public class UICardSpin : UICanvas // Đảm bảo class này kế thừa UICanvas (nếu bạn có)
{
    [Header("UI References")]
    [Tooltip("Image để hiển thị thẻ được chọn (sẽ bay từ ngoài vào)")]
    [SerializeField] private Image cardToShow;
    [Tooltip("Image sẽ thực hiện animation xoay (giống spinningImage cũ)")]
    [SerializeField] private Image spinningImage;
    [Tooltip("Image hiển thị thẻ tướng kết quả")]
    [SerializeField] private Image soldierCardImage;
    [SerializeField] private Image soldierCardImageBg;
    [Tooltip("Nút để đóng UI này (ví dụ: nút 'X' hoặc bấm ra ngoài)")]
    [SerializeField] private Button closeButton; // Hoặc một overlay button

    [Header("Animation Settings")]
    [Tooltip("Vị trí thẻ bay vào (thường là 0,0,0)")]
    [SerializeField] private Vector3 cardCenterPosition = Vector3.zero;
    [Tooltip("Vị trí thẻ bay ra (ví dụ: góc phải màn hình)")]
    [SerializeField] private Vector3 cardStartPosition = new Vector3(1000f, 0, 0);
    [SerializeField] private float cardFlyInDuration = 0.5f;
    [SerializeField] private float postSpinScale = 1.3f;

    // Biến nội bộ
    private int selectedTierIndex;
    private Sprite selectedCardSprite;
    private bool isRecruiting = false;

    void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
        
        // Ẩn tất cả các đối tượng khi bắt đầu
        cardToShow.gameObject.SetActive(false);
        spinningImage.gameObject.SetActive(false);
        soldierCardImage.gameObject.SetActive(false);
        soldierCardImageBg.gameObject.SetActive(false);
    }

    /// <summary>
    /// Hàm này được gọi bởi UIRecruit để truyền dữ liệu
    /// </summary>
    /// <param name="tierIndex">Bậc lính được chọn (0-4)</param>
    /// <param name="cardSprite">Sprite của thẻ được chọn</param>
    public void Setup(int tierIndex, Sprite cardSprite)
    {
        this.selectedTierIndex = tierIndex;
        this.selectedCardSprite = cardSprite;
        
        // Đặt sprite cho thẻ sẽ bay vào
        if (cardToShow != null && cardSprite != null)
        {
            cardToShow.sprite = cardSprite;
        }
        
        // Bắt đầu chuỗi animation
        StartCoroutine(SpinSequence());
    }

    private IEnumerator SpinSequence()
    {
        isRecruiting = true;
        if(closeButton != null) closeButton.interactable = false;

        // --- BƯỚC 1: Thẻ bay từ ngoài vào ---
        cardToShow.gameObject.SetActive(true);
        cardToShow.transform.localPosition = cardStartPosition;
        cardToShow.transform.localScale = Vector3.one * 0.8f; // Bắt đầu nhỏ
        
        // Bay vào giữa và phóng to
        cardToShow.transform.DOLocalMove(cardCenterPosition, cardFlyInDuration).SetEase(Ease.OutBack);
        yield return cardToShow.transform.DOScale(1f, cardFlyInDuration).SetEase(Ease.OutBack).WaitForCompletion();

        // Chờ 1 chút
        yield return new WaitForSeconds(0.25f);

        // --- BƯỚC 2: Thẻ "biến hình" thành animation xoay ---
        // (Đây là logic từ RecruitSequence cũ của UIRecruit)
        
        cardToShow.gameObject.SetActive(false); // Ẩn thẻ bay vào
        
        soldierCardImage.gameObject.SetActive(false);
        soldierCardImageBg.gameObject.SetActive(false);
        spinningImage.gameObject.SetActive(true);
        spinningImage.transform.localPosition = Vector3.zero;
        spinningImage.transform.localScale = Vector3.zero;
        spinningImage.color = new Color(1, 1, 1, 0.5f);

        // Lấy lính ngẫu nhiên (chuyển hàm này từ UIRecruit sang đây)
        int randomSoldierId = GetRandomSoldierByTier(selectedTierIndex);
        if (randomSoldierId == -1)
        {
            Debug.LogError("Không có lính nào trong BuildingData! Không thể quay.");
            OnRecruitFinished();
            yield break;
        }

        Sprite resultSprite = GameController.Instance.soldierData.GetSpriteById(randomSoldierId);
        if (resultSprite == null)
        {
            Debug.LogError($"Không tìm thấy Sprite cho lính có ID: {randomSoldierId} trong SoldierData!");
            OnRecruitFinished();
            yield break;
        }

        // Tạo animation
        Sequence mySequence = DOTween.Sequence();
        float animationDuration = 0.7f;
        mySequence.Append(spinningImage.transform.DOScale(postSpinScale * 1.2f, animationDuration).SetEase(Ease.OutExpo));
        mySequence.Join(spinningImage.DOFade(1, animationDuration / 2));
        mySequence.InsertCallback(animationDuration - 0.1f, () => {
            spinningImage.gameObject.SetActive(false);
            soldierCardImage.sprite = resultSprite;
            soldierCardImage.transform.localScale = Vector3.one * postSpinScale * 1.2f;
            soldierCardImage.gameObject.SetActive(true);
            soldierCardImageBg.gameObject.SetActive(true);
        });
        mySequence.Append(soldierCardImage.transform.DOScale(1f, 0.5f).SetEase(Ease.OutQuad));
        yield return mySequence.Play().WaitForCompletion();

        // Thêm lính vào kho
        GameController.Instance.AddSoldier(randomSoldierId);

        yield return soldierCardImage.transform.DOScale(1.1f, 0.15f).SetEase(Ease.OutQuad).WaitForCompletion();
        yield return soldierCardImage.transform.DOScale(1.0f, 0.2f).SetEase(Ease.InQuad).WaitForCompletion();

        // --- BƯỚC 3: Kết thúc ---
        OnRecruitFinished();
    }

    /// <summary>
    /// Lấy lính ngẫu nhiên (đã chuyển từ UIRecruit sang)
    /// </summary>
    private int GetRandomSoldierByTier(int tierIndex)
    {
        Debug.Log($"Đang quay lính cho bậc có index: {tierIndex}");

        // (Code giả lập) - Bạn cần tự implement logic quay số dựa trên tierIndex
        var availableSoldierIds = GameController.Instance.BuildingData.soldierLayerItems
                                    .Select(s => s.id)
                                    .ToList();

        if (availableSoldierIds.Count == 0) return -1;
        return availableSoldierIds[Random.Range(0, availableSoldierIds.Count)];
    }

    /// <summary>
    /// Dọn dẹp và cho phép đóng UI
    /// </summary>
    private void OnRecruitFinished()
    {
        isRecruiting = false;
        if(closeButton != null) closeButton.interactable = true;
    }

    /// <summary>
    /// Được gọi khi bấm nút Close
    /// </summary>
    private void OnCloseButtonClicked()
    {
        if (isRecruiting) return; // Không cho đóng khi đang quay
        
        // Reset lại trạng thái của UI này trước khi đóng
        cardToShow.gameObject.SetActive(false);
        spinningImage.gameObject.SetActive(false);
        soldierCardImage.gameObject.SetActive(false);
        soldierCardImageBg.gameObject.SetActive(false);

        Close(0); // Giả sử Close(0) là hàm đóng UI của bạn
    }

    private void OnDestroy()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
        }
        // Hủy các animation DOTween
        DOTween.Kill(cardToShow.transform);
        DOTween.Kill(spinningImage.transform);
        DOTween.Kill(soldierCardImage.transform);
    }
}