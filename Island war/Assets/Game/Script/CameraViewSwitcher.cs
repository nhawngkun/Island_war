// CameraViewSwitcher.cs

using UnityEngine;
using UnityEngine.UI; // Cần thiết để làm việc với UI Text
using TMPro; // Cần thiết để làm việc với TextMeshPro

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
    [SerializeField] private TextMeshProUGUI     buttonText;

    private bool isAtMainView = true;

    // Hàm này sẽ được gọi bởi sự kiện OnClick của Button
    public void OnSwitchViewClicked()
    {
        isAtMainView = !isAtMainView; // Đảo ngược trạng thái

        if (isAtMainView)
        {
            // Chuyển về đảo chính và BẬT điều khiển
            mainCameraController.MoveToTarget(mainIslandView, true);
            if (buttonText != null) buttonText.text = "Xem khu vực khác";
        }
        else
        {
            // Chuyển đến view phụ và TẮT điều khiển
            mainCameraController.MoveToTarget(secondaryView, false);
            if (buttonText != null) buttonText.text = "Về đảo chính";
        }
    }
}