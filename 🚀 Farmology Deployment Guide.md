# 🚀 Farmology Deployment Guide

This guide covers deploying Farmology to Pi Network and GitHub Pages, following your preferred hosting setup.

## 📋 Prerequisites

- Unity 2021.3 LTS or newer
- Git installed and configured
- GitHub account with access to your GameChanger repository
- Pi Network developer account
- Node.js (for build tools)

## 🌐 GitHub Pages Deployment

### Step 1: Prepare WebGL Build

1. **Configure Unity for WebGL**
   ```
   File > Build Settings
   Platform: WebGL
   Player Settings:
   - Publishing Settings > Compression Format: Gzip
   - Publishing Settings > Decompression Fallback: true
   - Resolution and Presentation > WebGL Template: Default
   ```

2. **Build the Game**
   ```
   Build Settings > Build
   Output folder: /Build/WebGL/
   ```

### Step 2: Prepare GitHub Repository Structure

Based on your preferred structure, organize files as follows:

```
GameChanger/
├── index.html (main landing page)
├── css/
│   └── farmology.css
├── js/
│   └── farmology.js
├── games/
│   └── farmology/
│       ├── index.html
│       ├── Build/
│       ├── TemplateData/
│       └── StreamingAssets/
└── assets/
    └── images/
        └── farmology/
```

### Step 3: Create Game Integration Files

1. **Create games/farmology/index.html**
   ```html
   <!DOCTYPE html>
   <html lang="en-us">
   <head>
       <meta charset="utf-8">
       <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
       <title>Farmology - Pi Network Farming Game</title>
       <link rel="shortcut icon" href="TemplateData/favicon.ico">
       <link rel="stylesheet" href="TemplateData/style.css">
       <meta name="viewport" content="width=device-width, initial-scale=1.0">
       
       <!-- Pi Network SDK -->
       <script src="https://sdk.minepi.com/pi-sdk.js"></script>
   </head>
   <body>
       <div id="unity-container" class="unity-desktop">
           <canvas id="unity-canvas" width=1920 height=1080></canvas>
           <div id="unity-loading-bar">
               <div id="unity-logo"></div>
               <div id="unity-progress-bar-empty">
                   <div id="unity-progress-bar-full"></div>
               </div>
           </div>
           <div id="unity-warning"> </div>
           <div id="unity-footer">
               <div id="unity-webgl-logo"></div>
               <div id="unity-fullscreen-button"></div>
               <div id="unity-build-title">Farmology</div>
           </div>
       </div>
       
       <!-- Game Description -->
       <div class="game-info">
           <h1>🌾 Farmology</h1>
           <p>A fun farming simulation game with Pi Network integration!</p>
           <ul>
               <li>Plant and harvest crops</li>
               <li>Raise farm animals</li>
               <li>Buy premium items with Pi coins</li>
               <li>Save progress to Pi Network</li>
           </ul>
           
           <div class="controls">
               <h3>Controls:</h3>
               <p><strong>WASD</strong> - Move | <strong>Space</strong> - Pause | <strong>F5</strong> - Restart</p>
               <p><strong>1-5</strong> - Tools | <strong>E</strong> - Interact | <strong>B</strong> - Shop</p>
           </div>
       </div>
       
       <script src="Build/farmology.loader.js"></script>
       <script>
           var container = document.querySelector("#unity-container");
           var canvas = document.querySelector("#unity-canvas");
           var loadingBar = document.querySelector("#unity-loading-bar");
           var progressBarFull = document.querySelector("#unity-progress-bar-full");
           var fullscreenButton = document.querySelector("#unity-fullscreen-button");
           var warningBanner = document.querySelector("#unity-warning");

           function unityShowBanner(msg, type) {
               function updateBannerVisibility() {
                   warningBanner.style.display = warningBanner.children.length ? 'block' : 'none';
               }
               var div = document.createElement('div');
               div.innerHTML = msg;
               warningBanner.appendChild(div);
               if (type == 'error') div.style = 'background: red; padding: 10px;';
               else {
                   if (type == 'warning') div.style = 'background: yellow; padding: 10px;';
                   setTimeout(function() {
                       warningBanner.removeChild(div);
                       updateBannerVisibility();
                   }, 5000);
               }
               updateBannerVisibility();
           }

           var buildUrl = "Build";
           var loaderUrl = buildUrl + "/farmology.loader.js";
           var config = {
               dataUrl: buildUrl + "/farmology.data",
               frameworkUrl: buildUrl + "/farmology.framework.js",
               codeUrl: buildUrl + "/farmology.wasm",
               streamingAssetsUrl: "StreamingAssets",
               companyName: "GameChanger Studios",
               productName: "Farmology",
               productVersion: "1.0.0",
               showBanner: unityShowBanner,
           };

           if (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) {
               var meta = document.createElement('meta');
               meta.name = 'viewport';
               meta.content = 'width=device-width, height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes';
               document.getElementsByTagName('head')[0].appendChild(meta);
               container.className = "unity-mobile";
               canvas.className = "unity-mobile";
           } else {
               canvas.style.width = "1920px";
               canvas.style.height = "1080px";
           }

           loadingBar.style.display = "block";

           var script = document.createElement("script");
           script.src = loaderUrl;
           script.onload = () => {
               createUnityInstance(canvas, config, (progress) => {
                   progressBarFull.style.width = 100 * progress + "%";
               }).then((unityInstance) => {
                   loadingBar.style.display = "none";
                   fullscreenButton.onclick = () => {
                       unityInstance.SetFullscreen(1);
                   };
               }).catch((message) => {
                   alert(message);
               });
           };
           document.body.appendChild(script);
       </script>
   </body>
   </html>
   ```

