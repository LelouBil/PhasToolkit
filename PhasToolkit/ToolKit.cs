using System.Collections;
using System.Linq;
using System.Reflection;
using Harmony;
using MelonLoader;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace PhasToolkit
{
    
    public class ToolKit : MelonMod
    {
        private static InventoryManager _inventoryManager;
        
        public static Button AddButton;
        
        private static string _oldText = "";
        private Text _buttonText;

        public override void OnApplicationStart()
        {
            MelonLogger.Log("Starting PhasToolKit");
           
        }

        public override void OnLevelWasLoaded(int level)
        {
            if (level == 1)
            {
                HookAddButton();
            }
        }

        public override void OnUpdate()
        {
            if (AddButton == null) return;
            if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
            {
                MelonLogger.Log("changing text");
                _oldText = _buttonText.text;
                _buttonText.text = "Add all"; //todo localization
            }
            else if (Keyboard.current.leftShiftKey.wasReleasedThisFrame)
            {
                _buttonText.text = _oldText;
            }

        }

        private void HookAddButton()
        {
            var server = GameObject.Find("Canvas").transform.FindChild("_Server");
            _inventoryManager = server.GetComponent<InventoryManager>();
            AddButton = server.transform.FindChild("_Main")
                .transform.FindChild("_Main").transform.FindChild("EquipmentList").transform.FindChild("AddButton").GetComponent<Button>();
            _buttonText = AddButton.GetComponentInChildren<Text>();
            MelonLogger.Log("Add Button found !");
        }

        public static void AddMaxItems()
        {
            foreach (var inventoryItem in _inventoryManager.field_Public_List_1_InventoryItem_0)
            {
                
                var itemName = inventoryItem.field_Public_String_0;
                var keyConfig = itemName + "Inventory";
                var nb = FileBasedPrefs.GetInt(keyConfig);
                //MelonLogger.Log("Adding " + itemName);
                var max = inventoryItem.field_Public_Int32_2;
                var cur = inventoryItem.field_Public_Int32_1;
                var remain = max - cur;

                //MelonLogger.Log($"Current count: {cur} Maximum: {max} Player count: {nb}");
                if (nb < remain || remain <= 0) continue;
                //FileBasedPrefs.SetInt(keyConfig, nb - remain);
                MelonCoroutines.Start(AddNumberOf(inventoryItem, remain));
            }

            MelonLogger.Log("finished");
        }

        private static IEnumerator AddNumberOf(InventoryItem inventoryItem, int remain)
        {
            while(remain-- > 0)
            {
                inventoryItem.ChangeTotalAmount(PhotonNetwork.LocalPlayer,1);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}