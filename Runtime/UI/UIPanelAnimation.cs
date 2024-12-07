using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIPanelAnimation : UIAnimation, IUIPanelState
{
    [Header("Panel Animation States")]
    public StatePreset onOpenPreset;
    public StatePreset onClosePreset;

    private CanvasGroup canvasGroup;

    public override void Awake()
    {
        base.Awake();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnClosePanel()
    {
        if (onClosePreset == null) return;
        currentState = UIAnimationState.ForcePlay;
        PlayAnimation(onClosePreset);
    }

    public void OnOpenPanel()
    {
        if (onOpenPreset == null) return;
        currentState = UIAnimationState.ForcePlay;
        PlayAnimation(onOpenPreset);
    }

    protected override void PlayAnimation(StatePreset state)
    {
        if (state?.preset == null) return;

        // Only handle FADE_CANVAS_GROUP animations
        base.PlayAnimation(state);


        switch (state.AnimationType)
        {
            case MOTION_TYPE.FADE_CANVAS_GROUP:
                canvasGroup.alpha = state.StartAlpha;
                LeanTween.alphaCanvas(canvasGroup, state.EndAlpha, state.Time)
                    .setDelay(state.Delay)
                    .setEase(state.EaseType)
                    .setLoopType(currentState == UIAnimationState.Update ? state.LoopEaseType : LeanTweenType.once)
                    .setOnComplete(() => OnAnimationComplete(state))
                    .setIgnoreTimeScale(!state.PlayInDeltaTime)
                    .destroyOnComplete = state.DestroyOnComplete;
                break;

        }
    }
}

