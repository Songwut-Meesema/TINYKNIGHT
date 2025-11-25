using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Data")]
    [HideInInspector] public Skill_SO skillData;
    [HideInInspector] public SkillTreeManager skillTreeManager;

    [Header("UI Elements")]
    public Image iconImage;
    public Image frameImage;
    public Button button;

    [Header("Visual States Colors")]
    public Color unlockedColor = Color.white;
    public Color unlockableFrameColor = Color.green;
    public Color lockedColor = Color.gray;

    private SkillState _currentState;
    private enum SkillState { Unlocked, Unlockable, Locked }

    public void Initialize(Skill_SO skill, SkillTreeManager manager)
    {
        skillData = skill;
        skillTreeManager = manager;

        // --- [อัปเกรด] ---
        // ตอนเริ่มต้น ให้ใช้ไอคอน "ล็อก" เป็นค่าเริ่มต้นเสมอ
        iconImage.sprite = skillData.lockedIcon;

        button.onClick.AddListener(() => skillTreeManager.UnlockSkill(skillData));
    }

    public void UpdateVisuals(List<Skill_SO> unlockedSkills, int currentSkillPoints)
    {
        bool isUnlocked = unlockedSkills.Contains(skillData);
        if (isUnlocked)
        {
            SetState(SkillState.Unlocked);
            return;
        }

        bool hasEnoughPoints = currentSkillPoints >= skillData.cost;
        bool requirementsMet = true;
        foreach (var requiredSkill in skillData.requiredSkills)
        {
            if (!unlockedSkills.Contains(requiredSkill))
            {
                requirementsMet = false;
                break;
            }
        }

        if (hasEnoughPoints && requirementsMet)
        {
            SetState(SkillState.Unlockable);
        }
        else
        {
            SetState(SkillState.Locked);
        }
    }

    private void SetState(SkillState newState)
    {
        _currentState = newState;

        // --- [อัปเกรดครั้งใหญ่!] ---
        // LEAD COMMENT: นี่คือตรรกะใหม่ที่ควบคุมทั้ง "Sprite" และ "Color"
        // ทำให้การแสดงผลของเราชัดเจนและสวยงามยิ่งขึ้น
        switch (_currentState)
        {
            case SkillState.Unlocked:
                iconImage.sprite = skillData.unlockedIcon; // ใช้ Sprite ที่ปลดล็อกแล้ว
                iconImage.color = unlockedColor;           // ตั้งค่าสีให้สว่างเต็มที่
                frameImage.color = unlockedColor;
                button.interactable = false;
                break;
            case SkillState.Unlockable:
                iconImage.sprite = skillData.lockedIcon;    // ยังคงใช้ Sprite ที่ล็อกอยู่
                iconImage.color = lockedColor;              // แต่ตั้งค่าสีพื้นฐานเป็นสีเทา
                frameImage.color = unlockableFrameColor;    // และใช้กรอบสีเขียวเพื่อดึงดูด
                button.interactable = true;
                break;
            case SkillState.Locked:
                iconImage.sprite = skillData.lockedIcon;    // ใช้ Sprite ที่ล็อกอยู่
                iconImage.color = lockedColor;              // ตั้งค่าสีเป็นสีเทา
                frameImage.color = lockedColor;
                button.interactable = false;
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_currentState == SkillState.Unlockable)
        {
            // --- [อัปเกรด] ---
            // เราจะเปลี่ยน Sprite เป็นเวอร์ชันปลดล็อกชั่วคราว เพื่อพรีวิวให้ผู้เล่นเห็น
            iconImage.sprite = skillData.unlockedIcon;
            iconImage.color = unlockedColor; // ทำให้สว่างขึ้น
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_currentState == SkillState.Unlockable)
        {
            // เปลี่ยนกลับไปเป็นสถานะดั้งเดิม
            iconImage.sprite = skillData.lockedIcon;
            iconImage.color = lockedColor;
        }
    }
}