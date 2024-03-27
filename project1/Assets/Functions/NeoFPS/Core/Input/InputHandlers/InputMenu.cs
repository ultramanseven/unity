using UnityEngine;

namespace NeoFPS
{
    public class InputMenu : FpsInput
    {
        [SerializeField, Tooltip("Should the mouse cursor be released when this object is activated (or component enabled). Set to false for things like loading screens.")]
        private bool m_ShowMouseCursor = true;

        private bool m_PreviousMouseCursor = false;

        public override FpsInputContext inputContext
        {
            get { return FpsInputContext.Menu; }
        }

        protected override void OnGainFocus()
        {
            base.OnGainFocus();

            // Store mouse cursor state and apply setting
            m_PreviousMouseCursor = NeoFpsInputManagerBase.captureMouseCursor;
            NeoFpsInputManagerBase.captureMouseCursor = !m_ShowMouseCursor;
        }

        protected override void OnLoseFocus()
        {
            base.OnLoseFocus();

            // Reset mouse cursor state
            NeoFpsInputManagerBase.captureMouseCursor = m_PreviousMouseCursor;
        }

        protected override void UpdateInput() { }
    }
}
