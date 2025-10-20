using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Cần cho Event Trigger
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;

public class UIRecruit : UICanvas
{
    [Header("Scroll & Snap Settings")]
    [SerializeField] private ScrollRect scrollRect; 
    [SerializeField] private RectTransform contentPanel;
    [Tooltip("Danh sách RectTransform của 5 THẺ, theo đúng thứ tự")]
    [SerializeField] private List<RectTransform> cardRects;
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.0f;
    [SerializeField] private float scaleLerpSpeed = 10f;

    [Header("Paging Logic")]
    [Tooltip("5 vị trí 'Pos X' (Left) của Content, tương ứng với 5 thẻ ở giữa")]
    [SerializeField] private List<float> cardCenterPositions = new List<float> { -740f, -480f, -320f, 20f, 260f };
    [Tooltip("Khoảng cách vuốt (pixel) tối thiểu để lật sang thẻ mới")]
    [SerializeField] private float swipeThreshold = 50f;
    [Tooltip("Thời gian (giây) để thẻ lướt sang vị trí mới")]
    [SerializeField] private float animationDuration = 0.3f;
    [Tooltip("Thẻ sẽ hiển thị lúc bắt đầu (0 = thẻ 1, 2 = thẻ 3...)")]
    [SerializeField] private int startingCardIndex = 2; 

    [Header("Dynamic UI References")]
    [SerializeField] private Button recruitButton;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI probabilityText;

    [Header("Recruit Data")]
    [SerializeField] private List<int> recruitCosts;
    [SerializeField] private List<string> recruitProbabilities;
    
    // --- BỎ HẾT BIẾN ANIMATION CŨ ---
    // [SerializeField] private Image spinningImage;
    // [SerializeField] private Image soldierCardImage;
    // [SerializeField] private Image soldierCardImageBg;

    // Biến nội bộ
    private bool isDragging = false;
    private bool isAnimating = false; // Đang lật trang
    private int currentCardIndex = 0; // Vị trí thẻ hiện tại (0-4)
    private Vector2 dragStartPosition;

    void Start()
    {
        if (recruitButton != null)
        {
            recruitButton.onClick.AddListener(OnRecruitButtonClicked);
        }

        if (scrollRect != null)
        {
            scrollRect.enabled = false;
        }

        if (cardRects.Count != 5 || cardCenterPositions.Count != 5 ||
            recruitCosts.Count != 5 || recruitProbabilities.Count != 5)
        {
            Debug.LogError("Tất cả các danh sách (Cards, Positions, Costs, Probabilities) PHẢI có đúng 5 phần tử!");
            return;
        }

        currentCardIndex = Mathf.Clamp(startingCardIndex, 0, cardCenterPositions.Count - 1);
        AnimateToCard(currentCardIndex, true);
    }
     public void Back()
    {
        UIManager.Instance.CloseUI<UIRecruit>(0f);
        UIManager.Instance.OpenUI<UIHome>();
    }


    void Update()
    {
        // Vẫn cần UpdateScaling để thẻ co dãn khi lật
        UpdateScaling();
    }
    
    /// <summary>
    /// Hàm này được gọi từ Event Trigger (PointerDown)
    /// </summary>
    public void HandlePointerDown(BaseEventData data)
    {
        if (isAnimating) return; 
        
        PointerEventData pointerData = (PointerEventData)data;
        dragStartPosition = pointerData.position;
        isDragging = true;
    }

    /// <summary>
    /// Hàm này được gọi từ Event Trigger (PointerUp)
    /// </summary>
    public void HandlePointerUp(BaseEventData data)
    {
        if (!isDragging || isAnimating) return;
        
        isDragging = false;
        PointerEventData pointerData = (PointerEventData)data;
        
        float dragDistance = pointerData.position.x - dragStartPosition.x;

        if (Mathf.Abs(dragDistance) > swipeThreshold)
        {
            if (dragDistance < 0) { currentCardIndex++; }
            else { currentCardIndex--; }
            
            currentCardIndex = Mathf.Clamp(currentCardIndex, 0, cardCenterPositions.Count - 1);
            AnimateToCard(currentCardIndex);
        }
    }

