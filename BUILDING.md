# Playing a build without opening Unity

Every push to `claude_start` (or `start_new`) builds the game on GitHub Actions
(`.github/workflows/build.yml`). You can also start it by hand from the
**Actions** tab: pick **Build and publish**, then **Run workflow**.

- **Browser (WebGL):** published to https://novvator.github.io/ImHungry/
  Works on desktop (the mouse acts as your finger) and in a phone browser
  (real touch, including two-finger stages).
- **Android:** open the finished run in the Actions tab and download the
  `ImHungry-apk` artifact. Unzip it and install the `.apk` on your phone
  (allow "install unknown apps" for your browser or file manager).

The first build takes 20-40 minutes. Later builds reuse a cached `Library`
folder and are much faster.

## One-time setup

### 1. Unity license secrets

Unity has to be activated inside the build machine, which needs your Unity
account. A free Personal license is fine.

1. On your PC, open Unity Hub, sign in, and make sure a Personal license is
   active (Hub > Settings > Licenses > Add > Get a free personal license).
2. Open the license file in a text editor:
   - Windows: `C:\ProgramData\Unity\Unity_lic.ulf`
   - macOS: `/Library/Application Support/Unity/Unity_lic.ulf`
   - Linux: `~/.local/share/unity3d/Unity/Unity_lic.ulf`
3. On GitHub go to the repo **Settings > Secrets and variables > Actions >
   New repository secret** and add three secrets:
   - `UNITY_LICENSE`: the entire contents of `Unity_lic.ulf`
   - `UNITY_EMAIL`: the email of your Unity account
   - `UNITY_PASSWORD`: its password (if you sign in to Unity with Google or
     Apple, set a password at id.unity.com first)

### 2. Turn on GitHub Pages

1. Repo **Settings > Pages > Build and deployment > Source**: choose
   **GitHub Actions**.
2. Repo **Settings > Environments > github-pages > Deployment branches**: add
   `claude_start` (and `start_new` if you build from it too; by default only
   `master` may publish).

After that, re-run the workflow (or push) and the link above goes live.

## Mouse support

`Assets/TouchInput.cs` wraps `Input.touches`: real touches are used when there
are any, otherwise the left mouse button is reported as finger 0. All touch
scripts read `TouchInput.touches`, so the same code works on phones, in the
browser and in the Editor's Game view. Two-finger stages need a real touch
screen.
