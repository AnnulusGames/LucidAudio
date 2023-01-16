using System;
using UnityEngine;

namespace AnnulusGames.LucidTools.Audio
{
    internal static class DebugUtil
    {
        public static bool logEnabled = true;

        public static void LogWarning(object message)
        {
            if (logEnabled) Debug.LogWarning("[Lucid Audio] " + message);
        }

        public static void ThrowException(Exception exception)
        {
            throw exception;
        }
    }

}