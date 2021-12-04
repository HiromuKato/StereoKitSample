using StereoKit;

namespace StereoKitSample.Samples
{
    /// <summary>
    /// ハンドメニューのサンプル
    /// </summary>
    public class HandMenuApp : IApp
    {
        private string text = "Show Hand!";
        private Pose textPose;

        public void Initialize()
        {
        }

        public void Update()
        {
            DrawHandMenu(Handed.Right);
            DrawHandMenu(Handed.Left);

            // テキスト表示
            textPose.position = Input.Head.position + Input.Head.Forward * 1;
            textPose.orientation = Quat.LookAt(textPose.position, Input.Head.position);
            Text.Add(text, textPose.ToMatrix(1), TextAlign.CenterLeft, TextAlign.CenterLeft);
        }

        public void Shutdown()
        {
        }

        /// <summary>
        /// ハンドメニューを描画する
        /// (枠のないWindowを手の横に配置する)
        /// </summary>
        /// <param name="handed"></param>
        private void DrawHandMenu(Handed handed)
        {
            if (!HandFacingHead(handed))
            {
                // ユーザーが手のひらを見ていない場合
                return;
            }

            // メニューのサイズとオフセットを設定する
            Vec2 size = new Vec2(4, 16);
            // 手のひらのポーズ右方向は各手の異なる側を指しているため、各手に異なるXオフセットが必要
            float offset = handed == Handed.Left ? -2 - size.x : 2 + size.x;

            // 手の側面を基準にしてメニューを配置する
            Hand hand = Input.Hand(handed);
            Vec3 at = hand[FingerId.Little, JointId.KnuckleMajor].position;
            Vec3 down = hand[FingerId.Little, JointId.Root].position;
            Vec3 across = hand[FingerId.Index, JointId.KnuckleMajor].position;

            Pose menuPose = new Pose(
                at,
                Quat.LookAt(at, across, at - down) * Quat.FromAngles(0, handed == Handed.Left ? 90 : -90, 0));
            menuPose.position += menuPose.Right * offset * U.cm;
            menuPose.position += menuPose.Up * (size.y / 2) * U.cm;

            // メニューの作成
            UI.WindowBegin("HandMenu", ref menuPose, size * U.cm, UIWin.Empty);
            {
                if (UI.Button("Test")) { text = "Test"; }
                if (UI.Button("That")) { text = "That"; }
                if (UI.Button("Hand")) { text = "Hand"; }
            }
            UI.WindowEnd();
        }

        /// <summary>
        /// ユーザーが手のひらを見ているかどうかを判定する
        /// </summary>
        /// <param name="handed">右手か左手か</param>
        /// <returns>手のひらを見ているときはtrueを返す</returns>
        private bool HandFacingHead(Handed handed)
        {
            Hand hand = Input.Hand(handed);
            if (!hand.IsTracked)
            {
                // 手がトラッキングできていない場合
                return false;
            }

            // 手のひらが向いている方向
            Vec3 palmDirection = (hand.palm.Forward).Normalized;
            // 手のひらから頭への方向
            Vec3 directionToHead = (Input.Head.position - hand.palm.position).Normalized;

            // ２つのベクトルの内積から手のひらを見ているかどうかを判定
            return Vec3.Dot(palmDirection, directionToHead) > 0.5f;
        }
    }
}
