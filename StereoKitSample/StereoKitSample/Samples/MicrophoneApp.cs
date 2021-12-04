using StereoKit;
using System;

namespace StereoKitSample.Samples
{
    /// <summary>
    /// マイクを利用するサンプル
    /// （Package.appxmanifest の 機能 > マイク にチェックを入れる）
    /// </summary>
    class MicrophoneApp : IApp
    {
        private Pose menuPose = new Pose(0.2f, 0, -0.5f, Quat.LookDir(0, 0, 1));
        Sprite micSprite;
        Material micMaterial;
        float[] micBuffer = new float[0];
        float micIntensity = 0;

        public void Initialize()
        {
            micSprite = Sprite.FromFile("mic_icon.png", SpriteType.Single);
            micMaterial = Default.MaterialUnlit.Copy();
            micMaterial.Transparency = Transparency.Blend;

            Microphone.Start();
        }

        public void Update()
        {
            // メニュー
            UI.WindowBegin("Menu", ref menuPose, UIWin.Normal);
            {
                if (!Microphone.IsRecording)
                {
                    if (UI.Button("Start"))
                    {
                        Microphone.Start();
                    }
                }
                else
                {
                    if (UI.Button("Stop"))
                    {
                        Microphone.Stop();
                    }
                }
            }
            UI.WindowEnd();

            if (Microphone.IsRecording)
            {
                // マイクの強さ
                float intensity = GetMicIntensity();
                // 初期応答を早くするための計算
                intensity = 1 - intensity;
                intensity = 1 - (intensity * intensity);

                float scale = 0.1f + 0.06f * intensity;
                Color color = new Color(1, 1, 1, Math.Max(0.1f, intensity));
                Default.MeshSphere.Draw(micMaterial, Matrix.TS(0, 0, -0.5f, scale), color);
                micSprite.Draw(Matrix.TS(-0.03f, 0.03f, -0.5f, 0.06f));
            }
            else
            {
                // レコーディング中でない場合は赤色表示
                Default.MeshSphere.Draw(micMaterial, Matrix.TS(0, 0, -0.5f, 0.1f), new Color(1, 0, 0, 0.1f));
                micSprite.Draw(Matrix.TS(-0.03f, 0.03f, -0.5f, 0.06f));
            }
        }

        public void Shutdown()
        {
            Microphone.Stop();
        }

        /// <summary>
        /// マイクの強さを取得する
        /// </summary>
        /// <returns></returns>
        float GetMicIntensity()
        {
            if (!Microphone.IsRecording)
            {
                return 0;
            }

            // このフレームでマイクのデータをすべて取得できるように、
            // バッファが十分な大きさであることを確認する
            if (Microphone.Sound.UnreadSamples > micBuffer.Length)
            {
                micBuffer = new float[Microphone.Sound.UnreadSamples];
            }

            // マイクストリームからバッファにデータを読み取る
            // 実際にバッファーに書き込まれたサンプル数を返す
            int samples = Microphone.Sound.ReadSamples(ref micBuffer);

            // 過去1000サンプルの累積移動平均
            // オーディオ波形は半分負の値であるため、サンプルの絶対値にする
            for (int i = 0; i < samples; i++)
            {
                micIntensity = (micIntensity * 999.0f + Math.Abs(micBuffer[i])) / 1000.0f;
            }

            return micIntensity;
        }
    }
}
