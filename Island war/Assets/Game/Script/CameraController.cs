
using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

public class CameraController : MonoBehaviour
{
    [Title("Control State")]
    [ShowInInspector, ReadOnly]
    public bool controlsEnabled = true;

    [Title("Target & Pivot")]
    [Tooltip("Điểm trung tâm mà camera sẽ xoay quanh.")]
    [SerializeField] private Vector3 pivotPoint = Vector3.zero;

    [Title("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 150f;
    [SerializeField, Range(20f, 85f)] private float fixedPitchAngle = 45f;

    [Title("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 25f; // Giờ có thể không cần dùng đến, nhưng giữ lại không ảnh hưởng
    [Tooltip("Khoảng cách cho mỗi lần cuộn chuột.")]
    [SerializeField] private float zoomStep = 10f; // << BIẾN MỚI CHO ZOOM TỪNG NẤC
    [SerializeField] private Vector2 distanceLimit = new Vector2(20f, 70f);
    [SerializeField] private float zoomDampening = 10f;
    
    [Title("View Transition")]
    [SerializeField] private float transitionDuration = 1.5f;

    private float targetDistance;
    private float smoothedDistance;
    private float currentYaw = 0f;

    void Start()
    {
        float initialDistance = Vector3.Distance(transform.position, pivotPoint);
        targetDistance = Mathf.Clamp(initialDistance, distanceLimit.x, distanceLimit.y);
        smoothedDistance = targetDistance; 
        currentYaw = transform.eulerAngles.y;
        UpdateCameraTransform();
    }

    void LateUpdate()
    {
        if (!controlsEnabled)
        {
            return;
        }

        if (Input.GetMouseButton(1))
        {
            currentYaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        }

        // --- LOGIC ZOOM THEO TỪNG NẤC ---
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        // Nếu cuộn chuột lên (zoom vào)
        if (scroll > 0f) 
        {
            targetDistance -= zoomStep;
        }
        // Nếu cuộn chuột xuống (zoom ra)
        else if (scroll < 0f) 
        {
            targetDistance += zoomStep;
        }

        // Đảm bảo targetDistance luôn nằm trong giới hạn sau khi thay đổi
        targetDistance = Mathf.Clamp(targetDistance, distanceLimit.x, distanceLimit.y);
        
        // Vẫn dùng Lerp để di chuyển mượt mà đến vị trí zoom tiếp theo
        smoothedDistance = Mathf.Lerp(smoothedDistance, targetDistance, Time.deltaTime * zoomDampening);
        UpdateCameraTransform();
    }
    
    private void UpdateCameraTransform()
    {
        Quaternion rotation = Quaternion.Euler(fixedPitchAngle, currentYaw, 0);
        Vector3 position = pivotPoint - (rotation * Vector3.forward * smoothedDistance); 
        transform.SetPositionAndRotation(position, rotation);
    }
    
    public void MoveToTarget(Transform target, bool enableControlsAfterMove)
    {
        StopAllCoroutines();
        StartCoroutine(MoveToPositionCoroutine(target, enableControlsAfterMove));
    }

    private IEnumerator MoveToPositionCoroutine(Transform target, bool enableControlsAfterMove)
    {
        controlsEnabled = false;

        float elapsedTime = 0f;
        Vector3 startingPos = transform.position;
        Quaternion startingRot = transform.rotation;

        while (elapsedTime < transitionDuration)
        {
            float t = Mathf.SmoothStep(0.0f, 1.0f, elapsedTime / transitionDuration);
            transform.position = Vector3.Lerp(startingPos, target.position, t);
            transform.rotation = Quaternion.Slerp(startingRot, target.rotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = target.position;
        transform.rotation = target.rotation;
        
        controlsEnabled = enableControlsAfterMove;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pivotPoint, 0.5f);
        Gizmos.color = Color.cyan;
        DrawOrbitCircle(distanceLimit.x);
        Gizmos.color = Color.yellow;
        DrawOrbitCircle(distanceLimit.y);
    }
    private void DrawOrbitCircle(float distance)
    {
        if (distance <= 0) return;
        float horizontalRadius = distance * Mathf.Cos(fixedPitchAngle * Mathf.Deg2Rad);
        Vector3 lastPoint = pivotPoint + new Vector3(horizontalRadius, 0, 0);
        int segments = 40;
        for (int i = 1; i <= segments; i++)
        {
            float angle = i / (float)segments * 360f;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 nextPoint = pivotPoint + rotation * new Vector3(horizontalRadius, 0, 0);
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
    }
    #endif
}