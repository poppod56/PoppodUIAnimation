using UnityEditor;

public static class UIAnimationPresetUtility
{
    public static AnimationPreset[] GetAllPresets()
    {
        string[] guids = AssetDatabase.FindAssets("t:UIAnimationPreset");
        AnimationPreset[] presets = new AnimationPreset[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            presets[i] = AssetDatabase.LoadAssetAtPath<AnimationPreset>(path);
        }

        return presets;
    }
}