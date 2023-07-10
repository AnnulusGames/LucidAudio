using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
#if LUCIDAUDIO_UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
#endif

namespace AnnulusGames.LucidTools.Audio
{
    public sealed class AudioPlayer
    {
        public enum State
        {
            Wait,
            Delay,
            Playing,
            Pause,
            Stop
        }

        public State state { get; private set; }
        public bool isPlaying
        {
            get
            {
                return state == State.Delay || state == State.Playing;
            }
        }
        public float time
        {
            get
            {
                return audioSource.time;
            }
        }
        public int timeSamples
        {
            get
            {
                return audioSource.timeSamples;
            }
        }
        public AudioClip clip
        {
            get
            {
                return audioSource.clip;
            }
        }

        public bool mute
        {
            get
            {
                return audioSource.mute;
            }
        }
        public string id { get; set; }

        internal bool isAudioSourceDestroyed
        {
            get
            {
                return audioSource == null;
            }
        }

        private Action onStart;
        private Action onPlay;
        private Action onPause;
        private Action onStop;
        private Action onComplete;

        private float delay;
        private bool syncPitchWithTimeScale;
        private bool autoStop = true;
        private List<AudioLinkTrigger> linkTriggers = new List<AudioLinkTrigger>();

        private bool isFadeVolumePlaying;
        private float fadeVolumeStartTime;
        private float fadeVolumeDuration;
        private float fadeVolumeStartValue;
        private float fadeVolumeEndValue;
        private Action onFadeVolumeComplete;

        private bool isFadePitchPlaying;
        private float fadePitchStartTime;
        private float fadePitchDuration;
        private float fadePitchStartValue;
        private float fadePitchEndValue;
        private Action onFadePitchComplete;

        private float t;
        private float pitch = 1f;
        private float volume = 1f;
        private float startFadeDuration;
        private bool started;

        internal readonly AudioType audioType;
        internal AudioSource audioSource;

        internal AudioPlayer(AudioSource audioSource, AudioType audioType, AudioClip clip)
        {
            this.audioSource = audioSource;
            this.audioType = audioType;
            audioSource.clip = clip;
            audioSource.playOnAwake = false;
            Init();
        }

        internal void Update()
        {
            if (isAudioSourceDestroyed) return;
            if (!isPlaying) return;

            SetPitch(pitch);
            SetVolume(volume);
            t += Time.unscaledDeltaTime;

            onPlay?.Invoke();

            if (t < delay)
            {
                state = State.Delay;
                return;
            }

            state = State.Playing;
            if (!started)
            {
                Start();
            }
            else if (!audioSource.isPlaying)
            {
                onComplete?.Invoke();
                if (!autoStop)
                {
                    t = 0;
                    started = false;
                    state = State.Wait;
                }
                else
                {
                    Stop();
                }
            }

            UpdateFadeVolume();
            UpdateFadePitch();
        }

        private void UpdateFadeVolume()
        {
            if (!isFadeVolumePlaying) return;
            if (audioSource == null) return;

            float fadeTime = Time.realtimeSinceStartup - fadeVolumeStartTime;
            SetVolume(Mathf.Lerp(fadeVolumeStartValue, fadeVolumeEndValue, Mathf.InverseLerp(0, fadeVolumeDuration, fadeTime)));
            if (fadeTime >= fadeVolumeDuration)
            {
                isFadeVolumePlaying = false;
                onFadeVolumeComplete?.Invoke();
            }
        }
        private void UpdateFadePitch()
        {
            if (!isFadePitchPlaying) return;
            if (audioSource == null) return;

            float fadeTime = Time.realtimeSinceStartup - fadePitchStartTime;
            SetPitch(Mathf.Lerp(fadePitchStartValue, fadePitchEndValue, Mathf.InverseLerp(0, fadePitchDuration, fadeTime)));
            if (fadeTime >= fadePitchDuration)
            {
                isFadePitchPlaying = false;
                onFadePitchComplete?.Invoke();
            }
        }

        private void Init()
        {
            t = 0;
            started = false;
            startFadeDuration = 0f;

            state = State.Wait;
            delay = 0;
            syncPitchWithTimeScale = false;
            autoStop = true;

            pitch = 1f;
            volume = 1f;

            isFadePitchPlaying = false;
            isFadeVolumePlaying = false;

            foreach (AudioLinkTrigger trigger in linkTriggers)
            {
                if (trigger != null) trigger.RemoveAudioPlayer(this);
            }
            linkTriggers.Clear();

            audioSource.Stop();
            audioSource.outputAudioMixerGroup = null;
            audioSource.loop = false;
            audioSource.mute = false;
            audioSource.pitch = 1f;
            audioSource.volume = 1f;
            audioSource.spatialBlend = 0f;
            audioSource.panStereo = 0f;
            audioSource.velocityUpdateMode = AudioVelocityUpdateMode.Auto;
            audioSource.priority = 128;
            audioSource.dopplerLevel = 1f;
            audioSource.spread = 0f;
            audioSource.reverbZoneMix = 1f;
            audioSource.minDistance = 1;
            audioSource.maxDistance = 500;
            audioSource.ignoreListenerPause = false;
            audioSource.ignoreListenerVolume = false;
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
            audioSource.transform.position = Vector3.zero;

            onStart = null;
            onPlay = null;
            onPause = null;
            onStop = null;
            onComplete = null;
        }

