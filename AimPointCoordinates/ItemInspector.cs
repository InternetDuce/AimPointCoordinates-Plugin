using BepInEx;
using BepInEx.Configuration;
using Comfort.Common;
using EFT;
using EFT.Interactive;
using UnityEngine;

namespace AimPointCoordinates
{
    [BepInPlugin("com.yourname.aimpointcoordinates", "AimPoint Coordinates", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private ConfigEntry<bool> showCoordinates;
        private string infoText = "None";

        private void Awake()
        {
            showCoordinates = Config.Bind("General", "ShowCoordinates", true, "Показать информацию о предмете под прицелом");
        }

        private void Update()
        {
            if (!showCoordinates.Value)
            {
                infoText = "";
                return;
            }

            if (!Singleton<GameWorld>.Instantiated)
            {
                infoText = "None";
                return;
            }

            var camera = Camera.main;
            if (camera == null)
            {
                infoText = "None";
                return;
            }

            Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMaskClass.HighPolyWithTerrainMask))
            {
                var loot = hit.collider.GetComponent<LootItem>();
                if (loot != null && loot.Item != null)
                {
                    string name = loot.Item.LocalizedName();
                    string tpl = loot.Item.TemplateId;
                    Vector3 pos = loot.transform.position;
                    Vector3 rot = loot.transform.eulerAngles;

                    infoText = $"{name} ({tpl})\n" +
                               $"Pos: {pos.x:F2}, {pos.y:F2}, {pos.z:F2}\n" +
                               $"Rot: {rot.x:F1}, {rot.y:F1}, {rot.z:F1}";
                    return;
                }
            }

            infoText = "None";
        }

        private void OnGUI()
        {
            if (!showCoordinates.Value)
                return;

            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                normal = { textColor = infoText == "None" ? Color.red : Color.cyan }
            };

            Vector2 size = style.CalcSize(new GUIContent(infoText));
            float x = (Screen.width - size.x) / 2;
            float y = (Screen.height / 2) + 20;

            GUI.Label(new Rect(x, y, size.x, size.y), infoText, style);
        }
    }
}
