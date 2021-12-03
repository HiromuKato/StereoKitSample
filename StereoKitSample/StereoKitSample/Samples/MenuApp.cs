using StereoKit;
using System.Diagnostics;

namespace StereoKitSample.Samples
{
    /// <summary>
    /// メニューの表示と StereoKit で利用可能な UI のサンプル
    /// </summary>
    public class MenuApp : IApp
    {
        private Pose menuPose = new Pose(0, 0, -0.5f, Quat.LookDir(0, 0, 1));
        private Pose menu2Pose = new Pose(0.5f, 0, -0.5f, Quat.LookDir(0, 0, 1));
        private Sprite appLogo;
        private string text = "";
        private float sliderValue = 1;
        private bool isToggled = false;
        private int radioBtnNum = 1;

        public void Initialize()
        {
            // 画像の読み込み
            appLogo = Sprite.FromFile("StereoKitWide.png");
        }

        public void Update()
        {
            // メニューウィンドウを開始する
            UI.WindowBegin("Menu", ref menuPose, UIWin.Body);
            {
                // 画像の追加
                UI.Image(appLogo, V.XY(UI.LayoutRemaining.x, 0));

                // ボタンの追加
                if (UI.Button("Foo"))
                {
                    text = "Foo";
                }
                // 同じ行にボタンを追加
                UI.SameLine();
                if (UI.Button("Bar"))
                {
                    text = "Bar";
                }
                UI.SameLine();
                // ラベルの追加
                UI.Label(text);

                UI.Label("Value:" + sliderValue.ToString("F3"));
                // スライダーの追加
                UI.HSlider("ScaleSlider", ref sliderValue, 0, 1, 0);

                // トグルボタンの追加
                UI.Toggle("Toggle Button", ref isToggled);

                // ラジオボタンの追加
                if (UI.Radio("Radio1", radioBtnNum == 1)) radioBtnNum = 1;
                UI.SameLine();
                if (UI.Radio("Radio2", radioBtnNum == 2)) radioBtnNum = 2;
                UI.SameLine();
                if (UI.Radio("Radio3", radioBtnNum == 3)) radioBtnNum = 3;

                // セパレーターの追加
                UI.HSeparator();

                // アプリ終了ボタンの追加
                if (UI.Button("Quit"))
                {
                    SK.Quit();
                }
            }
            // メニューウィンドウを終了する
            UI.WindowEnd();

            // 別のメニュー（ヘッダー付き）を表示する
            UI.WindowBegin("Menu2", ref menu2Pose, UIWin.Normal);
            {
                UI.Label("StereoKit");
            }
            UI.WindowEnd();
        }

        public void Shutdown()
        {
        }
    }
}