        private void Start()
        {
            if (startFadeDuration > 0)
            {
                SetVolume(0);
                FadeVolume(1, startFadeDuration);
            }
            audioSource.Play();
            onStart?.Invoke();

            started = true;
        }

        public void Play()
        {
            ThrowExceptionIfStop();
            state = t < delay ? State.Delay : State.Playing;
        }

        public void Play(float fadeInDuration)
        {
            ThrowExceptionIfStop();
            startFadeDuration = fadeInDuration;
            Play();
        }

        public void Restart()
        {
            ThrowExceptionIfStop();
            t = 0;
            started = false;

            if (audioSource != null) audioSource.Stop();
            Play();
        }

        public void Stop()
        {
            if (audioSource != null) audioSource.Stop();
            onStop?.Invoke();
            LucidAudioManager.Instance.Release(this);
            audioSource = null;
            state = State.Stop;
        }

        public void Stop(float fadeOutDuration)
        {
            if (fadeOutDuration <= 0) Stop();
            else FadeVolume(0, fadeOutDuration, Stop);
        }

        public void Pause()
        {
            ThrowExceptionIfStop();

            if (audioSource != null) audioSource.Pause();
            onPause?.Invoke();
            state = State.Pause;
        }

        public void Pause(float fadeOutDuration)
        {
            ThrowExceptionIfStop();
            if (fadeOutDuration <= 0) Pause();
            else FadeVolume(0, fadeOutDuration, Pause);
        }

        public void UnPause()
        {
            ThrowExceptionIfStop();
            if (state != State.Pause) return;

            audioSource.UnPause();
            state = t < delay ? State.Delay : State.Playing;
        }

        public void UnPause(float fadeInDuration)
        {
            ThrowExceptionIfStop();
            if (state != State.Pause) return;

            audioSource.UnPause();
            state = t < delay ? State.Delay : State.Playing;
            float v = audioSource.volume;
            SetVolume(0);
            FadeVolume(v, fadeInDuration);
        }

        public AudioPlayer OnStart(Action callback)
        {
            onStart += callback;
            return this;
        }

        public AudioPlayer OnPlay(Action callback)
        {
            onPlay += callback;
            return this;
        }

        public AudioPlayer OnPause(Action callback)
        {
            onPause += callback;
            return this;
        }

        public AudioPlayer OnStop(Action callback)
        {
            onStop += callback;
            return this;
        }

        public AudioPlayer OnComplete(Action callback)
        {
            onComplete += callback;
            return this;
        }

        public AudioPlayer SetClip(AudioClip clip)
        {
            audioSource.clip = clip;
            return this;
        }

        public AudioPlayer SetVolume(float volume)
        {
            this.volume = volume;
            switch (audioType)
            {
                default:
                    audioSource.volume = volume;
                    break;
                case AudioType.BGM:
                    audioSource.volume = volume * LucidAudio.BGMVolume;
                    break;
                case AudioType.SE:
                    audioSource.volume = volume * LucidAudio.SEVolume;
                    break;
            }
            return this;
        }

        public AudioPlayer SetPitch(float pitch)
        {
            this.pitch = pitch;
            audioSource.pitch = pitch * (syncPitchWithTimeScale ? Time.timeScale : 1);
            return this;
        }

        public AudioPlayer SetDelay(float delay)
        {
            this.delay = delay;
            return this;
        }

        public AudioPlayer SetAudioMixerGroup(AudioMixerGroup audioMixerGroup)
        {
            audioSource.outputAudioMixerGroup = audioMixerGroup;
            return this;
        }

        public AudioPlayer SetLoop(bool loop = true)
        {
            audioSource.loop = loop;
            return this;
        }

        public AudioPlayer SetPosition(Vector3 point)
        {
            audioSource.transform.position = point;
            return this;
        }

        public AudioPlayer SetSpatialBlend(float value)
        {
            audioSource.spatialBlend = value;
            return this;
        }

        public AudioPlayer SetPanStereo(float value)
        {
            audioSource.panStereo = value;
            return this;
        }

        public AudioPlayer SetPriority(int value)
        {
            audioSource.priority = value;
            return this;
        }

