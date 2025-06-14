> [!CAUTION]
> [UniVerseKit](https://github.com/chinpanGX/UniVerseKit)へ、本機能を統合しました。こちらの利用を推奨しています。


# AudioService

# 概要
Unity標準の最低限のオーディオ機能を提供するパッケージ

# セットアップ手順
**Addressables**の導入が必須

以下のパッケージの導入手順は、各リポジトリのREADME.mdを参照してください。

[AppCore](https://github.com/chinpanGX/AppCore)

[AppService](https://github.com/chinpanGX/AppService)

```
https://github.com/chinpanGX/AudioService.git?path=AudioServiceUnityProject/Assets/src/AudioService
```

# 提供している機能
1. Labelが **"Audio"** の Addressablesに登録してあるAudioClipを一括でロードをする
2. BGMの再生、停止
3. SEの再生
4. マスター、BGM、SEのボリューム変更
5. ボリュームの値をPlayerPrefsに保存する
6. AppServiceのCustomButtonを拡張して、ボタンタップ時に任意のSEを再生する