    /// <summary>
    /// Di chuyển Content Panel đến vị trí của thẻ (index) được chọn
    /// </summary>
    private void AnimateToCard(int index, bool instant = false)
    {
        if (contentPanel == null || index < 0 || index >= cardCenterPositions.Count) return;
        
        isAnimating = true; 
        float targetX = cardCenterPositions[index]; 
        
        if (instant)
        {
            contentPanel.anchoredPosition = new Vector2(targetX, contentPanel.anchoredPosition.y);
            isAnimating = false;
        }
        else
        {
            contentPanel.DOAnchorPosX(targetX, animationDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    isAnimating = false; 
                });
        }
        
        UpdateDynamicUI(index);
    }

    /// <summary>
    /// Cập nhật scale của các thẻ (chạy trong Update)
    /// </summary>
    private void UpdateScaling()
    {
        if (cardRects == null) return;
        for (int i = 0; i < cardRects.Count; i++)
        {
            if (cardRects[i] == null) continue;
            float targetScale = (i == currentCardIndex) ? maxScale : minScale;
            
            Vector3 newScale = cardRects[i].localScale;
            newScale.x = Mathf.Lerp(newScale.x, targetScale, Time.deltaTime * scaleLerpSpeed);
            newScale.y = Mathf.Lerp(newScale.y, targetScale, Time.deltaTime * scaleLerpSpeed);
            newScale.z = 1f;
            cardRects[i].localScale = newScale;
        }
    }

    /// <summary>
    /// Cập nhật Text giá tiền và xác suất
    /// </summary>
    private void UpdateDynamicUI(int centerIndex)
    {
        if (centerIndex < 0 || centerIndex >= recruitCosts.Count || centerIndex >= recruitProbabilities.Count) return;
        
        int currentCost = recruitCosts[centerIndex];
        string currentProb = recruitProbabilities[centerIndex];

        if (costText != null)
        {
            costText.text = (currentCost >= 1000) ? $"{(currentCost / 1000f):F2}k" : currentCost.ToString();
        }
        if (probabilityText != null)
        {
            probabilityText.text = currentProb;
        }
    }

    /// <summary>
    /// Hàm được gọi khi người dùng bấm nút quay tướng.
    /// (ĐÃ SỬA LẠI HOÀN TOÀN)
    /// </summary>
    private void OnRecruitButtonClicked()
    {
        if (isAnimating) return; // Không cho bấm khi đang lật thẻ

        int selectedCost = recruitCosts[currentCardIndex];
        int selectedTierIndex = currentCardIndex; // Đây là index của bậc lính (0-4)
        
        // Lấy Sprite của thẻ đang ở giữa để truyền đi
        Sprite selectedCardSprite = null;
        Image cardImage = cardRects[currentCardIndex].GetComponent<Image>();
        if(cardImage != null)
        {
            selectedCardSprite = cardImage.sprite;
        }
        else
        {
            Debug.LogError("Không tìm thấy Image component trên thẻ được chọn!");
            // Vẫn có thể tiếp tục mà không có sprite, uiCardSpin sẽ dùng sprite mặc định
        }

        // 1. Kiểm tra tiền
        if (!GameManager.Instance.CanAfford(selectedCost))
        {
            Debug.LogWarning($"Không đủ tiền! Cần {selectedCost}");
            return;
        }

        // 2. Trừ tiền
        GameManager.Instance.SpendMoney(selectedCost);

        // 3. Mở UI UICardSpin VÀ truyền dữ liệu qua
        UICardSpin spinUI = UIManager.Instance.OpenUI<UICardSpin>();

        // Gọi hàm Setup (sẽ tạo ở bước 2) để truyền dữ liệu
        spinUI.Setup(selectedTierIndex, selectedCardSprite);

        // 4. (Tùy chọn) Đóng UI Recruit lại nếu muốn
        // Close(0);
    }
    

    // --- XÓA HẾT CÁC HÀM LIÊN QUAN ĐẾN ANIMATION ---
    // IEnumerator RecruitSequence()
    // int GetRandomSoldierByTier(int tierIndex)
    // OnDestroy() (chỉ giữ lại phần kill contentPanel)

    private void OnDestroy()
    {
        if (recruitButton != null)
        {
            recruitButton.onClick.RemoveAllListeners();
        }
        if(contentPanel != null) DOTween.Kill(contentPanel);
    }
}