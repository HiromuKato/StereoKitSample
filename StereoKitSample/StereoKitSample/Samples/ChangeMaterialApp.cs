using StereoKit;

namespace StereoKitSample.Samples
{
    /// <summary>
    /// マテリアルを変更するサンプル
    /// </summary>
    class ChangeMaterialApp : IApp
    {
        private Mesh cubeMesh = null;
        private Vec3 pos = new Vec3(0, 0, -1);
        private Pose menuPose = new Pose(0, 0, -0.5f, Quat.LookDir(0, 0, 1));
        private Material matColor;
        private Material matWireframe;
        private Material matAlphaAdd;
        private Material matTextured;
        private Material matUnlit;
        private Matrix floorTransform;
        private Material floorMaterial;

        private enum MaterialType
        {
            Default = 0,
            Color,
            Wireframe,
            AlphaAdd,
            Texture,
            TextureUnlit
        }
        private MaterialType matType = MaterialType.Default;

        public void Initialize()
        {
            // Cubeモデルを生成
            cubeMesh = Mesh.GenerateCube(Vec3.One * 0.4f);

            // 床の生成
            floorTransform = Matrix.TS(0, -1.5f, 0, new Vec3(30, 0.1f, 30));
            floorMaterial = Default.Material.Copy();
            floorMaterial[MatParamName.DiffuseTex] = Tex.FromFile("floor.png");

            // 青色
            matColor = Default.Material.Copy();
            matColor[MatParamName.ColorTint] = new Color(0, 0, 1);

            // ワイヤフレーム
            matWireframe = Default.Material.Copy();
            matWireframe.Wireframe = true;

            // 半透明
            matAlphaAdd = Default.Material.Copy();
            matAlphaAdd.Transparency = Transparency.Add;
            matAlphaAdd.DepthWrite = false;
            matAlphaAdd[MatParamName.ColorTint] = new Color(0, 0.5f, 0.5f, 1.0f);

            // テクスチャ
            matTextured = Default.Material.Copy();
            matTextured[MatParamName.DiffuseTex] = Tex.FromFile("test.png");

            // テクスチャ Unlit
            matUnlit = Default.MaterialUnlit.Copy();
            matUnlit[MatParamName.DiffuseTex] = Tex.FromFile("test.png");
        }

        public void Update()
        {
            // 床の描画 
            Default.MeshCube.Draw(floorMaterial, floorTransform);

            // マテリアルに応じたキューブの描画
            switch (matType)
            {
                case MaterialType.Default:
                    cubeMesh.Draw(Default.Material, Matrix.T(pos.x, pos.y, pos.z));
                    break;
                case MaterialType.Color:
                    cubeMesh.Draw(matColor, Matrix.T(pos.x, pos.y, pos.z));
                    break;
                case MaterialType.Wireframe:
                    cubeMesh.Draw(matWireframe, Matrix.T(pos.x, pos.y, pos.z));
                    break;
                case MaterialType.AlphaAdd:
                    cubeMesh.Draw(matAlphaAdd, Matrix.T(pos.x, pos.y, pos.z));
                    break;
                case MaterialType.Texture:
                    cubeMesh.Draw(matTextured, Matrix.T(pos.x, pos.y, pos.z));
                    break;
                case MaterialType.TextureUnlit:
                    cubeMesh.Draw(matUnlit, Matrix.T(pos.x, pos.y, pos.z));
                    break;
                default:
                    cubeMesh.Draw(Default.Material, Matrix.T(pos.x, pos.y, pos.z));
                    break;
            }

            // マテリアル選択メニュー
            UI.WindowBegin("Menu", ref menuPose, UIWin.Normal);
            {
                if (UI.Radio("Default", matType == MaterialType.Default)) matType = MaterialType.Default;
                if (UI.Radio("Color", matType == MaterialType.Color)) matType = MaterialType.Color;
                if (UI.Radio("Wireframe", matType == MaterialType.Wireframe)) matType = MaterialType.Wireframe;
                if (UI.Radio("AlphaAdd", matType == MaterialType.AlphaAdd)) matType = MaterialType.AlphaAdd;
                if (UI.Radio("Texture", matType == MaterialType.Texture)) matType = MaterialType.Texture;
                if (UI.Radio("TextureUnlit", matType == MaterialType.TextureUnlit)) matType = MaterialType.TextureUnlit;
            }
            UI.WindowEnd();
        }

        public void Shutdown()
        {
        }
    }
}
