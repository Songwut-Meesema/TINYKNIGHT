using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// LEAD COMMENT: นี่คือสคริปต์ที่จัดการ "Vignette ปลอม" ผ่านระบบ UI
// วิธีนี้เป็นการแก้ปัญหาที่แข็งแกร่งมากเมื่อต้องทำงานร่วมกับ Custom Render Pipeline หรือ Full-screen Shader
// เพราะ UI Canvas จะถูกวาด "ทับ" ทุกอย่างเป็นลำดับสุดท้ายเสมอ ทำให้การันตีว่าจะมองเห็นได้
[RequireComponent(typeof(CanvasGroup))]
public class DamageVignetteUI : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("ความเข้มสูงสุดของ Vignette (ความทึบ)")]
    [Range(0f, 1f)]
    public float maxAlpha = 0.8f;
    [Tooltip("ความเร็วที่ Vignette จะปรากฏและหายไป")]
    public float fadeSpeed = 4f;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0; // เริ่มจากโปร่งใส
    }

    // ฟังก์ชันนี้จะถูกเรียกโดย GameEventListener
    public void TriggerEffect()
    {
        // หยุด Coroutine เก่า (ถ้ามี) ก่อนเริ่มอันใหม่
        StopAllCoroutines();
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        // --- Fade In ---
        while (_canvasGroup.alpha < maxAlpha)
        {
            _canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
        _canvasGroup.alpha = maxAlpha;

        // --- Fade Out ---
        while (_canvasGroup.alpha > 0f)
        {
            _canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        _canvasGroup.alpha = 0f;
    }
}