using System.Collections.Generic;
using StereoKit;

namespace StereoKitSample.BoxelPaint
{
    class BoxelPaintApp : IApp
    {
        class CubeData
        {
            public Vec3 pos;
            public Color color;
            public Material material;
            public Mesh mesh;
        }

        // 全体
        Pose _pose = new Pose(0, 0, -0.6f, Quat.Identity);

        // メニュー
        private Pose menuPose = new Pose(0, 0, -0.5f, Quat.LookDir(0, 0, 1));
        private float sliderValue = 8;
        private Color selectedColor = new Color(1, 1, 1, 1);
        private bool isErase = false;

        // グリッド
        private Color gridColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        private static int gridMaxNum = 8;
        private int gridNum = 8;

        // 描画内容を保持したリスト
        private List<CubeData> cubeData = new List<CubeData>();
        // ペイント済みかどうかを格納する
        private bool[,,] painted = new bool[gridMaxNum, gridMaxNum, gridMaxNum];

        // カラー
        Color red = new Color(1, 0, 0, 1);
        Color green = new Color(0, 1, 0, 1);
        Color blue = new Color(0, 0, 1, 1);
        Color white = new Color(1, 1, 1, 1);
        Color black = new Color(0, 0, 0, 1);

        public void Initialize()
        {
        }

        public void Update()
        {
            /*
            // ワールド座標原点
            Lines.AddAxis(new Pose(0, 0, 0, Quat.Identity), 5 * U.cm);
            Text.Add("ワールド座標原点", Matrix.Identity);
            */

            DrawMenu();

            // 全体をハンドルの子にして動かせるようにする
            UI.HandleBegin("PaintingRoot", ref _pose, new Bounds(Vec3.One * 5 * U.cm), true);
            {
                DrawGrid();

                // ピンチ操作のチェック
                PaintCube();

                // キューブの描画
                foreach (var data in cubeData)
                {
                    data.mesh.Draw(data.material, Matrix.T(data.pos));
                }
            }
            UI.HandleEnd();
        }

        public void Shutdown()
        {
        }

