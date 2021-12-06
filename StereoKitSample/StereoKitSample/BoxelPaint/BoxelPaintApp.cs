using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
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
                //Text.Add("グリッド原点", Matrix.R(0, 180, 0));

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
                    LoadDataFromText(text);
                }
            }, null, ".bxm");
        }

        CubeData tmpCubeData = null;
        private void LoadDataFromText(string text)
        {
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
            cubeData.Clear();

            var lines = text.Split("\n");

            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0)
                {
                    if (lines[i] != "BoxelModeler")
                    {
                        // 不正なファイル
                        Log.Err("Invalid file!");
                        return;
                    }
                    continue;
                }

                var line = lines[i];
                var words = line.Split(' ');
                if (words[0] == "p")
                {
                    // データ生成
                    tmpCubeData = new CubeData();

                    float x = float.Parse(words[1]);
                    float y = float.Parse(words[2]);
                    float z = float.Parse(words[3]);
                    tmpCubeData.pos = new Vec3(x, y, z);
                }
                else if (words[0] == "c")
                {
                    if (tmpCubeData == null) continue;

                    float r = float.Parse(words[1]);
                    float g = float.Parse(words[2]);
                    float b = float.Parse(words[3]);
                    float a = float.Parse(words[4]);
                    tmpCubeData.color = new Color(r, g, b, a);

                    // Cubeメッシュ生成
                    var mesh = Mesh.GenerateCube(Vec3.One * 5 * U.cm);
                    var colorMat = Default.Material.Copy();
                    colorMat[MatParamName.ColorTint] = tmpCubeData.color;
                    tmpCubeData.material = colorMat;
                    tmpCubeData.mesh = mesh;
                    cubeData.Add(tmpCubeData);

                    float x = (tmpCubeData.pos.x - 2.5f * U.cm) / (5 * U.cm);
                    float y = (tmpCubeData.pos.y - 2.5f * U.cm) / (5 * U.cm);
                    float z = (tmpCubeData.pos.z - 2.5f * U.cm) / (5 * U.cm);
                    painted[(int)x, (int)y, (int)z] = true;

                    tmpCubeData = null;
                }
            }
        }

        /// <summary>
        /// データを保存する
        /// </summary>
        /// <param name="value"></param>
        private void OnSaveData(string value)
        {
            /*
            Platform.FilePicker(PickerMode.Save, file =>
            {
                StringBuilder sb = new StringBuilder("BoxelModeler\n");

                foreach (var data in cubeData)
                {
                    sb.Append("p " + data.pos.x + " " + data.pos.y + " " + data.pos.z + "\n");
                    sb.Append("c " + data.color.r + " " + data.color.g + " " + data.color.b + " " + data.color.a + "\n");
                }

                Platform.WriteFile(file + ".bxm", sb.ToString());
            }, null, ".bxm"); // 中身はテキストだがtxtと見分けるため拡張子は独自のもの
            */

            // 上記の処理でセーブできない（OSの不具合？）ため以下暫定処理
            StringBuilder sb = new StringBuilder("BoxelModeler\n");
            foreach (var data in cubeData)
            {
                sb.Append("p " + data.pos.x + " " + data.pos.y + " " + data.pos.z + "\n");
                sb.Append("c " + data.color.r + " " + data.color.g + " " + data.color.b + " " + data.color.a + "\n");
            }

            SaveTextFileForUWP(".bxm", sb.ToString());
        }

        private void SaveTextFileForUWP(string ext, string value, string filename = null)
        {
#if WINDOWS_UWP
            try
            {
                Task.Run(async () =>
                {
                    // アプリ内に保存
                    if (filename == null)
                    {
                        DateTime dt = DateTime.Now;
                        filename = dt.ToString($"{dt:yyyyMMddHHmmss}");
                    }
                    var storageFolder = ApplicationData.Current.LocalFolder;
                    var file = await storageFolder.CreateFileAsync(filename + ext, CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(file, value);

                    // アプリ内のデータをDocuments/BoxelModelerにコピー
                    var documentsFolder = KnownFolders.DocumentsLibrary;
                    var files = await storageFolder.GetFilesAsync();
                    var targetFolder = await ((StorageFolder)documentsFolder).CreateFolderAsync("BoxelModeler", CreationCollisionOption.ReplaceExisting);
                    foreach (var f in files)
                    {
                        await f.CopyAsync(targetFolder);
                    }
                });
            }
            catch (Exception ex)
            {
            }
#endif
        }

        /// <summary>
        /// objフォーマットのデータをエクスポートする
        /// </summary>
        private void OnExportObj()
        {
            // Cubeの形状しかないため簡易的なエクスポート処理
            // 一般的なobjのエクスポートで利用できるものではない
            Vec3[] baseVecs_v = new Vec3[]
            {
                // 以下の場合cubeの原点は(0, 0, 0)だが、
                // アプリ上で描画したモデル座標の原点に一番近いcubeの原点は(0.025, 0.025, 0.025)なので
                // エクスポートしたモデルは位置が異なっている（がこのままとしておく）
                new Vec3( -1.000000f, -1.000000f,  1.000000f ),
                new Vec3( -1.000000f,  1.000000f,  1.000000f ),
                new Vec3( -1.000000f, -1.000000f, -1.000000f ),
                new Vec3( -1.000000f,  1.000000f, -1.000000f ),
                new Vec3(  1.000000f, -1.000000f,  1.000000f ),
                new Vec3(  1.000000f,  1.000000f,  1.000000f ),
                new Vec3(  1.000000f, -1.000000f, -1.000000f ),
                new Vec3(  1.000000f,  1.000000f, -1.000000f ),
            };

            Vec3[] baseVecs_vn = new Vec3[]
            {
                new Vec3(-1.0000f, 0.0000f, 0.0000f),
                new Vec3(0.0000f, 0.0000f, -1.0000f),
                new Vec3(1.0000f, 0.0000f, 0.0000f),
                new Vec3(0.0000f, 0.0000f, 1.0000f),
                new Vec3(0.0000f, -1.0000f, 0.0000f),
                new Vec3(0.0000f, 1.0000f, 0.0000f)
            };

            Vec3[] baseVecs_fv = new Vec3[]
            {
                new Vec3(2, 3, 1),
                new Vec3(4, 7, 3),
                new Vec3(8, 5, 7),
                new Vec3(6, 1, 5),
                new Vec3(7, 1, 3),
                new Vec3(4, 6, 8),
                new Vec3(2, 4, 3),
                new Vec3(4, 8, 7),
                new Vec3(8, 6, 5),
                new Vec3(6, 2, 1),
                new Vec3(7, 5, 1),
                new Vec3(4, 2, 6),
            };
            Vec3[] baseVecs_fn = new Vec3[]
{
                new Vec3(1, 1, 1),
                new Vec3(2, 2, 2),
                new Vec3(3, 3, 3),
                new Vec3(4, 4, 4),
                new Vec3(5, 5, 5),
                new Vec3(6, 6, 6),
                new Vec3(1, 1, 1),
                new Vec3(2, 2, 2),
                new Vec3(3, 3, 3),
                new Vec3(4, 4, 4),
                new Vec3(5, 5, 5),
                new Vec3(6, 6, 6),
            };

            string filename = "color";
            int index = 0;
            string matName = "WhiteMaterial";

            StringBuilder sb = new StringBuilder("# Exported from BoxelModeler\n");
            sb.Append("mtllib " + filename + ".mtl\n");
            sb.Append("o Boxels\n");

            // cubeData から obj ファイルを生成する
            foreach (var data in cubeData)
            {
                sb.Append(Calc_v(baseVecs_v[0], data.pos, index));
                sb.Append(Calc_v(baseVecs_v[1], data.pos, index));
                sb.Append(Calc_v(baseVecs_v[2], data.pos, index));
                sb.Append(Calc_v(baseVecs_v[3], data.pos, index));
                sb.Append(Calc_v(baseVecs_v[4], data.pos, index));
                sb.Append(Calc_v(baseVecs_v[5], data.pos, index));
                sb.Append(Calc_v(baseVecs_v[6], data.pos, index));
                sb.Append(Calc_v(baseVecs_v[7], data.pos, index));

                sb.Append("vn " + baseVecs_vn[0].x + " " + baseVecs_vn[0].y + " " + baseVecs_vn[0].z + "\n");
                sb.Append("vn " + baseVecs_vn[1].x + " " + baseVecs_vn[1].y + " " + baseVecs_vn[1].z + "\n");
                sb.Append("vn " + baseVecs_vn[2].x + " " + baseVecs_vn[2].y + " " + baseVecs_vn[2].z + "\n");
                sb.Append("vn " + baseVecs_vn[3].x + " " + baseVecs_vn[3].y + " " + baseVecs_vn[3].z + "\n");
                sb.Append("vn " + baseVecs_vn[4].x + " " + baseVecs_vn[4].y + " " + baseVecs_vn[4].z + "\n");
                sb.Append("vn " + baseVecs_vn[5].x + " " + baseVecs_vn[5].y + " " + baseVecs_vn[5].z + "\n");

                matName = GetMaterialName(data.color);
                sb.Append("usemtl " + matName + "\n");
                sb.Append("s off\n");

                sb.Append(Calc_f(baseVecs_fv[0], baseVecs_fn[0], index));
                sb.Append(Calc_f(baseVecs_fv[1], baseVecs_fn[1], index));
                sb.Append(Calc_f(baseVecs_fv[2], baseVecs_fn[2], index));
                sb.Append(Calc_f(baseVecs_fv[3], baseVecs_fn[3], index));
                sb.Append(Calc_f(baseVecs_fv[4], baseVecs_fn[4], index));
                sb.Append(Calc_f(baseVecs_fv[5], baseVecs_fn[5], index));
                sb.Append(Calc_f(baseVecs_fv[6], baseVecs_fn[6], index));
                sb.Append(Calc_f(baseVecs_fv[7], baseVecs_fn[7], index));
                sb.Append(Calc_f(baseVecs_fv[8], baseVecs_fn[8], index));
                sb.Append(Calc_f(baseVecs_fv[9], baseVecs_fn[9], index));
                sb.Append(Calc_f(baseVecs_fv[10], baseVecs_fn[10], index));
                sb.Append(Calc_f(baseVecs_fv[11], baseVecs_fn[11], index));
                index++;
            }

            Log.Info(sb.ToString());

            /*
            Platform.FilePicker(PickerMode.Save, file =>
            {
                Platform.WriteFile(file + ".obj", sb.ToString());
            }, null, ".obj");
            */

            // 上記の処理でセーブできない（OSの不具合？）ため以下暫定処理
            SaveTextFileForUWP(".obj", sb.ToString());

            // マテリアルファイルの出力
            StringBuilder matsb = new StringBuilder();
            matsb.Append("newmtl WhiteMaterial\n");
            matsb.Append("Kd 1.000000 1.000000 1.000000\n");
            matsb.Append("newmtl RedMaterial\n");
            matsb.Append("Kd 1.000000 0.000000 0.000000\n");
            matsb.Append("newmtl GreenMaterial\n");
            matsb.Append("Kd 0.000000 1.000000 0.000000\n");
            matsb.Append("newmtl BlueMaterial\n");
            matsb.Append("Kd 0.000000 0.000000 1.000000\n");
            SaveTextFileForUWP(".mtl", matsb.ToString(), "color");
        }

        private string Calc_v(Vec3 v, Vec3 pos, int index)
        {
            v = v * 2.5f * U.cm;
            float x = (pos.x - 2.5f * U.cm) / (5 * U.cm);
            float y = (pos.y - 2.5f * U.cm) / (5 * U.cm);
            float z = (pos.z - 2.5f * U.cm) / (5 * U.cm);

            var str = "v " + (v.x + x * 5 * U.cm) + " " + (v.y + y * 5 * U.cm) + " " + (v.z + z * 5 * U.cm) + "\n";
            return str;
        }

        private string Calc_f(Vec3 fv, Vec3 fn, int index)
        {
            var str = "f " +
                (int)(fv.x + index * 8) + "//" + (int)(fn.x + index * 6) + " " +
                (int)(fv.y + index * 8) + "//" + (int)(fn.y + index * 6) + " " +
                (int)(fv.z + index * 8) + "//" + (int)(fn.z + index * 6) + "\n";
            return str;
        }

        private string GetMaterialName(Color color)
        {
            if (color.r == 1 && color.g == 1 && color.b == 1)
            {
                return "WhiteMaterial";
            }
            else if (color.r == 1 && color.g == 0 && color.b == 0)
            {
                return "RedMaterial";
            }
            else if (color.r == 0 && color.g == 1 && color.b == 0)
            {
                return "GreenMaterial";
            }
            else if (color.r == 0 && color.g == 0 && color.b == 1)
            {
                return "BlueMaterial";
            }
            return "WhiteMaterial";
        }

    } // class BoxelPaintApp
} // namespace StereoKitSample.BoxelPaint
