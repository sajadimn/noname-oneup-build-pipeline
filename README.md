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
`Assets/Settings/BuildPipeline/Market~` is created empty by the package. Put there the files that differ per store build, such as the store `AndroidManifest.xml`. The `~` at the end makes Unity ignore the folder, so nothing in it ships by itself or compiles into the game; a build config copies a file into the project only for that build, and the project is restored afterwards. A global Git ignore rule such as `*~` would also skip this folder, so the package writes `Assets/Settings/BuildPipeline/.gitignore` (with `!/Market~/` and `!/Market~/**`) when it is missing; commit that file and the folder is tracked on every computer without touching your root `.gitignore`.

A file only enters a build when a build config attaches it: `manifestPath`, `resPath`, `marketFilesPath`, or a row in `replaceExtraFile` or `replaceExtraDirectory` with where it comes from and where it goes. A missing file stops the build before it starts.

Files that come from another package, such as the `Stores~` folder of `noname-oneup-marketplace` (store jars and manifests Unity must not import), are written as `Packages/<package name>/...`. The build pipeline resolves that to the folder where the package is installed on the computer. Never store a `Library/PackageCache/<name>@<hash>` path: for a tarball package the hash differs on every computer, so the build breaks anywhere else.

## Bundle signer
Turn on `bundleSignerState` to create the signature file for Cafe Bazaar AAB builds. When the package is imported, every config with `bundleSignerState` on and an empty `bundleSignerJarFile` (or an old `Library/PackageCache/...` path) gets `Packages/noname-oneup-build-pipeline/Tools~/BundleSigner/bundlesigner-0.1.13.jar`, the jar in the package, written in the portable `Packages/...` form that works on every computer. Any other path you set is kept.

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
