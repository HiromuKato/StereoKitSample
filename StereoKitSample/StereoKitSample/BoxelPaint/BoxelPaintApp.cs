using System.Collections.Generic;
using StereoKit;

namespace StereoKitSample.BoxelPaint
{
    class BoxelPaintApp : IApp
    {
        struct CubeData
        {
            public Pose pose;
            public Material material;
            public Mesh mesh;
        }

        // 全体
        Pose _pose = new Pose(0, 0, -0.6f, Quat.Identity);

        // メニュー
        private Pose menuPose = new Pose(0, 0, -0.5f, Quat.LookDir(0, 0, 1));
        private float sliderValue = 8;
        private Color selectedColor = new Color(1, 1, 1, 1);

        // グリッド
        private Color gridColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        private int gridMaxCount = 8;

        // 描画内容を保持したリスト
        private List<CubeData> cubeData = new List<CubeData>();

        public void Initialize()
        {
        }

        public void Update()
        {
            // ワールド座標原点
            Lines.AddAxis(new Pose(0, 0, 0, Quat.Identity), 5 * U.cm);
            Text.Add("ワールド座標原点", Matrix.Identity);

            DrawMenu();

            // 全体をハンドルの子にして動かせるようにする
            UI.HandleBegin("PaintingRoot", ref _pose, new Bounds(Vec3.One * 5 * U.cm), true);
            {
                DrawGrid();
            }
            UI.HandleEnd();

            // キューブの描画
            foreach (var data in cubeData)
            {
                data.mesh.Draw(data.material, data.pose.ToMatrix(1));
            }
        }

        public void Shutdown()
        {
        }

        /// <summary>
        /// メニューを描画する
        /// </summary>
        private void DrawMenu()
        {
            // メニューウィンドウを開始する
            UI.WindowBegin("Menu", ref menuPose, UIWin.Normal);
            {
                // ボタンの追加
                if (UI.Button("Save"))
                {
                    selectedColor = new Color(1, 0, 0, 1);
                }
                // 同じ行にボタンを追加
                UI.SameLine();
                if (UI.Button("Load"))
                {
                    selectedColor = new Color(0, 0, 1, 1);
                }

                UI.SameLine();
                if (UI.Button("Add"))
                {
                    // 右手人差し指の位置取得
                    Pose indexPose = Input.Hand(Handed.Right)[FingerId.Index, JointId.Tip].Pose;

                    // Cubeメッシュ生成
                    var mesh = Mesh.GenerateCube(Vec3.One * 5 * U.cm);
                    var data = new CubeData();
                    data.pose = indexPose;
                    var colorMat = Default.Material.Copy();
                    colorMat[MatParamName.ColorTint] = selectedColor;
                    data.material = colorMat;
                    data.mesh = mesh;
                    cubeData.Add(data);
                }
                if (UI.Button("Clear"))
                {
                    cubeData.Clear();
                }


                // スライダーの追加
                UI.Label("Grid", V.XY(5 * U.cm, UI.LineHeight));
                UI.SameLine();
                if (UI.HSlider("Grid", ref sliderValue, 2, 8, 1, 16 * U.cm, UIConfirm.Pinch))
                {
                    gridMaxCount = (int)sliderValue;
                }
            }
            // メニューウィンドウを終了する
            UI.WindowEnd();
        }

        /// <summary>
        /// グリッドを描画する
        /// </summary>
        private void DrawGrid()
        {
            // 変換マトリクスをスタックにプッシュ（以降、Pop まで相対的な変換になる）
            Hierarchy.Push(Matrix.TRS(V.XYZ(0, 0, -0.4f), Quat.LookDir(0, 0, -1), 1));
            {
                var gridScale = 5 * U.cm;

                // グリッドの原点
                Lines.AddAxis(new Pose(0, 0, 0, Quat.Identity), 5 * U.cm);
                Text.Add("グリッド原点", Matrix.R(0, 180, 0));

                for (int z = 0; z <= gridMaxCount; z++)
                {
                    for (int y = 0; y <= gridMaxCount; y++)
                    {
                        for (int x = 0; x <= gridMaxCount; x++)
                        {
                            Vec3 pos1 = new Vec3(0, y, z) * gridScale;
                            Vec3 pos2 = new Vec3(gridMaxCount, y, z) * gridScale;
                            var p1 = new LinePoint { pt = pos1, color = gridColor, thickness = U.mm };
                            var p2 = new LinePoint { pt = pos2, color = gridColor, thickness = U.mm };
                            Lines.Add(new LinePoint[] { p1, p2 });

                            pos1 = new Vec3(x, 0, z) * gridScale;
                            pos2 = new Vec3(x, gridMaxCount, z) * gridScale;
                            p1 = new LinePoint { pt = pos1, color = gridColor, thickness = U.mm };
                            p2 = new LinePoint { pt = pos2, color = gridColor, thickness = U.mm };
                            Lines.Add(new LinePoint[] { p1, p2 });

                            pos1 = new Vec3(x, y, 0) * gridScale;
                            pos2 = new Vec3(x, y, gridMaxCount) * gridScale;
                            p1 = new LinePoint { pt = pos1, color = gridColor, thickness = U.mm };
                            p2 = new LinePoint { pt = pos2, color = gridColor, thickness = U.mm };
                            Lines.Add(new LinePoint[] { p1, p2 });
                        }
                    }
                }
            }
            Hierarchy.Pop();
        }
    }
}