2. **Create css/farmology.css**
   ```css
   .game-info {
       max-width: 1200px;
       margin: 20px auto;
       padding: 20px;
       background: #f5f5f5;
       border-radius: 10px;
       font-family: Arial, sans-serif;
   }

   .game-info h1 {
       color: #2c5530;
       text-align: center;
       margin-bottom: 10px;
   }

   .game-info p {
       font-size: 16px;
       line-height: 1.6;
       color: #333;
   }

   .game-info ul {
       list-style-type: none;
       padding: 0;
   }

   .game-info li {
       background: #e8f5e8;
       margin: 5px 0;
       padding: 10px;
       border-left: 4px solid #4caf50;
   }

   .controls {
       background: #fff;
       padding: 15px;
       border-radius: 5px;
       margin-top: 20px;
   }

   .controls h3 {
       color: #2c5530;
       margin-top: 0;
   }

   .controls p {
       margin: 5px 0;
       font-family: monospace;
       background: #f0f0f0;
       padding: 5px;
       border-radius: 3px;
   }

   #unity-container {
       margin: 20px auto;
   }

   @media (max-width: 768px) {
       .game-info {
           margin: 10px;
           padding: 15px;
       }
       
       #unity-container.unity-mobile {
           width: 100%;
           height: 70vh;
       }
   }
   ```

### Step 4: Update Main Index.html

Add Farmology to your main GameChanger index.html:

```html
<!-- Add to your games section -->
<div class="game-card">
    <h3>🌾 Farmology</h3>
    <p>Pi Network farming simulation game</p>
    <a href="games/farmology/" class="play-button">Play Now</a>
    <div class="game-features">
        <span class="feature">Pi Network</span>
        <span class="feature">Farming</span>
        <span class="feature">Blockchain</span>
    </div>
</div>
```

### Step 5: Deploy to GitHub Pages

1. **Commit and Push**
   ```bash
   cd /path/to/GameChanger
   git add .
   git commit -m "Add Farmology - Pi Network farming game"
   git push origin main
   ```

2. **Enable GitHub Pages**
   - Go to your repository settings
   - Scroll to "Pages" section
   - Source: Deploy from branch
   - Branch: main
   - Folder: / (root)
   - Save

3. **Verify Deployment**
   - Visit: `https://gamingmerchant.github.io/GameChanger/games/farmology/`
   - Test all game features
   - Verify Pi Network integration

## 🥧 Pi Network Integration

### Step 1: Register Pi App

1. **Pi Developer Portal**
   - Visit: https://developers.minepi.com/
   - Create new app: "Farmology"
   - App Type: Game
   - Platform: Web

2. **Configure App Settings**
   ```json
   {
     "name": "Farmology",
     "description": "A fun farming simulation game with Pi Network integration",
     "url": "https://gamingmerchant.github.io/GameChanger/games/farmology/",
     "icon": "https://gamingmerchant.github.io/GameChanger/assets/images/farmology/icon.png",
     "category": "Games",
     "tags": ["farming", "simulation", "blockchain"]
   }
   ```

### Step 2: Implement Pi SDK

1. **Add Pi SDK to HTML**
   ```html
   <script src="https://sdk.minepi.com/pi-sdk.js"></script>
   ```

2. **Initialize Pi SDK in Unity**
   ```javascript
   // Add to your Unity WebGL template
   window.PiNetwork = {
       init: function() {
           Pi.init({
               version: "2.0",
               sandbox: false // Set to true for testing
           });
       },
       
       authenticate: function() {
           return Pi.authenticate(["username", "payments"], {
               onIncompletePaymentFound: function(payment) {
                   console.log("Incomplete payment found:", payment);
               }
           });
       },
       
       createPayment: function(amount, memo) {
           return Pi.createPayment({
               amount: amount,
               memo: memo,
               metadata: { gameId: "farmology" }
           });
       }
   };
   ```

### Step 3: Configure Payment Endpoints

1. **Create Payment Handler** (for Render deployment)
   ```javascript
   // deploy to Render.com as per your preference
   const express = require('express');
   const app = express();

   app.post('/payments/approve', async (req, res) => {
       // Handle payment approval
       const { paymentId } = req.body;
       
       try {
           // Verify payment with Pi Network
           const payment = await Pi.getPayment(paymentId);
           
           if (payment.status === 'completed') {
               // Grant in-game rewards
               await grantGameRewards(payment);
               res.json({ success: true });
           }
       } catch (error) {
           res.status(400).json({ error: error.message });
       }
   });

   app.listen(process.env.PORT || 3000);
   ```

