# PeekLab

PeekLab is a Unity prototype for practicing and inspecting movement around cover in a first-person environment. Record a peek, replay it from an opponent's viewpoint, and compare how quickly the movement becomes visible with how stable the first shot was.

## Current prototype

- First-person movement with collision and mouse look
- Camera-based raycast shooting and reaction-time targets
- Movement and shot recording
- Smooth movement replay through a visible ghost
- Switchable player and opponent viewpoints
- Multi-point visibility detection for the ghost's head and shoulders
- First-visibility and exposure-to-shot timing
- Shot stability, movement speed, and stop-before-shot measurement
- Repeatable countdown, recording, review, replay, and results states
- Attempt history with best-result comparison
- Peek labels for wide swings, slow peeks, jiggle peeks, and custom attempts
- In-game status and results display

## Controls

- **WASD:** Move
- **Mouse:** Look
- **Left click:** Shoot
- **1-4:** Select the peek label
- **R:** Start or stop an attempt
- **P:** Replay the latest recording
- **V:** Switch between player and opponent viewpoints
- **Escape:** Release the cursor

## Development updates

| Milestone | What changed | Commit |
| --- | --- | --- |
| First-person foundation | Added movement, collision, mouse pitch, and player yaw. | [`386fc99`](https://github.com/YolunaFruit/PeekLab/commit/386fc99) |
| Target and replay prototype | Added timed targets, reaction measurement, recording, and smooth ghost replay. | [`77fa58f`](https://github.com/YolunaFruit/PeekLab/commit/77fa58f) |
| Opponent-view analysis | Added viewpoint switching, multi-point visibility checks, and exposure timing. | [`18bb25b`](https://github.com/YolunaFruit/PeekLab/commit/18bb25b) |
| Repeatable training sessions | Added session states, countdowns, input locking, results, and attempt comparisons. | [`88094ce`](https://github.com/YolunaFruit/PeekLab/commit/88094ce) |
| Shot stability analysis | Added movement speed and stop-before-shot measurements. | [`8861f97`](https://github.com/YolunaFruit/PeekLab/commit/8861f97) |
| Technique comparison | Added selectable peek labels and saved each technique with its result. | [`7a58830`](https://github.com/YolunaFruit/PeekLab/commit/7a58830) |

## Project status

The core training loop works: select a technique, record a peek, inspect it from the opponent's view, and review timing and shot-stability results. The next milestone focuses on visual clarity, followed by real slow-walk and crouch movement.

## Built with

- Unity 6.6 (6000.6.4f1)
- C#
- Unity Input System
- TextMesh Pro

## Asset note

The current prototype uses Unity primitive geometry and contains no third-party game assets.
