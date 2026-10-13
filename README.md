# PeekLab

PeekLab is an early Unity prototype for practicing and inspecting movement around cover in a first-person environment. The longer-term goal is to let a player record a peek, replay it from an opposing viewpoint, and compare the timing of both attempts.

## Current prototype

- First-person movement with collision
- Mouse-controlled camera pitch and player yaw
- A simple practice scene with a wall and opposing target
- Movement relative to the player's facing direction
- Camera-based raycast shooting
- Randomized target appearances with reaction-time measurement
- Player position and rotation recording
- Smooth movement replay through a visible ghost

## Controls

- **WASD:** Move
- **Mouse:** Look
- **Left click:** Shoot
- **R:** Start or stop movement recording
- **P:** Replay the latest movement recording
- **Escape:** Release the cursor after leaving Play mode

## Project status

This is an early learning prototype. The core movement recording and ghost replay loop is working. Opposing-viewpoint scenarios, replay controls, and performance summaries are planned next.

## Built with

- Unity 6.6 (6000.6.4f1)
- C#
- Unity Input System

## Asset note

The current prototype uses Unity primitive geometry and contains no third-party game assets.
