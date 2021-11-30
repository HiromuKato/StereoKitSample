using StereoKit;
using System;
using StereoKitSample.Samples;

namespace StereoKitSample
{
    /*
     * PC での操作
     * 参考：https://github.com/maluoi/StereoKit/blob/master/StereoKitC/systems/platform/flatscreen_input.cpp
     *  - マウスの移動：手の上下左右移動
     *  - マウスホイール：手の奥行き移動
     *  - マウスの左クリック：つかむ
     *  - マウスの右クリック：つつく
     *  - Shift(またはCaps Lock) + マウスの右クリック + マウスの移動：カメラの回転
     *  - Shift(またはCaps Lock) + W A S D Q Eキー：カメラの移動
     *  - Alt + マウス操作：アイトラッキングのシミュレート
     */
    class Program
    {
        static void Main(string[] args)
        {
            // StereoKit を初期化する
            SKSettings settings = new SKSettings
            {
                // アプリ名
                appName = "StereoKitSample",
                // アセットフォルダの相対パス（StereoKit はこの配下のアセットを探しに行く）
                assetsFolder = "Assets",
            };

            // 初期化に失敗した場合はアプリ終了する
            if (!SK.Initialize(settings))
            {
                Environment.Exit(1);
            }

            // サンプルクラスの生成
            var app = new FirstApp(); // ★ここを確認したいサンプルのクラスに変更する
            app.Initialize();
            Action step = app.Update;

            // アプリのメインループ
            // ここのコールバックが入力・システムイベントの後、描画イベントの前に毎フレーム呼ばれる
            while (SK.Step(step)) { }

            app.Shutdown();

            // アプリ終了処理（StereoKit と全てのリソースをクリーンアップする）
            SK.Shutdown();
        }
    }
}