        private void PaintCube()
        {
            Hand hand = Input.Hand(Handed.Right);
            if (!hand.IsPinched)
            {
                return;
            }
            Vec3 fingertip = hand[FingerId.Index, JointId.Tip].position;
            fingertip = Hierarchy.ToLocal(fingertip);

            if (isErase)
            {
                foreach (var data in cubeData)
                {
                    Bounds genVolume = new Bounds(data.pos, new Vec3(5 * U.cm, 5 * U.cm, 5 * U.cm));
                    bool contains = genVolume.Contains(fingertip);
                    if (contains)
                    {
                        float x = (data.pos.x - 2.5f * U.cm) / (5 * U.cm);
                        float y = (data.pos.y - 2.5f * U.cm) / (5 * U.cm);
                        float z = (data.pos.z - 2.5f * U.cm) / (5 * U.cm);
                        /*
                        Log.Info("pos: " + data.pos.x + ", " + data.pos.y + ", " + data.pos.z);
                        Log.Info(((data.pos.x - 2.5f * U.cm) / 5 * U.cm).ToString());
                        Log.Info("Erase: " + x + ", " + y + ", " + z);
                        */
                        painted[(int)x, (int)y, (int)z] = false;
                        cubeData.Remove(data);
                        break;
                    }
                }
            }
            else
            {
                // ピンチ操作時にグリッド内に人差し指がある場合の処理
                for (int z = 0; z < gridNum; z++)
                {
                    for (int y = 0; y < gridNum; y++)
                    {
                        for (int x = 0; x < gridNum; x++)
                        {
                            // 既に描画済みの場合はスキップ
                            if (painted[x, y, z] == true)
                            {
                                continue;
                            }

                            // 各グリッド(5cm幅)の中心
                            Vec3 center = new Vec3(x * 5 * U.cm + 2.5f * U.cm, y * 5 * U.cm + 2.5f * U.cm, z * 5 * U.cm + 2.5f * U.cm);
                            Bounds genVolume = new Bounds(center, new Vec3(5 * U.cm, 5 * U.cm, 5 * U.cm));
                            bool contains = genVolume.Contains(fingertip);
                            if (contains)
                            {
                                Log.Info(x + ", " + y + ", " + z);
                                // 描画されたフラグを立てる
                                painted[x, y, z] = true;

                                // Cubeメッシュ生成
                                var mesh = Mesh.GenerateCube(Vec3.One * 5 * U.cm);
                                var data = new CubeData();
                                data.pos = center;
                                data.color = selectedColor;
                                var colorMat = Default.Material.Copy();
                                colorMat[MatParamName.ColorTint] = selectedColor;
                                data.material = colorMat;
                                data.mesh = mesh;
                                cubeData.Add(data);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// メニューを描画する
        /// </summary>
        private void DrawMenu()
        {
            // メニューウィンドウを開始する
            UI.WindowBegin("Menu", ref menuPose, UIWin.Normal);
            {
                if (UI.Button("Red")) { selectedColor = red; isErase = false; }
                UI.SameLine();
                if (UI.Button("Green")) { selectedColor = green; isErase = false; }
                UI.SameLine();
                if (UI.Button("Blue")) { selectedColor = blue; isErase = false; }
                UI.SameLine();
                if (UI.Button("White")) { selectedColor = white; isErase = false; }
                UI.SameLine();
                if (UI.Button("Erase")) { selectedColor = black; isErase = true; }

                UI.HSeparator();

                // ボタンの追加
                if (UI.Button("Save"))
                {
                    OnSaveData("");
                }
                // 同じ行にボタンを追加
                UI.SameLine();
                if (UI.Button("Load") && !Platform.FilePickerVisible)
                {
                    //Platform.FilePicker(PickerMode.Open, OnLoadData, null, ".obj");
                    OnLoadData("");
                }
                UI.SameLine();
                if (UI.Button("Export"))
                {
                    OnExportObj();
                }

                UI.HSeparator();

                if (UI.Button("All Clear"))
                {
                    cubeData.Clear();

                    for (int z = 0; z < gridNum; z++)
                    {
                        for (int y = 0; y < gridNum; y++)
                        {
                            for (int x = 0; x < gridNum; x++)
                            {
                                painted[x, y, z] = false;
                            }
                        }
                    }
                }


                // スライダーの追加
                UI.Label("Grid", V.XY(5 * U.cm, UI.LineHeight));
                UI.SameLine();
                if (UI.HSlider("Grid", ref sliderValue, 2, 8, 1, 16 * U.cm, UIConfirm.Pinch))
                {
                    gridNum = (int)sliderValue;
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
            //Hierarchy.Push(Matrix.TRS(V.XYZ(0, 0, -0.4f), Quat.LookDir(0, 0, -1), 1));
            {
                var gridScale = 5 * U.cm;

                // グリッドの原点
                Lines.AddAxis(new Pose(0, 0, 0, Quat.Identity), 5 * U.cm);
                Text.Add("グリッド原点", Matrix.R(0, 180, 0));

                for (int z = 0; z <= gridNum; z++)
                {
                    for (int y = 0; y <= gridNum; y++)
                    {
                        for (int x = 0; x <= gridNum; x++)
                        {
                            Vec3 pos1 = new Vec3(0, y, z) * gridScale;
                            Vec3 pos2 = new Vec3(gridNum, y, z) * gridScale;
                            var p1 = new LinePoint { pt = pos1, color = gridColor, thickness = U.mm };
                            var p2 = new LinePoint { pt = pos2, color = gridColor, thickness = U.mm };
                            Lines.Add(new LinePoint[] { p1, p2 });

                            pos1 = new Vec3(x, 0, z) * gridScale;
                            pos2 = new Vec3(x, gridNum, z) * gridScale;
                            p1 = new LinePoint { pt = pos1, color = gridColor, thickness = U.mm };
                            p2 = new LinePoint { pt = pos2, color = gridColor, thickness = U.mm };
                            Lines.Add(new LinePoint[] { p1, p2 });

                            pos1 = new Vec3(x, y, 0) * gridScale;
                            pos2 = new Vec3(x, y, gridNum) * gridScale;
                            p1 = new LinePoint { pt = pos1, color = gridColor, thickness = U.mm };
                            p2 = new LinePoint { pt = pos2, color = gridColor, thickness = U.mm };
                            Lines.Add(new LinePoint[] { p1, p2 });
                        }
                    }
                }
            }
            //Hierarchy.Pop();
        }

        /// <summary>
        /// データを読み込む
        /// </summary>
        /// <param name="value"></param>
        private void OnLoadData(string value)
        {
            Platform.FilePicker(PickerMode.Open, file =>
            {
                if (Platform.ReadFile(file, out string text))
                {
                    Log.Info(text);
                }
            }, null, ".bxp");
        }

        /// <summary>
        /// データを保存する
        /// </summary>
        /// <param name="value"></param>
        private void OnSaveData(string value)
        {
            Platform.FilePicker(PickerMode.Save, file =>
            {
                Platform.WriteFile(file, "Text for the file.\n- Thanks!");
            }, null, ".bxp"); // 中身はテキストだがtxtと見分けるため拡張子は独自のもの
        }

        /// <summary>
        /// objフォーマットのデータをエクスポートする
        /// </summary>
        private void OnExportObj()
        {
            // cubeData から obj ファイルを生成する
            string modelData =
@"# Blender v2.79 (sub 0) OBJ File: ''
# www.blender.org
mtllib cube_obj_test.mtl
o Cube_Cube.001
v -1.000000 -1.000000 1.000000
v -1.000000 1.000000 1.000000
v -1.000000 -1.000000 -1.000000
v -1.000000 1.000000 -1.000000
v 1.000000 -1.000000 1.000000
v 1.000000 1.000000 1.000000
v 1.000000 -1.000000 -1.000000
v 1.000000 1.000000 -1.000000
vn -1.0000 0.0000 0.0000
vn 0.0000 0.0000 -1.0000
vn 1.0000 0.0000 0.0000
vn 0.0000 0.0000 1.0000
vn 0.0000 -1.0000 0.0000
vn 0.0000 1.0000 0.0000
usemtl None
s off
f 2//1 3//1 1//1
f 4//2 7//2 3//2
f 8//3 5//3 7//3
f 6//4 1//4 5//4
f 7//5 1//5 3//5
f 4//6 6//6 8//6
f 2//1 4//1 3//1
f 4//2 8//2 7//2
f 8//3 6//3 5//3
f 6//4 2//4 1//4
f 7//5 5//5 1//5
f 4//6 2//6 6//6";

            Platform.FilePicker(PickerMode.Save, file =>
            {
                Platform.WriteFile(file, modelData);
            }, null, ".obj");
        }

    } // class BoxelPaintApp
} // namespace StereoKitSample.BoxelPaint
