# Endless mode and Google Play leaderboard setup

The Endless mode works out of the box (it saves a local best score). Posting to a Google Play Games leaderboard needs the steps below, which only you can do because they need your Play Console account.

## How Endless plays

- Start from the new **ENDLESS** button on the main menu (scene `Assets/Scenes/Endless.unity`).
- 30 seconds on the clock, which starts when you touch the first start circle.
- Each food eaten scores `10 x multiplier` and adds time (3s at first, shrinking by 0.1s per food down to 1s).
- The multiplier goes up by 1 every 3 foods in a row (max x5). Slipping off the path resets it.
- After 5 foods the patterns start rotating and flipping.
- When time runs out you see your score and best, and the score is posted to the leaderboard.

All of these numbers can be changed in the Inspector on the `EndlessMode` object in the Endless scene.

## 1. Play Console

1. Upload a build of the game to an internal testing track (Play Games Services needs the app to exist in Play Console).
2. Go to **Grow users > Play Games Services > Setup and management > Configuration** and create a Play Games Services project for the game.
3. Under **Credentials**, add an Android credential. Use your package name and the SHA-1 of your **upload key**, and also add one for the **app signing key** (found under **Setup > App signing**). Without the right SHA-1 sign-in fails silently.
4. Under **Leaderboards**, create one leaderboard:
   - Name: `Endless`
   - Format: Numeric, no decimals
   - Ordering: Larger is better
5. Under **Testers**, add your own Google account so you can test before publishing.
6. Publish the Play Games Services configuration (the button on the Publishing page). Testers can use it before that, everyone else needs it published.

## 2. Unity

1. Download the latest `GooglePlayGamesPlugin-*.unitypackage` from https://github.com/playgameservices/play-games-plugin-for-unity/releases and import it (Assets > Import Package > Custom Package).
2. Let the External Dependency Manager resolve Android dependencies (Assets > External Dependency Manager > Android Resolver > Force Resolve).
3. In Play Console, on the Leaderboards page, click **Get resources** and copy the Android XML.
4. In Unity: **Window > Google Play Games > Setup > Android setup**. Paste the XML, set the constants class name to `GPGSIds`, and click Setup.
5. Open `Assets/LeaderboardService.cs` and replace
   `"REPLACE_WITH_ENDLESS_LEADERBOARD_ID"` with `GPGSIds.leaderboard_endless` (or paste the ID string, it looks like `CgkI...`).
6. **Project Settings > Player > Android > Other Settings > Scripting Define Symbols**: add `GPGS`.
   Until this is set, all leaderboard code is switched off so the project still compiles without the plugin.
7. Build to a device that is signed in with a tester account. Note that sign-in does not work in the Unity editor.

## How it behaves

- The game signs in silently when Endless starts. Play Games v2 signs players in automatically, so most players never see a prompt.
- If a score can't be posted (offline, not signed in), the best unsent score is kept and sent the next time sign-in succeeds.
- The **LEADERBOARD** button on the results screen opens the Play Games leaderboard and asks the player to sign in if needed. It only appears once the leaderboard ID is set and `GPGS` is defined.

## Things to check in the editor

These were made by editing the scene files directly, so give them a quick look:

- Main menu: the ENDLESS button sits between PLAY and TRAILS, and TRAILS was moved down a bit and made slightly smaller to fit.
- Results screen: the score text and LEADERBOARD button are created in code (`EndlessModeScript.BuildResultsUI`). Move or restyle them there if they overlap the burger art.
