using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // --- [สำคัญ!] --- ต้องมีเพื่อจัดการกับการโหลดฉาก

// LEAD COMMENT: นี่คือสคริปต์ที่เรียบง่ายและมีเป้าหมายเดียว คือจัดการ Main Menu
// มันไม่มีความซับซ้อน, ไม่ใช่ Singleton, และไม่ยุ่งเกี่ยวกับระบบเกมอื่นเลย
// นี่คือตัวอย่างที่ดีของการแยกความรับผิดชอบ (Separation of Concerns)
public class MainMenuManager : MonoBehaviour
{
    // LEAD COMMENT: เราสร้างตัวแปร string สำหรับชื่อฉาก
    // เพื่อให้ง่ายต่อการแก้ไขใน Inspector ในอนาคต และลดโอกาสการพิมพ์ผิดในโค้ด
    [Header("Scene To Load")]
    [Tooltip("ใส่ชื่อของฉากเกมหลักที่นี่ (ต้องตรงกับชื่อไฟล์ Scene)")]
    public string mainGameSceneName = "MainScene";

    // ฟังก์ชันนี้จะถูกเรียกเป็นอันดับแรกๆ
    private void Start()
    {
        // --- [สำคัญมาก!] ---
        // LEAD COMMENT: เราต้อง "บังคับ" ให้เคอร์เซอร์เมาส์ปรากฏและปลดล็อกเสมอ
        // เมื่อเข้ามาที่ Main Menu ไม่ว่าผู้เล่นจะมาจากไหน (เช่น ออกจากเกมมาที่เมนู)
        // เพื่อให้แน่ใจว่า UI สามารถใช้งานได้ 100%
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // --- Button Functions ---
    // LEAD COMMENT: ฟังก์ชันเหล่านี้เป็น Public เพื่อให้เราสามารถลากไปวาง
    // ใน Event 'OnClick()' ของปุ่มใน Inspector ได้

    public void StartGame()
    {
        Debug.Log("Starting game... Loading scene: " + mainGameSceneName);
        SceneManager.LoadScene(mainGameSceneName);
    }

    public void ExitGame()
    {
        Debug.Log("Exiting game...");

        // LEAD COMMENT: Application.Quit() จะทำงานเฉพาะในเกมที่ Build แล้วเท่านั้น
        // เราจึงเพิ่มเงื่อนไขพิเศษสำหรับ Unity Editor เพื่อให้เห็นว่าปุ่มทำงาน
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}