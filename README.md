# imhungry

![Gameplay Screenshot](imhungry%20game%20assets/Screenshot_1.webp)

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [How to Play](#how-to-play)
- [Development](#development)

## Overview

**imhungry** is a fun and engaging mobile game built with Unity where players navigate patterns using touch controls to complete checkpoints. The objective is to follow the sequence of checkpoints within a time limit, using intuitive touch mechanics. This game features multiple levels, each with unique patterns that challenge the player's reflexes and pattern recognition skills.

![Gameplay Screenshot 2](imhungry%20game%20assets/Screenshot_2.webp) ![Gameplay Screenshot 3](imhungry%20game%20assets/Screenshot_3.webp)

## Features

- **Touch-Based Controls**: Use intuitive touch interactions to navigate patterns and complete challenges.
- **Multiple Levels**: Progress through various levels with increasing complexity and unlock new worlds.
- **Dynamic UI**: Includes an interactive pause menu and pop-up messages for achievements.
- **Audio Feedback**: Engaging sound effects to enhance the gaming experience, providing audio cues for actions.
- **Challenging Gameplay**: Time-based challenges that require quick thinking and precision.

## How to Play

1. **Controls:**
   - Touch Controls: Use your finger to touch and navigate the patterns.
   - Checkpoints: Tap to hit checkpoints in the correct sequence.

2. **Objective:**
   - Complete the sequence of checkpoints within the allotted time to unlock new levels.
   - Avoid resetting the pattern by following the correct path and hitting each checkpoint in order.
    
3. **Tips:**
   - Watch the pattern carefully and plan your moves.
   - You can use the pause button to take a break.
   - Keep an eye on the timer and strategize accordingly.

## Development

**Technologies Used:**

  - Unity: Version 2022.3.7f1 (URP)
  - C# Scripting: For game logic and mechanics
  - UI Design: Built with Unity's UI system for interactive elements

**Game Architecture:**
  - Pattern scripts: `PatternGameBase` runs the shared stage loop (food spawning, pattern rotation, timer, scoring, win/lose). `PatternScript` (one finger) and `PatternDualScript` (two fingers) only handle touch input and checkpoints.
  - Progression: `StageProgress` holds the world/stage layout (5 stages per world) and the saved unlock state used by the menus and `LevelChanger`.
  - Foods: `FoodSpawner` loads sprites from `Resources/foods/<name>/<name>1-3`. Each stage scene picks its foods in the spawner's Food Name Overrides.
  - UI Management: Includes scripts for handling the pause menu and pop-up notifications.
  - Audio Manager: Plays sound effects for various game actions like hitting checkpoints or failing a pattern.
