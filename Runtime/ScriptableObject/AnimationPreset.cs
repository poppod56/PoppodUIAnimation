using UnityEngine;


[CreateAssetMenu(fileName = "AnimationPreset", menuName = "Animation/Preset")]
public class AnimationPreset : ScriptableObject
{
    public string presetName;
    public AnimationState animation;

}
[System.Serializable]
public class AnimationState
{
    public MOTION_TYPE motionType;
    public float delay;
    public float time;
    public LeanTweenType easeType;
    public bool destroyOnComplete;
    public bool playInDeltaTime = true;

    public Vector3 startPosition;
    public Vector3 endPosition;
    public Vector3 startScale;
    public Vector3 endScale;
    public Vector3 startRotation;
    public Vector3 endRotation;
    public float rotationPerSec;
    public AXIS rotationAxis;
    public float startAlpha;
    public float endAlpha;
    public LeanTweenType loopEaseType = LeanTweenType.pingPong;
}
