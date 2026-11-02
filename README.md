Build Pipeline Unity Package
===

## Description
This package contains a build window and per-store build configs for Android and iOS. Keys, keystores and game values stay in your project, never in this package.

## How to use
1. Add this package. It creates `BuildTypesConfig` in `Assets/Settings/BuildPipeline`.
2. Use `OneUp/Build/Create Android Build Config` or `Create iOS Build Config`, rename the asset, fill it, and add it to `BuildTypesConfig`.
3. Open `OneUp/Build/Build Setting` and build.

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
