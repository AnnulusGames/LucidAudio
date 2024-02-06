using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace AnnulusGames.LucidTools.Audio
{
    public static class LucidAudio
    {
        public static int ActiveSECount
        {
            get
            {
                return LucidAudioManager.Instance.ActivePlayers.Count(x => x.audioType == AudioType.SE);
            }
        }

        public static int ActiveBGMCount
        {
            get
            {
                return LucidAudioManager.Instance.ActivePlayers.Count(x => x.audioType == AudioType.BGM);
            }
        }

        public static int ActivePlayerCount
        {
            get
            {
                return LucidAudioManager.Instance.ActivePlayers.Count;
            }
        }

        public static float BGMVolume { get; set; } = 1f;
        public static float SEVolume { get; set; } = 1f;

        public static AudioMixerGroup defaultBGMMixerGroup
        {
            get => MixerGroups[(int)AudioType.BGM];
            set => MixerGroups[(int)AudioType.BGM] = value;
        }

        public static AudioMixerGroup defaultSEMixerGroup
        {
            get => MixerGroups[(int)AudioType.SE];
            set => MixerGroups[(int)AudioType.SE] = value;
        }

        public static AudioMixerGroup defaultSpatialSEMixerGroup
        {
            get => MixerGroups[(int)AudioType.SpatialSE];
            set => MixerGroups[(int)AudioType.SpatialSE] = value;
        }
        
        public static readonly AudioMixerGroup[] MixerGroups = new AudioMixerGroup[3];

        public static AudioPlayer[] GetPlayers()
        {
            return LucidAudioManager.Instance.ActivePlayers.ToArray();
        }

        public static AudioPlayer[] GetPlayersByID(string id)
        {
            return LucidAudioManager.Instance.ActivePlayers.Where(x => x.id == id).ToArray();
        }
        
        public static AudioPlayer[] GetPlayersByClip(AudioClip clip)
        {
            return LucidAudioManager.Instance.ActivePlayers.Where(x => x.clip == clip).ToArray();
        }

        public static AudioPlayer Play(AudioType type, AudioClip clip, float fadeInDuration = 0f)
        {
            return LucidAudioManager.Instance.Play(type, clip, fadeInDuration).SetAudioMixerGroup(MixerGroups[(int)type]);
        }

        public static AudioPlayer PlayBGM(AudioClip clip, float fadeInDuration = 0f)
        {
            return Play(AudioType.BGM, clip, fadeInDuration);
        }
        public static AudioPlayer PlaySE(AudioClip clip, float fadeInDuration = 0f)
        {
            return Play(AudioType.SE, clip, fadeInDuration);
        }
        public static AudioPlayer PlaySpatial(AudioClip clip, float fadeInDuration = 0f)
        {
            return Play(AudioType.SpatialSE, clip, fadeInDuration);
        }
        public static void RestartAllBGM()
        {
            LucidAudioManager.Instance.Restart(AudioType.BGM);
        }

        public static void RestartAllBGM(AudioClip clip)
        {
            LucidAudioManager.Instance.Restart(AudioType.BGM, clip);
        }

        public static void RestartAllBGM(string id)
        {
            LucidAudioManager.Instance.Restart(AudioType.BGM, null, id);
        }

        public static void RestartAllSE()
        {
            LucidAudioManager.Instance.Restart(AudioType.SE);
        }

        public static void RestartAllSE(AudioClip clip)
        {
            LucidAudioManager.Instance.Restart(AudioType.SE, clip);
        }

        public static void RestartAllSE(string id)
        {
            LucidAudioManager.Instance.Restart(AudioType.SE, null, id);
        }

        public static void RestartAll()
        {
            RestartAllBGM();
            RestartAllSE();
        }

        public static void RestartAll(AudioClip clip)
        {
            RestartAllBGM(clip);
            RestartAllSE(clip);
        }

        public static void RestartAll(string id)
        {
            RestartAllBGM(id);
            RestartAllSE(id);
        }

        public static void StopAllBGM()
        {
            LucidAudioManager.Instance.Stop(AudioType.BGM);
        }

        public static void StopAllBGM(float fadeOutDuration)
        {
            LucidAudioManager.Instance.Stop(AudioType.BGM, null, null, fadeOutDuration);
        }

        public static void StopAllBGM(AudioClip clip)
        {
            LucidAudioManager.Instance.Stop(AudioType.BGM, clip);
        }

        public static void StopAllBGM(AudioClip clip, float fadeOutDuration)
        {
            LucidAudioManager.Instance.Stop(AudioType.BGM, clip);
        }

        public static void StopAllBGM(string id)
        {
            LucidAudioManager.Instance.Stop(AudioType.BGM, null, id);
        }

        public static void StopAllBGM(string id, float fadeOutDuration)
        {
            LucidAudioManager.Instance.Stop(AudioType.BGM, null, id);
        }

        public static void StopAllSE()
        {
            LucidAudioManager.Instance.Stop(AudioType.SE);
        }

        public static void StopAllSE(float fadeOutDuration)
        {
            LucidAudioManager.Instance.Stop(AudioType.SE, null, null, fadeOutDuration);
        }

        public static void StopAllSE(AudioClip clip)
        {
            LucidAudioManager.Instance.Stop(AudioType.SE, clip);
        }

        public static void StopAllSE(AudioClip clip, float fadeOutDuration)
        {
            LucidAudioManager.Instance.Stop(AudioType.SE, clip, null, fadeOutDuration);
        }

        public static void StopAllSE(string id)
        {
            LucidAudioManager.Instance.Stop(AudioType.SE, null, id);
        }

        public static void StopAllSE(string id, float fadeOutDuration)
        {
            LucidAudioManager.Instance.Stop(AudioType.SE, null, id, fadeOutDuration);
        }

        public static void StopAll()
        {
            StopAllBGM();
            StopAllSE();
        }

        public static void StopAll(float fadeOutDuration)
        {
            StopAllBGM(fadeOutDuration);
            StopAllSE(fadeOutDuration);
        }

        public static void StopAll(AudioClip clip)
        {
            StopAllBGM(clip);
            StopAllSE(clip);
        }

        public static void StopAll(AudioClip clip, float fadeOutDuration)
        {
            StopAllBGM(clip, fadeOutDuration);
            StopAllSE(clip, fadeOutDuration);
        }

        public static void StopAll(string id)
        {
            StopAllBGM(id);
            StopAllSE(id);
        }

        public static void StopAll(string id, float fadeOutDuration)
        {
            StopAllBGM(id, fadeOutDuration);
            StopAllSE(id, fadeOutDuration);
        }

        public static void PauseAllBGM()
        {
            LucidAudioManager.Instance.Pause(AudioType.BGM);
        }

        public static void PauseAllBGM(float fadeOutDuration)
        {
            LucidAudioManager.Instance.Pause(AudioType.BGM, null, null, fadeOutDuration);
        }

        public static void PauseAllBGM(AudioClip clip)
        {
            LucidAudioManager.Instance.Pause(AudioType.BGM, clip);
        }

        public static void PauseAllBGM(AudioClip clip, float fadeOutDuration)
        {
            LucidAudioManager.Instance.Pause(AudioType.BGM, clip, null, fadeOutDuration);
        }

        public static void PauseAllBGM(string id)
        {
            LucidAudioManager.Instance.Pause(AudioType.BGM, null, id);
        }

        public static void PauseAllBGM(string id, float fadeOutDuration)
        {
            LucidAudioManager.Instance.Pause(AudioType.BGM, null, id, fadeOutDuration);
        }

        public static void PauseAllSE()
        {
            LucidAudioManager.Instance.Pause(AudioType.SE);
        }

        public static void PauseAllSE(float fadeOutDuration)
        {
            LucidAudioManager.Instance.Pause(AudioType.SE, null, null, fadeOutDuration);
        }

        public static void PauseAllSE(AudioClip clip)
        {
            LucidAudioManager.Instance.Pause(AudioType.SE, clip);
        }

        public static void PauseAllSE(AudioClip clip, float fadeOutDuration)
        {
            LucidAudioManager.Instance.Pause(AudioType.SE, clip, null, fadeOutDuration);
        }

        public static void PauseAllSE(string id)
        {
            LucidAudioManager.Instance.Pause(AudioType.SE, null, id);
        }

        public static void PauseAllSE(string id, float fadeOutDuration)
        {
            LucidAudioManager.Instance.Pause(AudioType.SE, null, id, fadeOutDuration);
        }

        public static void PauseAll()
        {
            PauseAllBGM();
            PauseAllSE();
        }

        public static void PauseAll(float fadeOutDuration)
        {
            PauseAllBGM(fadeOutDuration);
            PauseAllSE(fadeOutDuration);
        }

        public static void PauseAll(AudioClip clip)
        {
            PauseAllBGM(clip);
            PauseAllSE(clip);
        }

        public static void PauseAll(AudioClip clip, float fadeOutDuration)
        {
            PauseAllBGM(clip, fadeOutDuration);
            PauseAllSE(clip, fadeOutDuration);
        }

        public static void PauseAll(string id)
        {
            PauseAllBGM(id);
            PauseAllSE(id);
        }

        public static void PauseAll(string id, float fadeOutDuration)
        {
            PauseAllBGM(id, fadeOutDuration);
            PauseAllSE(id, fadeOutDuration);
        }

        public static void UnPauseAllBGM()
        {
            LucidAudioManager.Instance.UnPause(AudioType.BGM);
        }

        public static void UnPauseAllBGM(float fadeInDuration)
        {
            LucidAudioManager.Instance.UnPause(AudioType.BGM, null, null, fadeInDuration);
        }

        public static void UnPauseAllBGM(AudioClip clip)
        {
            LucidAudioManager.Instance.UnPause(AudioType.BGM, clip);
        }

        public static void UnPauseAllBGM(AudioClip clip, float fadeInDuration)
        {
            LucidAudioManager.Instance.UnPause(AudioType.BGM, clip, null, fadeInDuration);
        }

        public static void UnPauseAllBGM(string id)
        {
            LucidAudioManager.Instance.UnPause(AudioType.BGM, null, id);
        }

        public static void UnPauseAllBGM(string id, float fadeInDuration)
        {
            LucidAudioManager.Instance.UnPause(AudioType.BGM, null, id, fadeInDuration);
        }

        public static void UnPauseAllSE()
        {
            LucidAudioManager.Instance.UnPause(AudioType.SE);
        }

        public static void UnPauseAllSE(float fadeInDuration)
        {
            LucidAudioManager.Instance.UnPause(AudioType.SE, null, null, fadeInDuration);
        }

        public static void UnPauseAllSE(AudioClip clip)
        {
            LucidAudioManager.Instance.UnPause(AudioType.SE, clip);
        }

        public static void UnPauseAllSE(AudioClip clip, float fadeInDuration)
        {
            LucidAudioManager.Instance.UnPause(AudioType.SE, clip, null, fadeInDuration);
        }

        public static void UnPauseAllSE(string id)
        {
            LucidAudioManager.Instance.UnPause(AudioType.SE, null, id);
        }

        public static void UnPauseAllSE(string id, float fadeInDuration)
        {
            LucidAudioManager.Instance.UnPause(AudioType.SE, null, id, fadeInDuration);
        }

        public static void UnPauseAll()
        {
            UnPauseAllBGM();
            UnPauseAllSE();
        }

        public static void UnPauseAll(float fadeInDuration)
        {
            UnPauseAllBGM(fadeInDuration);
            UnPauseAllSE(fadeInDuration);
        }

        public static void UnPauseAll(AudioClip clip)
        {
            UnPauseAllBGM(clip);
            UnPauseAllSE(clip);
        }

        public static void UnPauseAll(AudioClip clip, float fadeInDuration)
        {
            UnPauseAllBGM(clip, fadeInDuration);
            UnPauseAllSE(clip, fadeInDuration);
        }

        public static void UnPauseAll(string id)
        {
            UnPauseAllBGM(id);
            UnPauseAllSE(id);
        }

        public static void UnPauseAll(string id, float fadeInDuration)
        {
            UnPauseAllBGM(id, fadeInDuration);
            UnPauseAllSE(id, fadeInDuration);
        }
    }
}