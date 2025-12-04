using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 수중 Post-Processing Volume Profile을 자동 생성하는 에디터 유틸리티
/// </summary>
public class CreateUnderwaterVolumeProfiles : EditorWindow
{
    [MenuItem("Tools/Create Underwater Volume Profiles")]
    public static void CreateProfiles()
    {
        CreateSurfaceWaterProfile();
        CreateShallowWaterProfile();
        CreateMediumWaterProfile();
        CreateDeepWaterProfile();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[CreateUnderwaterVolumeProfiles] 4개의 Volume Profile이 생성되었습니다.");
        EditorUtility.DisplayDialog("완료", "수중 Volume Profile 4개가 Assets/Settings/ 폴더에 생성되었습니다!", "확인");
    }

    /// <summary>
    /// 수면 근처 Volume Profile 생성 (Y > -10)
    /// </summary>
    private static void CreateSurfaceWaterProfile()
    {
        string path = "Assets/Settings/SurfaceWater_VolumeProfile.asset";

        if (AssetDatabase.LoadAssetAtPath<VolumeProfile>(path) != null)
        {
            AssetDatabase.DeleteAsset(path);
        }

        VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
        profile.name = "SurfaceWater_VolumeProfile";
        AssetDatabase.CreateAsset(profile, path);

        // Color Adjustments - 매우 밝고 맑음
        ColorAdjustments colorAdjustments = ScriptableObject.CreateInstance<ColorAdjustments>();
        colorAdjustments.active = true;
        colorAdjustments.colorFilter.overrideState = true;
        colorAdjustments.colorFilter.value = new Color(0.9f, 1.0f, 1.0f, 1f); // 거의 흰색에 가까운 밝은 청색
        colorAdjustments.saturation.overrideState = true;
        colorAdjustments.saturation.value = 0f; // 원래 색상 유지
        colorAdjustments.postExposure.overrideState = true;
        colorAdjustments.postExposure.value = 0.1f; // 약간 밝게
        colorAdjustments.name = "ColorAdjustments";
        AssetDatabase.AddObjectToAsset(colorAdjustments, profile);
        profile.components.Add(colorAdjustments);

        // Bloom - 강한 햇빛 효과
        Bloom bloom = ScriptableObject.CreateInstance<Bloom>();
        bloom.active = true;
        bloom.intensity.overrideState = true;
        bloom.intensity.value = 1.0f;
        bloom.threshold.overrideState = true;
        bloom.threshold.value = 0.8f;
        bloom.scatter.overrideState = true;
        bloom.scatter.value = 0.6f;
        bloom.name = "Bloom";
        AssetDatabase.AddObjectToAsset(bloom, profile);
        profile.components.Add(bloom);

        // Vignette - 거의 없음
        Vignette vignette = ScriptableObject.CreateInstance<Vignette>();
        vignette.active = true;
        vignette.intensity.overrideState = true;
        vignette.intensity.value = 0.02f;
        vignette.smoothness.overrideState = true;
        vignette.smoothness.value = 1.0f;
        vignette.color.overrideState = true;
        vignette.color.value = new Color(0.2f, 0.3f, 0.4f, 1f);
        vignette.name = "Vignette";
        AssetDatabase.AddObjectToAsset(vignette, profile);
        profile.components.Add(vignette);

        // Chromatic Aberration - 거의 없음
        ChromaticAberration chromatic = ScriptableObject.CreateInstance<ChromaticAberration>();
        chromatic.active = true;
        chromatic.intensity.overrideState = true;
        chromatic.intensity.value = 0.01f;
        chromatic.name = "ChromaticAberration";
        AssetDatabase.AddObjectToAsset(chromatic, profile);
        profile.components.Add(chromatic);

        EditorUtility.SetDirty(profile);
    }

