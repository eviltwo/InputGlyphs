using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace InputGlyphs.Loaders.Steam.Editor
{
    [CustomEditor(typeof(SteamGamepadGlyphInitializer))]
    public class SteamGamepadGlyphInitializerEditor : UnityEditor.Editor
    {
        private bool _foldoutSteamworks;
        private bool _foldoutAdapter;
        private bool _foldoutLoader;
        private Dictionary<string, string> _versionCache = new Dictionary<string, string>();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var requirePackages = 0;
            var installedPackages = 0;

            var isSteamworksInstalled = false;
            requirePackages++;
#if SUPPORT_STEAMWORKS
            isSteamworksInstalled = true;
            installedPackages++;
#endif

            var isAdapterInstalled = false;
            requirePackages++;
#if SUPPORT_ADAPTER
            isAdapterInstalled = true;
            installedPackages++;
#endif

            var isLoaderInstalled = false;
            requirePackages++;
#if SUPPORT_LOADER
            isLoaderInstalled = true;
            installedPackages++;
#endif

            var labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.richText = true;
            var packageCompleted = installedPackages == requirePackages;
            var labelTitle = packageCompleted ? "All packages are installed." : $"Packages installed: {installedPackages}/{requirePackages}";
            EditorGUILayout.LabelField(labelTitle, labelStyle);

            DrawPackageInfo(
                "Steamworks.NET",
                GetPackageVersion("com.rlabrecque.steamworks.net"),
                isSteamworksInstalled,
                "https://github.com/rlabrecque/Steamworks.NET",
                "https://github.com/rlabrecque/Steamworks.NET.git?path=/com.rlabrecque.steamworks.net",
                ref _foldoutSteamworks);

            DrawPackageInfo(
                "SteamInputAdapter",
                GetPackageVersion("com.eviltwo.unity-steam-input-adapter"),
                isAdapterInstalled,
                "https://github.com/eviltwo/UnitySteamInputAdapter",
                "https://github.com/eviltwo/UnitySteamInputAdapter.git?path=UnitySteamInputAdapter/Assets/UnitySteamInputAdapter",
                ref _foldoutAdapter);

            DrawPackageInfo(
                "SteamInputGlyphLoader",
                GetPackageVersion("com.eviltwo.unity-steam-input-glyph-loader"),
                isLoaderInstalled,
                "https://github.com/eviltwo/UnitySteamInputGlyphLoader",
                "https://github.com/eviltwo/UnitySteamInputGlyphLoader.git?path=UnitySteamInputGlyphLoader/Assets/UnitySteamInputGlyphLoader",
                ref _foldoutLoader);
        }

        private static void DrawPackageInfo(
            string packageTitle,
            string installedPackageVersion,
            bool installed,
            string packagePageUrl,
            string packageUrl,
            ref bool foldout)
        {
            var foldoutStyle = new GUIStyle(EditorStyles.foldoutHeader);
            foldoutStyle.richText = true;
            var foldoutTitle = installed ? $"{packageTitle} <color=green>(Installed)</color>" : "Steamworks <color=red>(Not installed)</color>";
            foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, foldoutTitle, foldoutStyle);
            if (foldout)
            {
                if (EditorGUILayout.LinkButton(packagePageUrl))
                {
                    Application.OpenURL(packagePageUrl);
                }
                EditorGUILayout.LabelField($"Installed version : {installedPackageVersion}");
                if (GUILayout.Button("Import package using Unity Package Manager"))
                {
                    Client.Add(packageUrl);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private string GetPackageVersion(string packageName)
        {
            if (_versionCache.TryGetValue(packageName, out var cachedVersion))
            {
                return cachedVersion;
            }
            var packageInfo = FindPackageInfo(packageName);
            var version = packageInfo?.version ?? string.Empty;
            _versionCache[packageName] = version;
            return version;
        }

        private static UnityEditor.PackageManager.PackageInfo FindPackageInfo(string packageName)
        {
            var guids = AssetDatabase.FindAssets("package t:TextAsset");
            for (var i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                if (textAsset == null)
                {
                    continue;
                }
                var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(path);
                if (packageInfo != null && packageInfo.name == packageName)
                {
                    return packageInfo;
                }
            }

            return null;
        }
    }
}
