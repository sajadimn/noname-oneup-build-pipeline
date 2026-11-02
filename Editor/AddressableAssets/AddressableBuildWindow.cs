#if ONEUP_ADDRESSABLES
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class AddressableBuildWindow : EditorWindow
{
    private string sdkVersion = "Unity SDK: v2.1.1";
    private static string addressableVersion;
    private static AddressableProfiles currentProfile = AddressableProfiles.Default;
    private static bool clearBuildCacheState = true;
    private static string contentFileSavePath = System.IO.Path.Combine(Application.dataPath, "../AddressableContent");
    private static string contentFileName = "addressables_content_state.bin";
    private static BuildTarget currentBuildTarget = BuildTarget.Android;
    private static BuildTargetGroup currentBuildTargetGroup = BuildTargetGroup.Android;
    private static bool personalAddressableState = false;
    private static string personalAddressableName = "";
    private int separator = 10;
    private int space = 2;
    private static Single addressableTextFieldSize = 270;
    private static Single addressableStateSize = 185;
    private static Single windowWidthSize = 470;
    private static Single windowHeightSize = 440;
    private static Single buttonWidthSize = 220;
    private static Single buttonHeightSize = 30;
    private static string platforms = "";

    private static List<AvailableBuildTarget> availableBuildTargets = new List<AvailableBuildTarget>()
    {
        new AvailableBuildTarget() {buildTarget = BuildTarget.Android, buildTargetGroup = BuildTargetGroup.Android},
        new AvailableBuildTarget() {buildTarget = BuildTarget.iOS, buildTargetGroup = BuildTargetGroup.iOS},
    };


    [MenuItem("OneUp/Build/Addressable Build Setting")]
    public static void AddressableBuildSetting()
    {
        var buildWindow = GetWindow(typeof(AddressableBuildWindow));
        buildWindow.titleContent = new GUIContent("Addressable Build Window");
        buildWindow.minSize = new Vector2(windowWidthSize, windowHeightSize);
        buildWindow.maxSize = new Vector2(windowWidthSize, windowHeightSize);
        personalAddressableState = false;
        personalAddressableName = "";
        addressableVersion = AddressableAssetSettingsDefaultObject.Settings.OverridePlayerVersion;
        Enum.TryParse(AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetProfileName(
            AddressableAssetSettingsDefaultObject.Settings.activeProfileId), out currentProfile);
        SetSupportPlatforms();
    }

    private async void OnGUI()
    {
        GUI.backgroundColor = Color.cyan;
        EditorGUILayout.Space(separator);
        BuildWindowGUI.BeginBox();
        EditorGUILayout.Space();
        BuildWindowGUI.Title("Build Addressable Profiles", platforms, TextAnchor.MiddleCenter);
        EditorGUILayout.Space();
        BuildWindowGUI.EndBox();
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Separator();
        BuildWindowGUI.BeginBox();
        EditorGUILayout.BeginHorizontal();
        clearBuildCacheState = EditorGUILayout.Toggle("Clear Build Cache: ", clearBuildCacheState, GUILayout.Width(addressableStateSize));
        addressableVersion = EditorGUILayout.TextField("Addressable Version: ", addressableVersion,
            GUILayout.Width(addressableTextFieldSize));
        EditorGUILayout.EndHorizontal();
        BuildWindowGUI.EndBox();

        if (string.IsNullOrEmpty(addressableVersion))
        {
            EditorGUILayout.Space(space);
            BuildWindowGUI.Title("Enter Addressable Version To Build", "Note: Warning", TextAnchor.MiddleCenter);
            EditorGUILayout.Separator();
            SDKVersion();
            return;
        }

        AddressableAssetSettingsDefaultObject.Settings.OverridePlayerVersion = addressableVersion;

        BuildWindowGUI.BeginBox();
        EditorGUILayout.BeginHorizontal();
        personalAddressableState =
            EditorGUILayout.Toggle("Personal Addressable: ", personalAddressableState, GUILayout.Width(addressableStateSize));
        if (personalAddressableState)
        {
            personalAddressableName =
                EditorGUILayout.TextField("Addressable Name: ", personalAddressableName,
                    GUILayout.Width(addressableTextFieldSize));
            if (string.IsNullOrEmpty(personalAddressableName))
            {
                EditorGUILayout.EndHorizontal();
                BuildWindowGUI.EndBox();
                EditorGUILayout.Separator();
                BuildWindowGUI.Title("Addressable Name Can't Be Empty", "Note: Error", TextAnchor.MiddleCenter);
                EditorGUILayout.Separator();
                SDKVersion();
                return;
            }
        }

        EditorGUILayout.EndHorizontal();
        BuildWindowGUI.EndBox();

        EditorGUILayout.Separator();
        BuildWindowGUI.BeginBox();
        EditorGUILayout.Separator();
        BuildWindowGUI.Title($"Support Platforms: {platforms}",
            "Add More Platform In *AddressableBuildWindow* Script On *availableBuildTargets* Property", TextAnchor.MiddleCenter);

        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Update All Profiles", GUILayout.Height(buttonHeightSize),
                GUILayout.Width(buttonWidthSize)))
        {
            await UpdateAllAddressableProfiles();
            Close();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("New Build All Profiles", GUILayout.Height(buttonHeightSize),
                GUILayout.Width(buttonWidthSize)))
        {
            await NewBuildAllAddressableProfiles();
            Close();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        BuildWindowGUI.EndBox();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        BuildWindowGUI.BeginBox();
        EditorGUILayout.BeginHorizontal();
        currentProfile = (AddressableProfiles) EditorGUILayout.EnumPopup("Current Profile In Use: ", currentProfile);
        EditorGUILayout.EndHorizontal();
        BuildWindowGUI.EndBox();
        
        if (!AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetAllProfileNames()
                .Exists(x => x == currentProfile.ToString()))
        {
            EditorGUILayout.Separator();
            BuildWindowGUI.Title("Selected ProfileName Is UnAvailable In Addressable Profiles",
                "Note: Error", TextAnchor.MiddleCenter);
            EditorGUILayout.Separator();
            SDKVersion();
            return;
        }

        AddressableAssetSettingsDefaultObject.Settings.activeProfileId =
            AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetProfileId(currentProfile.ToString());

        BuildWindowGUI.BeginBox();
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button($"Update {currentProfile.ToString()} Profile", GUILayout.Height(buttonHeightSize),
                GUILayout.Width(buttonWidthSize)))
        {
            bool state = await UpdateAddressableProfile(currentProfile, clearBuildCacheState,
                personalAddressableState ? personalAddressableName : "");
            if (state == false)
                return;
            Close();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button($"New Build {currentProfile.ToString()} Profile", GUILayout.Height(buttonHeightSize),
                GUILayout.Width(buttonWidthSize)))
        {
            bool state = await NewBuildAddressableProfile(currentProfile, clearBuildCacheState,
                personalAddressableState ? personalAddressableName : "");
            if (state == false)
                return;
            Close();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        BuildWindowGUI.EndBox();
        SDKVersion();
    }

    private static async Task<bool> UpdateAllAddressableProfiles()
    {
        currentBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        currentBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        int count = 0;
        while (count < availableBuildTargets.Count)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(availableBuildTargets[count].buildTargetGroup,
                availableBuildTargets[count].buildTarget);
            await UpdateAllProfilesInCurrentPlatform();
            count++;
        }

        EditorUserBuildSettings.SwitchActiveBuildTarget(currentBuildTargetGroup, currentBuildTarget);
        return true;
    }

    private static async Task<bool> UpdateAllProfilesInCurrentPlatform()
    {
        int count = 0;
        while (count < AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetAllProfileNames().Count)
        {
            string profile = AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetAllProfileNames()[count];
            Enum.TryParse(profile, out AddressableProfiles addressableProfile);
            bool state =
                await UpdateAddressableProfile(addressableProfile, clearBuildCacheState,
                    personalAddressableState ? personalAddressableName : "");
            count++;
        }

        return true;
    }


    public static async Task<bool> UpdateAddressableProfile(AddressableProfiles _addressableProfile,
        bool _clearBuildCacheState, string _addressableName)
    {
        Debug.Log(
            $"{EditorUserBuildSettings.activeBuildTarget} - {_addressableProfile.ToString()} - Update Addressable Build Processor Start");

        _addressableName = _addressableName != "" ? $"_{_addressableName}" : "";
        addressableVersion = AddressableAssetSettingsDefaultObject.Settings.OverridePlayerVersion;
        string addressableProfileContent = contentFileSavePath
                                           + "/" + _addressableProfile.ToString()
                                           + "/" + addressableVersion + _addressableName
                                           + "/" + EditorUserBuildSettings.activeBuildTarget.ToString()
                                           + "/" + contentFileName;
        if (!File.Exists(addressableProfileContent))
        {
            Debug.Log(
                $"{EditorUserBuildSettings.activeBuildTarget} - {_addressableProfile.ToString()} - Addressable Asset Update Build Failed - AddressableProfileContent Not Found - Require To New Build First");
            return false;
        }

        string projectContent = AddressableAssetSettingsDefaultObject.Settings.ContentStateBuildPath + "/" +
                                contentFileName;
        if (File.Exists(projectContent))
            FileUtil.DeleteFileOrDirectory(projectContent);
        FileUtil.CopyFileOrDirectory(addressableProfileContent, projectContent);

        AddressableAssetSettingsDefaultObject.Settings.activeProfileId =
            AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetProfileId(_addressableProfile.ToString());

        string lastAddressableVersion = AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetValueByName(
            AddressableAssetSettingsDefaultObject.Settings.activeProfileId, "AddressableVersion");
        if (string.IsNullOrEmpty(lastAddressableVersion))
        {
            AddressableAssetSettingsDefaultObject.Settings.profileSettings.CreateValue("AddressableVersion",
                addressableVersion);
        }

        AddressableAssetSettingsDefaultObject.Settings.profileSettings.SetValue(
            AddressableAssetSettingsDefaultObject.Settings.activeProfileId, "AddressableVersion",
            addressableVersion + _addressableName);

        if (_clearBuildCacheState)
        {
            AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings
                .ActivePlayerDataBuilder);
        }

        ContentUpdateScript.BuildContentUpdate(AddressableAssetSettingsDefaultObject.Settings, projectContent);
        Debug.Log(
            $"{EditorUserBuildSettings.activeBuildTarget} - {_addressableProfile.ToString()} - Update Addressable Build Processor Done");
        return true;
    }

    private static async Task<bool> NewBuildAllAddressableProfiles()
    {
        currentBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        currentBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        foreach (var availableBuildTarget in availableBuildTargets)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(availableBuildTarget.buildTargetGroup,
                availableBuildTarget.buildTarget);
            await NewBuildAllProfilesInCurrentPlatform();
        }

        EditorUserBuildSettings.SwitchActiveBuildTarget(currentBuildTargetGroup, currentBuildTarget);
        return true;
    }

    private static async Task<bool> NewBuildAllProfilesInCurrentPlatform()
    {
        foreach (var profile in AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetAllProfileNames())
        {
            Enum.TryParse(profile, out AddressableProfiles addressableProfile);
            bool state = await NewBuildAddressableProfile(addressableProfile, clearBuildCacheState,
                personalAddressableState ? personalAddressableName : "");
            if (state == false)
                continue;
        }

        return true;
    }

    public static async Task<bool> NewBuildAddressableProfile(AddressableProfiles _addressableProfile,
        bool _clearBuildCacheState, string _addressableName)
    {
        Debug.Log(
            $"{EditorUserBuildSettings.activeBuildTarget} - {_addressableProfile.ToString()} - New Addressable Build Processor Start");

        _addressableName = _addressableName != "" ? $"_{_addressableName}" : "";
        AddressableAssetSettingsDefaultObject.Settings.activeProfileId =
            AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetProfileId(_addressableProfile.ToString());

        addressableVersion = AddressableAssetSettingsDefaultObject.Settings.OverridePlayerVersion;
        string lastAddressableVersion = AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetValueByName(
            AddressableAssetSettingsDefaultObject.Settings.activeProfileId, "AddressableVersion");
        if (string.IsNullOrEmpty(lastAddressableVersion))
        {
            AddressableAssetSettingsDefaultObject.Settings.profileSettings.CreateValue("AddressableVersion",
                addressableVersion);
        }

        AddressableAssetSettingsDefaultObject.Settings.profileSettings.SetValue(
            AddressableAssetSettingsDefaultObject.Settings.activeProfileId, "AddressableVersion",
            addressableVersion + _addressableName);


        if (_clearBuildCacheState)
        {
            AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings
                .ActivePlayerDataBuilder);
        }

        AddressableAssetSettings.BuildPlayerContent();

        string projectContent = AddressableAssetSettingsDefaultObject.Settings.ContentStateBuildPath + "/" +
                                contentFileName;
        if (!File.Exists(projectContent))
        {
            Debug.Log(
                $"{EditorUserBuildSettings.activeBuildTarget} - {_addressableProfile.ToString()} - Addressable Asset New Build Failed - AddressableProfileContent Not Found - Require To New Build Again");
            return false;
        }

        string addressableProfileContentDirectory = contentFileSavePath
                                                    + "/" + _addressableProfile.ToString()
                                                    + "/" + addressableVersion + _addressableName
                                                    + "/" + EditorUserBuildSettings.activeBuildTarget.ToString();
        if (!Directory.Exists(addressableProfileContentDirectory))
            Directory.CreateDirectory(addressableProfileContentDirectory);

        string addressableProfileContent = addressableProfileContentDirectory + "/" + contentFileName;
        if (File.Exists(addressableProfileContent))
            FileUtil.DeleteFileOrDirectory(addressableProfileContent);

        FileUtil.CopyFileOrDirectory(projectContent, addressableProfileContent);

        Debug.Log(
            $"{EditorUserBuildSettings.activeBuildTarget} - {_addressableProfile.ToString()} - New Addressable Build Processor Done");
        return true;
    }

    public static async Task<bool> SetAddressableProfile(AddressableProfiles _addressableProfile,
        bool _clearBuildCacheState, string _addressableName)
    {
        _addressableName = _addressableName != "" ? $"_{_addressableName}" : "";
        string addressableProfileContent = contentFileSavePath
                                           + "/" + _addressableProfile.ToString()
                                           + "/" +
                                           AddressableAssetSettingsDefaultObject.Settings.OverridePlayerVersion +
                                           _addressableName
                                           + "/" + EditorUserBuildSettings.activeBuildTarget.ToString()
                                           + "/" + contentFileName;

        if (!File.Exists(addressableProfileContent))
        {
            bool state = await NewBuildAddressableProfile(_addressableProfile, _clearBuildCacheState, _addressableName);
            return state;
        }
        else
        {
            bool state = await UpdateAddressableProfile(_addressableProfile, _clearBuildCacheState, _addressableName);
            return state;
        }
    }

    private static void SetSupportPlatforms()
    {
        platforms = "";
        for (int i = 0; i < availableBuildTargets.Count; i++)
        {
            platforms += availableBuildTargets[i].buildTarget;
            if (i < availableBuildTargets.Count - 1)
            {
                platforms += " & ";
            }
        }
    }

    private void SDKVersion()
    {
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        BuildWindowGUI.Title("", sdkVersion, TextAnchor.MiddleRight);
        EditorGUILayout.EndHorizontal();
    }
}

public class AvailableBuildTarget
{
    public BuildTarget buildTarget;
    public BuildTargetGroup buildTargetGroup;
}
#endif
