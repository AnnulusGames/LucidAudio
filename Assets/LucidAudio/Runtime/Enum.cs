namespace AnnulusGames.LucidTools.Audio
{
    public enum AudioType
    {
        BGM       =0,
        SE        =1,
        SpatialSE =2
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