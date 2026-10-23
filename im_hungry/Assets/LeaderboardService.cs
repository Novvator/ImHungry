using System;
using UnityEngine;
#if GPGS && UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

/// <summary>
/// Thin wrapper around Google Play Games Services leaderboards.
///
/// The real calls only compile when the "GPGS" scripting define symbol is set
/// (Project Settings > Player > Android > Scripting Define Symbols) after the
/// Google Play Games plugin for Unity has been imported. Without it every call
/// is a safe no-op, so the project still builds in the editor and on iOS.
///
/// Scores that fail to post (offline, not signed in) are kept in PlayerPrefs
/// and retried on the next successful sign-in.
/// </summary>
public static class LeaderboardService
{
    // Replace with the leaderboard ID from Play Console
    // (Play Games Services > Leaderboards), or with GPGSIds.leaderboard_endless
    // once you've run Window > Google Play Games > Setup > Android setup.
    public const string EndlessLeaderboardId = "REPLACE_WITH_ENDLESS_LEADERBOARD_ID";

    private const string PendingScoreKey = "Endless Pending Score";

#if GPGS && UNITY_ANDROID
    private static bool activated = false;
#endif

    public static bool IsAvailable
    {
        get
        {
#if GPGS && UNITY_ANDROID
            return !EndlessLeaderboardId.StartsWith("REPLACE_");
#else
            return false;
#endif
        }
    }

    public static bool IsSignedIn
    {
        get
        {
#if GPGS && UNITY_ANDROID
            return activated && PlayGamesPlatform.Instance.IsAuthenticated();
#else
            return false;
#endif
        }
    }

    /// <summary>
    /// Silent sign-in. Play Games v2 signs players in automatically on launch,
    /// this just picks up the result. Safe to call more than once.
    /// </summary>
    public static void SignIn(Action<bool> onDone = null)
    {
#if GPGS && UNITY_ANDROID
        if (!activated)
        {
            PlayGamesPlatform.Activate();
            activated = true;
        }
        PlayGamesPlatform.Instance.Authenticate(status =>
        {
            bool ok = status == SignInStatus.Success;
            if (ok) FlushPendingScore();
            onDone?.Invoke(ok);
        });
#else
        onDone?.Invoke(false);
#endif
    }

    /// <summary>
    /// Sign-in with the Play Games UI (use when the player taps a button).
    /// </summary>
    public static void SignInInteractive(Action<bool> onDone = null)
    {
#if GPGS && UNITY_ANDROID
        if (!activated)
        {
            PlayGamesPlatform.Activate();
            activated = true;
        }
        PlayGamesPlatform.Instance.ManuallyAuthenticate(status =>
        {
            bool ok = status == SignInStatus.Success;
            if (ok) FlushPendingScore();
            onDone?.Invoke(ok);
        });
#else
        onDone?.Invoke(false);
#endif
    }

    public static void SubmitScore(long score, Action<bool> onDone = null)
    {
        if (score <= 0)
        {
            onDone?.Invoke(false);
            return;
        }

        // Keep the best unsent score so nothing is lost if this attempt fails
        if (score > PlayerPrefs.GetInt(PendingScoreKey, 0))
        {
            PlayerPrefs.SetInt(PendingScoreKey, (int)score);
            PlayerPrefs.Save();
        }

        if (!IsAvailable)
        {
            onDone?.Invoke(false);
            return;
        }

        if (IsSignedIn)
        {
            ReportPending(onDone);
        }
        else
        {
            SignIn(ok => onDone?.Invoke(ok));
        }
    }

    public static void ShowLeaderboard()
    {
#if GPGS && UNITY_ANDROID
        if (!IsAvailable) return;

        if (IsSignedIn)
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI(EndlessLeaderboardId);
        }
        else
        {
            SignInInteractive(ok =>
            {
                if (ok) PlayGamesPlatform.Instance.ShowLeaderboardUI(EndlessLeaderboardId);
            });
        }
#else
        Debug.Log("LeaderboardService: Play Games not enabled, add the GPGS define to use leaderboards.");
#endif
    }

    private static void FlushPendingScore()
    {
        if (PlayerPrefs.GetInt(PendingScoreKey, 0) > 0)
        {
            ReportPending(null);
        }
    }

    private static void ReportPending(Action<bool> onDone)
    {
#if GPGS && UNITY_ANDROID
        long pending = PlayerPrefs.GetInt(PendingScoreKey, 0);
        PlayGamesPlatform.Instance.ReportScore(pending, EndlessLeaderboardId, success =>
        {
            // Only clear if nothing higher was queued while this was in flight
            if (success && PlayerPrefs.GetInt(PendingScoreKey, 0) == pending)
            {
                PlayerPrefs.DeleteKey(PendingScoreKey);
                PlayerPrefs.Save();
            }
            onDone?.Invoke(success);
        });
#else
        onDone?.Invoke(false);
#endif
    }
}
