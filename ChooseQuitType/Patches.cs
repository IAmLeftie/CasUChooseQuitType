using System;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Windows;

namespace ChooseQuitType
{
    internal class Patches
    {
        public static Sprite originalSprite;

        [HarmonyPatch(typeof(PauseHandler), "TryQuit")]
        public static class PauseHandlerTryQuitPatches
        {
            [HarmonyPrefix]
            private static void Prefix(PauseHandler __instance)
            {
                if (UnityEngine.Input.GetKey(KeyCode.LeftShift))
                {
                    PlayerCamera.main.ToMainMenu();
                    return;
                }
            }
        }

        [HarmonyPatch(typeof(PauseHandler), "Start")]
        public static class PauseHandlerStartPatches
        {
            [HarmonyPrefix]
            private static void Prefix(PauseHandler __instance)
            {
                originalSprite = __instance.quitButton.sprite;
            }
        }

        [HarmonyPatch(typeof(PauseHandler), "Update")]
        public static class PauseHandlerUpdatePatches
        {
            [HarmonyPostfix]
            private static void Postfix(PauseHandler __instance)
            {
                if (WorldGeneration.world.biomeOverride == WorldGeneration.OverrideSceneType.Tutorial)
                {
                    return;
                }

                if (UnityEngine.Input.GetKey(KeyCode.LeftShift))
                {
                    bool flag = __instance.exitSprite != null;
                    if (flag)
                    {
                        __instance.quitButton.sprite = __instance.exitSprite;
                    }
                    UITooltip component = __instance.quitButton.GetComponent<UITooltip>();
                    bool flag2 = component != null;
                    if (flag2)
                    {
                        component.tipName = Locale.GetOther("pauseexit");
                        component.tipDesc = "";
                        component.skipLocale = true;
                    }
                    __instance.quitButton.color = Color.white;
                    __instance.timesQuitPressed = 0;
                }
                else if (UnityEngine.Input.GetKeyUp(KeyCode.LeftShift))
                {
                    __instance.quitButton.sprite = originalSprite;
                    UITooltip component = __instance.quitButton.GetComponent<UITooltip>();
                    bool flag2 = component != null;
                    if (flag2)
                    {
                        component.tipName = Locale.GetOther("pausegiveup");
                        component.tipDesc = Locale.GetOther("pausegiveupdsc");
                        component.skipLocale = true;
                    }
                    __instance.quitButton.color = Color.white;
                    __instance.timesQuitPressed = 0;
                }
            }
        }
    }
}
