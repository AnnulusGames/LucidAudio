# Lucid Audio
Simple audio player for Unity

<img src="https://github.com/AnnulusGames/LucidAudio/blob/main/Assets/LucidAudio/Documentation~/Header.png" width="800">

[![license](https://img.shields.io/badge/LICENSE-MIT-green.svg)](LICENSE)

[日本語版READMEはこちら](README_JP.md)

## Overview
Lucid Audio is a library that handles audio playback in Unity. You can easily describe how to play and stop sounds, set callbacks, etc. using method chaining.

### Features
* Simple description method using method chaining
* Advanced settings such as delays and callbacks
* Play/stop timing can be linked to GameObject
* Create playlist

### Requirement
* Unity 2019.4 or higher

### Install
1. Open the Package Manager from Window > Package Manager
2. "+" button > Add package from git URL
3. Enter the following to install
   * https://github.com/AnnulusGames/LucidAudio.git?path=/Assets/LucidAudio


or open Packages/manifest.json and add the following to the dependencies block.

```json
{
    "dependencies": {
        "com.annulusgames.lucid-audio": "https://github.com/AnnulusGames/LucidAudio.git?path=/Assets/LucidAudio"
    }
}
```

### Namespace
When using Lucid Audio, add the following line at the beginning of the file.

```cs
using AnnulusGames.LucidTools.Audio;
```

## Basic Usage

Use LucidAudio class to play audio.

```cs
public Audioclip clip;

private void Start()
{
    // play sound effect
    LucidAudio.PlaySE(clip);
}
```

Use method chaining to set delays, callbacks, etc.

```cs
// After waiting for 1 second, play BGM with Volume 0.7 and display "Complete!"
LucidAudio.PlayBGM(clip)
    .SetVolume(0.7f)
    .SetDelay(1f)
    .OnComplete(() => Debug.Log("Complete!"));
```

These methods have an AudioPlayer class as a return value. If you want to pause the audio or change settings during playback, call AudioPlayer methods.

```cs
AudioPlayer player = LucidAudio.PlaySE(clip);

// pause/resume audio
player.Pause();
player.UnPause();

// play audio from beginning
player.Restart();
```

Use FadeVolume or add an argument when playing/stopping to fade the volume.

```cs

// fade the first second
AudioPlayer player = LucidAudio.PlayBGM(clip, 1f);

// Change volume to 0.7 over 1 second
player.FadeVolume(0.7f, 1f);

// stop after 2 seconds fade out
player.Stop(2f);
```

### Constraint
AudioPlayer dispose its internal AudioSource when you call Stop, so it cannot be used again after stopping. Attempting to call Play or Restart on a stopped AudioPlayer will throw InvalidOperationException.

```cs
AudioPlayer player = LucidAudio.PlayBGM(clip);
// stop playing audio
player.Stop();

// InvalidOperationException: A stopped AudioPlayer is not allowed to play again. You need to create a new AudioPlayer.
player.Restart();
```

If you want to play the stopped AudioPlayer again, use Pause instead of Stop.

Also, in the default setting, Stop is automatically called at the end. By setting SetAutoStop to false, you can set the AudioPlayer to return to its pre-play state when playback ends.

```cs
AudioPlayer player = LucidAudio.PlayBGM(clip)
    .SetAutoPause(false);

// Play, UnPause, and Restart can be used even after termination
player.Play();

// Explicitly call Stop when finished, as it is not automatically disposed
player.Stop();
```

## Callbacks
Lucid Audio makes it easy to set callbacks using methods.
Below is a list of callbacks that can be set on the AudioPlayer.

### OnStart
Called at the moment the audio starts playing. If a delay is set with SetDelay, it will be called when the delay ends.

### OnPlay
Called every frame while audio is playing. Not called while paused.

### OnPause
Called when the audio is paused.

### OnComplete
Called when the audio has finished playing. It is not called if you stop playback with Stop.

### OnStop
Called when the audio has stopped. Also called on completion if SetAutoStop is not set.

## 3D Sound
By setting the value with SetSpatialBlend, you can play audio considering the distance to the sound source.

```cs
// Set Spatial Blend to 1
LucidAudio.PlaySE(clip)
    .SetSpatialBlend(1f);
```

Use SetPosition to specify the position of the sound source.

```cs
// play sound effect at position (1, 2, 3)
LucidAudio.PlaySE(clip)
    .SetSpatialBlend(1f)
    .SetPosition(new Vector3(1f, 2f, 3f));
```

Parameters such as Rolloff Mode and Max Distance can also be set using methods.

```cs
LucidAudio.PlaySE(clip)
    .SetSpatialBlend(1f)
    .SetPosition(new Vector3(1f, 2f, 3f))
    .SetMaxDistanc(10f)
    .SetRolloffMode(AudioRolloffMode.Logarithmic);
```

## Grouping
By setting an ID for AudioPlayer, multiple AudioPlayers can be handled collectively.
Use SetID to set the ID.

```cs
// Set ID to AudioPlayer
LucidAudio.PlaySE(clip)
    .SetID("GroupName");
```

If you want to collectively manipulate audios with IDs, add the IDs to the arguments of the LucidAudio class methods.

```cs
// Stop all AudioPlayers whose ID is set to "GroupName"
LucidAudio.StopAll("GroupName");
```

Also, by using GetPlayersByID, you can get all AudioPlayers with matching IDs.

```cs
// Get all AudioPlayers whose ID is set to "GroupName"
AudioPlayer[] players = LucidAudio.GetPlayersByID("GroupName");

// Set Volume to 0.5
foreach (AudioPlayer player in players)
{
    player.SetVolume(0.5f);
}
```

## SetLink
By using SetLink, it is possible to link the audio playback/stop timing to the state of the GameObject.

```cs
// Call Stop when gameObject is destroyed
LucidAudio.PlayBGM(clip)
    .SetLink(gameObject);
```

By specifying AudioLinkBehaviour as an argument, detailed behavior can be set. (Stop is called at OnDestroy regardless of which option is specified.)

```cs
// pause when inactive, play when active
LucidAudio.PlayBGM(clip)
    .SetLink(gameObject, AudioLinkBehaviour.PauseOnDisableUnPauseOnEnable);
```

Available options are:
* StopOnDestroy
* StopOnDisable
* PlayOnEnable
* RestartOnEnable
* PauseOnDisable
* PauseOnDisableUnPauseOnEnable
* PauseOnDisableRestartOnEnable

## Coroutines & async/await
You can also use coroutines or async/await to wait for audio to finish. To use AudioPlayer in coroutine, call WaitForCompletion.

```cs
IEnumerator Coroutine()
{
    // wait until playback ends
    yield return LucidAudio.PlaySE(clip).WaitForCompletion();
}
```

It is also possible to convert to Task by using WaitForCompletionAsync.

```cs
async void MethodAsync()
{
    // Convert to Task, wait until playback ends
    await LucidAudio.PlaySE(clip).WaitForCompletionAsync();
}
```

### UniTask

LucidAudio supports UniTask. ToUniTask becomes available by introducing UniTask to the project from Package Manager.

```cs
async UniTask MethodAsync(CancellationToken token = default)
{
    try
    {
        // Convert to UniTask, wait until playback ends
        await LucidAudio.PlaySE(clip).ToUniTask(cancellationToken: token);
    }
    catch (OperationCanceledException ex)
    {
        Debug.Log("Canceled");
    }
}
```

Also, you can set the behavior when canceling by specifying AudioCancelBehaviour as an argument. (Stop is set by default)

```cs
async UniTask MethodAsync(CancellationToken token = default)
{
    try
    {
        // Pause AudioPlayer on cancel
        await LucidAudio.PlaySE(clip).ToUniTask(AudioCancelBehaviour.Pause, cancellationToken: token);
    }
    catch (OperationCanceledException ex)
    {
        Debug.Log("Canceled");
    }
}
```


## Playlist
By using the Playlist class, you can create a playlist containing multiple sounds. A Playlist has an AudioClip as an element, and elements can be added/deleted like a normal list.

```cs
public AudioClip clip;

private void Start()
{
    // create a new playlist
    Playlist playlist = new Playlist();

    // add element
    playlist.Add(clip);

    // play playlist
    playlist.Play();
}
```

Use Pause/UnPause to pause/resume playback.

```cs
// Pause playlist playback
playlist.Pause();

// Resume playlist playback
playlist.UnPause();
```
If you want to skip a clip or play the previous clip, use PlayNext/PlayPrevious.

```cs
// Play previous clip (play from beginning if not already played)
playlist.PlayPrevious();

// play next clip (stop if next clip doesn't exist)
playlist.PlayNext();
```

Use PlayShuffle for shuffle playback. The playback order of sounds is set when PlayShuffle is called, and the order does not change even if you call PlayNext or PlayPrevious until Play or PlayShuffle is called again.

```cs
// Shuffle Play
playlist.PlayShuffle();
```
You can get the AudioPlayer currently held by the Playlist from the player property.
A Playlist creates an AudioPlayer each time it starts playing, and disposes it when it finishes playing.

```cs
playlist.player.SetVolume(0.5f);
```

By changing the AudioType, you can set whether to treat the clip as BGM or SE.

```cs
// Play clip using LucidAudio.PlaySE
playlist.audioType = AudioType.SE;
```

Also, Playlist can be edited from the Inspector.

<img src="https://github.com/AnnulusGames/LucidAudio/blob/main/Assets/LucidAudio/Documentation~/img1.png" width="500">

## ライセンス

[Mit License](LICENSE)


