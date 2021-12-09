# StereoKitSample

[StereoKit](https://github.com/maluoi/StereoKit) を利用した、部品として利用できそうなアプリのサンプル集です。

## 動作確認方法
Program.cs の以下行を Samples フォルダ配下にある確認したいクラス名に変更してビルドすることで動作確認できます。
```
var app = new FirstApp(); // ★ここを確認したいサンプルのクラスに変更する
```

## サンプル

以下のサンプルを用意しています。StereoKit リポジトリ配下にもデモコンテンツが用意されておりそれらと同等の物も含まれていますが、日本語のコメントを付けたり、より部品として使いやすいように最低限の内容に絞ったりしています。

- [ChangeMaterialApp](./StereoKitSample/StereoKitSample/Samples/ChangeMaterialApp.cs)
  - マテリアルを変更するサンプル

    ![ChangeMaterialApp](./docs/img/ChangeMaterialApp.png)

- [CubeAnimApp](./StereoKitSample/StereoKitSample/Samples/CubeAnimApp.cs)
  - Cubeのアニメーションサンプル

    ![CubeAnimApp](./docs/img/CubeAnimApp.png)

- [DefaultMeshApp](./StereoKitSample/StereoKitSample/Samples/DefaultMeshApp.cs)
  - StereoKit に用意されているメッシュを表示するサンプル

    ![DefaultMeshApp](./docs/img/DefaultMeshApp.png)

- [EyeTrackingApp](./StereoKitSample/StereoKitSample/Samples/EyeTrackingApp.cs)
  - アイトラッキングのサンプル

    ![EyeTrackingApp](./docs/img/EyeTrackingApp.png)

- [FilePickerApp](./StereoKitSample/StereoKitSample/Samples/FilePickerApp.cs)
  - ファイルの保存・読み込みを行うサンプル

    ![FilePickerApp](./docs/img/FilePickerApp.png)

- [FirstApp](./StereoKitSample/StereoKitSample/Samples/FirstApp.cs)
  - プロジェクト作成時に生成されるひな形を別クラスに分けて動作するようにしたもの

    ![FirstApp](./docs/img/FirstApp.png)

- [HandMenuApp](./StereoKitSample/StereoKitSample/Samples/HandMenuApp.cs)
  - ハンドメニューのサンプル

    ![HandMenuApp](./docs/img/HandMenuApp.jpg)

- [HandMenuRadialApp](./StereoKitSample/StereoKitSample/Samples/HandMenuRadialApp.cs)
  - ラジアルハンドメニューのサンプル

    ![HandMenuRadialApp](./docs/img/HandMenuRadialApp.jpg)

- [InstantiateModelApp](./StereoKitSample/StereoKitSample/Samples/InstantiateModelApp.cs)
  - 動的にモデルを生成するサンプル

    ![InstantiateModelApp](./docs/img/InstantiateModelApp.png)

- [MenuApp](./StereoKitSample/StereoKitSample/Samples/MenuApp.cs)
  - メニューの表示と StereoKit で利用可能な UI のサンプル

    ![MenuApp](./docs/img/MenuApp.png)

- [MicrophoneApp](./StereoKitSample/StereoKitSample/Samples/MicrophoneApp.cs)
  - マイクを利用するサンプル

    ![MicrophoneApp](./docs/img/MicrophoneApp.png)

- [ModelApp](./StereoKitSample/StereoKitSample/Samples/ModelApp.cs)
  - 3Dモデルを表示するサンプル

    ![ModelApp](./docs/img/ModelApp.png)

- [PointerApp](./StereoKitSample/StereoKitSample/Samples/PointerApp.cs)
  - ポインターのサンプル

    ![PointerApp](./docs/img/PointerApp.jpg)

- [PoseApp](./StereoKitSample/StereoKitSample/Samples/PoseApp.cs)
  - ヘッド、ハンド、人差し指、オブジェクトの位置・前方ベクトル、マウスの位置を表示するサンプル

    ![PoseApp](./docs/img/PoseApp.png)

- [QRCodeApp](./StereoKitSample/StereoKitSample/Samples/QRCodeApp.cs)
  - QRコードトラッキングのサンプル

    ![QRCodeApp](./docs/img/QRCodeApp.jpg)

- [RayCastApp](./StereoKitSample/StereoKitSample/Samples/RayCastApp.cs)
  - レイキャストのサンプル

    ![RayCastApp](./docs/img/RayCastApp.png)

- [SoundApp](./StereoKitSample/StereoKitSample/Samples/SoundApp.cs)
  - サウンドを再生するサンプル

    ![SoundApp](./docs/img/SoundApp.png)

- [SpatialMappingApp](./StereoKitSample/StereoKitSample/Samples/SpatialMappingApp.cs)
  - 空間メッシュを表示するサンプル

    ![SpatialMappingApp](./docs/img/SpatialMappingApp.jpg)
