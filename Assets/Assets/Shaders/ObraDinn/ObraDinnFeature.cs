using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ObraDinnFeature : ScriptableRendererFeature
{
    // สร้าง class ย่อยเพื่อจัดกลุ่ม setting ใน Inspector
    [System.Serializable]
    public class ObraDinnSettings
    {
        public Material ditherMat;
        public Material thresholdMat;
    }

    // เอา settings ที่เราสร้างมาใช้งาน
    public ObraDinnSettings settings = new ObraDinnSettings();

    // ตัวแปรเก็บ Pass ที่จะทำงานจริง
    private ObraDinnPass m_ObraDinnPass;

    // 1. นี่คือเมธอดแรกที่ URP จะเรียก
    // เราจะสร้าง Pass ของเราขึ้นมาที่นี่
    public override void Create()
    {
        m_ObraDinnPass = new ObraDinnPass(settings.ditherMat, settings.thresholdMat);

        // ตั้งค่าว่า Pass นี้จะรันเมื่อไหร่
        // .BeforeRenderingPostProcessing คือรันก่อน Post-processing ตัวอื่น
        m_ObraDinnPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    // 2. นี่คือเมธอดที่ URP จะเรียกทุกเฟรม
    // เราแค่สั่งให้มัน "เพิ่ม Pass นี้เข้าไปในคิวงาน"
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // ตรวจสอบก่อนว่า materials ไม่ว่าง
        if (settings.ditherMat == null || settings.thresholdMat == null)
        {
            Debug.LogWarning("Obra Dinn materials ไม่ได้ตั้งค่าใน Feature");
            return;
        }

        // ส่ง Pass ของเราไปให้ Renderer
        renderer.EnqueuePass(m_ObraDinnPass);
    }
}
