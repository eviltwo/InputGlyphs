using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace InputGlyphs.Loaders.Steam.Editor
{
    [CustomEditor(typeof(SteamGamepadGlyphInitializer))]
    public class SteamGamepadGlyphInitializerEditor : UnityEditor.Editor
    {
        private class RequiredPackageInfo
        {
            public string title;
            public string name;
            public string pageUrl;
            public string packageUrl;
            public string requiredVersion;
            public bool isInstalled;
            public double installTime;
            public bool isFoldout;
        }

        private RequiredPackageInfo[] _requiredPackages = new RequiredPackageInfo[]
        {
            new RequiredPackageInfo
            {
                title = "Steamworks.NET",
                name = "com.rlabrecque.steamworks.net",
                pageUrl = "https://github.com/rlabrecque/Steamworks.NET",
                packageUrl = "https://github.com/rlabrecque/Steamworks.NET.git?path=/com.rlabrecque.steamworks.net",
                requiredVersion = "20.2.0",
#if SUPPORT_STEAMWORKS
                isInstalled = true,
#endif
            },
            new RequiredPackageInfo
            {
                title = "Steam Input Adapter",
                name = "com.eviltwo.unity-steam-input-adapter",
                pageUrl = "https://github.com/eviltwo/UnitySteamInputAdapter",
                packageUrl = "https://github.com/eviltwo/UnitySteamInputAdapter.git?path=UnitySteamInputAdapter/Assets/UnitySteamInputAdapter",
                requiredVersion = "1.0.2",
#if SUPPORT_ADAPTER
                isInstalled = true,
#endif
            },
        };

        private Dictionary<string, string> _versionCache = new Dictionary<string, string>();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var installedPackages = 0;
            for (var i = 0; i < _requiredPackages.Length; i++)
            {
                if (_requiredPackages[i].isInstalled)
                {
                    installedPackages++;
                }
            }
            var requirePackages = _requiredPackages.Length;
            var labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.richText = true;
            var packageCompleted = installedPackages == requirePackages;
            var labelTitle = packageCompleted ? "All packages are installed." : $"Packages installed: {installedPackages}/{requirePackages}";
            EditorGUILayout.LabelField(labelTitle, labelStyle);

            for (var i = 0; i < _requiredPackages.Length; i++)
            {
                var requiredPackage = _requiredPackages[i];
                DrawPackageInfo(
                    requiredPackage.title,
                    GetPackageVersion(requiredPackage.name),
                    requiredPackage.requiredVersion,
                    requiredPackage.pageUrl,
                    requiredPackage.packageUrl,
                    requiredPackage.isInstalled,
                    ref requiredPackage.installTime,
                    ref requiredPackage.isFoldout);
            }
        }

        private static void DrawPackageInfo(
            string packageTitle,
            string installedPackageVersion,
            string requiredPackageVersion,
            string packagePageUrl,
            string packageUrl,
            bool installed,
            ref double installTime,
            ref bool foldout)
        {
            var foldoutStyle = new GUIStyle(EditorStyles.foldoutHeader);
            foldoutStyle.richText = true;
            var isSameVersion = installedPackageVersion == requiredPackageVersion;
            string foldoutTitle;
            if (installed)
            {
                if (isSameVersion)
                {
                    foldoutTitle = $"{packageTitle} <color=green>(Installed)</color>";
                }
                else
                {
                    foldoutTitle = $"{packageTitle} <color=green>(Installed, <color=red>but version is different</color>)</color>";
                }
            }
            else
            {
                foldoutTitle = $"{packageTitle} <color=red>(Not installed)</color>";
            }
            foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, foldoutTitle, foldoutStyle);
            if (foldout)
            {
                if (EditorGUILayout.LinkButton(packagePageUrl))
                {
                    Application.OpenURL(packagePageUrl);
                }
                EditorGUILayout.LabelField($"Installed version : {installedPackageVersion}");
                EditorGUILayout.LabelField($"Required version : {requiredPackageVersion}");
                if (EditorApplication.timeSinceStartup < installTime + 5.0)
                {
                    EditorGUILayout.LabelField("Installing...");
                }
                else if (GUILayout.Button("Import package using Package Manager"))
                {
                    installTime = EditorApplication.timeSinceStartup;
                    Client.Add($"{packageUrl}#{requiredPackageVersion}");
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
