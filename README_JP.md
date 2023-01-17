# Lucid Audio
Simple audio player for Unity

<img src="https://github.com/AnnulusGames/LucidAudio/blob/main/Assets/LucidAudio/Documentation~/Header.png" width="800">

[![license](https://img.shields.io/badge/LICENSE-MIT-green.svg)](LICENSE)

[English README](README.md)

## 概要
Lucid Audioは、Unityにおけるオーディオの再生を扱うライブラリです。音の再生や停止、コールバックの設定などをメソッドチェーンを用いて簡単に記述することができます。

### 特徴
* メソッドチェーンを用いたシンプルな記述方式
* 遅延やコールバックなどの高度な設定
* 再生・停止のタイミングをGameObjectに紐付け可能
* プレイリストの作成

## セットアップ

### 要件
* Unity 2019.4 以上

### インストール
1. Window > Package ManagerからPackage Managerを開く
2. 「+」ボタン > Add package from git URL
3. 以下を入力する
   * https://github.com/AnnulusGames/LucidAudio.git?path=/Assets/LucidAudio


あるいはPackages/manifest.jsonを開き、dependenciesブロックに以下を追記

```json
{
    "dependencies": {
        "com.annulusgames.lucid-audio": "https://github.com/AnnulusGames/LucidAudio.git?path=/Assets/LucidAudio"
    }
}
```

### Namespace
Lucid Audioを利用する場合は、ファイルの冒頭に以下の一行を追加します。

```cs
using AnnulusGames.LucidTools.Audio;
```

## 基本的な使い方

オーディオの再生を行う際には、LucidAudioクラスを利用します。

```cs
public Audioclip clip;

private void Start()
{
    // 効果音を再生
    LucidAudio.PlaySE(clip);
}
```

遅延やコールバックなどの設定を行いたい場合は、メソッドチェーンを用いて記述します。

```cs
// 1秒間待機した後にVolume0.7でBGMを再生、終了時に"Complete!"と表示
LucidAudio.PlayBGM(clip)
    .SetVolume(0.7f)
    .SetDelay(1f)
    .OnComplete(() => Debug.Log("Complete!"));
```

これらのメソッドはAudioPlayerクラスを戻り値に持ちます。オーディオの一時停止や、再生中に設定の変更を行いたい場合には、AudioPlayerのメソッドを呼び出します。

```cs
AudioPlayer player = LucidAudio.PlaySE(clip);

// オーディオの一時停止・再開
player.Pause();
player.UnPause();

// オーディオを初めから再生
player.Restart();
```

音量をフェードさせたい場合はFadeVolumeを用いるか、再生・停止時に引数を追加します。

```cs
// 最初の1秒間をフェードさせる
AudioPlayer player = LucidAudio.PlayBGM(clip, 1f);

// 音量を0.7に1秒間かけて変化させる
player.FadeVolume(0.7f, 1f);

// 2秒間でフェードアウトした後に停止
player.Stop(2f);
```

### 制約
AudioPlayerはStopを呼ぶと内部のAudioSourceを破棄するため、停止後に再び利用することはできません。停止したAudioPlayerに再びPlayやRestartを呼ぼうとするとInvalidOperationExceptionをスローします。

```cs
AudioPlayer player = LucidAudio.PlayBGM(clip);
// オーディオの再生を停止
player.Stop();

// InvalidOperationException: A stopped AudioPlayer is not allowed to play again. You need to create a new AudioPlayer.
player.Restart();
```

一度停止したAudioPlayerを再び再生したい場合は、StopではなくPauseを利用します。

また、デフォルトの設定では終了時に自動でStopが呼ばれます。SetAutoStopをfalseにすることで、再生終了時にAudioPlayerを再生前の状態に戻すように設定できます。

```cs
AudioPlayer player = LucidAudio.PlayBGM(clip)
    .SetAutoPause(false);

// 終了後でもPlayやUnPause、Restartが使用可能
player.Play();

// 自動で破棄されないため、終了時には明示的にStopを呼び出す
player.Stop();
```

## コールバック
LucidAudioでは、メソッドを利用することで簡単にコールバックを設定できます。
以下は、AudioPlayerに設定可能なコールバックの一覧です。

### OnStart
オーディオが再生を開始した瞬間に呼び出されます。SetDelayで遅延が設定されている場合は、遅延が終了したタイミングで呼ばれます。

### OnPlay
オーディオが再生中の間、毎フレーム呼び出されます。一時停止中の間には呼び出されません。

### OnPause
オーディオが一時停止した際に呼び出されます。

### OnComplete
オーディオの再生が完了した際に呼び出されます。Stopで再生を停止した場合には呼び出されません。

### OnStop
オーディオが停止した際に呼び出されます。SetAutoStopが設定されていない場合には、完了時にも呼び出されます。

## 3Dサウンド
SetSpatialBlendで値を設定することで、音源との距離を考慮したオーディオの再生を行うことができます。

```cs
// Spatial Blendを1に設定
LucidAudio.PlaySE(clip)
    .SetSpatialBlend(1f);
```

音源の位置を指定する場合は、SetPositionを利用します。

```cs
// (1, 2, 3)の位置で効果音を再生
LucidAudio.PlaySE(clip)
    .SetSpatialBlend(1f)
    .SetPosition(new Vector3(1f, 2f, 3f));
```

Rolloff ModeやMax Distanceなどのパラメーターも、メソッドを利用して設定することができます。

```cs
LucidAudio.PlaySE(clip)
    .SetSpatialBlend(1f)
    .SetPosition(new Vector3(1f, 2f, 3f))
    .SetMaxDistanc(10f)
    .SetRolloffMode(AudioRolloffMode.Logarithmic);
```

## グループ化
AudioPlayerにIDを設定することで、複数のAudioPlayerをまとめて扱うことができます。
IDを設定するには、SetIDを利用します。

```cs
// AudioPlayerにIDを設定
LucidAudio.PlaySE(clip)
    .SetID("GroupName");
```

IDを設定したオーディオをまとめて操作したい場合は、LucidAudioクラスのメソッドの引数にIDを追加します。

```cs
// IDが"GroupName"に設定されているAudioPlayerを全て停止させる
LucidAudio.StopAll("GroupName");
```

また、GetPlayersByIDを利用することで、IDが一致する全てのAudioPlayerを取得できます。

```cs
// IDが"GroupName"に設定されている全てのAudioPlayerを取得
AudioPlayer[] players = LucidAudio.GetPlayersByID("GroupName");

// Volumeを0.5に設定
foreach (AudioPlayer player in players)
{
    player.SetVolume(0.5f);
}
```

## SetLink
SetLinkを用いることで、オーディオの再生・停止のタイミングをGameObjectの状態に紐付けることが可能です。

```cs
// gameObjectが破棄されたタイミングでStopを呼び出す
LucidAudio.PlayBGM(clip)
    .SetLink(gameObject);
```

引数にAudioLinkBehaviourを指定することで、細かい挙動を設定することができます。(どのオプションを指定しても、OnDestroy時にはStopが呼ばれます。)

```cs
// 非アクティブ時にポーズ、アクティブ時に再生する
LucidAudio.PlayBGM(clip)
    .SetLink(gameObject, AudioLinkBehaviour.PauseOnDisableUnPauseOnEnable);
```

利用可能なオプションは以下の通りです。
* StopOnDestroy
* StopOnDisable
* PlayOnEnable
* RestartOnEnable
* PauseOnDisable
* PauseOnDisableUnPauseOnEnable
* PauseOnDisableRestartOnEnable

## コルーチン・非同期処理
コルーチンやasync/awaitを利用してオーディオの終了を待機することも可能です。
AudioPlayerをコルーチンで利用するには、WaitForCompletionを利用します。

```cs
IEnumerator Coroutine()
{
    // 再生終了まで待機
    yield return LucidAudio.PlaySE(clip).WaitForCompletion();
}
```

WaitForCompletionAsyncを利用することで、Taskに変換することも可能です。

```cs
async void MethodAsync()
{
    // Taskに変換、再生終了まで待機
    await LucidAudio.PlaySE(clip).WaitForCompletionAsync();
}
```

### UniTask

LucidAudioはUniTaskに対応しています。Package ManagerからプロジェクトにUniTaskを導入することで、ToUniTaskが利用可能になります。

```cs
async UniTask MethodAsync(CancellationToken token = default)
{
    try
    {
        // UniTaskに変換、再生終了まで待機
        await LucidAudio.PlaySE(clip).ToUniTask(cancellationToken: token);
    }
    catch (OperationCanceledException ex)
    {
        Debug.Log("Canceled");
    }
}
```

また、引数にAudioCancelBehaviourを指定することでキャンセル時の挙動を設定できます。(デフォルトではStopが設定されています)

```cs
async UniTask MethodAsync(CancellationToken token = default)
{
    try
    {
        // キャンセル時にAudioPlayerを一時停止
        await LucidAudio.PlaySE(clip).ToUniTask(AudioCancelBehaviour.Pause, cancellationToken: token);
    }
    catch (OperationCanceledException ex)
    {
        Debug.Log("Canceled");
    }
}
```

## Playlist
Playlistクラスを利用することで、複数のサウンドをまとめたプレイリストを作成できます。PlaylistはAudioclipを要素として持ち、通常のリストのように要素の追加・削除を行うことができます。

```cs
public AudioClip clip;

private void Start()
{
    // 新たなプレイリストを作成
    Playlist playlist = new Playlist();

    // 要素を追加
    playlist.Add(clip);

    // プレイリストを再生
    playlist.Play();
}
```

再生を一時停止・再開したい場合はPause・UnPauseを利用します。

```cs
// プレイリストの再生を一時停止
playlist.Pause();

// プレイリストの再生を再開
playlist.UnPause();
```

サウンドをスキップしたり、前のサウンドを再生したい場合にはPlayNext・PlayPreviousを利用します。

```cs
// 前のサウンドを再生 (再生していない場合は最初から再生)
playlist.PlayPrevious();

// 次のサウンドを再生 (次のサウンドが存在しない場合は停止)
playlist.PlayNext();
```

シャッフル再生を行うにはPlayShuffleを利用します。サウンドの再生順はPlayShuffleが呼ばれた時点で設定され、再びPlay・PlayShuffleが呼ばれるまでは、PlayNextやPlayPreviousを呼んでも順番は変わりません。

```cs
// シャッフル再生
playlist.PlayShuffle();
```

playerプロパティから現在Playlistが保持するAudioPlayerを取得できます。
Playlistは再生を開始するたびにAudioPlayerを生成し、再生が終了するとそれを破棄します。

```cs
playlist.player.SetVolume(0.5f);
```

AudioTypeを変更することで、BGM・SEのどちらとしてサウンドを扱うかを設定できます。

```cs
// LucidAudio.PlaySEを利用してサウンドを再生する
playlist.audioType = AudioType.SE;
```

また、PlaylistはInspector上から編集が可能です。

<img src="https://github.com/AnnulusGames/LucidAudio/blob/main/Assets/LucidAudio/Documentation~/img1.png" width="500">

## ライセンス

[Mit License](LICENSE)