        public AudioPlayer SetMute(bool value = true)
        {
            audioSource.mute = value;
            return this;
        }

        public AudioPlayer SetReverbZoneMix(float value)
        {
            audioSource.reverbZoneMix = value;
            return this;
        }

        public AudioPlayer SetVelocityUpdateMode(AudioVelocityUpdateMode mode)
        {
            audioSource.velocityUpdateMode = mode;
            return this;
        }

        public AudioPlayer SetDopplerLevel(float value)
        {
            audioSource.dopplerLevel = value;
            return this;
        }

        public AudioPlayer SetSpread(float value)
        {
            audioSource.spread = value;
            return this;
        }

        public AudioPlayer SetRolloffMode(AudioRolloffMode mode)
        {
            audioSource.rolloffMode = mode;
            return this;
        }

        public AudioPlayer SetMinDistance(float value)
        {
            audioSource.minDistance = value;
            return this;
        }

        public AudioPlayer SetMaxDistance(float value)
        {
            audioSource.maxDistance = value;
            return this;
        }

        public AudioPlayer SetIgnoreListenerPause(bool value = true)
        {
            audioSource.ignoreListenerPause = value;
            return this;
        }

        public AudioPlayer SetIgnoreListenerVolume(bool value = true)
        {
            audioSource.ignoreListenerVolume = value;
            return this;
        }

        public AudioPlayer SetSyncPitchWithTimeScale(bool value = true)
        {
            syncPitchWithTimeScale = value;
            return this;
        }

        public AudioPlayer SetAutoStop(bool value = true)
        {
            autoStop = value;
            return this;
        }

        public AudioPlayer SetLink(GameObject target, AudioLinkBehaviour behaviour = AudioLinkBehaviour.StopOnDestroy)
        {
            AudioLinkTrigger trigger;
            if (!target.TryGetComponent<AudioLinkTrigger>(out trigger))
            {
                trigger = target.AddComponent<AudioLinkTrigger>();
            }

            if (!linkTriggers.Contains(trigger)) linkTriggers.Add(trigger);

            trigger.AddAudioPlayer(this, behaviour);

            return this;
        }

        public AudioPlayer SetLink(Component component, AudioLinkBehaviour behaviour = AudioLinkBehaviour.StopOnDestroy)
        {
            return SetLink(component.gameObject, behaviour);
        }

        public AudioPlayer SetID(string id)
        {
            this.id = id;
            return this;
        }

        public void FadeVolume(float endValue, float duration, Action callback = null)
        {
            if (duration > 0)
            {
                fadeVolumeStartTime = Time.realtimeSinceStartup;
                fadeVolumeDuration = duration;
                switch (audioType)
                {
                    default:
                        fadeVolumeStartValue = audioSource.volume;
                        break;
                    case AudioType.BGM:
                        fadeVolumeStartValue = LucidAudio.BGMVolume == 0 ? 1 : (audioSource.volume / LucidAudio.BGMVolume);
                        break;
                    case AudioType.SE:
                        fadeVolumeStartValue = LucidAudio.SEVolume == 0 ? 1 : (audioSource.volume / LucidAudio.SEVolume);
                        break;
                }
                fadeVolumeEndValue = endValue;

                onFadeVolumeComplete = callback;
                isFadeVolumePlaying = true;
            }
            else
            {
                audioSource.volume = endValue;
                callback?.Invoke();
            }
        }

        public void FadePitch(float endValue, float duration, Action callback = null)
        {
            if (duration > 0)
            {
                fadePitchStartTime = Time.realtimeSinceStartup;
                fadePitchDuration = duration;
                fadePitchStartValue = audioSource.pitch;
                fadePitchEndValue = endValue;

                onFadePitchComplete = callback;
                isFadePitchPlaying = true;
            }
            else
            {
                audioSource.pitch = endValue;
                callback?.Invoke();
            }
        }

        public IEnumerator WaitForCompletion()
        {
            return new WaitWhile(() => isPlaying || state == State.Pause);
        }

        public async Task WaitForCompletionAsync()
        {
            while (isPlaying || state == State.Pause) await Task.Yield();
        }

#if LUCIDAUDIO_UNITASK_SUPPORT
        public async UniTask ToUniTask(AudioCancelBehaviour audioCancelBehaviour = AudioCancelBehaviour.Stop, CancellationToken cancellationToken = default)
        {
            try
            {
                while (isPlaying || state == State.Pause) await UniTask.Yield(cancellationToken: cancellationToken);
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

        private void ThrowExceptionIfStop()
        {
            if (state == State.Stop) DebugUtil.ThrowException(new InvalidOperationException("A stopped AudioPlayer is not allowed to play again. You need to create a new AudioPlayer."));
        }

    }

}
