using StereoKit;

namespace StereoKitSample.Samples
{
    /// <summary>
    /// ファイルの保存・読み込みを行うサンプル
    /// </summary>
    class FilePickerApp : IApp
    {
        private Pose menuPose = new Pose(0, 0, -0.5f, Quat.LookDir(0, 0, 1));

        public void Initialize()
        {
        }

        public void Update()
        {
            UI.WindowBegin("Menu", ref menuPose, UIWin.Normal);
            {
                if (UI.Button("Save"))
                {
                    OnSaveData();
                }
                UI.SameLine();
                if (UI.Button("Load") && !Platform.FilePickerVisible)
                {
                    OnLoadData();
                }
            }
            UI.WindowEnd();
        }

        public void Shutdown()
        {
        }

        /// <summary>
        /// データを読み込む
        /// </summary>
        private void OnLoadData()
        {
            Platform.FilePicker(PickerMode.Open, file =>
            {
                if (Platform.ReadFile(file, out string text))
                {
                    Log.Info(text);
                }
            }, null, ".txt");
        }

        /// <summary>
        /// データを保存する
        /// </summary>
        private void OnSaveData()
        {
            var outputText = "Hello World!";

            Platform.FilePicker(PickerMode.Save, file =>
            {
                Platform.WriteFile(file, outputText);
            }, null, ".txt");
        }
    }
}
