using Harmony;
using MelonLoader;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
// ReSharper disable InconsistentNaming

namespace PhasToolkit
{
    [HarmonyPatch(typeof(Button),"Press")]
    public class AddButtonPatch
    {

        [HarmonyPrefix]
        public static bool Prefix(Button __instance)
        {
            if (__instance != ToolKit.AddButton || !Keyboard.current.leftShiftKey.isPressed) return true;
            
            ToolKit.AddMaxItems();
            __instance.isPointerInside = false;
            //MelonLogger.Log("Just added all, blocking call");
            return false;
            

        }
    }
}