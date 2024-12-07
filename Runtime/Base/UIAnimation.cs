using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIAnimation : MonoBehaviour, IUIState
{
    [System.Serializable]
    public class StatePreset
    {
        public AnimationPreset preset;
        public MOTION_TYPE AnimationType;
        public float Delay;
        public float Time;
        public LeanTweenType EaseType;
        public bool DestroyOnComplete;
        public bool PlayInDeltaTime = true;

        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public Vector3 StartScale;
        public Vector3 EndScale;

        public Vector3 StartRotation;
        public Vector3 EndRotation;

        public LeanTweenType LoopEaseType;

        [Header("Fade Settings")]
        public float StartAlpha = 0f;
        public float EndAlpha = 0f;

        [Header("Rotation Settings")]
        public AXIS RotationAxis;
        public float RotationPerSec = 1f;

        public bool Equals(StatePreset other)
        {
            if (other == null) return false;

            return this.AnimationType == other.AnimationType &&
                   this.Delay == other.Delay &&
                   this.Time == other.Time &&
                   this.EaseType == other.EaseType &&
                   this.DestroyOnComplete == other.DestroyOnComplete &&
                   this.PlayInDeltaTime == other.PlayInDeltaTime &&
                   this.StartPosition == other.StartPosition &&
                   this.EndPosition == other.EndPosition &&
                   this.StartScale == other.StartScale &&
                   this.EndScale == other.EndScale &&
                   this.StartRotation == other.StartRotation &&
                   this.EndRotation == other.EndRotation &&
                   this.LoopEaseType == other.LoopEaseType &&
                   this.StartAlpha == other.StartAlpha &&
                   this.EndAlpha == other.EndAlpha &&
                   this.RotationPerSec == other.RotationPerSec &&
                   this.RotationAxis == other.RotationAxis;
        }

        public override bool Equals(object obj)
        {
            if (obj is StatePreset other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Combine hash codes of all properties
            return AnimationType.GetHashCode() ^
                   Delay.GetHashCode() ^
                   Time.GetHashCode() ^
                   EaseType.GetHashCode() ^
                   DestroyOnComplete.GetHashCode() ^
                   PlayInDeltaTime.GetHashCode() ^
                   StartPosition.GetHashCode() ^
                   EndPosition.GetHashCode() ^
                   StartScale.GetHashCode() ^
                   EndScale.GetHashCode() ^
                   LoopEaseType.GetHashCode() ^
                   StartAlpha.GetHashCode() ^
                   EndAlpha.GetHashCode() ^
                   RotationPerSec.GetHashCode() ^
                   RotationAxis.GetHashCode();
        }
    }
    public bool playOnStart = false;
    [Header("Animation States")]
    public StatePreset onStartPreset;
    public StatePreset onUpdatePreset;
    public StatePreset onEndPreset;


    // Optional: to test on game start

    protected RectTransform rect;
    protected UIAnimationState currentState;

    public virtual void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public virtual void Start()
    {
        if (playOnStart)
        {
            //TestAnimation();
            StartAnimation();
        }
    }
    public void StartAnimation()
    {
        OnStart();
    }



    public void LoadPreset(AnimationPreset newPreset, StatePreset state)
    {
        state.preset = newPreset;
        if (newPreset != null)
        {
            state.AnimationType = newPreset.animation.motionType;
            state.Delay = newPreset.animation.delay;
            state.Time = newPreset.animation.time;
            state.EaseType = newPreset.animation.easeType;
            state.DestroyOnComplete = newPreset.animation.destroyOnComplete;
            state.PlayInDeltaTime = newPreset.animation.playInDeltaTime;

            // Update transform parameters
            state.StartPosition = newPreset.animation.startPosition;
            state.EndPosition = newPreset.animation.endPosition;
            state.StartScale = newPreset.animation.startScale;
            state.EndScale = newPreset.animation.endScale;
            state.StartRotation = newPreset.animation.startRotation;
            state.EndRotation = newPreset.animation.endRotation;
            state.StartAlpha = newPreset.animation.startAlpha;
            state.EndAlpha = newPreset.animation.endAlpha;
            state.RotationPerSec = newPreset.animation.rotationPerSec;
            state.LoopEaseType = newPreset.animation.loopEaseType;
            state.RotationAxis = newPreset.animation.rotationAxis;
        }
    }

    protected virtual void PlayAnimation(StatePreset state)
    {
        if (state?.preset == null) return;

        BeforeAnimationStart(state);
        switch (state.AnimationType)
        {
            case MOTION_TYPE.SCALE:

                rect.localScale = state.StartScale;
                // LeanTween.scale(rect, state.StartScale, state.Time).setDelay(state.Delay)
                //     .setEase(state.EaseType).setOnComplete(() =>
                // {
                LeanTween.scale(rect, state.EndScale, state.Time)
                .setDelay(state.Delay)
               .setEase(state.EaseType)
               .setLoopType(currentState == UIAnimationState.Update ? state.LoopEaseType : LeanTweenType.once)
               .setOnComplete(() => OnAnimationComplete(state))
               .setIgnoreTimeScale(!state.PlayInDeltaTime)
               .destroyOnComplete = state.DestroyOnComplete;
                // }).setIgnoreTimeScale(!state.PlayInDeltaTime)
                //     .destroyOnComplete = state.DestroyOnComplete;
                break;
            case MOTION_TYPE.MOVE:


                //rect.anchoredPosition = state.StartPosition;
                LeanTween.move(rect, state.StartPosition, state.Time).setDelay(state.Delay)
                    .setEase(state.EaseType).setOnComplete(() =>
                {
                    LeanTween.move(rect, state.EndPosition, state.Time)
                    .setDelay(state.Delay)
                    .setEase(state.EaseType)
                    .setLoopType(currentState == UIAnimationState.Update ? state.LoopEaseType : LeanTweenType.once)
                    // .setLoopClamp(state == onUpdatePreset ? -1 : 0)
                    // .setLoopPingPong(state == onUpdatePreset ? -1 : 0)
                    .setOnStart(() => { /* OnStart.Invoke(); */ })
                    .setOnUpdate((Vector3 val) => { /* OnUpdate_Vector.Invoke(val); */ })
                    .setOnComplete(() =>
                    OnAnimationComplete(state))
                    .setIgnoreTimeScale(!state.PlayInDeltaTime)
                    .destroyOnComplete = state.DestroyOnComplete;
                }).setIgnoreTimeScale(!state.PlayInDeltaTime)
                    .destroyOnComplete = state.DestroyOnComplete;
                break;

            case MOTION_TYPE.ROTATE:

                // Vector3 currentRotation = rect.rotation.eulerAngles;

                //initial rotation
                LeanTween.rotateLocal(rect.gameObject, state.StartRotation, state.Time).setDelay(state.Delay)
                    .setEase(state.EaseType).setOnComplete(() =>
                {
                    LeanTween.rotateLocal(rect.gameObject, state.EndRotation, state.Time)
                    .setDelay(state.Delay)
                    .setEase(state.EaseType)
                   .setLoopType(currentState == UIAnimationState.Update ? state.LoopEaseType : LeanTweenType.once)
                    .setOnComplete(() =>
                    OnAnimationComplete(state))
                    .setIgnoreTimeScale(!state.PlayInDeltaTime)
                    .destroyOnComplete = state.DestroyOnComplete;
                }).setIgnoreTimeScale(!state.PlayInDeltaTime)
                    .destroyOnComplete = state.DestroyOnComplete;
                break;

            case MOTION_TYPE.FADE:

                //rect.alpha = state.StartAlpha;
                LeanTween.alpha(rect, state.StartAlpha, 0f).setOnComplete(() =>
                LeanTween.alpha(rect, state.EndAlpha, state.Time).setDelay(state.Delay)
                    .setEase(state.EaseType).setLoopType(currentState == UIAnimationState.Update ? state.LoopEaseType : LeanTweenType.once).setOnComplete(() =>
                {
                    OnAnimationComplete(state);
                }).setIgnoreTimeScale(!state.PlayInDeltaTime)
                    .destroyOnComplete = state.DestroyOnComplete);
                break;

            case MOTION_TYPE.SPIN:
                Debug.Log("Spin Animation");

                switch (state.RotationAxis)
                {
                    case AXIS.Forward:
                        state.EndRotation = Vector3.forward;
                        break;
                    case AXIS.Backward:
                        state.EndRotation = Vector3.back;
                        break;
                    case AXIS.Up:
                        state.EndRotation = Vector3.up;
                        break;
                    case AXIS.Down:
                        state.EndRotation = Vector3.down;
                        break;
                    case AXIS.Right:
                        state.EndRotation = Vector3.right;
                        break;
                    case AXIS.Left:
                        state.EndRotation = Vector3.left;
                        break;
                }
                LeanTween.rotateAround(rect.gameObject, state.EndRotation, state.RotationPerSec * 60, state.Time)
                    .setDelay(state.Delay)
                    .setEase(state.EaseType)
                    .setLoopType(currentState == UIAnimationState.Update ? state.LoopEaseType : LeanTweenType.once)
                    .setOnComplete(() => OnAnimationComplete(state))
                    .setIgnoreTimeScale(!state.PlayInDeltaTime)
                    .destroyOnComplete = state.DestroyOnComplete;
                break;

            default:
                Debug.LogWarning("Unsupported Animation Type");
                break;
        }
    }

    public void OnStart()
    {
        currentState = UIAnimationState.Start;
        PlayAnimation(onStartPreset);
    }

    public void OnUpdate()
    {
        currentState = UIAnimationState.Update;
        PlayAnimation(onUpdatePreset);
    }



    public void OnEnd()
    {
        currentState = UIAnimationState.End;
        PlayAnimation(onEndPreset);
    }

    protected virtual void OnAnimationComplete(StatePreset state)
    {
        if (currentState == UIAnimationState.Start)
        {
            OnUpdate();
        }
    }

    protected virtual void BeforeAnimationStart(StatePreset state)
    {
        // Override in derived classes if needed
    }
}

public enum MOTION_TYPE { SCALE = 0, MOVE = 1, ROTATE = 2, FADE = 3, SPIN = 4, FADE_CANVAS_GROUP = 5 };
public enum UIAnimationState { Start, Update, End, ForcePlay };
public enum AXIS { Forward, Backward, Up, Down, Right, Left }