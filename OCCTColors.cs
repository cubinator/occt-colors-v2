// [mscorlib.dll]
using System.IO;   // Path
using System.Linq; // IEnumerable::ToArray

// [UnityEngine.CoreModule.dll]
// [UnityEngine.dll]
using UnityEngine; // Color, Color32

// [Assembly-CSharp-firstpass.dll]
// OCCTApp

// [BepInEx.dll]
using BepInEx;               // BaseUnityPlugin, BepInPlugin, Paths
using BepInEx.Configuration; // ConfigEntry, ConfigFile
using BepInEx.Logging;       // Logger, ManualLogSource

// [0Harmony.dll]
using HarmonyLib;  // HarmonyPatch, MethodType


using System.Reflection;
[assembly: AssemblyTitleAttribute(OCCTColors.OCCTColors.ModName)]
[assembly: AssemblyVersionAttribute(OCCTColors.OCCTColors.ModVersion)]
[assembly: AssemblyDescriptionAttribute(
    "BepInEx plugin for PC Building Simulator to change the font colors in the OCCT app")]


namespace OCCTColors
{
    [BepInPlugin(ModGuid, ModName, ModVersion)]
    [BepInProcess("PCBS.exe")]
    internal class OCCTColors : BaseUnityPlugin
    {
        public const string ModGuid = "com.bepinex.cubi.pcbs.occtcolors";
        public const string ModName = "OCCT Colors";
        public const string ModVersion = "2.0.1";

        public new static readonly ManualLogSource Logger
            = BepInEx.Logging.Logger.CreateLogSource(OCCTColors.ModName);

        public static readonly Color[] Colors = new Color[]
        {
            new Color(0,   220, 255), // cyan
            new Color(0,   220, 255), // cyan
            new Color(0,   255, 0  ), // lime
            new Color(255, 220, 0  ), // yellow
            new Color(255, 255, 255)  // white
            // No sixth color as in the original OCCTApp::m_colours array,
            // because this color is unused
        };

        /**
         * PlugIn entry point
         */
        private void Awake ()
        {
            OCCTColors.LoadConfig();
            Harmony.CreateAndPatchAll(typeof(Patch_OCCTApp_ctor), ModGuid);
        }

        /**
         * PlugIn configuration
         */
        private static void LoadConfig ()
        {
            ConfigFile config = new ConfigFile(Path.Combine(Paths.ConfigPath, "OCCTColors.ini"), true);

            for (int idx = 0; idx < OCCTColors.Colors.Length; idx++)
            {
                Color32 color255 = (Color32)OCCTColors.Colors[idx];

                ConfigEntry<string> setting = config.Bind(
                    /* Config section */ "Colors",
                    /* Setting key    */ string.Format("Line{0}", idx + 1),
                    /* Default value  */ string.Format("{0},{1},{2}", color255.r, color255.g, color255.b),
                    /* Description    */ string.Format("R,G,B values of OCCT row {0}", idx + 1)
                );

                // TODO Implement more color parsers, e.g. #00ccff, #0cf, cyan
                string[] channelStrs = (
                    from channel in setting.Value.Split(',')
                    select channel.Trim()
                ).ToArray();

                try
                {
                    OCCTColors.Colors[idx] = (Color)new Color32(
                        byte.Parse(channelStrs[0]),
                        byte.Parse(channelStrs[1]),
                        byte.Parse(channelStrs[2]),
                        0xFF
                    );
                }
                catch
                {
                    OCCTColors.Logger.LogWarning(
                        string.Format(
                            "Unable to parse color \"{0}\" for Line{1}. Using default color instead.",
                            setting.Value, idx
                        )
                    );
                }
            }
        }
    }

    /**
     * Patch of OCCTApp::.ctor()
     */
    [HarmonyPatch(typeof(OCCTApp), MethodType.Constructor)]
    internal static class Patch_OCCTApp_ctor
    {
        private static void Postfix (
            // OCCTApp::.ctor() has no arguments

            // Harmony injections
            ref Color[] ___m_colours // OCCTApp::m_colours
        ) {
            ___m_colours = OCCTColors.Colors;
        }
    }
}
