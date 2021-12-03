using StereoKit;

namespace StereoKitSample.Samples
{
    /// <summary>
    /// サウンドを再生するサンプル
    /// </summary>
    class SoundApp : IApp
    {
        private Pose menuPose = new Pose(0, 0, -0.5f, Quat.LookDir(0, 0, 1));
        private Sound fileSound = null;
        private Sound genSound = null;

        public void Initialize()
        {
            // ファイルから読み込み
            fileSound = Sound.FromFile("BlipNoise.wav");

            // 440Hz(A) のサイン波生成
            genSound = Sound.Generate((t) =>
            {
                float band = SKMath.Sin(t * 440.00f * SKMath.Tau);
                const float volume = 0.2f;
                return band * volume;
            }, 1);

        }

        public void Update()
        {
            UI.WindowBegin("Menu", ref menuPose, UIWin.Normal);
            {
                if (UI.Button("BlipNoise.wav"))
                {
                    fileSound.Play(menuPose.position);
                }

                if (UI.Button("440 Hz"))
                {
                    genSound.Play(Vec3.Zero);
                }
            }
            UI.WindowEnd();
        }

        public void Shutdown()
        {
        }
    }
}
