using System.Collections.Generic;
using System.Text;
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
            Vec3[] basevVecs_v = new Vec3[]
            {
                new Vec3( -1.000000f, -1.000000f,  1.000000f ),
                new Vec3( -1.000000f,  1.000000f,  1.000000f ),
                new Vec3( -1.000000f, -1.000000f, -1.000000f ),
                new Vec3( -1.000000f,  1.000000f, -1.000000f ),
                new Vec3(  1.000000f, -1.000000f,  1.000000f ),
                new Vec3(  1.000000f,  1.000000f,  1.000000f ),
                new Vec3(  1.000000f, -1.000000f, -1.000000f ),
                new Vec3(  1.000000f,  1.000000f, -1.000000f ),
            };

            Vec3[] basevVecs_vn = new Vec3[]
            {
                new Vec3(-1.0000f, 0.0000f, 0.0000f),
                new Vec3(0.0000f, 0.0000f, -1.0000f),
                new Vec3(1.0000f, 0.0000f, 0.0000f),
                new Vec3(0.0000f, 0.0000f, 1.0000f),
                new Vec3(0.0000f, -1.0000f, 0.0000f),
                new Vec3(0.0000f, 1.0000f, 0.0000f)
            };

            string[] basevVecs_f = new string[]
            {
                "2//1 3//1 1//1",
                "4//2 7//2 3//2",
                "8//3 5//3 7//3",
                "6//4 1//4 5//4",
                "7//5 1//5 3//5",
                "4//6 6//6 8//6",
                "2//1 4//1 3//1",
                "4//2 8//2 7//2",
                "8//3 6//3 5//3",
                "6//4 2//4 1//4",
                "7//5 5//5 1//5",
                "4//6 2//6 6//6"
            };

            string filename = "test";
            int index = 0;
            string matName = "BlueMaterial";

            StringBuilder sb = new StringBuilder("# Exported from BoxelModeler\n");
            sb.Append("mtllib " + filename + ".mtl\n");

            sb.Append("o cube" + index + "\n");
            sb.Append("v " + basevVecs_v[0].x + " " + basevVecs_v[0].y + " " + basevVecs_v[0].z + "\n");
            sb.Append("v " + basevVecs_v[1].x + " " + basevVecs_v[1].y + " " + basevVecs_v[1].z + "\n");
            sb.Append("v " + basevVecs_v[2].x + " " + basevVecs_v[2].y + " " + basevVecs_v[2].z + "\n");
            sb.Append("v " + basevVecs_v[3].x + " " + basevVecs_v[3].y + " " + basevVecs_v[3].z + "\n");
            sb.Append("v " + basevVecs_v[4].x + " " + basevVecs_v[4].y + " " + basevVecs_v[4].z + "\n");
            sb.Append("v " + basevVecs_v[5].x + " " + basevVecs_v[5].y + " " + basevVecs_v[5].z + "\n");
            sb.Append("v " + basevVecs_v[6].x + " " + basevVecs_v[6].y + " " + basevVecs_v[6].z + "\n");
            sb.Append("v " + basevVecs_v[7].x + " " + basevVecs_v[7].y + " " + basevVecs_v[7].z + "\n");

            sb.Append("vn " + basevVecs_vn[0].x + " " + basevVecs_vn[0].y + " " + basevVecs_vn[0].z + "\n");
            sb.Append("vn " + basevVecs_vn[1].x + " " + basevVecs_vn[1].y + " " + basevVecs_vn[1].z + "\n");
            sb.Append("vn " + basevVecs_vn[2].x + " " + basevVecs_vn[2].y + " " + basevVecs_vn[2].z + "\n");
            sb.Append("vn " + basevVecs_vn[3].x + " " + basevVecs_vn[3].y + " " + basevVecs_vn[3].z + "\n");
            sb.Append("vn " + basevVecs_vn[4].x + " " + basevVecs_vn[4].y + " " + basevVecs_vn[4].z + "\n");
            sb.Append("vn " + basevVecs_vn[5].x + " " + basevVecs_vn[5].y + " " + basevVecs_vn[5].z + "\n");

            sb.Append("usemtl " + matName + "\n");

            sb.Append("f " + basevVecs_f[0] + "\n");
            sb.Append("f " + basevVecs_f[1] + "\n");
            sb.Append("f " + basevVecs_f[2] + "\n");
            sb.Append("f " + basevVecs_f[3] + "\n");
            sb.Append("f " + basevVecs_f[4] + "\n");
            sb.Append("f " + basevVecs_f[5] + "\n");
            sb.Append("f " + basevVecs_f[6] + "\n");
            sb.Append("f " + basevVecs_f[7] + "\n");
            sb.Append("f " + basevVecs_f[8] + "\n");
            sb.Append("f " + basevVecs_f[9] + "\n");
            sb.Append("f " + basevVecs_f[10] + "\n");
            sb.Append("f " + basevVecs_f[11] + "\n");

            // cubeData から obj ファイルを生成する
            foreach (var data in cubeData)
            {
                /*
                data.pos;
                data.color;
                data.mesh.GetVerts;
                */
            }
            Log.Info(sb.ToString());

            Platform.FilePicker(PickerMode.Save, file =>
                {
                    Platform.WriteFile(file + ".obj", sb.ToString());
                }, null, ".obj");
        }

    } // class BoxelPaintApp
} // namespace StereoKitSample.BoxelPaint
