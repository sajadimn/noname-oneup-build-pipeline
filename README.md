Build Pipeline Unity Package
===

## Description
This package contains a build window and per-store build configs for Android and iOS. Keys, keystores and game values stay in your project, never in this package.

## How to use
1. Add this package. It creates `BuildTypesConfig` in `Assets/Settings/BuildPipeline`. When `BuildTypesConfig` has no build types yet, it also creates one config per store and adds them to it: `InGame`, `CafeBazaarApk`, `CafeBazaarAAB`, `CafeBazaarDevelop`, `MyKetApk`, `MyKetDevelop`, `GooglePlayApk`, `GooglePlayAAB`, `GooglePlayDevelop`, `ZarinPalApk`, `ZarinPalDevelop`, `AppStore`, `AppStoreDevelop`.
2. Fill your game's values in each config: project, company, product and package name, and, when the game needs them, manifest, market files, Firebase, keystore and Google Play Games. Store values are already set: build type, apk/aab/Xcode, Develop flag, store defines (`CAFE_BAZAAR`, `MY_KET`, `GOOGLE_PLAY`, `ZARIN_PAL`, `IN_GAME`, `APP_STORE`, plus `DEVELOP`), SDK, IL2CPP, ARMv7 + ARM64, iOS 12, and the bundle signer for Cafe Bazaar AAB. Scenes come from the enabled scenes in Build Settings when a config is created.
3. Open `OneUp/Build/Build Setting` and build.

`OneUp/Build/Create Store Build Configs` adds any store config that is missing, and its row in `BuildTypesConfig`. Configs and rows you already have are never changed. `Create Android Build Config` and `Create iOS Build Config` make an empty config for anything else.

## Files for one build
Keep them in `Assets/Settings/BuildPipeline/Market~`, which the package creates empty. Unity ignores `~` folders, so nothing there ships by itself. To track it in Git despite a global `*~` ignore rule, add `!/Assets/Settings/BuildPipeline/Market~/**` to your `.gitignore`. A file only enters a build when a build config attaches it: `manifestPath`, `resPath`, `marketFilesPath`, or a row in `replaceExtraFile` or `replaceExtraDirectory` with where it comes from and where it goes. A missing file stops the build before it starts.

## Bundle signer
Turn on `bundleSignerState` to create the signature file for Cafe Bazaar AAB builds. An empty `bundleSignerJarFile` uses the jar in `Tools~/BundleSigner`.

## Optional packages
| Package | What it adds | Without it |
|---|---|---|
| `com.unity.addressables` | `OneUp/Build/Addressable Build Setting`, and Addressable profiles in builds | A config with `addressableUpdateBuildState` on stops with an error |
| `com.google.play.games` | Sets up Google Play Games for configs with `hasGooglePlayGames` on | Those configs stop with an error |

Install Google Play Games with `OneUp/Build/Install Google Play Games`. Its version is in `BuildTypesConfig`.

## Options in BuildTypesConfig
- `cleanBeeCache`: deletes `Library/Bee` before each build. Turn it off for faster builds.
- `restoreFilesAfterBuild`: puts back the files, folders and Player Settings a build changes. The version and build number stay.
- `restorePaths`: extra paths to restore, such as files an SDK rewrites during a build.
