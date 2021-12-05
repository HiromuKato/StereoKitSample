using StereoKit;

namespace StereoKitSample.Samples
{
    /// <summary>
    /// ポインターのサンプル
    /// </summary>
    public class PointerApp : IApp
    {
        public void Initialize()
        {
        }

        public void Update()
        {
            DrawHandPointers();
        }

        public void Shutdown()
        {
        }

        /// <summary>
        /// 手からポインターを表示する
        /// </summary>
        private void DrawHandPointers()
        {
            int hands = Input.PointerCount(InputSource.Hand);
            for (int i = 0; i < hands; i++)
            {
                Pointer pointer = Input.Pointer(i, InputSource.Hand);
                // 長さ 50cm, 太さ1cm の線を描画する
                Lines.Add(pointer.ray, 0.5f, Color.White, Units.mm2m);
                // ポインターのポーズを示す座標軸を描画する
                Lines.AddAxis(pointer.Pose);
            }
        }
    }
}
