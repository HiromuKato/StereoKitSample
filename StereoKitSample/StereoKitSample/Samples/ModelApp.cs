using StereoKit;

namespace StereoKitSample.Samples
{
    /// <summary>
    /// 3Dモデルを表示するサンプル
    /// </summary>
    class ModelApp : IApp
    {
        private Model model;
        private Pose pose;
        private float scale = 0.1f;

        public void Initialize()
        {
            // モデルの読み込み
            model = Model.FromFile("DamagedHelmet.gltf");
            pose.position = Input.Head.position + Input.Head.Forward * 0.3f;
            pose.orientation = Quat.LookAt(pose.position, Input.Head.position);
        }

        public void Update()
        {
            // モデルを掴んで移動可能にする
            UI.Handle("Model", ref pose, model.Bounds * scale);

            // モデルを描画する
            model.Draw(pose.ToMatrix(scale));
        }

        public void Shutdown()
        {
        }
    }
}
