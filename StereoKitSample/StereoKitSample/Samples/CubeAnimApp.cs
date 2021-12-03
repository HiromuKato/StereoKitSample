using System;
using StereoKit;

namespace StereoKitSample.Samples
{
    /// <summary>
    /// Cubeのアニメーションサンプル
    /// </summary>
    class CubeAnimApp : IApp
    {
        private Mesh cubeMesh = null;
        private float val1 = 0;
        private float val2 = 0;
        private float angle = 0;

        public void Initialize()
        {
            // Cubeメッシュの生成
            cubeMesh = Mesh.GenerateCube(Vec3.One * 0.5f);
        }

        public void Update()
        {
            // マトリックスの計算
            val1 += 0.01f;
            val2 += 0.02f;
            angle += 5;
            float posX = SKMath.Sin(val1);
            float scale = Math.Abs(SKMath.Sin(val2));
            Matrix cubeTransform = Matrix.TRS(
                new Vec3(posX, 0, -1),
                Quat.FromAngles(0, angle, 0),
                new Vec3(scale, scale, scale));

            // Cubeメッシュの描画
            cubeMesh.Draw(Default.Material, cubeTransform);
        }

        public void Shutdown()
        {
        }
    }
}
