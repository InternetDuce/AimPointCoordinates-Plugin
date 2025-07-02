using BepInEx;
using BepInEx.Configuration;
using Comfort.Common;
using EFT;
using UnityEngine;

namespace AimPointCoordinates
{
    [BepInPlugin("com.yourname.aimpointcoordinates", "AimPoint Coordinates", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private ConfigEntry<bool> showCoordinates;
        private Vector3 aimPoint;
        private bool hasHit;

        private void Awake()
        {
            showCoordinates = Config.Bind("General", "ShowCoordinates", true, "Показать координаты точки прицеливания");
        }

        private void Update()
        {
            if (!showCoordinates.Value)
                return;

            if (!Singleton<GameWorld>.Instantiated || Camera.main == null)
                return;

            var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMaskClass.HighPolyWithTerrainMask))
            {
                aimPoint = hit.point;
                hasHit = true;
            }
            else
            {
                hasHit = false;
            }
        }

        private void OnGUI()
        {
            if (!showCoordinates.Value || !hasHit)
                return;

            string coords = $"Aim: X:{aimPoint.x:F2} Y:{aimPoint.y:F2} Z:{aimPoint.z:F2}";
            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                normal = { textColor = Color.cyan }
            };

            Vector2 size = style.CalcSize(new GUIContent(coords));
            float x = (Screen.width - size.x) / 2;
            float y = (Screen.height / 2) + 20;

            GUI.Label(new Rect(x, y, size.x, size.y), coords, style);
        }
    }
}
