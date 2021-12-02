using StereoKit;

namespace StereoKitSample.Samples
{
    /// <summary>
    /// StereoKit に用意されているメッシュを表示するサンプル
    /// </summary>
    public class DefaultMeshApp : IApp
    {
        Mesh cubeMesh = null;
        Model cubeModel = null;
        Mesh roundedCubeMesh = null;
        Mesh sphereMesh = null;
        Mesh cylinderMesh = null;
        Mesh planeMesh = null;
        private float textScale = 3.0f;

        public void Initialize()
        {
            // Cube
            // メッシュを生成
            cubeMesh = Mesh.GenerateCube(Vec3.One * 0.4f);
            // モデルにメッシュを割り当てることも可能
            cubeModel = Model.FromMesh(cubeMesh, Default.Material);

            // Rounded Cube
            roundedCubeMesh = Mesh.GenerateRoundedCube(Vec3.One * 0.4f, 0.05f);

            // Sphere
            sphereMesh = Mesh.GenerateSphere(0.4f);

            // Cylinder
            cylinderMesh = Mesh.GenerateCylinder(0.4f, 0.4f, Vec3.Up);

            // Plane
            planeMesh = Mesh.GeneratePlane(Vec2.One * 0.4f);
        }

        public void Update()
        {
            // 変換マトリクスをスタックにプッシュ（以降、Pop まで相対的な変換になる）
            Hierarchy.Push(Matrix.TRS(V.XYZ(0.5f, -0.25f, -0.5f), Quat.LookDir(-1, 0, 1), 0.2f));

            // Cube(メッシュ)の描画
            Matrix cubeTransform = Matrix.T(-.5f, -.5f, 0);
            cubeMesh.Draw(Default.Material, cubeTransform);
            // text
            Matrix cubeTextTransform = Matrix.TS(-1.0f, -.5f, 0, textScale);
            Text.Add("Cube", cubeTextTransform, TextAlign.CenterLeft, TextAlign.CenterLeft);

            // Cube(モデル)の描画
            cubeTransform = Matrix.T(.5f, -.5f, 0);
            cubeModel.Draw(cubeTransform);

            // Rounded Cube
            Matrix roundedCubeTransform = Matrix.T(-.5f, 0, 0);
            roundedCubeMesh.Draw(Default.Material, roundedCubeTransform);
            // text
            Matrix roundedCubeTextTransform = Matrix.TS(-1.0f, 0, 0, textScale);
            Text.Add("Rounded Cube", roundedCubeTextTransform, TextAlign.CenterLeft, TextAlign.CenterLeft);

            // Sphere
            Matrix sphereTransform = Matrix.T(-.5f, .5f, 0);
            sphereMesh.Draw(Default.Material, sphereTransform);
            // text
            Matrix sphereTextTransform = Matrix.TS(-1.0f, .5f, 0, textScale);
            Text.Add("Sphere", sphereTextTransform, TextAlign.CenterLeft, TextAlign.CenterLeft);

            // Cylinder
            Matrix cylinderTransform = Matrix.T(-.5f, 1, 0);
            cylinderMesh.Draw(Default.Material, cylinderTransform);
            // text
            Matrix cylinderTextTransform = Matrix.TS(-1.0f, 1, 0, textScale);
            Text.Add("Cylinder", cylinderTextTransform, TextAlign.CenterLeft, TextAlign.CenterLeft);

            // Plane
            Matrix planeTransform = Matrix.T(-.5f, -1, 0);
            planeMesh.Draw(Default.Material, planeTransform);
            // text
            Matrix planeTextTransform = Matrix.TS(-1.0f, -1, 0, textScale);
            Text.Add("Plane", planeTextTransform, TextAlign.CenterLeft, TextAlign.CenterLeft);

            Hierarchy.Pop();
        }

        public void Shutdown()
        {
        }
    }
}
