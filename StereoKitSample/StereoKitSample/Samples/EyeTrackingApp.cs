using System.Collections.Generic;
using StereoKit;

namespace StereoKitSample.Samples
{
    /// <summary>
    /// アイトラッキングのサンプル
    /// （Package.appxmanifest の 機能 > 視線入力 にチェックを入れる）
    /// </summary>
    public class EyeTrackingApp : IApp
    {
        List<LinePoint> points = new List<LinePoint>();
        Vec3 previous;

        public void Initialize()
        {
        }

        public void Update()
        {
            // レイのあたり判定用の平面
            Plane plane = new Plane(new Vec3(0.5f, 0, -0.5f), V.XYZ(-0.5f, 0, 0.5f));

            // 四角形の描画
            Mesh.Quad.Draw(Material.Default,
                Matrix.TRS(new Vec3(0.54f, 0, -0.468f),
                    Quat.LookDir(plane.normal),
                    0.5f));

            // 視線が平面とヒットした場合
            if (Input.Eyes.Ray.Intersect(plane, out Vec3 at))
            {
                // アイトラッキングがアクティブな場合は緑、アクティブでない場合は赤
                Color stateColor = Input.EyesTracked.IsActive()
                    ? new Color(0, 1, 0)
                    : new Color(1, 0, 0);

                // 視線がヒットした場所に球を描画
                Default.MeshSphere.Draw(Default.Material, Matrix.TS(at, 3 * U.cm), stateColor);

                // 以前の視線のヒット位置と現フレームの視線のヒット位置の距離(の二乗)が一定値以上の場合
                if (Vec3.DistanceSq(at, previous) > U.cm * U.cm)
                {
                    previous = at;
                    // リストに LinePoint(ライン上の点を表すデータ) を追加する
                    points.Add(new LinePoint { pt = at, color = Color.White });

                    // 20個分のデータを保持、古いものから削除
                    if (points.Count > 20)
                    {
                        points.RemoveAt(0);
                    }
                }

                // 上のif文を通らない場合も最新のヒットした点の位置は更新する
                LinePoint pt = points[points.Count - 1];
                pt.pt = at;
                points[points.Count - 1] = pt;
            }

            // 新しいヒット位置になるほど線を太くする処理
            for (int i = 0; i < points.Count; i++)
            {
                LinePoint pt = points[i];
                pt.thickness = (i / (float)points.Count) * 3 * U.cm;
                points[i] = pt;
            }

            // 線を描画
            Lines.Add(points.ToArray());
        }

        public void Shutdown()
        {
        }
    }
}
