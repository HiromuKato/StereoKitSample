#if WINDOWS_UWP
using System;
using System.Collections.Generic;
using Microsoft.MixedReality.QR;
using StereoKit;

namespace StereoKitSample.Samples
{
    /// <summary>
    /// QRコードトラッキングのサンプル
    ///   - UWPアプリのみで動作
    ///   - Package.appxmanifest の 機能 > Webカメラ にチェックを入れる
    ///   - Microsoft.MixedReality.QR の NuGet パッケージをインストールする
    ///     https://www.nuget.org/Packages/Microsoft.MixedReality.QR
    /// </summary>
    class QRCodeApp : IApp
    {
        struct QRData
        {
            public Pose pose;
            public float size;
            public string text;
            public static QRData FromCode(QRCode qr)
            {
                QRData result = new QRData();
                // ポーズの取得に失敗することがあるが珍しいことではない
                // 特に最初のフレームで見られる
                World.FromSpatialNode(qr.SpatialGraphNodeId, out result.pose);
                result.size = qr.PhysicalSideLength;
                result.text = qr.Data == null ? "" : qr.Data;
                return result;
            }
        }

        QRCodeWatcher watcher;
        DateTime watcherStart;
        Dictionary<Guid, QRData> poses = new Dictionary<Guid, QRData>();

        public void Initialize()
        {
            // QRコード追跡システムの使用許可を求める
            var status = QRCodeWatcher.RequestAccessAsync().Result;
            if (status != QRCodeWatcherAccessStatus.Allowed)
            {
                return;
            }

            // ウォッチャーを設定し、QRコードイベントをリッスンする
            watcherStart = DateTime.Now;
            watcher = new QRCodeWatcher();

            // QRコードが追加（認識）されたときのイベントを設定
            watcher.Added += (o, qr) =>
            {
                // QRCodeWatcher はセッション開始前の QR コードを提供するため、
                // 多くの場合、それらを除外する必要がある
                if (qr.Code.LastDetectedTime > watcherStart)
                {
                    poses.Add(qr.Code.Id, QRData.FromCode(qr.Code));
                }
            };

            // QRコードが更新されたときのイベントを設定
            watcher.Updated += (o, qr) => poses[qr.Code.Id] = QRData.FromCode(qr.Code);

            // QRコードが削除されたときのイベントを設定
            watcher.Removed += (o, qr) => poses.Remove(qr.Code.Id);

            // QRコードトラッキングを開始する
            watcher.Start();
        }

        public void Update()
        {
            // 認識されたQRコードの座標軸・文字列を表示する
            foreach (QRData d in poses.Values)
            {
                Lines.AddAxis(d.pose, d.size);
                Text.Add(
                    d.text,
                    d.pose.ToMatrix(),
                    Vec2.One * d.size,
                    TextFit.Squeeze,
                    TextAlign.XLeft | TextAlign.YTop,
                    TextAlign.Center,
                    d.size, d.size);
            }
        }

        public void Shutdown()
        {
            // QRコードトラッキングを終了する
            watcher?.Stop();
        }
    }
}
#endif
