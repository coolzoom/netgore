using System.Linq;
using System.Windows.Forms;
using NetGore;
using NetGore.EditorTools.WinForms;

namespace DemoGame.MapEditor
{
    class GameScreenControl : GraphicsDeviceControl
    {
        ScreenForm _screen;

        public ScreenForm Screen
        {
            get { return _screen; }
            set { _screen = value; }
        }

        protected override void Draw()
        {
            ScreenForm screenToUse = Screen;
            if (screenToUse == null)
                return;

            Screen.UpdateGame();
            Screen.DrawGame();
        }

        protected override void Initialize()
        {
            Application.Idle += delegate { Invalidate(); };
        }
    }
}