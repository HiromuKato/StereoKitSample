using StereoKit;

namespace StereoKitSample.Samples
{
    /// <summary>
    /// レイキャストのサンプル
    /// </summary>
    public class RayCastApp : IApp
    {
        Mesh sphereMesh = Default.MeshSphere;
        Mesh boxMesh = Mesh.GenerateRoundedCube(Vec3.One * 0.2f, 0.05f);
        Pose boxPose = new Pose(0, 0, -1f, Quat.Identity);
        Material redMat;

        public void Initialize()
        {
            // 赤色のマテリアル
            redMat = Default.Material.Copy();
            redMat[MatParamName.ColorTint] = new Color(1, 0, 0);
        }

        public void Update()
        {
            // 操作可能なボックスの描画
            UI.Handle("Box", ref boxPose, boxMesh.Bounds);
            boxMesh.Draw(Default.MaterialUI, boxPose.ToMatrix());

            // ハンドの位置に球を描画
            Pose hand = Input.Hand(Handed.Right).palm;
            Default.MeshSphere.Draw(Default.MaterialUI, hand.ToMatrix(0.02f));

            // ボックスとハンドの間にラインを描画
            Lines.Add(hand.position, boxPose.position, Color.White, 0.01f);

            // メッシュのモデル空間にあるレイを作成
            Matrix transform = boxPose.ToMatrix();
            Ray ray = transform
                .Inverse
                .Transform(Ray.FromTo(hand.position, boxPose.position));

            // レイがヒットした場所に赤い球を描画
            if (ray.Intersect(boxMesh, out Ray at))
            {
                sphereMesh.Draw(redMat, Matrix.TS(transform.Transform(at.position), 0.03f));
            }
        }

        public void Shutdown()
        {
        }
    }
}
