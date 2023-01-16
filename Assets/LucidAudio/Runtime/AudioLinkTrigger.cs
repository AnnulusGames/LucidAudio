using System.Collections.Generic;
using UnityEngine;

namespace AnnulusGames.LucidTools.Audio
{
    [AddComponentMenu("")]
    internal class AudioLinkTrigger : MonoBehaviour
    {
        private List<AudioLink> audioLinkList = new List<AudioLink>();

        public void AddAudioPlayer(AudioPlayer player, AudioLinkBehaviour behaviour)
        {
            AudioLink link = audioLinkList.Find(x => x.player == player);
            if (link == null)
            {
                audioLinkList.Add(new AudioLink(player, behaviour));
            }
            else
            {
                link.behaviour = behaviour;
            }
        }

        public void RemoveAudioPlayer(AudioPlayer player)
        {
            AudioLink link = audioLinkList.Find(x => x.player == player);
            if (link != null)
            {
                audioLinkList.Remove(link);
            }
        }

        private void OnEnable()
        {
            foreach (AudioLink link in audioLinkList)
            {
                if (link.player.isAudioSourceDestroyed) continue;
                if (link.player.state == AudioPlayer.State.Stop) continue;
                switch (link.behaviour)
                {
                    case AudioLinkBehaviour.PlayOnEnable:
                        link.player.Play();
                        break;
                    case AudioLinkBehaviour.RestartOnEnable:
                        link.player.Restart();
                        break;
                    case AudioLinkBehaviour.PauseOnDisableUnPauseOnEnable:
                        link.player.UnPause();
                        break;
                    case AudioLinkBehaviour.PauseOnDisableRestartOnEnable:
                        link.player.Restart();
                        break;
                }
            }
        }

        private void OnDisable()
        {
            foreach (AudioLink link in audioLinkList)
            {
                if (link.player.isAudioSourceDestroyed) continue;
                if (link.player.state == AudioPlayer.State.Stop) continue;
                switch (link.behaviour)
                {
                    case AudioLinkBehaviour.StopOnDisable:
                        link.player.Stop();
                        break;
                    case AudioLinkBehaviour.PauseOnDisable:
                    case AudioLinkBehaviour.PauseOnDisableUnPauseOnEnable:
                    case AudioLinkBehaviour.PauseOnDisableRestartOnEnable:
                        link.player.Pause();
                        break;
                }
            }
        }

        private void OnDestroy()
        {
            foreach (AudioLink link in audioLinkList)
            {
                if (link.player.isAudioSourceDestroyed) continue;
                if (link.player.state == AudioPlayer.State.Stop) continue;
                link.player.Stop();
            }
        }
    }

    internal class AudioLink
    {
        public readonly AudioPlayer player;
        public AudioLinkBehaviour behaviour;

        public AudioLink(AudioPlayer player, AudioLinkBehaviour behaviour)
        {
            this.player = player;
            this.behaviour = behaviour;
        }
    }
}
