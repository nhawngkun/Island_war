// CameraViewSwitcher.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraViewSwitcher : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Kéo đối tượng Main Camera vào đây")]
    [SerializeField] private CameraController mainCameraController;
    
    [Tooltip("Kéo Transform của điểm nhìn đảo chính vào đây")]
    [SerializeField] private Transform mainIslandView;
    
    [Tooltip("Kéo Transform của điểm nhìn phụ vào đây")]
    [SerializeField] private Transform secondaryView;

    [Header("UI (Optional)")]
    [Tooltip("(Tùy chọn) Kéo Text của button vào đây để đổi chữ")]
    [SerializeField] private TextMeshProUGUI buttonText;

    private bool isAtMainView = true;

    // Hàm này sẽ được gọi bởi sự kiện OnClick của Button (nếu bạn vẫn dùng)
    public void OnSwitchViewClicked()
    {
        if (isAtMainView)
        {
            MoveToSecondaryView();
        }
        else
        {
            MoveToMainView();
        }
    }

    /// <summary>
    /// HÀM MỚI: Bắt buộc camera di chuyển về đảo chính
    /// </summary>
    public void MoveToMainView()
    {
        isAtMainView = true;
        mainCameraController.MoveToTarget(mainIslandView, true);
        if (buttonText != null) buttonText.text = "Xem khu vực khác";
    }

    /// <summary>
    /// HÀM MỚI: Bắt buộc camera di chuyển đến khu vực lính (phụ)
    /// </summary>
    public void MoveToSecondaryView()
    {
        isAtMainView = false;
        mainCameraController.MoveToTarget(secondaryView, false); // Tắt điều khiển camera
        if (buttonText != null) buttonText.text = "Về đảo chính";
    }
}