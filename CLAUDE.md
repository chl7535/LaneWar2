# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project state

This is a freshly created Unity project (`LaneWar2`) still on the stock **Universal 3D** template — no gameplay code has been written yet. `Assets/` currently only contains the default URP settings, sample scene, and the template's tutorial Readme. When implementing features, you are largely starting from a blank slate rather than extending existing systems.

## Repository layout

The git repo root (`E:\unity3d-LaneWar2`) contains a single Unity project in the `LaneWar2/` subfolder — note the doubled name (repo root `unity3d-LaneWar2` vs. project folder `LaneWar2`). Always resolve Unity paths (e.g. `Assets/...`) relative to `LaneWar2/`, not the repo root.

- `LaneWar2/Assets/` — project assets (scenes, scripts, settings). `Assets/Scenes/SampleScene.unity` is the only scene.
- `LaneWar2/Packages/manifest.json` — package dependencies.
- `LaneWar2/ProjectSettings/` — editor/build settings, including `ProjectVersion.txt` (Unity **6000.3.18f1**, i.e. Unity 6.3).
- `LaneWar2/Library/`, `Temp/`, `Logs/`, `UserSettings/` — Unity-generated caches, gitignored; never edit or hand-craft files here.

## Working with the Unity Editor via MCP

This project has the `com.coplaydev.unity-mcp` package installed, and an MCP server (`UnityMCP`) is available in this environment. This is the primary way to interact with the running Unity Editor — prefer it over hand-editing `.unity`/`.prefab`/`.asset` YAML files directly, since those are easy to corrupt by hand and the Editor won't pick up out-of-band changes cleanly.

- Use the `unity-mcp-skill` skill for orchestration guidance before doing any nontrivial Editor automation (creating/editing GameObjects, scenes, prefabs, materials, scripts).
- Check the `mcpforunity://custom-tools` resource for any project-specific custom tools before assuming only the built-in tool set is available.
- After creating/editing a C# script (whether via the MCP script tools or the `Write`/`Edit` tools), run `read_console` to check for compiler errors before relying on the new type existing — Unity needs a domain reload to compile, and you can poll `editor_state`'s `isCompiling` field.
- New scenes should always include a Camera and a directional Light.

## Building, testing, running

There are no command-line build/test scripts in this repo (no Makefile, npm scripts, or CI config) — all build and test workflows go through the Unity Editor:

- **Tests**: use the MCP `run_tests` tool (Unity Test Framework is installed via `com.unity.test-framework`), or the Editor's Test Runner window. There are no tests in the project yet, so there's no existing suite/convention to match — pick EditMode vs. PlayMode based on what the code under test needs.
- **Builds**: use the MCP `manage_build` tool, or Editor menu `File > Build Settings`.
- **Play mode**: use `manage_editor` (MCP) to enter/exit/pause play mode rather than instructing the user to click Play, when the goal is to verify behavior programmatically.

## C# script conventions

No scripts exist yet to infer conventions from. Default to standard Unity/C# conventions: PascalCase for types/methods/public fields, one MonoBehaviour per file matching the filename, scripts under `Assets/Scripts/` (create this folder on first use since it doesn't exist).
