using StereoKit;

namespace StereoKitSample.Samples
{
    /// <summary>
    /// ヘッド、ハンド、人差し指、オブジェクトの位置・前方ベクトル、マウスの位置を表示するサンプル
    /// </summary>
    public class PoseApp : IApp
    {
        private Pose menuPose = new Pose(0, 0, -0.5f, Quat.LookDir(0, 0, 1));

        public void Initialize()
        {
        }

        public void Update()
        {
            UI.WindowBegin("Position / Forward", ref menuPose, UIWin.Normal);
            {
                // 頭のポーズ表示
                UI.Label("Head: " + Input.Head);

                // ハンド（てのひら）のポーズ表示
                Hand handR = Input.Hand(Handed.Right);
                Hand handL = Input.Hand(Handed.Left);
                UI.Label("Right Hand(palm): " + handR.palm);
                UI.Label("Left Hand(palm): " + handL.palm);

                // 右手人差し指のポーズ表示
                HandJoint index = handR[FingerId.Index, JointId.Tip];
                UI.Label("Index: " + index.Pose);

                // オブジェクト（メニュー）のポーズ表示
                UI.Label("Menu: " + menuPose);

                // マウス(Vec2)
                UI.HSeparator();
                UI.Label("Mouse(x, y): " + Input.Mouse.pos);
            }
            UI.WindowEnd();
        }

        public void Shutdown()
        {
        }
    }
}
