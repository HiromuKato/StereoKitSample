using StereoKit;

namespace StereoKitSample.Samples
{
    /// <summary>
    /// プロジェクト作成時に生成されるひな形を別クラスに分けて動作するようにしたもの
    /// </summary>
    public class FirstApp : IApp
    {
        private Pose cubePose;
        private Model cube;
        private Matrix floorTransform;
        private Material floorMaterial;

        public void Initialize()
        {
            // アプリで利用するアセットを生成する
            // キューブ
            cubePose = new Pose(0, 0, -0.5f, Quat.Identity);
            cube = Model.FromMesh(
                Mesh.GenerateRoundedCube(Vec3.One * 0.1f, 0.02f),
                Default.MaterialUI);
            // 床
            floorTransform = Matrix.TS(0, -1.5f, 0, new Vec3(30, 0.1f, 30));
            floorMaterial = new Material(Shader.FromFile("floor.hlsl"));
            floorMaterial.Transparency = Transparency.Blend;
        }

        public void Update()
        {
            // ディスプレイタイプが Opaque(VR ヘッドセットや PC) の場合は床を描画する
            if (SK.System.displayType == Display.Opaque)
            {
                Default.MeshCube.Draw(floorMaterial, floorTransform);
            }

            // UI クラス：ユーザインターフェースとインタラクションメソッドのコレクション
            // Handle メソッド：掴んだり移動する機能の開始・終了をハンドルする（掴んでいる間はtrueを返す）
            UI.Handle("Cube", ref cubePose, cube.Bounds);
            // キューブを描画する
            cube.Draw(cubePose.ToMatrix());
        }

        public void Shutdown()
        {
        }
    }
}
