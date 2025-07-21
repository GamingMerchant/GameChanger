# 🌾 Farmology - Pi Network Farming Game

**A fun and engaging farming simulation game built in Unity with full Pi Network integration**

![Farmology Logo](Assets/Sprites/logo.png)

## 🎮 Game Overview

Farmology is a comprehensive farming simulation game that combines classic farming mechanics with modern blockchain integration through Pi Network. Players can build and manage their own virtual farm, grow crops, raise animals, and participate in the Pi Network ecosystem through in-game transactions and rewards.

### 🌟 Key Features

- **Complete Farming System**: Plant, water, and harvest over 8 different crop types
- **Animal Management**: Raise chickens, cows, pigs, sheep, goats, and ducks
- **Pi Network Integration**: Buy premium items with Pi cryptocurrency
- **Cloud Save System**: Save your progress to Pi Network
- **Weather System**: Dynamic weather affects crop growth and gameplay
- **Day/Night Cycle**: Time-based gameplay with realistic farming cycles
- **Leveling System**: Gain experience and unlock new features
- **Interactive Environment**: Chop trees, mine rocks, and explore your farm

## 🎯 Game Controls

### Movement
- **W, A, S, D** - Move around the farm
- **Left Shift** - Run faster

### Tools & Actions
- **1** - Hoe (till soil)
- **2** - Watering Can (water crops)
- **3** - Seeds (plant crops)
- **4** - Axe (chop trees)
- **5** - Pickaxe (mine rocks)
- **Left Click** - Use selected tool
- **E** - Interact with objects

### Game Controls
- **Space** - Pause/Unpause game
- **F5** - Restart game
- **B** - Open Shop
- **I** - Open Inventory
- **H** - Help Menu
- **C** - Controls Guide
- **ESC** - Pause Menu

## 🏗️ Technical Architecture

### Core Systems

1. **GameManager.cs** - Main game state management
2. **PlayerController.cs** - Player movement and interaction
3. **FarmingSystem.cs** - Crop management and farm tiles
4. **AnimalSystem.cs** - Animal behavior and production
5. **UISystem.cs** - User interface and menus
6. **PiNetworkIntegration.cs** - Blockchain integration
7. **EnvironmentSystem.cs** - Trees, rocks, weather, and buildings

### Pi Network Features

- **User Authentication** - Login with Pi Network account
- **Pi Payments** - Purchase premium items with Pi coins
- **Cloud Save** - Store game progress on Pi Network
- **Leaderboards** - Compare progress with other players
- **Premium Content** - Exclusive items and features for Pi users

## 🚀 Getting Started

### Prerequisites

- Unity 2021.3 LTS or newer
- Pi Network account (for blockchain features)
- Internet connection for Pi Network integration

### Installation

1. **Clone the Repository**
   ```bash
   git clone https://github.com/gamingmerchant/Farmology.git
   cd Farmology
   ```

2. **Open in Unity**
   - Launch Unity Hub
   - Click "Open Project"
   - Select the Farmology folder

3. **Configure Pi Network**
   - Open `Assets/Scripts/PiNetworkIntegration.cs`
   - Replace `your-pi-api-key` with your actual Pi API key
   - Set `isTestnet` to `false` for production

4. **Build and Run**
   - Go to File > Build Settings
   - Select your target platform
   - Click "Build and Run"

## 🌐 Pi Network Integration

### Setting Up Pi Network

