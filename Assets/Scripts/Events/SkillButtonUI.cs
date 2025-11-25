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
        
        switch (_currentState)
        {
            case SkillState.Unlocked:
                iconImage.sprite = skillData.unlockedIcon; 
                iconImage.color = unlockedColor;          
                frameImage.color = unlockedColor;
                button.interactable = false;
                break;

            case SkillState.Unlockable:
                iconImage.sprite = skillData.lockedIcon;    
                iconImage.color = lockedColor;             
                frameImage.color = unlockableFrameColor;
                button.interactable = true;
                break;

            case SkillState.Locked:     
                iconImage.sprite = skillData.lockedIcon;    
                iconImage.color = lockedColor;              
                frameImage.color = lockedColor;
                button.interactable = false;
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_currentState == SkillState.Unlockable)
        {
            iconImage.sprite = skillData.unlockedIcon;
            iconImage.color = unlockedColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_currentState == SkillState.Unlockable)
        {
            iconImage.sprite = skillData.lockedIcon;
            iconImage.color = lockedColor;
        }
    }
}