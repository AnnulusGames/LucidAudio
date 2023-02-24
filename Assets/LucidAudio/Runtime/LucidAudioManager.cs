using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnnulusGames.LucidTools.Audio
{
    [AddComponentMenu("")]
    [DefaultExecutionOrder(-1)]
    internal class LucidAudioManager : MonoBehaviour
    {
        private static LucidAudioManager instance;
        public static LucidAudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    Initialize();
                }

                return instance;
            }
        }

        internal static bool isInstanceNull => instance == null;

        private Queue<AudioSource> seSourcePool = new Queue<AudioSource>();
        private Queue<AudioSource> bgmSourcePool = new Queue<AudioSource>();
        private List<AudioPlayer> activeAudioPlayers = new List<AudioPlayer>();
        private List<AudioPlayer> waitingAudioPlayers = new List<AudioPlayer>();

        public IReadOnlyList<AudioPlayer> ActivePlayers => activeAudioPlayers;

        public int ActiveSECount
        {
            get
            {
                return activeAudioPlayers.Count(x => x.audioType == AudioType.SE);
            }
        }

        public int ActiveBGMCount
        {
            get
            {
                return activeAudioPlayers.Count(x => x.audioType == AudioType.BGM);
            }
        }

        internal static void Initialize()
        {
            if (instance != null)
            {
                Destroy(instance);
            }

            GameObject obj = new GameObject("[Lucid Audio]");
            DontDestroyOnLoad(obj);
            instance = obj.AddComponent<LucidAudioManager>();
        }

        private void Update()
        {
            activeAudioPlayers.RemoveAll(x => x.state == AudioPlayer.State.Stop);
            activeAudioPlayers.AddRange(waitingAudioPlayers);
            waitingAudioPlayers.Clear();
            foreach (AudioPlayer player in activeAudioPlayers)
            {
                player.Update();
            }
        }

        private void OnDestroy()
        {
            foreach (AudioPlayer audioPlayer in instance.activeAudioPlayers)
            {
                audioPlayer.Stop();
            }
        }

        internal AudioPlayer Play(AudioType audioType, AudioClip clip, float duration = 0)
        {
            AudioPlayer player = GetAudioPlayer(audioType, clip);
            if (duration > 0) player.Play(duration);
            else player.Play();

            return player;
        }

        internal void Restart(AudioType audioType, AudioClip clip = null, string id = null)
        {
            foreach (AudioPlayer audioPlayer in FindAudioPlayers(audioType, clip, id))
            {
                audioPlayer.Restart();
            }
        }

        internal void Pause(AudioType audioType, AudioClip clip = null,string id = null, float duration = 0)
        {
            foreach (AudioPlayer audioPlayer in FindAudioPlayers(audioType, clip, id))
            {
                if (duration > 0) audioPlayer.Pause(duration);
                else audioPlayer.Pause();
            }
        }

        internal void UnPause(AudioType audioType, AudioClip clip = null,string id = null, float duration = 0)
        {
            foreach (AudioPlayer audioPlayer in FindAudioPlayers(audioType, clip, id))
            {
                if (duration > 0) audioPlayer.UnPause(duration);
                else audioPlayer.UnPause();
            }
        }

        internal void Stop(AudioType audioType, AudioClip clip = null, string id = null, float duration = 0)
        {
            foreach (AudioPlayer audioPlayer in FindAudioPlayers(audioType, clip, id))
            {
                if (duration > 0) audioPlayer.Stop(duration);
                else audioPlayer.Stop();
            }
        }

        internal void Release(AudioPlayer player)
        {
            ReleaseAudioSource(player.audioSource, player.audioType);
        }

        private void ReleaseAudioSource(AudioSource source, AudioType audioType)
        {
            if (source == null) return;
            var pool = GetAudioSourcePool(audioType);
            if (pool.Contains(source)) return;

            GetAudioSourcePool(audioType).Enqueue(source);
        }

        private IEnumerable<AudioPlayer> FindAudioPlayers(AudioType audioType, AudioClip clip = null, string id = null)
        {
            IEnumerable<AudioPlayer> audioPlayers = activeAudioPlayers.Where(p => p.audioType == audioType && p.state != AudioPlayer.State.Wait);
            if (clip != null)
            {
                audioPlayers = audioPlayers.Where(p => p.clip == clip);
            }
            if (id != null)
            {
                audioPlayers = audioPlayers.Where(p => p.id == id);
            }
            return audioPlayers;
        }

        private AudioPlayer GetAudioPlayer(AudioType audioType, AudioClip clip)
        {
            AudioPlayer audioPlayer;
            var pool = GetAudioSourcePool(audioType);

            if (pool.TryDequeue(out var source))
            {
                audioPlayer = new AudioPlayer(source, audioType, clip);
            }
            else
            {
                GameObject obj = new GameObject(audioType.ToString() + " Player");
                obj.transform.SetParent(Instance.transform);
                audioPlayer = new AudioPlayer(obj.AddComponent<AudioSource>(), audioType, clip);
            }

            waitingAudioPlayers.Add(audioPlayer);

            return audioPlayer;
        }

        private Queue<AudioSource> GetAudioSourcePool(AudioType audioType)
        {
            switch (audioType)
            {
                case AudioType.BGM: return bgmSourcePool;
                case AudioType.SE: return seSourcePool;
            }
            return null;
        }

    }
}
