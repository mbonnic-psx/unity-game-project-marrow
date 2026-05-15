using System.Collections.Generic;
using UnityEngine;

namespace ShaderToolboxPro.URP
{
    [ExecuteInEditMode]
    public class OpenAssetButton : MonoBehaviour
    {
#if UNITY_EDITOR
        public string assetName;
        public Object assetToOpen;

        private static List<OpenAssetButton> _buttons = new(40);

        public static List<OpenAssetButton> Buttons
        {
            get { return _buttons; }
            private set { _buttons = value; }
        }

        private void OnEnable()
        {
            if (!Buttons.Contains(this))
            {
                Buttons.Add(this);
            }
        }

        private void OnDisable()
        {
            Buttons.Remove(this);
        }
#endif
    }
}
