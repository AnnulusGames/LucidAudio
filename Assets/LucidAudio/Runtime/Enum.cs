namespace AnnulusGames.LucidTools.Audio
{
    public enum AudioType
    {
        BGM,
        SE
    }

    public enum AudioCancelBehaviour
    {
        Stop,
        Pause
    }

    public enum AudioLinkBehaviour
    {
        StopOnDestroy,
        StopOnDisable,
        PlayOnEnable,
        RestartOnEnable,
        PauseOnDisable,
        PauseOnDisableUnPauseOnEnable,
        PauseOnDisableRestartOnEnable,
    }
}