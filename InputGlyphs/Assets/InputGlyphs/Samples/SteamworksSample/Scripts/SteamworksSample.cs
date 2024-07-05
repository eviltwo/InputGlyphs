#if STEAMWORKS_NET && !DISABLESTEAMWORKS
using Steamworks;
using UnityEngine;

namespace InputGlyphs.Samples
{
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
