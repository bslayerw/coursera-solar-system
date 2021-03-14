using UnityEngine;
using Utils;

namespace Settings
{
    [CreateAssetMenu(fileName = "UISettings", menuName = "Solar System/UISettings")]
    public class UISettings : SingletonScriptableObject<UISettings>
    {
        [Range(-50, 50)]
        [Header("Rotation Speed Multiplier.")]
        [Tooltip("Adjust the speed at which the planets rotate around the sun. This affects all planets.")]
        public float speedMultiplier = 1.0f;
        [HideInInspector]
        public Vector2 uiPositionMultiplier = new Vector2(0.0f, 0.0f);
    }
}