    /// <summary>
    /// 얕은 수심 Volume Profile 생성 (-10 ~ -40)
    /// </summary>
    private static void CreateShallowWaterProfile()
    {
        string path = "Assets/Settings/ShallowWater_VolumeProfile.asset";

        // 기존 파일 삭제
        if (AssetDatabase.LoadAssetAtPath<VolumeProfile>(path) != null)
        {
            AssetDatabase.DeleteAsset(path);
        }

        VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
        profile.name = "ShallowWater_VolumeProfile";
        AssetDatabase.CreateAsset(profile, path);

        // Color Adjustments - 밝은 청록색
        ColorAdjustments colorAdjustments = ScriptableObject.CreateInstance<ColorAdjustments>();
        colorAdjustments.active = true;
        colorAdjustments.colorFilter.overrideState = true;
        colorAdjustments.colorFilter.value = new Color(0.7f, 0.9f, 1.0f, 1f);
        colorAdjustments.saturation.overrideState = true;
        colorAdjustments.saturation.value = -10f;
        colorAdjustments.name = "ColorAdjustments";
        AssetDatabase.AddObjectToAsset(colorAdjustments, profile);
        profile.components.Add(colorAdjustments);

        // Bloom - 중간 강도
        Bloom bloom = ScriptableObject.CreateInstance<Bloom>();
        bloom.active = true;
        bloom.intensity.overrideState = true;
        bloom.intensity.value = 0.8f;
        bloom.threshold.overrideState = true;
        bloom.threshold.value = 0.9f;
        bloom.scatter.overrideState = true;
        bloom.scatter.value = 0.5f;
        bloom.name = "Bloom";
        AssetDatabase.AddObjectToAsset(bloom, profile);
        profile.components.Add(bloom);

        // Vignette - 약한 어두움
        Vignette vignette = ScriptableObject.CreateInstance<Vignette>();
        vignette.active = true;
        vignette.intensity.overrideState = true;
        vignette.intensity.value = 0.05f;
        vignette.smoothness.overrideState = true;
        vignette.smoothness.value = 0.8f;
        vignette.color.overrideState = true;
        vignette.color.value = new Color(0.1f, 0.2f, 0.3f, 1f);
        vignette.name = "Vignette";
        AssetDatabase.AddObjectToAsset(vignette, profile);
        profile.components.Add(vignette);

        // Chromatic Aberration - 미세한 왜곡
        ChromaticAberration chromatic = ScriptableObject.CreateInstance<ChromaticAberration>();
        chromatic.active = true;
        chromatic.intensity.overrideState = true;
        chromatic.intensity.value = 0.02f;
        chromatic.name = "ChromaticAberration";
        AssetDatabase.AddObjectToAsset(chromatic, profile);
        profile.components.Add(chromatic);

        EditorUtility.SetDirty(profile);
    }

    /// <summary>
    /// 중간 수심 Volume Profile 생성 (-40 ~ -100)
    /// </summary>
    private static void CreateMediumWaterProfile()
    {
        string path = "Assets/Settings/MediumWater_VolumeProfile.asset";

        if (AssetDatabase.LoadAssetAtPath<VolumeProfile>(path) != null)
        {
            AssetDatabase.DeleteAsset(path);
        }

        VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
        profile.name = "MediumWater_VolumeProfile";
        AssetDatabase.CreateAsset(profile, path);

        // Color Adjustments - 진한 파란색
        ColorAdjustments colorAdjustments = ScriptableObject.CreateInstance<ColorAdjustments>();
        colorAdjustments.active = true;
        colorAdjustments.colorFilter.overrideState = true;
        colorAdjustments.colorFilter.value = new Color(0.5f, 0.7f, 1.0f, 1f);
        colorAdjustments.saturation.overrideState = true;
        colorAdjustments.saturation.value = -20f;
        colorAdjustments.postExposure.overrideState = true;
        colorAdjustments.postExposure.value = -0.2f;
        colorAdjustments.name = "ColorAdjustments";
        AssetDatabase.AddObjectToAsset(colorAdjustments, profile);
        profile.components.Add(colorAdjustments);

        // Bloom - 낮은 강도
        Bloom bloom = ScriptableObject.CreateInstance<Bloom>();
        bloom.active = true;
        bloom.intensity.overrideState = true;
        bloom.intensity.value = 0.6f;
        bloom.threshold.overrideState = true;
        bloom.threshold.value = 1.0f;
        bloom.scatter.overrideState = true;
        bloom.scatter.value = 0.4f;
        bloom.name = "Bloom";
        AssetDatabase.AddObjectToAsset(bloom, profile);
        profile.components.Add(bloom);

        // Vignette - 중간 압박감
        Vignette vignette = ScriptableObject.CreateInstance<Vignette>();
        vignette.active = true;
        vignette.intensity.overrideState = true;
        vignette.intensity.value = 0.12f;
        vignette.smoothness.overrideState = true;
        vignette.smoothness.value = 0.7f;
        vignette.color.overrideState = true;
        vignette.color.value = new Color(0.05f, 0.1f, 0.2f, 1f);
        vignette.name = "Vignette";
        AssetDatabase.AddObjectToAsset(vignette, profile);
        profile.components.Add(vignette);

        // Chromatic Aberration - 중간 왜곡
        ChromaticAberration chromatic = ScriptableObject.CreateInstance<ChromaticAberration>();
        chromatic.active = true;
        chromatic.intensity.overrideState = true;
        chromatic.intensity.value = 0.04f;
        chromatic.name = "ChromaticAberration";
        AssetDatabase.AddObjectToAsset(chromatic, profile);
        profile.components.Add(chromatic);

        EditorUtility.SetDirty(profile);
    }

