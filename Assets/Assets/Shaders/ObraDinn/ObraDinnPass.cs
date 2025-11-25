using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ObraDinnPass : ScriptableRenderPass
{
    // ตัวแปรสำหรับเก็บ Materials ที่ได้รับมา
    private Material m_DitherMat;
    private Material m_ThresholdMat;

    // ตัวแปรสำหรับเก็บ ID ของ Texture ชั่วคราว
    private int m_LargeTempID;
    private int m_MainTempID;

    private Vector3[] m_FrustumCorners = new Vector3[4];

    // Constructor: รับ Materials มาจาก Feature
    public ObraDinnPass(Material ditherMat, Material thresholdMat)
    {
        m_DitherMat = ditherMat;
        m_ThresholdMat = thresholdMat;

        // แปลงชื่อ string เป็น ID (เร็วกว่าใช้ string ตรงๆ)
        m_LargeTempID = Shader.PropertyToID("_LargeTemp");
        m_MainTempID = Shader.PropertyToID("_MainTemp");
    }

    // เมธอดหลัก: ที่นี่คือ OnRenderImage เวอร์ชั่น URP
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        // --- 0. เตรียมของ ---
        // เอา CommandBuffer (ชุดคำสั่ง) มาจาก Pool
        CommandBuffer cmd = CommandBufferPool.Get("ObraDinn Post Effect");

        // เอา Camera ที่กำลัง render อยู่
        Camera cam = renderingData.cameraData.camera;
        Transform camTransform = cam.transform;

        // เอา "Source" (ภาพที่ render มาล่าสุด)
        // ใน URP 10+ เราใช้ RTHandles, แต่สำหรับ Blit แบบนี้ใช้ .cameraColorTarget ก็ได้
        var sourceIdentifier = renderingData.cameraData.renderer.cameraColorTargetHandle;

        // เอา "Descriptor" (ข้อมูล texture) มาเพื่อสร้าง texture ใหม่
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

        // --- 1. คำนวณ Frustum Corners (เหมือนโค้ดเดิมของคุณ) ---
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, m_FrustumCorners);

        for (int i = 0; i < 4; i++)
        {
            m_FrustumCorners[i] = camTransform.TransformVector(m_FrustumCorners[i]);
            m_FrustumCorners[i].Normalize();
        }

        m_DitherMat.SetVector("_BL", m_FrustumCorners[0]);
        m_DitherMat.SetVector("_TL", m_FrustumCorners[1]);
        m_DitherMat.SetVector("_TR", m_FrustumCorners[2]);
        m_DitherMat.SetVector("_BR", m_FrustumCorners[3]);

        // --- 2. สร้าง Render Texture ชั่วคราว (เหมือนโค้ดเดิม) ---
        // RenderTexture large = RenderTexture.GetTemporary(...)
        descriptor.width = 1640;
        descriptor.height = 940;
        cmd.GetTemporaryRT(m_LargeTempID, descriptor, FilterMode.Bilinear);

        // RenderTexture main = RenderTexture.GetTemporary(...)
        descriptor.width = 820;
        descriptor.height = 470;
        cmd.GetTemporaryRT(m_MainTempID, descriptor, FilterMode.Bilinear);

        // --- 3. ทำการ Blit (เหมือนโค้ดเดิม) ---
        // Graphics.Blit(src, large, ditherMat);
        cmd.Blit(sourceIdentifier, m_LargeTempID, m_DitherMat);

        // Graphics.Blit(large, main, thresholdMat);
        cmd.Blit(m_LargeTempID, m_MainTempID, m_ThresholdMat);

        // Graphics.Blit(main, dst);
        // "dst" ในที่นี้คือ "source" (เราเขียนทับกลับไปที่เดิม)
        cmd.Blit(m_MainTempID, sourceIdentifier);

        // --- 4. คืนค่า Texture ชั่วคราว (สำคัญมาก!) ---
        cmd.ReleaseTemporaryRT(m_LargeTempID);
        cmd.ReleaseTemporaryRT(m_MainTempID);

        // --- 5. สั่งให้ CommandBuffer ทำงาน ---
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
