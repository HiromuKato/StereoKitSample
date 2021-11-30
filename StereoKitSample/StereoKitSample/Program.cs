using StereoKit;
using System;

namespace StereoKitSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // StereoKit の初期化
            SKSettings settings = new SKSettings
            {
                // アプリ名
                appName = "StereoKitSample",
                // アセットフォルダの相対パス（StereoKit はこの配下のアセットを探しに行く）
                assetsFolder = "Assets",
            };

            // 初期化に失敗した場合はアプリ終了
            if (!SK.Initialize(settings))
            {
                Environment.Exit(1);
            }

            // アプリで利用するアセットを生成する
            // キューブ
            Pose cubePose = new Pose(0, 0, -0.5f, Quat.Identity);
            Model cube = Model.FromMesh(
                Mesh.GenerateRoundedCube(Vec3.One * 0.1f, 0.02f),
                Default.MaterialUI);
            // 床
            Matrix floorTransform = Matrix.TS(0, -1.5f, 0, new Vec3(30, 0.1f, 30));
            Material floorMaterial = new Material(Shader.FromFile("floor.hlsl"));
            floorMaterial.Transparency = Transparency.Blend;

            // アプリのメインループ
            // ここのコールバックが入力・システムイベントの後、描画イベントの前に毎フレーム呼ばれる
            while (SK.Step(() =>
            {
                // Esc キーでアプリ終了
                if (Input.Key(Key.Esc).IsJustActive())
                {
                    SK.Quit();
                }

                // ディスプレイタイプが Opaque(VR ヘッドセットや PC) の場合は床を描画する
                if (SK.System.displayType == Display.Opaque)
                {
                    Default.MeshCube.Draw(floorMaterial, floorTransform);
                }

                // UI クラス：ユーザインターフェースとインタラクションメソッドのコレクション
                // Handle メソッド：掴んだり移動する機能の開始・終了をハンドルする（掴んでいる間はtrueを返す）
                UI.Handle("Cube", ref cubePose, cube.Bounds);
                // キューブを描画する
                cube.Draw(cubePose.ToMatrix());
            })) ;

            // アプリ終了処理（StereoKit と全てのリソースをクリーンアップする）
            SK.Shutdown();
        }
    }
}
