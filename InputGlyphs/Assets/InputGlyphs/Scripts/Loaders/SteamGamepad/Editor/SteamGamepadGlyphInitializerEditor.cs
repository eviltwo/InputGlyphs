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
                isSteamworksInstalled,
                "https://github.com/rlabrecque/Steamworks.NET",
                "https://github.com/rlabrecque/Steamworks.NET.git?path=/com.rlabrecque.steamworks.net",
                ref _foldoutSteamworks);

            DrawPackageInfo(
                "SteamInputAdapter",
                isAdapterInstalled,
                "https://github.com/eviltwo/UnitySteamInputAdapter",
                "https://github.com/eviltwo/UnitySteamInputAdapter.git?path=UnitySteamInputAdapter/Assets/UnitySteamInputAdapter",
                ref _foldoutAdapter);

            DrawPackageInfo(
                "SteamInputGlyphLoader",
                isLoaderInstalled,
                "https://github.com/eviltwo/UnitySteamInputGlyphLoader",
                "https://github.com/eviltwo/UnitySteamInputGlyphLoader.git?path=UnitySteamInputGlyphLoader/Assets/UnitySteamInputGlyphLoader",
                ref _foldoutLoader);
        }

        private static void DrawPackageInfo(
            string packageName,
            bool installed,
            string packagePageUrl,
            string packageUrl,
            ref bool foldout)
        {
            var foldoutStyle = new GUIStyle(EditorStyles.foldoutHeader);
            foldoutStyle.richText = true;
            var foldoutTitle = installed ? $"{packageName} <color=green>(Installed)</color>" : "Steamworks <color=red>(Not installed)</color>";
            foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, foldoutTitle, foldoutStyle);
            if (foldout)
            {
                if (EditorGUILayout.LinkButton(packagePageUrl))
                {
                    Application.OpenURL(packagePageUrl);
                }
                if (GUILayout.Button("Import package using Unity Package Manager"))
                {
                    Client.Add(packageUrl);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}