1. **Register as Pi Developer**
   - Visit [Pi Developer Portal](https://developers.minepi.com/)
   - Create your developer account
   - Register your Farmology app

2. **Get API Credentials**
   - Generate your Pi API key
   - Configure your app settings
   - Set up payment endpoints

3. **Configure the Game**
   ```csharp
   // In PiNetworkIntegration.cs
   public string appId = "farmology-game";
   public string apiKey = "your-actual-pi-api-key";
   public bool isTestnet = false; // Set to false for production
   ```

### Pi Network Features Implementation

#### User Authentication
```csharp
// Authenticate user with Pi Network
piNetworkManager.AuthenticateUser();
```

#### Pi Payments
```csharp
// Purchase premium items with Pi
piNetworkManager.BuySeedPackWithPi();
piNetworkManager.BuyPremiumAnimalWithPi();
```

#### Cloud Save
```csharp
// Save/Load game data to Pi Network
piNetworkManager.SaveGameDataToPi();
piNetworkManager.LoadGameDataFromPi();
```

## 🎨 Asset Credits

This game uses free assets from the following sources:

- **Sprout Lands Asset Pack** by Cup Nooble (itch.io)
- **LPC Farm Assets** by bluecarrot16 (OpenGameArt.org)
- **LPC Crops** by bluecarrot16 (OpenGameArt.org)
- **LPC Style Farm Animals** by Daniel Eddeland (OpenGameArt.org)

All assets are used under their respective licenses (CC-BY, CC-BY-SA, GPL).

## 📱 Deployment Options

### For Pi Network

1. **Pi Browser Deployment**
   - Build as WebGL
   - Upload to Pi App Platform
   - Configure Pi SDK integration

2. **Mobile Deployment**
   - Build for Android/iOS
   - Submit to Pi App Store
   - Enable Pi Network features

### For Traditional Platforms

1. **Steam/Itch.io**
   - Build standalone version
   - Disable Pi Network features
   - Package for distribution

2. **Mobile App Stores**
   - Build for Android/iOS
   - Remove Pi Network dependencies
   - Submit to Google Play/App Store

## 🔧 Build Instructions

### WebGL Build (Recommended for Pi Network)

1. **Configure Build Settings**
   ```
   File > Build Settings
   Platform: WebGL
   Compression Format: Gzip
   ```

2. **Player Settings**
   ```
   Company Name: GameChanger Studios
   Product Name: Farmology
   Version: 1.0.0
   ```

3. **Build**
   ```
   Click "Build"
   Select output folder
   Wait for build completion
   ```

### Mobile Build

1. **Android**
   ```
   Platform: Android
   Package Name: com.gamechangerstudios.farmology
   Minimum API Level: 19
   Target API Level: 30
   ```

2. **iOS**
   ```
   Platform: iOS
   Bundle Identifier: com.gamechangerstudios.farmology
   Target iOS Version: 11.0
   ```

## 🎯 Gameplay Guide

### Starting Your Farm

1. **Tutorial Phase**
   - Learn basic controls
   - Plant your first crops
   - Meet your first animals

2. **Early Game**
   - Focus on wheat and carrots (fast-growing crops)
   - Buy chickens for steady income
   - Upgrade your tools

3. **Mid Game**
   - Expand to premium crops
   - Build barn and greenhouse
   - Invest in cows and pigs

4. **Late Game**
   - Maximize efficiency with automation
   - Participate in Pi Network economy
   - Unlock all achievements

### Farming Tips

- **Water crops daily** for 50% faster growth
- **Feed animals regularly** to maintain happiness
- **Plant diverse crops** to maximize profits
- **Check weather forecasts** to plan activities
- **Upgrade tools** to increase efficiency

### Pi Network Economy

- **Earn Pi coins** through gameplay achievements
- **Spend Pi coins** on premium content
- **Trade with other players** (future feature)
- **Participate in events** for bonus rewards

## 🏆 Achievements System

### Farming Achievements
- **Green Thumb** - Harvest 100 crops
- **Master Farmer** - Reach level 50
- **Crop Diversity** - Grow all 8 crop types
- **Speed Farmer** - Harvest 50 crops in one day

### Animal Achievements
- **Animal Lover** - Own 20 animals
- **Productive Farm** - Collect 1000 animal products
- **Happy Animals** - Maintain 100% happiness for 7 days

### Pi Network Achievements
- **Pi Pioneer** - Make your first Pi purchase
- **Blockchain Farmer** - Save game to Pi Network 10 times
- **Community Member** - Connect with 10 Pi friends

## 🔄 Update Roadmap

### Version 1.1 (Coming Soon)
- Multiplayer features
- Trading system
- Seasonal events
- New crop varieties

### Version 1.2
- Advanced automation
- Farm decorations
- Achievement rewards
- Social features

### Version 2.0
- 3D graphics upgrade
- VR support
- Advanced Pi Network features
- Cross-platform play

## 🐛 Known Issues

- Weather effects may cause minor frame drops on older devices
- Pi Network authentication requires stable internet connection
- Some asset animations may not loop perfectly

## 🤝 Contributing

We welcome contributions to Farmology! Please follow these guidelines:

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

### Asset Licenses
- Sprout Lands: Free for non-commercial use
- LPC Assets: CC-BY-SA 3.0+
- Custom code: MIT License

## 📞 Support

For support and questions:

- **Email**: support@gamechangerstudios.com
- **Discord**: [Farmology Community](https://discord.gg/farmology)
- **Pi Network**: @FarmologyGame
- **GitHub Issues**: [Report bugs here](https://github.com/gamingmerchant/Farmology/issues)

## 🙏 Acknowledgments

- **Pi Network Team** for blockchain integration support
- **Unity Technologies** for the game engine
- **OpenGameArt Community** for free assets
- **Cup Nooble** for the beautiful Sprout Lands assets
- **Our Beta Testers** for valuable feedback

---

**Made with ❤️ by GameChanger Studios for the Pi Network Community**

*Farmology - Where farming meets the future of cryptocurrency!*

