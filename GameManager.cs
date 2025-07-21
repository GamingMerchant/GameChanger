using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Farmology
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        public static GameManager Instance;
        
        [Header("Game State")]
        public bool isPaused = false;
        public int playerMoney = 100;
        public int playerLevel = 1;
        public int playerExperience = 0;
        public int experienceToNextLevel = 100;
        
        [Header("Time System")]
        public float dayLength = 300f; // 5 minutes per day
        public int currentDay = 1;
        public float currentTime = 0f;
        public bool isNight = false;
        
        [Header("UI References")]
        public GameObject pauseMenu;
        public TMPro.TextMeshProUGUI moneyText;
        public TMPro.TextMeshProUGUI dayText;
        public TMPro.TextMeshProUGUI levelText;
        
        [Header("Audio")]
        public AudioSource backgroundMusic;
        public AudioClip[] musicTracks;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            InitializeGame();
            StartCoroutine(DayNightCycle());
        }
        
        private void Update()
        {
            HandleInput();
            UpdateUI();
        }
        
        private void HandleInput()
        {
            // Spacebar to pause/unpause (user preference)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TogglePause();
            }
            
            // F5 to restart game (user preference)
            if (Input.GetKeyDown(KeyCode.F5))
            {
                RestartGame();
            }
            
            // ESC for menu
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }
        
        public void TogglePause()
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f;
            
            if (pauseMenu != null)
            {
                pauseMenu.SetActive(isPaused);
            }
            
            // Pause/unpause audio
            if (backgroundMusic != null)
            {
                if (isPaused)
                    backgroundMusic.Pause();
                else
                    backgroundMusic.UnPause();
            }
        }
        
        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        private void InitializeGame()
        {
            Time.timeScale = 1f;
            isPaused = false;
            
            // Initialize background music
            if (backgroundMusic != null && musicTracks.Length > 0)
            {
                backgroundMusic.clip = musicTracks[0];
                backgroundMusic.Play();
            }
        }
        
        private IEnumerator DayNightCycle()
        {
            while (true)
            {
                currentTime += Time.deltaTime;
                
                if (currentTime >= dayLength)
                {
                    currentTime = 0f;
                    currentDay++;
                    OnNewDay();
                }
                
                // Determine if it's night (last 25% of the day)
                isNight = (currentTime / dayLength) > 0.75f;
                
                yield return null;
            }
        }
        
        private void OnNewDay()
        {
            Debug.Log($"New day started: Day {currentDay}");
            
            // Grow all crops
            CropManager cropManager = FindObjectOfType<CropManager>();
            if (cropManager != null)
            {
                cropManager.GrowAllCrops();
            }
            
            // Add daily income from animals
            AnimalManager animalManager = FindObjectOfType<AnimalManager>();
            if (animalManager != null)
            {
                int dailyIncome = animalManager.GetDailyIncome();
                AddMoney(dailyIncome);
            }
        }
        
        public void AddMoney(int amount)
        {
            playerMoney += amount;
            Debug.Log($"Money added: {amount}. Total: {playerMoney}");
        }
        
        public bool SpendMoney(int amount)
        {
            if (playerMoney >= amount)
            {
                playerMoney -= amount;
                Debug.Log($"Money spent: {amount}. Remaining: {playerMoney}");
                return true;
            }
            return false;
        }
        
        public void AddExperience(int amount)
        {
            playerExperience += amount;
            
            while (playerExperience >= experienceToNextLevel)
            {
                playerExperience -= experienceToNextLevel;
                playerLevel++;
                experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.2f);
                Debug.Log($"Level up! New level: {playerLevel}");
            }
        }
        
        private void UpdateUI()
        {
            if (moneyText != null)
                moneyText.text = $"Money: ${playerMoney}";
                
            if (dayText != null)
                dayText.text = $"Day {currentDay}";
                
            if (levelText != null)
                levelText.text = $"Level {playerLevel}";
        }
        
        public float GetTimeOfDay()
        {
            return currentTime / dayLength;
        }
        
        public bool IsNight()
        {
            return isNight;
        }
        
        // Pi Network integration methods
        public void SaveGameToPiNetwork()
        {
            // This will be implemented in the Pi Network integration phase
            Debug.Log("Saving game progress to Pi Network...");
        }
        
        public void LoadGameFromPiNetwork()
        {
            // This will be implemented in the Pi Network integration phase
            Debug.Log("Loading game progress from Pi Network...");
        }
    }
}

