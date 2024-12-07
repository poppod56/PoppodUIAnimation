using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonAnimation : UIAnimation, IUIButtonState, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Interactive Button States")]
    public StatePreset onClickPreset;
    public StatePreset onHoverPreset;
    public StatePreset onUnhoverPreset;
    public StatePreset onSelectPreset;
    public StatePreset onUnselectPreset;

    [Header("Non-Interactive Button States")]
    public StatePreset onDisabledHoverPreset;
    public StatePreset onDisabledClickPreset;

    private Button button;

    public override void Awake()
    {
        base.Awake();
        button = GetComponent<Button>();
        SetupButtonListeners();
    }
    public override void Start()
    {
        base.Start();
    }

    private void SetupButtonListeners()
    {
        button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if (!button.interactable)
        {
            OnDisabledClick();
            return;
        }
        LeanTween.cancel(gameObject);
        currentState = UIAnimationState.ForcePlay; // Reset state before playing click animation
        PlayAnimation(onClickPreset);
    }

    public void OnHover()
    {
        LeanTween.cancel(gameObject);
        currentState = UIAnimationState.ForcePlay;
        PlayAnimation(onHoverPreset);
    }

    public void OnUnhover()
    {
        LeanTween.cancel(gameObject);
        currentState = UIAnimationState.ForcePlay;
        PlayAnimation(onUnhoverPreset);
    }

    public void OnSelect()
    {
        LeanTween.cancel(gameObject);
        currentState = UIAnimationState.ForcePlay;
        PlayAnimation(onSelectPreset);
    }

    public void OnUnselect()
    {
        LeanTween.cancel(gameObject);
        currentState = UIAnimationState.ForcePlay;
        PlayAnimation(onUnselectPreset);
    }

    #region Pointer Events

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable)
        {
            OnDisabledHover();
            return;
        }
        OnHover();

    }


    public void OnPointerExit(PointerEventData eventData)
    {
        OnUnhover();

    }

    #endregion

    private void OnDisabledHover()
    {
        LeanTween.cancel(gameObject);
        currentState = UIAnimationState.ForcePlay;
        PlayAnimation(onDisabledHoverPreset);
    }

    private void OnDisabledClick()
    {
        LeanTween.cancel(gameObject);
        currentState = UIAnimationState.ForcePlay;
        PlayAnimation(onDisabledClickPreset);
    }

    protected override void BeforeAnimationStart(StatePreset state)
    {
        if (state == onHoverPreset || state == onDisabledHoverPreset)
        {
            rect.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }

    protected override void OnAnimationComplete(StatePreset state)
    {
        if (state == onUnhoverPreset)
        {
            OnUpdate();
        }
        else
        {
            base.OnAnimationComplete(state);
        }
    }
}