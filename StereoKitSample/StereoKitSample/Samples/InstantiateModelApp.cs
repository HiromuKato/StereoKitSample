using StereoKit;
using System.Threading.Tasks;

namespace StereoKitSample.Samples
{
    /// <summary>
    /// 動的にモデルを生成するサンプル
    /// </summary>
    public class InstantiateModelApp : IApp
    {
        private Model model = null;
        private float modelScale = 1;
        private float menuScale = 1;
        private Pose modelPose = new Pose(0, 0, -1.0f, Quat.LookDir(-Vec3.Forward));
        private Pose menuPose = new Pose(0.5f, 0, -0.5f, Quat.LookDir(-1, 0, 1));
        // バウンディングボックスのマテリアル
        private Material volumeMat;

        public void Initialize()
        {
            // バウンディングボックスのマテリアル設定
            volumeMat = Default.MaterialUIBox.Copy();
            volumeMat["border_size"] = 0;
            volumeMat["border_affect_radius"] = 0.3f;
        }

        public void Update()
        {
            // メニュー
            UI.WindowBegin("Menu", ref menuPose, UIWin.Body);
            {
                if (UI.Button("Open Model") && !Platform.FilePickerVisible)
                {
                    // モデルを読み込むファイルピッカーを表示する
                    Platform.FilePicker(PickerMode.Open, OnLoadModel, null,
                        ".gltf", ".glb", ".obj", ".stl", ".fbx", ".ply");
                }

                if (UI.Button("Clear"))
                {
                    model = null;
                }

                UI.Label("Scale");
                UI.HSlider("ScaleSlider", ref menuScale, 0, 1, 0);
            }
            UI.WindowEnd();

            // モデルの表示
            if (model != null)
            {
                // バウンディングボックスの描画
                Bounds scaled = model.Bounds * modelScale * menuScale;
                UI.HandleBegin("Model", ref modelPose, scaled);
                {
                    Default.MeshCube.Draw(volumeMat, Matrix.TS(scaled.center, scaled.dimensions));
                }
                UI.HandleEnd();

                // モデルの描画、アニメーション再生
                Hierarchy.Push(modelPose.ToMatrix(modelScale * menuScale));
                {
                    model.StepAnim();
                    model.Draw(Matrix.Identity);
                }
                Hierarchy.Pop();
            }

        }

        public void Shutdown()
        {
        }

        /// <summary>
        /// モデルを非同期で読み込む
        /// </summary>
        /// <param name="filename"></param>
        private void OnLoadModel(string filename)
        {
            Task.Run(() =>
            {
                model = Model.FromFile(filename);
                modelScale = 1 / model.Bounds.dimensions.Magnitude;
                if (model.Anims.Count > 0)
                {
                    model.PlayAnim(model.Anims[0], AnimMode.Loop);
                }
            });
        }

    }
}
