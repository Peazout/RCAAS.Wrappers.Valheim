# RCAAS.Wrappers.Valheim

[![.NET 9](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download)
[![License](https://img.shields.io/badge/license-Dubratz.net-lightgrey.svg)](https://dubratz.net)

A Valheim dedicated server wrapper plugin for **RCAAS**, enabling automated management, monitoring, and control of Valheim game servers through the RCAAS application.

---

## 📋 Table of Contents

- [About](#about)
- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [Configuration](#configuration)
- [Contributing](#contributing)
- [License](#license)

---

## About

**RCAAS.Wrappers.Valheim** is a plugin for the RCAAS application that provides integration with Valheim dedicated servers. It handles server lifecycle management, player tracking, world backups.

---

## Features

### Player Tracking
- **Player Connection Events** (handshake, login, logout, disconnect)
- **Session Logging** with detailed player activity tracking

### Configuration
- **Comprehensive Server Settings**
  - Server name and password
  - World name and save directory
  - Network port configuration
  - Public server visibility
- **Game Modifiers Support**
  - Combat difficulty
  - Death penalty settings
  - Resource availability
  - Raid frequency
  - Portal restrictions

---

## Requirements

### Runtime Requirements
- **.NET 9.0** or higher
- **Windows** operating system (Valheim dedicated server requirement)
- **RCAAS.Core** package (v1.1.0.144 or higher)
- **SteamCMD** for server installation
- **Newtonsoft.Json** for configuration serialization

### Development Requirements
- **Visual Studio 2022** (v17.8+) or **Visual Studio 2026** (v18.0+)
- **.NET 9 SDK**

---

## Installation

**Copy the DLL** to your RCAAS plugins directory:
   ```
   RCAAS/Plugins/RCAAS.Wrappers.Valheim.dll
   ```

**Restart RCAAS** to load the plugin

---

## Configuration

### Basic Configuration

The plugin uses JSON-based configuration through the `ValheimArgsExt` class:

```json
{
  "Name": "My Valheim Server",
  "Port": 2456,
  "Password": "your-secure-password",
  "Combat": "normal",
  "DeathPenalty": "normal",
  "Resources": "normal",
  "Raids": "normal",
  "Portals": "normal"
}
```

### Configuration Options

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Name` | string | Required | Server name displayed in the server list |
| `Port` | int | 2456 | UDP port for server connections |
| `Password` | string | Required | Server password (keep secure!) |
| `Combat` | string | `null` | Combat difficulty modifier |
| `DeathPenalty` | string | `null` | Death penalty severity |
| `Resources` | string | `null` | Resource availability modifier |
| `Raids` | string | `null` | Raid frequency modifier |
| `Portals` | string | `null` | Portal restrictions |

### Game Modifier Values

Each modifier accepts the following values:
- `"veryeasy"` - Easiest difficulty
- `"easy"` - Easy difficulty
- `"normal"` - Default difficulty
- `"hard"` - Hard difficulty
- `"veryhard"` - Hardest difficulty

---
### World Backup

World files are automatically stored in:
```
{RCAAS_ROOT}/CmdApps/{ServerId}/World/
```

The plugin automatically handles world backups during server operations.

---

## Troubleshooting

### Server Won't Start

**Issue:** Server process fails to start

**Solutions:**
1. Verify SteamCMD is installed and accessible
2. Check that port 2456 (or configured port) is not in use
3. Ensure valheim_server.exe exists in the installation directory
4. Check RCAAS logs for detailed error messages

### Players Can't Connect

**Issue:** Server running but players can't join

**Solutions:**
1. Verify server password matches
2. Check firewall settings (UDP port must be open)
3. Ensure server is set to public (`-public 1`)
4. Verify Steam authentication is working

### World Not Saving

**Issue:** World progress is lost on restart

**Solutions:**
1. Check world save directory has write permissions
2. Verify disk space is available
3. Check logs for save errors

---

## Changelog

### Version 1.1.0.x

#### Added
- ✅ .NET 9 support

---

## Acknowledgments

- **Iron Gate Studio** for creating Valheim
- **Valve Corporation** for SteamCMD
- **RCAAS Team** for the core platform
- **Contributors** for their valuable input
