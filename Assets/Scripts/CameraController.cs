using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target To Follow")]
    public Transform playerTarget;

    [Header("Camera Settings")]
    public float mouseSensitivity = 100f;
    public float distanceFromTarget = 5f;
    public float heightOffset = 2f;
    public Vector2 pitchMinMax = new Vector2(-40, 85);

    [Header("Camera Shake Settings")]
    public float shakeDuration = 0.25f;
    public float shakeMagnitude = 0.5f;

    private float _yaw;
    private float _pitch;

    private Vector3 _currentShakeOffset = Vector3.zero;

    // --- [แก้ไข] ---
    // LEAD COMMENT: เราได้ลบโค้ด Cursor.lockState และ Cursor.visible ออกจากฟังก์ชัน Start() แล้ว
    // เพราะตอนนี้ GameManager ได้กลายเป็น "ผู้มีอำนาจเพียงผู้เดียว" (Single Source of Truth)
    // ในการจัดการสถานะของเคอร์เซอร์ ซึ่งเป็นสถาปัตยกรรมที่ดีกว่า
    void Start()
    {
        // ที่นี่เคยมีโค้ดล็อกเมาส์ แต่ตอนนี้มันว่างเปล่า ซึ่งถูกต้องแล้ว!
    }

    void LateUpdate()
    {
        // --- [เพิ่มใหม่] ---
        // LEAD COMMENT: เพิ่มการตรวจสอบเพื่อให้แน่ใจว่ากล้องจะหมุนก็ต่อเมื่อ
        // อยู่ในโหมด Gameplay (เคอร์เซอร์ถูกล็อก) เท่านั้น
        // นี่เป็นการป้องกันไม่ให้มุมกล้องเหวี่ยงไปมาตอนที่ผู้เล่นกำลังคลิก UI
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        if (playerTarget == null) return;

        _yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        _pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, pitchMinMax.x, pitchMinMax.y);

        Vector3 currentRotation = new Vector3(_pitch, _yaw);
        transform.eulerAngles = currentRotation;

        Vector3 basePosition = playerTarget.position + Vector3.up * heightOffset - transform.forward * distanceFromTarget;
        transform.position = basePosition + _currentShakeOffset;
    }

    public void TriggerShake()
    {
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            _currentShakeOffset = new Vector3(x, y, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _currentShakeOffset = Vector3.zero;
    }
}