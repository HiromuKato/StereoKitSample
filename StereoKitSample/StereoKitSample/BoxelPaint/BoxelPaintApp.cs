using StereoKit;

namespace StereoKitSample.BoxelPaint
{
    class BoxelPaintApp : IApp
    {
        // 全体
        Pose _pose = new Pose(0, 0, -0.6f, Quat.Identity);

        // メニュー
        private Pose menuPose = new Pose(0, 0, -0.5f, Quat.LookDir(0, 0, 1));
        private float sliderValue = 8;

        // グリッド
        private Color gridColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        private int gridMaxCount = 8;

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
                }
                // 同じ行にボタンを追加
                UI.SameLine();
                if (UI.Button("Load"))
                {
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
