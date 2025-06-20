# MoneyDropMod (Developer Documentation)

Welcome to the developer documentation for **MoneyDropMod**, a C# mod for Grand Theft Auto V that allows players to spawn collectible money briefcases.

This guide is intended for developers who wish to contribute to the project, or fork it for their own modifications. For end-user installation, please see the `README.txt` in the root directory.

## Table of Contents
- [Project Overview](#project-overview)
- [Requirements](#requirements)
- [Setup and Building](#setup-and-building)
- [Usage in-game](#usage-in-game)
- [Contributing](#contributing)
- [License](#license)

---

### Project Overview

**MoneyDropMod** is built using the ScriptHookVDotNet3 (SHVDN3) API wrapper. It allows players to press a hotkey (default: `F5`) to drop a `prop_money_bag_01` prop at their location. When the player character touches this prop, it is deleted and a configurable amount of cash is added to their wallet.

- **Language:** C#
- **Framework:** .NET Framework 4.8
- **Core API:** ScriptHookVDotNet 3

### Requirements

To build and develop this mod, you will need the following software installed:

- **Microsoft Visual Studio:** 2019 or newer is recommended.
- **.NET Framework 4.8:** The Developer Pack must be installed.
- **ScriptHookVDotNet3:** The necessary `ScriptHookVDotNet3.dll` assembly is required for referencing. It is typically included in the project's `lib/` or `packages/` directory, or can be downloaded from the official SHVDN website.

### Setup and Building

1.  **Clone the Repository:**
    ```bash
    git clone <your-repo-url>
    cd MoneyDrop-Dev
    ```

2.  **Open in Visual Studio:**
    - Open the `MoneyDrop.csproj` file located in the `src/` directory with Visual Studio.

3.  **Restore Dependencies:**
    - Ensure the reference to `ScriptHookVDotNet3.dll` is correct. If it's missing, you may need to add it manually. Right-click "References" in the Solution Explorer, click "Add Reference...", and browse to the location of the DLL.

4.  **Build the Project:**
    - From the top menu in Visual Studio, select `Build > Build Solution`.
    - The output `MoneyDrop.dll` will be generated in the `bin/Debug/` or `bin/Release/` folder.

### Usage In-Game

1.  After building, copy the resulting `MoneyDrop.dll` to your GTA V `scripts/` folder.
2.  Launch the game.
3.  Press `F5` to drop a money bag.

### Contributing

Contributions are welcome! If you would like to contribute, please follow these steps:

1.  **Fork the repository.**
2.  **Create a new branch** for your feature or bug fix (`git checkout -b feature/my-new-feature`).
3.  **Commit your changes** (`git commit -am 'Add some feature'`).
4.  **Push to the branch** (`git push origin feature/my-new-feature`).
5.  **Create a new Pull Request.**

Please ensure your code is clean, well-commented where necessary, and adheres to the existing project structure.

### License

This project is licensed under the MIT License. See the `LICENSE` file in the root directory for full details.


