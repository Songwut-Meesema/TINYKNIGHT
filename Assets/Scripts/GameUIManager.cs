using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    [Header("End Screen Elements")]
    public GameObject endScreenParent;
    public TextMeshProUGUI resultText;

    [Header("Pause Screen Elements")]
    public GameObject pauseScreenParent;

    [Header("Fade Settings")]
    public float fadeDuration = 1.5f;

    private CanvasGroup _endScreenCanvasGroup;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }

        _endScreenCanvasGroup = endScreenParent.GetComponent<CanvasGroup>();
        endScreenParent.SetActive(false);
        pauseScreenParent.SetActive(false);
    }

    // --- [THE FIX!] ---
    // LEAD COMMENT: ตอนนี้ 'ShowEndScreen' ได้กลายเป็นศูนย์บัญชาการของฉากจบ
    // มันไม่เพียงแต่แสดง UI แต่ยังรับผิดชอบในการสั่ง GameManager ให้เข้าสู่ UI Mode ด้วย
    // การรวมศูนย์ตรรกะไว้ที่นี่ ทำให้การเชื่อมต่อแข็งแกร่งและทำงานได้ถูกต้องทุกครั้งที่ Restart
    public void ShowEndScreen(bool victory)
    {
        // 1. สั่ง GameManager ให้โชว์เมาส์และปลดล็อกการควบคุม
        GameManager.Instance.EnterUIMode();

        // 2. แสดง UI และเริ่ม Fade-in
        endScreenParent.SetActive(true);
        resultText.text = victory ? "VICTORY" : "GAME OVER";
        StartCoroutine(FadeInCoroutine(_endScreenCanvasGroup));
    }

    public void TogglePauseScreen()
    {
        bool isPaused = !pauseScreenParent.activeSelf;
        pauseScreenParent.SetActive(isPaused);
        GameManager.Instance.TogglePauseState(isPaused);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



    public void ExitGame()
    {
        Debug.Log("Exiting game...");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator FadeInCoroutine(CanvasGroup canvasGroup)
    {
        float elapsedTime = 0f;
        canvasGroup.alpha = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }
}