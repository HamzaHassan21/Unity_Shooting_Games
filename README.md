🎮 Multiplayer FPS – Unity & Photon PUN 2

Code-Only Multiplayer Architecture Demonstration

This repository contains the core scripts and project configuration for a Unity multiplayer FPS built with Photon PUN 2.
The full Assets folder is excluded; this repo focuses on the networking logic, gameplay systems, and overall architecture.

🚀 Key Features

Photon room creation, matchmaking, and room list UI

Deterministic spawn point system

Synchronized shooting & damage via RPCs

Health syncing across all clients

Player death + automatic respawn

Score tracking and winner detection

Master Client–controlled scene loading and game flow

📂 Repository Structure
/ProjectSettings     Unity project configuration
/UserSettings        Editor preferences

/Scripts
    /Gameplay        Movement, shooting, health, respawn
    /Networking      Room logic, game flow, spawning, scoring
    /UI              UI management + room list items

🔌 Networking Overview

Built with Photon PUN 2 using a Master Client–driven model:

RPC-based damage and health sync

PhotonView ownership for authority

Master Client handles scene loading and win conditions

Spawn points and scores synced across clients

🎯 Gameplay Flow

Join/create Photon room

Master Client loads game scene

Players spawn at predefined points

Shooting triggers RPCs → damage applied

Player respawns on death

Score updates → winner selected

🛠️ How to Use

Create a new Unity project

Import Photon PUN 2

Add your Photon App ID in PhotonServerSettings

Drop in:

/Scripts

/ProjectSettings

/UserSettings

Assign scripts to your prefabs and UI elements

Ideal for learning, coursework, or showcasing multiplayer logic.

🏆 Demonstrates

Photon networking fundamentals

RPC communication patterns

Multiplayer architecture & game flow

Scene management via Master Client

📜 License

MIT License — free to use and modify.
