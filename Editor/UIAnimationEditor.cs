using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(UIAnimation), true)]
public class UIAnimationEditor : Editor
{
    private static GUIContent logoContent;
    private static GUIStyle headerStyle;

    private bool showStartSettings = false;
    private bool showUpdateSettings = false;
    private bool showEndSettings = false;
    private bool showClickSettings = false;
    private bool showHoverSettings = false;
    private bool showUnhoverSettings = false;
    private bool showSelectSettings = false;
    private bool showUnselectSettings = false;
    private bool showDisabledHoverSettings = false;
    private bool showDisabledClickSettings = false;
    private bool showOpenPanelSettings = false;
    private bool showClosePanelSettings = false;

    private void OnEnable()
    {
        // Load logo from package path instead of Assets path
        LoadPackageResources();
    }

    private GUIStyle GetHeaderStyle()
    {
        if (headerStyle == null)
        {
            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleLeft,
                fixedHeight = 40
            };
        }
        return headerStyle;
    }

    private void LoadPackageResources()
    {
        if (logoContent == null)
        {
            // Look for the logo in the package directory
            string[] guids = AssetDatabase.FindAssets("PoppodLogo t:texture2d", new[] { "Packages/com.poppod.uianimation" });
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                logoContent = new GUIContent(icon);
            }
            else
            {
                // Fallback to default Unity icon if package icon not found
                logoContent = EditorGUIUtility.IconContent("Animation.Record");
            }
        }
    }

    public override void OnInspectorGUI()
    {
        UIAnimation animation = (UIAnimation)target;

        // Draw stylized header with lazy-loaded style
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label(logoContent, GUILayout.Width(40), GUILayout.Height(40));
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Poppod UI Animation", GetHeaderStyle());
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Version 0.1.2", EditorStyles.miniLabel);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(10);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        // Play On Start Section
        EditorGUILayout.LabelField("Animation Settings", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        animation.playOnStart = EditorGUILayout.Toggle("Play On Start", animation.playOnStart);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);


        EditorGUILayout.LabelField("Animation States", EditorStyles.boldLabel);
        // Original State Sections
        DrawStateSection("On Start ", ref showStartSettings, animation.onStartPreset, animation);
        DrawStateSection("On Update ", ref showUpdateSettings, animation.onUpdatePreset, animation);
        DrawStateSection("On End ", ref showEndSettings, animation.onEndPreset, animation);

        // Button States
        if (animation is UIButtonAnimation buttonAnimation)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Interactive Button States", EditorStyles.boldLabel);

            DrawStateSection("On Click ", ref showClickSettings, buttonAnimation.onClickPreset, animation);
            DrawStateSection("On Hover ", ref showHoverSettings, buttonAnimation.onHoverPreset, animation);
            DrawStateSection("On Unhover ", ref showUnhoverSettings, buttonAnimation.onUnhoverPreset, animation);
            DrawStateSection("On Select ", ref showSelectSettings, buttonAnimation.onSelectPreset, animation);
            DrawStateSection("On Unselect ", ref showUnselectSettings, buttonAnimation.onUnselectPreset, animation);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Non-Interactive Button States", EditorStyles.boldLabel);

            DrawStateSection("On Disabled Hover", ref showDisabledHoverSettings, buttonAnimation.onDisabledHoverPreset, animation);
            DrawStateSection("On Disabled Click", ref showDisabledClickSettings, buttonAnimation.onDisabledClickPreset, animation);
        }

        if (animation is UIPanelAnimation panelAnimation)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Panel States", EditorStyles.boldLabel);

            DrawStateSection("On Open Panel ", ref showOpenPanelSettings, panelAnimation.onOpenPreset, animation);
            DrawStateSection("On Close Panel ", ref showClosePanelSettings, panelAnimation.onClosePreset, animation);
        }
    }

    private void DrawStateSection(string title, ref bool foldout, UIAnimation.StatePreset statePreset, UIAnimation animation)
    {
        EditorGUILayout.Space(5);
        foldout = EditorGUILayout.Foldout(foldout, title, true);
        if (foldout)
        {
            EditorGUI.indentLevel++;

            // Preset field
            EditorGUI.BeginChangeCheck();
            statePreset.preset = (AnimationPreset)EditorGUILayout.ObjectField(
                "Preset", statePreset.preset, typeof(AnimationPreset), false);

            if (EditorGUI.EndChangeCheck() && statePreset.preset != null)
            {
                animation.LoadPreset(statePreset.preset, statePreset);
            }

            // Always show editable fields
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Animation Settings", EditorStyles.boldLabel);

            // Global settings
            EditorGUI.BeginChangeCheck();
            statePreset.AnimationType = (MOTION_TYPE)EditorGUILayout.EnumPopup("Animation Type", statePreset.AnimationType);
            statePreset.Delay = EditorGUILayout.FloatField("Delay", statePreset.Delay);
            statePreset.Time = EditorGUILayout.FloatField("Time", statePreset.Time);
            statePreset.EaseType = (LeanTweenType)EditorGUILayout.EnumPopup("Ease Type", statePreset.EaseType);
            statePreset.DestroyOnComplete = EditorGUILayout.Toggle("Destroy On Complete", statePreset.DestroyOnComplete);
            statePreset.PlayInDeltaTime = EditorGUILayout.Toggle("Play In Delta Time", statePreset.PlayInDeltaTime);
            statePreset.LoopEaseType = (LeanTweenType)EditorGUILayout.EnumPopup("Loop Ease Type", statePreset.LoopEaseType);
            // Scale-specific settings
            if (statePreset.AnimationType == MOTION_TYPE.SCALE)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Scale Settings", EditorStyles.boldLabel);
                statePreset.StartScale = EditorGUILayout.Vector3Field("Start Scale", statePreset.StartScale);
                statePreset.EndScale = EditorGUILayout.Vector3Field("End Scale", statePreset.EndScale);
            }

            // Move-specific settings
            if (statePreset.AnimationType == MOTION_TYPE.MOVE)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Move Settings", EditorStyles.boldLabel);
                statePreset.StartPosition = EditorGUILayout.Vector3Field("Start Position", statePreset.StartPosition);
                statePreset.EndPosition = EditorGUILayout.Vector3Field("End Position", statePreset.EndPosition);
            }

            // Rotate-specific settings
            if (statePreset.AnimationType == MOTION_TYPE.ROTATE)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Rotate Settings", EditorStyles.boldLabel);
                statePreset.StartRotation = EditorGUILayout.Vector3Field("Start Rotation", statePreset.StartRotation);
                statePreset.EndRotation = EditorGUILayout.Vector3Field("End Rotation", statePreset.EndRotation);
            }

            if (statePreset.AnimationType == MOTION_TYPE.FADE || statePreset.AnimationType == MOTION_TYPE.FADE_CANVAS_GROUP)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Fade Settings", EditorStyles.boldLabel);
                statePreset.StartAlpha = EditorGUILayout.FloatField("Start Alpha", statePreset.StartAlpha);
                statePreset.EndAlpha = EditorGUILayout.FloatField("End Alpha", statePreset.EndAlpha);
            }

            if (statePreset.AnimationType == MOTION_TYPE.SPIN)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Spin Settings", EditorStyles.boldLabel);
                statePreset.RotationPerSec = EditorGUILayout.FloatField("Rotation Per Second", statePreset.RotationPerSec);
                statePreset.RotationAxis = (AXIS)EditorGUILayout.EnumPopup("Rotation Axis", statePreset.RotationAxis);
            }

            bool hasChanges = EditorGUI.EndChangeCheck();

            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();

            // If we have a preset, show update options
            if (statePreset.preset != null)
            {
                if (hasChanges)
                {
                    EditorGUILayout.HelpBox("Unsaved changes in preset", MessageType.Info);
                }

                if (GUILayout.Button("Update Preset"))
                {
                    UpdateCurrentPreset(statePreset);
                }

                if (GUILayout.Button("Save As New"))
                {
                    CreateNewPreset(statePreset);
                }
            }
            // If no preset, show create new option
            else
            {
                if (GUILayout.Button("Create New Preset"))
                {
                    CreateNewPreset(statePreset);
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }
    }

    private void CreateNewPreset(UIAnimation.StatePreset statePreset)
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Animation Preset",
            "NewAnimationPreset",
            "asset",
            "Save animation preset as"
        );

        if (!string.IsNullOrEmpty(path))
        {
            AnimationPreset newPreset = CreateInstance<AnimationPreset>();
            UpdatePresetValues(newPreset, statePreset);
            AssetDatabase.CreateAsset(newPreset, path);
            AssetDatabase.SaveAssets();
            statePreset.preset = newPreset;
        }
    }

    private void UpdateCurrentPreset(UIAnimation.StatePreset statePreset)
    {
        if (statePreset.preset != null)
        {
            Undo.RecordObject(statePreset.preset, "Update Animation Preset");
            UpdatePresetValues(statePreset.preset, statePreset);
            EditorUtility.SetDirty(statePreset.preset);
            AssetDatabase.SaveAssets();
        }
    }

    private void UpdatePresetValues(AnimationPreset preset, UIAnimation.StatePreset statePreset)
    {
        preset.animation = new AnimationState
        {
            motionType = statePreset.AnimationType,
            delay = statePreset.Delay,
            time = statePreset.Time,
            easeType = statePreset.EaseType,
            destroyOnComplete = statePreset.DestroyOnComplete,
            startPosition = statePreset.StartPosition,
            endPosition = statePreset.EndPosition,
            startScale = statePreset.StartScale,
            endScale = statePreset.EndScale,
            startRotation = statePreset.StartRotation,
            endRotation = statePreset.EndRotation,
            loopEaseType = statePreset.LoopEaseType,
            rotationPerSec = statePreset.RotationPerSec,
            startAlpha = statePreset.StartAlpha,
            endAlpha = statePreset.EndAlpha,
            rotationAxis = statePreset.RotationAxis
        };
    }
}