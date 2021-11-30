using StereoKit;
using System.Diagnostics;

namespace StereoKitSample.Samples
{
    /// <summary>
    /// メニューを表示するサンプル
    /// </summary>
    public class MenuApp : IApp
    {
        static Pose menuPose = new Pose(0.4f, 0, -0.4f, Quat.LookDir(-1, 0, 1));
        static Sprite appLogo;

        public void Initialize()
        {
            appLogo = Sprite.FromFile("StereoKitWide.png");
        }

        public void Update()
        {
            // メニューウィンドウを開始する
            UI.WindowBegin("Menu", ref menuPose, UIWin.Body);

            // 画像を追加
            UI.Image(appLogo, V.XY(UI.LayoutRemaining.x, 0));

            // ボタンを追加
            if (UI.Button("Foo"))
            {
                Debug.WriteLine("Foo");
            }

            // 同じ行にボタンを追加
            UI.SameLine();
            if (UI.Button("Bar"))
            {
                Debug.WriteLine("Bar");
            }

            // セパレーターの追加
            UI.HSeparator();

            // アプリ終了ボタンの追加
            if (UI.Button("Quit"))
            {
                SK.Quit();
            }

            // メニューウィンドウを終了する
            UI.WindowEnd();
        }

        public void Shutdown()
        {
        }
    }
}
