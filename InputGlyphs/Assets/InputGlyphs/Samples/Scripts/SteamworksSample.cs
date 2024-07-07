#if SUPPORT_STEAMWORKS && STEAMWORKS_NET && !DISABLESTEAMWORKS
using Steamworks;
using UnityEngine;

namespace InputGlyphs.Samples
{
    [AddComponentMenu("")] // Disable the script from the Add Component menu
    public class SteamworksSample : MonoBehaviour
    {
        private void Awake()
        {
            SteamAPI.Init();
        }

        private void OnDestroy()
        {
            SteamAPI.Shutdown();
        }
    }
}
#endif
