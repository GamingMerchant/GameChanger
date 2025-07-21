// HTML5 Game Demo - Unity Alternative
class Game {
    constructor() {
        this.canvas = document.getElementById('gameCanvas');
        this.ctx = this.canvas.getContext('2d');
        this.scoreElement = document.getElementById('score');
        this.statusElement = document.getElementById('status');
        
        // Game state
        this.isPaused = false;
        this.score = 0;
        this.gameRunning = true;
        
        // Player object
        this.player = {
            x: 400,
            y: 300,
            width: 30,
            height: 30,
            speed: 5,
            color: '#4CAF50'
        };
        
        // Collectibles array
        this.collectibles = [];
        this.collectibleSpawnTimer = 0;
        
        // Enemies array
        this.enemies = [];
        this.enemySpawnTimer = 0;
        
        // Input handling
        this.keys = {};
        this.setupEventListeners();
        
        // Start game loop
        this.gameLoop();
    }
    
    setupEventListeners() {
        // Keyboard input
        document.addEventListener('keydown', (e) => {
            this.keys[e.code] = true;
            
            // Handle special keys
            if (e.code === 'Space') {
                e.preventDefault();
                this.togglePause();
            }
            
            if (e.code === 'F5') {
                e.preventDefault();
                this.restart();
            }
        });
        
        document.addEventListener('keyup', (e) => {
            this.keys[e.code] = false;
        });
    }
    
    togglePause() {
        this.isPaused = !this.isPaused;
        this.statusElement.textContent = this.isPaused ? 'Game Paused' : 'Game Running';
    }
    
    restart() {
        // Reset game state
        this.score = 0;
        this.isPaused = false;
        this.gameRunning = true;
        this.player.x = 400;
        this.player.y = 300;
        this.collectibles = [];
        this.enemies = [];
        this.collectibleSpawnTimer = 0;
        this.enemySpawnTimer = 0;
        this.statusElement.textContent = 'Game Running';
        this.updateScore();
    }
    
    update() {
        if (this.isPaused || !this.gameRunning) return;
        
        // Player movement using WASD
        if (this.keys['KeyW'] && this.player.y > 0) {
            this.player.y -= this.player.speed;
        }
        if (this.keys['KeyS'] && this.player.y < this.canvas.height - this.player.height) {
            this.player.y += this.player.speed;
        }
        if (this.keys['KeyA'] && this.player.x > 0) {
            this.player.x -= this.player.speed;
        }
        if (this.keys['KeyD'] && this.player.x < this.canvas.width - this.player.width) {
            this.player.x += this.player.speed;
        }
        
        // Spawn collectibles
        this.collectibleSpawnTimer++;
        if (this.collectibleSpawnTimer > 60) { // Every 60 frames (1 second at 60fps)
            this.spawnCollectible();
            this.collectibleSpawnTimer = 0;
        }
        
        // Spawn enemies
        this.enemySpawnTimer++;
        if (this.enemySpawnTimer > 120) { // Every 120 frames (2 seconds at 60fps)
            this.spawnEnemy();
            this.enemySpawnTimer = 0;
        }
        
        // Update collectibles
        this.collectibles.forEach((collectible, index) => {
            // Check collision with player
            if (this.checkCollision(this.player, collectible)) {
                this.collectibles.splice(index, 1);
                this.score += 10;
                this.updateScore();
            }
        });
        
        // Update enemies
        this.enemies.forEach((enemy, index) => {
            // Move enemy towards player
            const dx = this.player.x - enemy.x;
            const dy = this.player.y - enemy.y;
            const distance = Math.sqrt(dx * dx + dy * dy);
            
            if (distance > 0) {
                enemy.x += (dx / distance) * enemy.speed;
                enemy.y += (dy / distance) * enemy.speed;
            }
            
            // Check collision with player
            if (this.checkCollision(this.player, enemy)) {
                this.gameRunning = false;
                this.statusElement.textContent = 'Game Over! Press F5 to restart';
            }
        });
    }
    
    spawnCollectible() {
        this.collectibles.push({
            x: Math.random() * (this.canvas.width - 20),
            y: Math.random() * (this.canvas.height - 20),
            width: 20,
            height: 20,
            color: '#FFD700'
        });
    }
    
    spawnEnemy() {
        const side = Math.floor(Math.random() * 4);
        let x, y;
        
        switch (side) {
            case 0: // Top
                x = Math.random() * this.canvas.width;
                y = -20;
                break;
            case 1: // Right
                x = this.canvas.width + 20;
                y = Math.random() * this.canvas.height;
                break;
            case 2: // Bottom
                x = Math.random() * this.canvas.width;
                y = this.canvas.height + 20;
                break;
            case 3: // Left
                x = -20;
                y = Math.random() * this.canvas.height;
                break;
        }
        
        this.enemies.push({
            x: x,
            y: y,
            width: 25,
            height: 25,
            speed: 2,
            color: '#FF4444'
        });
    }
    
    checkCollision(rect1, rect2) {
        return rect1.x < rect2.x + rect2.width &&
               rect1.x + rect1.width > rect2.x &&
               rect1.y < rect2.y + rect2.height &&
               rect1.y + rect1.height > rect2.y;
    }
    
    render() {
        // Clear canvas
        this.ctx.fillStyle = '#000';
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
        
        // Draw player
        this.ctx.fillStyle = this.player.color;
        this.ctx.fillRect(this.player.x, this.player.y, this.player.width, this.player.height);
        
        // Draw collectibles
        this.collectibles.forEach(collectible => {
            this.ctx.fillStyle = collectible.color;
            this.ctx.fillRect(collectible.x, collectible.y, collectible.width, collectible.height);
        });
        
        // Draw enemies
        this.enemies.forEach(enemy => {
            this.ctx.fillStyle = enemy.color;
            this.ctx.fillRect(enemy.x, enemy.y, enemy.width, enemy.height);
        });
        
        // Draw pause overlay
        if (this.isPaused) {
            this.ctx.fillStyle = 'rgba(0, 0, 0, 0.7)';
            this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
            
            this.ctx.fillStyle = '#fff';
            this.ctx.font = '48px Arial';
            this.ctx.textAlign = 'center';
            this.ctx.fillText('PAUSED', this.canvas.width / 2, this.canvas.height / 2);
            this.ctx.textAlign = 'left';
        }
        
        // Draw game over overlay
        if (!this.gameRunning) {
            this.ctx.fillStyle = 'rgba(255, 0, 0, 0.7)';
            this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
            
            this.ctx.fillStyle = '#fff';
            this.ctx.font = '48px Arial';
            this.ctx.textAlign = 'center';
            this.ctx.fillText('GAME OVER', this.canvas.width / 2, this.canvas.height / 2);
            this.ctx.font = '24px Arial';
            this.ctx.fillText('Press F5 to restart', this.canvas.width / 2, this.canvas.height / 2 + 50);
            this.ctx.textAlign = 'left';
        }
    }
    
    updateScore() {
        this.scoreElement.textContent = `Score: ${this.score}`;
    }
    
    gameLoop() {
        this.update();
        this.render();
        requestAnimationFrame(() => this.gameLoop());
    }
}

// Start the game when page loads
window.addEventListener('load', () => {
    new Game();
});