2. **Deploy to Render**
   - Connect your GitHub repository
   - Set environment variables
   - Deploy payment handler service

### Step 4: Test Pi Integration

1. **Sandbox Testing**
   ```javascript
   // Enable sandbox mode for testing
   Pi.init({
       version: "2.0",
       sandbox: true
   });
   ```

2. **Production Deployment**
   ```javascript
   // Switch to production
   Pi.init({
       version: "2.0",
       sandbox: false
   });
   ```

## 🔧 Build Automation

### GitHub Actions Workflow

Create `.github/workflows/deploy.yml`:

```yaml
name: Deploy Farmology

on:
  push:
    branches: [ main ]
    paths: [ 'games/farmology/**' ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v2
    
    - name: Setup Node.js
      uses: actions/setup-node@v2
      with:
        node-version: '16'
    
    - name: Install dependencies
      run: npm install
    
    - name: Build and optimize
      run: |
        # Optimize WebGL build
        gzip -9 games/farmology/Build/*.data
        gzip -9 games/farmology/Build/*.wasm
    
    - name: Deploy to GitHub Pages
      uses: peaceiris/actions-gh-pages@v3
      with:
        github_token: ${{ secrets.GITHUB_TOKEN }}
        publish_dir: ./
```

## 📊 Analytics and Monitoring

### Google Analytics Integration

Add to your game's HTML:

```html
<!-- Google Analytics -->
<script async src="https://www.googletagmanager.com/gtag/js?id=GA_MEASUREMENT_ID"></script>
<script>
  window.dataLayer = window.dataLayer || [];
  function gtag(){dataLayer.push(arguments);}
  gtag('js', new Date());
  gtag('config', 'GA_MEASUREMENT_ID');
</script>
```

### Pi Network Analytics

```javascript
// Track Pi Network events
function trackPiEvent(eventName, data) {
    gtag('event', eventName, {
        event_category: 'Pi Network',
        event_label: data.label,
        value: data.value
    });
}
```

## 🚀 Performance Optimization

### WebGL Optimization

1. **Compression Settings**
   ```
   Compression Format: Gzip
   Code Optimization: Size
   Strip Engine Code: true
   ```

2. **Asset Optimization**
   ```
   Texture Compression: ASTC
   Audio Compression: Vorbis
   Mesh Compression: High
   ```

### CDN Integration

Use GitHub's CDN for faster loading:

```html
<!-- Preload critical resources -->
<link rel="preload" href="Build/farmology.data" as="fetch" crossorigin="anonymous">
<link rel="preload" href="Build/farmology.wasm" as="fetch" crossorigin="anonymous">
```

## 🔒 Security Considerations

### Pi Network Security

1. **API Key Protection**
   - Never expose API keys in client-side code
   - Use environment variables
   - Implement server-side validation

2. **Payment Verification**
   - Always verify payments server-side
   - Implement anti-fraud measures
   - Log all transactions

### HTTPS Requirements

Ensure your site uses HTTPS:
- GitHub Pages automatically provides HTTPS
- Pi Network requires HTTPS for production apps

## 📱 Mobile Optimization

### Responsive Design

```css
@media (max-width: 768px) {
    #unity-container.unity-mobile {
        width: 100vw;
        height: 70vh;
    }
    
    .game-info {
        padding: 10px;
        font-size: 14px;
    }
}
```

### Touch Controls

```javascript
// Add touch-friendly controls
if ('ontouchstart' in window) {
    // Enable touch controls in Unity
    unityInstance.SendMessage('GameManager', 'EnableTouchControls');
}
```

## 🎯 Launch Checklist

### Pre-Launch
- [ ] WebGL build tested and optimized
- [ ] Pi Network integration working
- [ ] GitHub Pages deployment successful
- [ ] Mobile responsiveness verified
- [ ] Analytics tracking implemented

### Launch
- [ ] Update main GameChanger index.html
- [ ] Announce on Pi Network social channels
- [ ] Submit to Pi App Directory
- [ ] Share with gaming communities

### Post-Launch
- [ ] Monitor analytics and performance
- [ ] Collect user feedback
- [ ] Plan updates and improvements
- [ ] Engage with Pi Network community

## 🆘 Troubleshooting

### Common Issues

1. **WebGL Build Errors**
   - Check Unity console for errors
   - Verify all scripts compile
   - Test in Unity editor first

2. **Pi Network Integration Issues**
   - Verify API keys are correct
   - Check sandbox vs production settings
   - Test payment flow thoroughly

3. **GitHub Pages Issues**
   - Ensure all files are committed
   - Check file paths are correct
   - Verify HTTPS is working

### Support Resources

- **Unity Documentation**: https://docs.unity3d.com/
- **Pi Network Developer Guide**: https://developers.minepi.com/
- **GitHub Pages Help**: https://docs.github.com/en/pages

---

**Ready to launch Farmology on Pi Network! 🚀**

Your farming game is now ready for the Pi Network community to enjoy!

