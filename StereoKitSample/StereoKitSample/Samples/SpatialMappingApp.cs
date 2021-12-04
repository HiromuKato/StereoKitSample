using StereoKit;

namespace StereoKitSample.Samples
{
    /// <summary>
    /// 空間マッピングを表示するサンプル
    /// （Package.appxmanifest の 機能 > 空間認識 にチェックを入れる）
    /// </summary>
    class SpatialMappingApp : IApp
    {
        Pose windowPose = new Pose(Vec3.Forward, Quat.LookDir(-Vec3.Forward));
        Material occlusionMaterial;
        Material oldMaterial;
        bool settingsWireframe = true;
        int settingsColor = 0;

        public void Initialize()
        {
            // デフォルトのマテリアルは不透明なブラックのため、
            // わかりやすいマテリアルを設定する
            occlusionMaterial = Default.Material.Copy();
            occlusionMaterial[MatParamName.ColorTint] = new Color(1, 0, 0);
            occlusionMaterial.Wireframe = true;

            oldMaterial = World.OcclusionMaterial;
            World.OcclusionMaterial = occlusionMaterial;
        }

        public void Update()
        {
            UI.WindowBegin("Settings", ref windowPose, Vec2.Zero);
            {
                // ワールドオクルージョン（空間マッピング）に対応しているかどうか
                if (SK.System.worldOcclusionPresent)
                {
                    bool occlusion = World.OcclusionEnabled;

                    // オクルージョンの有効・無効をトグルで設定
                    if (UI.Toggle("Enable Occlusion", ref occlusion))
                    {
                        World.OcclusionEnabled = occlusion;
                    }

                    // ワイヤフレーム表示にするかどうかをトグルで設定
                    if (UI.Toggle("Wireframe", ref settingsWireframe))
                    {
                        occlusionMaterial.Wireframe = settingsWireframe;
                    }

                    // マテリアルのカラーをラジオボタンで変更
                    if (UI.Radio("Red", settingsColor == 0))
                    {
                        settingsColor = 0;
                        occlusionMaterial.SetColor("color", Color.HSV(0, 1, 1));
                    }
                    UI.SameLine();
                    if (UI.Radio("White", settingsColor == 1))
                    {
                        settingsColor = 1;
                        occlusionMaterial.SetColor("color", Color.White);
                    }
                    UI.SameLine();
                    if (UI.Radio("Black", settingsColor == 2))
                    {
                        settingsColor = 2;
                        occlusionMaterial.SetColor("color", Color.BlackTransparent);
                    }
                }
                else
                {
                    UI.Label("World occlusion isn't available on this system");
                }

                UI.HSeparator();
                if (SK.System.worldRaycastPresent)
                {
                    // ワールドオクルージョン（空間マッピング）に対応している場合、
                    // レイキャストを有効・無効をトグルで設定
                    bool raycast = World.RaycastEnabled;
                    if (UI.Toggle("Enable Raycast", ref raycast))
                    {
                        World.RaycastEnabled = raycast;
                    }
                }
                else
                {
                    UI.Label("World raycasting isn't available on this system");
                }
            }
            UI.WindowEnd();

            // ハンドレイと空間メッシュがヒットした場所に球を描画する
            for (int i = 0; i < 2; i++)
            {
                Hand hand = Input.Hand(i);
                if (!hand.IsTracked)
                {
                    continue;
                }

                Ray fingerRay = hand[FingerId.Index, JointId.Tip].Pose.Ray;
                if (World.Raycast(fingerRay, out Ray at))
                {
                    Mesh.Sphere.Draw(Material.Default, Matrix.TS(at.position, 0.03f), new Color(1, 0, 0));
                }
            }
        }

        public void Shutdown()
        {
            // 元のマテリアルに戻す
            World.OcclusionMaterial = oldMaterial;
        }
    }
}
