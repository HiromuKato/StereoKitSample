using StereoKit;
using StereoKit.Framework;

namespace StereoKitSample.Samples
{
    /// <summary>
    /// ラジアルハンドメニューのサンプル
    /// </summary>
    public class HandMenuRadialApp : IApp
    {
        HandMenuRadial handMenu;
        private string text = "HandMenuRadial Sample";
        private Pose textPose;

        public void Initialize()
        {
            // ステッパーはIStepperインターフェイスを実装するクラス
            // StereoKitのステッパーリストに追加するとステッパーのStepメソッドが毎フレーム呼ばれるようになる
            // 登録だけしておけば後は操作する必要がない場合に有効な手段
            handMenu = SK.AddStepper(new HandMenuRadial(
                new HandRadialLayer("Root",
                    new HandMenuItem("File", null, () => text = "File", "File"),
                    new HandMenuItem("Edit", null, () => text = "Edit", "Edit"),
                    new HandMenuItem("About", null, () => text = "StereoKit " + SK.VersionName),
                    new HandMenuItem("Cancel", null, () => text = "Cancel")),
                new HandRadialLayer("File",
                    new HandMenuItem("New", null, () => text = "New"),
                    new HandMenuItem("Open", null, () => text = "Open"),
                    new HandMenuItem("Close", null, () => text = "Close"),
                    new HandMenuItem("Back", null, () => text = "Back", HandMenuAction.Back)),
                new HandRadialLayer("Edit",
                    new HandMenuItem("Copy", null, () => text = "Copy"),
                    new HandMenuItem("Paste", null, () => text = "Paste"),
                    new HandMenuItem("Back", null, () => text = "Back", HandMenuAction.Back))));
        }

        public void Update()
        {
            // テキスト表示
            textPose.position = Input.Head.position + Input.Head.Forward * 1;
            textPose.orientation = Quat.LookAt(textPose.position, Input.Head.position);
            Text.Add(text, textPose.ToMatrix(1), TextAlign.CenterLeft, TextAlign.CenterLeft);
        }

        public void Shutdown()
        {
            // ステッパーリストから削除する
            SK.RemoveStepper(handMenu);
        }
    }
}
