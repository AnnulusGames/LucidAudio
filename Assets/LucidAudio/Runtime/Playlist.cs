using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Threading;
using System.Threading.Tasks;
#if LUCIDAUDIO_UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
#endif


namespace AnnulusGames.LucidTools.Audio
{
    [Serializable]
    public class Playlist : IList<AudioClip>
    {
        public AudioType audioType;
        [SerializeField] private string displayName;

        public bool isPlaying
        {
            get
            {
                return currentPlayer != null && currentPlayer.isPlaying;
            }
        }

        public int currentIndex { get; private set; }

        public AudioPlayer player
        {
            get
            {
                return currentPlayer;
            }
        }

        [SerializeField] private List<AudioClip> list = new List<AudioClip>();
        private AudioPlayer currentPlayer;
        private List<AudioClip> playlist = new List<AudioClip>();
        private bool loop;

        public AudioClip this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return list.Count;
            }
        }
        public bool IsReadOnly { get { return false; } }

        public void Add(AudioClip item)
        {
            list.Add(item);
        }

        public void Insert(int index, AudioClip item)
        {
            list.Insert(index, item);
        }

        public bool Remove(AudioClip item)
        {
            return list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        public int RemoveAll(Predicate<AudioClip> match)
        {
            return list.RemoveAll(match);
        }

        public bool Contains(AudioClip item)
        {
            return list.Contains(item);
        }

        public int IndexOf(AudioClip item)
        {
            return list.IndexOf(item);
        }

        public void CopyTo(AudioClip[] array, int index)
        {
            list.CopyTo(array, index);
        }

        public void Clear()
        {
            list.Clear();
        }

        public IEnumerator<AudioClip> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)list).GetEnumerator();
        }

        public void Play(bool repeat = false)
        {
            Play(0, repeat);
        }

        public void Play(int startIndex, bool repeat = false)
        {
            loop = repeat;
            Stop();
            playlist.Clear();
            foreach (AudioClip AudioClip in list)
            {
                playlist.Add(AudioClip);
            }
            currentIndex = startIndex;
            PlayAudioClip(playlist[currentIndex]);
        }

        public void PlayShuffle(bool repeat = false)
        {
            loop = repeat;
            Stop();
            playlist.Clear();
            foreach (AudioClip AudioClip in list)
            {
                playlist.Add(AudioClip);
            }

            int n = playlist.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                AudioClip tmp = playlist[k];
                playlist[k] = playlist[n];
                playlist[n] = tmp;
            }

            currentIndex = 0;
            PlayAudioClip(playlist[0]);
        }

        public void Stop()
        {
            currentPlayer?.Stop();
            currentPlayer = null;
        }

        public void Pause()
        {
            currentPlayer?.Pause();
        }

        public void UnPause()
        {
            currentPlayer?.UnPause();
        }

        public void PlayNext()
        {
            if (!isPlaying && playlist.Count > 0)
            {
                currentIndex = 0;
                PlayAudioClip(playlist[currentIndex]);
                return;
            }
            else if (playlist.Count == 0)
            {
                Play(0);
                return;
            }

            if (currentIndex < playlist.Count - 1)
            {
                currentIndex++;
                PlayAudioClip(playlist[currentIndex]);
            }
            else if (loop)
            {
                currentIndex = 0;
                PlayAudioClip(playlist[currentIndex]);
            }
            else
            {
                Stop();
            }
        }

        public void PlayPrevious()
        {
            if (!isPlaying && playlist.Count > 0)
            {
                currentIndex = 0;
                PlayAudioClip(playlist[currentIndex]);
                return;
            }
            else if (playlist.Count == 0)
            {
                Play(0);
                return;
            }
            if (currentIndex > 0)
            {
                currentIndex--;
            }
            else
            {
                currentIndex = 0;
            }
            PlayAudioClip(playlist[currentIndex]);
        }

        private void PlayAudioClip(AudioClip clip)
        {
            if (currentPlayer == null)
            {
                currentPlayer = LucidAudio.Play(audioType, clip)
                    .SetAutoStop(false)
                    .OnComplete(() => PlayNext());
            }
            else
            {
                currentPlayer.audioSource.Stop();
                currentPlayer.SetClip(clip).Restart();
                currentPlayer.audioSource.Play();
            }
        }

        public IEnumerator WaitForCompletion()
        {
            return new WaitWhile(() => currentPlayer != null && (currentPlayer.isPlaying || currentPlayer.state == AudioPlayer.State.Pause));
        }

        public async Task WaitForCompletionAsync()
        {
            while (currentPlayer != null && (currentPlayer.isPlaying || currentPlayer.state == AudioPlayer.State.Pause)) await Task.Yield();
        }

#if LUCIDAUDIO_UNITASK_SUPPORT
        public async UniTask ToUniTask(AudioCancelBehaviour audioCancelBehaviour = AudioCancelBehaviour.Stop, CancellationToken cancellationToken = default)
        {
            try
            {
                while (currentPlayer != null && (currentPlayer.isPlaying || currentPlayer.state == AudioPlayer.State.Pause)) await UniTask.Yield(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException e)
            {
                if (!LucidAudioManager.isInstanceNull)
                {
                    switch (audioCancelBehaviour)
                    {
                        case AudioCancelBehaviour.Stop:
                            Stop();
                            break;
                        case AudioCancelBehaviour.Pause:
                            Pause();
                            break;
                    }
                }
                throw e;
            }
        }
#endif


    }

}