    /// <summary>
    /// 깊은 수심 Volume Profile 생성 (Y < -100)
    /// </summary>
    private static void CreateDeepWaterProfile()
    {
        string path = "Assets/Settings/DeepWater_VolumeProfile.asset";

        if (AssetDatabase.LoadAssetAtPath<VolumeProfile>(path) != null)
        {
            AssetDatabase.DeleteAsset(path);
        }

        VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
        profile.name = "DeepWater_VolumeProfile";
        AssetDatabase.CreateAsset(profile, path);

        // Color Adjustments - 어두운 남색
        ColorAdjustments colorAdjustments = ScriptableObject.CreateInstance<ColorAdjustments>();
        colorAdjustments.active = true;
        colorAdjustments.colorFilter.overrideState = true;
        colorAdjustments.colorFilter.value = new Color(0.3f, 0.5f, 0.9f, 1f);
        colorAdjustments.saturation.overrideState = true;
        colorAdjustments.saturation.value = -30f;
        colorAdjustments.postExposure.overrideState = true;
        colorAdjustments.postExposure.value = -0.5f;
        colorAdjustments.contrast.overrideState = true;
        colorAdjustments.contrast.value = 10f;
        colorAdjustments.name = "ColorAdjustments";
        AssetDatabase.AddObjectToAsset(colorAdjustments, profile);
        profile.components.Add(colorAdjustments);

        // Bloom - 약한 강도 (어두운 환경)
        Bloom bloom = ScriptableObject.CreateInstance<Bloom>();
        bloom.active = true;
        bloom.intensity.overrideState = true;
        bloom.intensity.value = 0.3f;
        bloom.threshold.overrideState = true;
        bloom.threshold.value = 1.2f;
        bloom.scatter.overrideState = true;
        bloom.scatter.value = 0.3f;
        bloom.name = "Bloom";
        AssetDatabase.AddObjectToAsset(bloom, profile);
        profile.components.Add(bloom);

        // Vignette - 강한 압박감
        Vignette vignette = ScriptableObject.CreateInstance<Vignette>();
        vignette.active = true;
        vignette.intensity.overrideState = true;
        vignette.intensity.value = 0.25f;
        vignette.smoothness.overrideState = true;
        vignette.smoothness.value = 0.6f;
        vignette.color.overrideState = true;
        vignette.color.value = new Color(0.0f, 0.05f, 0.15f, 1f);
        vignette.name = "Vignette";
        AssetDatabase.AddObjectToAsset(vignette, profile);
        profile.components.Add(vignette);

        // Chromatic Aberration - 강한 왜곡
        ChromaticAberration chromatic = ScriptableObject.CreateInstance<ChromaticAberration>();
        chromatic.active = true;
        chromatic.intensity.overrideState = true;
        chromatic.intensity.value = 0.06f;
        chromatic.name = "ChromaticAberration";
        AssetDatabase.AddObjectToAsset(chromatic, profile);
        profile.components.Add(chromatic);

        EditorUtility.SetDirty(profile);
    }
}
