using UnityEngine;

namespace Farmology
{
    public class Tree : MonoBehaviour, IInteractable
    {
        [Header("Tree Properties")]
        public int health = 100;
        public int woodReward = 20;
        public int experienceReward = 15;
        public float regrowthTime = 300f; // 5 minutes
        
        [Header("Visual Components")]
        public SpriteRenderer treeRenderer;
        public GameObject stumpPrefab;
        
        private bool isChopped = false;
        private float regrowthTimer = 0f;
        
        private void Update()
        {
            if (isChopped)
            {
                regrowthTimer += Time.deltaTime;
                if (regrowthTimer >= regrowthTime)
                {
                    Regrow();
                }
            }
        }
        
        public void Chop()
        {
            if (isChopped)
                return;
                
            isChopped = true;
            regrowthTimer = 0f;
            
            // Visual changes
            if (treeRenderer != null)
                treeRenderer.enabled = false;
                
            if (stumpPrefab != null)
            {
                GameObject stump = Instantiate(stumpPrefab, transform.position, transform.rotation);
                Destroy(stump, regrowthTime);
            }
            
            // Rewards
            GameManager.Instance.AddMoney(woodReward);
            GameManager.Instance.AddExperience(experienceReward);
            
            Debug.Log($"Tree chopped! Gained {woodReward} money and {experienceReward} XP");
        }
        
        private void Regrow()
        {
            isChopped = false;
            regrowthTimer = 0f;
            
            if (treeRenderer != null)
                treeRenderer.enabled = true;
                
            Debug.Log("Tree has regrown!");
        }
        
        public void Interact(PlayerController player)
        {
            if (!isChopped && player.currentToolType == PlayerController.ToolType.Axe)
            {
                Chop();
            }
        }
    }
    
    public class Rock : MonoBehaviour, IInteractable
    {
        [Header("Rock Properties")]
        public int health = 150;
        public int stoneReward = 15;
        public int experienceReward = 12;
        public float respawnTime = 600f; // 10 minutes
        
        [Header("Visual Components")]
        public SpriteRenderer rockRenderer;
        public GameObject debrisPrefab;
        
        private bool isMined = false;
        private float respawnTimer = 0f;
        
        private void Update()
        {
            if (isMined)
            {
                respawnTimer += Time.deltaTime;
                if (respawnTimer >= respawnTime)
                {
                    Respawn();
                }
            }
        }
        
        public void Mine()
        {
            if (isMined)
                return;
                
            isMined = true;
            respawnTimer = 0f;
            
            // Visual changes
            if (rockRenderer != null)
                rockRenderer.enabled = false;
                
            if (debrisPrefab != null)
            {
                GameObject debris = Instantiate(debrisPrefab, transform.position, transform.rotation);
                Destroy(debris, respawnTime);
            }
            
            // Rewards
            GameManager.Instance.AddMoney(stoneReward);
            GameManager.Instance.AddExperience(experienceReward);
            
            Debug.Log($"Rock mined! Gained {stoneReward} money and {experienceReward} XP");
        }
        
        private void Respawn()
        {
            isMined = false;
            respawnTimer = 0f;
            
            if (rockRenderer != null)
                rockRenderer.enabled = true;
                
            Debug.Log("Rock has respawned!");
        }
        
        public void Interact(PlayerController player)
        {
            if (!isMined && player.currentToolType == PlayerController.ToolType.Pickaxe)
            {
                Mine();
            }
        }
    }
    
    public class Well : MonoBehaviour, IInteractable
    {
        [Header("Well Properties")]
        public int waterCost = 5;
        public bool isUnlimited = false;
        
        [Header("Visual Components")]
        public SpriteRenderer wellRenderer;
        public GameObject waterEffect;
        
        public void Interact(PlayerController player)
        {
            if (player.currentToolType == PlayerController.ToolType.WateringCan)
            {
                RefillWateringCan();
            }
        }
        
        private void RefillWateringCan()
        {
            if (!isUnlimited && !GameManager.Instance.SpendMoney(waterCost))
            {
                Debug.Log("Not enough money to refill watering can!");
                return;
            }
            
            // Visual effect
            if (waterEffect != null)
            {
                waterEffect.SetActive(true);
                Invoke("HideWaterEffect", 2f);
            }
            
            UIManager uiManager = FindObjectOfType<UIManager>();
            uiManager?.ShowNotification("Watering can refilled!");
            
            Debug.Log("Watering can refilled!");
        }
        
        private void HideWaterEffect()
        {
            if (waterEffect != null)
                waterEffect.SetActive(false);
        }
    }
    
    public class Shop : MonoBehaviour, IInteractable
    {
        [Header("Shop Properties")]
        public string shopName = "General Store";
        public ShopType shopType = ShopType.General;
        
        [Header("Visual Components")]
        public SpriteRenderer shopRenderer;
        public GameObject shopUI;
        
        public enum ShopType
        {
            General,
            Seeds,
            Animals,
            Tools,
            Upgrades
        }
        
        public void Interact(PlayerController player)
        {
            OpenShop();
        }
        
        private void OpenShop()
        {
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.ToggleShop();
            }
            
            Debug.Log($"Opened {shopName}!");
        }
    }
    
    public class Barn : MonoBehaviour, IInteractable
    {
        [Header("Barn Properties")]
        public int capacity = 10;
        public int currentAnimals = 0;
        
        [Header("Visual Components")]
        public SpriteRenderer barnRenderer;
        public Transform animalSpawnPoint;
        
        private AnimalManager animalManager;
        
        private void Start()
        {
            animalManager = FindObjectOfType<AnimalManager>();
        }
        
        public void Interact(PlayerController player)
        {
            OpenBarnMenu();
        }
        
        private void OpenBarnMenu()
        {
            // This would open a barn-specific UI
            UIManager uiManager = FindObjectOfType<UIManager>();
            uiManager?.ShowNotification($"Barn: {currentAnimals}/{capacity} animals");
            
            Debug.Log("Opened barn menu!");
        }
        
        public bool CanAddAnimal()
        {
            return currentAnimals < capacity;
        }
        
        public void AddAnimal()
        {
            if (CanAddAnimal())
            {
                currentAnimals++;
                Debug.Log($"Animal added to barn. Count: {currentAnimals}/{capacity}");
            }
        }
        
        public void RemoveAnimal()
        {
            if (currentAnimals > 0)
            {
                currentAnimals--;
                Debug.Log($"Animal removed from barn. Count: {currentAnimals}/{capacity}");
            }
        }
    }
    
    public class Greenhouse : MonoBehaviour, IInteractable
    {
        [Header("Greenhouse Properties")]
        public float growthSpeedMultiplier = 2f;
        public int capacity = 20;
        public int currentCrops = 0;
        
        [Header("Visual Components")]
        public SpriteRenderer greenhouseRenderer;
        public Transform[] cropSpots;
        
        private CropManager cropManager;
        
        private void Start()
        {
            cropManager = FindObjectOfType<CropManager>();
        }
        
        public void Interact(PlayerController player)
        {
            OpenGreenhouseMenu();
        }
        
        private void OpenGreenhouseMenu()
        {
            UIManager uiManager = FindObjectOfType<UIManager>();
            uiManager?.ShowNotification($"Greenhouse: {currentCrops}/{capacity} crops (2x growth speed)");
            
            Debug.Log("Opened greenhouse menu!");
        }
        
        public bool CanAddCrop()
        {
            return currentCrops < capacity;
        }
        
        public void AddCrop()
        {
            if (CanAddCrop())
            {
                currentCrops++;
                Debug.Log($"Crop added to greenhouse. Count: {currentCrops}/{capacity}");
            }
        }
        
        public void RemoveCrop()
        {
            if (currentCrops > 0)
            {
                currentCrops--;
                Debug.Log($"Crop removed from greenhouse. Count: {currentCrops}/{capacity}");
            }
        }
    }
    
    public class WeatherSystem : MonoBehaviour
    {
        [Header("Weather Settings")]
        public WeatherType currentWeather = WeatherType.Sunny;
        public float weatherChangeInterval = 300f; // 5 minutes
        public float rainGrowthBonus = 1.5f;
        
        [Header("Visual Effects")]
        public GameObject rainEffect;
        public GameObject snowEffect;
        public GameObject stormEffect;
        
        public enum WeatherType
        {
            Sunny,
            Rainy,
            Stormy,
            Snowy,
            Cloudy
        }
        
        private float weatherTimer = 0f;
        
        private void Start()
        {
            InvokeRepeating("ChangeWeather", weatherChangeInterval, weatherChangeInterval);
        }
        
        private void Update()
        {
            ApplyWeatherEffects();
        }
        
        private void ChangeWeather()
        {
            WeatherType[] weatherTypes = System.Enum.GetValues(typeof(WeatherType)) as WeatherType[];
            WeatherType newWeather = weatherTypes[Random.Range(0, weatherTypes.Length)];
            
            if (newWeather != currentWeather)
            {
                currentWeather = newWeather;
                UpdateWeatherVisuals();
                
                UIManager uiManager = FindObjectOfType<UIManager>();
                uiManager?.ShowNotification($"Weather changed to {currentWeather}");
                
                Debug.Log($"Weather changed to: {currentWeather}");
            }
        }
        
        private void UpdateWeatherVisuals()
        {
            // Disable all weather effects
            if (rainEffect != null) rainEffect.SetActive(false);
            if (snowEffect != null) snowEffect.SetActive(false);
            if (stormEffect != null) stormEffect.SetActive(false);
            
            // Enable current weather effect
            switch (currentWeather)
            {
                case WeatherType.Rainy:
                    if (rainEffect != null) rainEffect.SetActive(true);
                    break;
                case WeatherType.Snowy:
                    if (snowEffect != null) snowEffect.SetActive(true);
                    break;
                case WeatherType.Stormy:
                    if (stormEffect != null) stormEffect.SetActive(true);
                    break;
            }
        }
        
        private void ApplyWeatherEffects()
        {
            // Weather effects on gameplay
            switch (currentWeather)
            {
                case WeatherType.Rainy:
                    // Crops grow faster, no need to water
                    ApplyRainEffects();
                    break;
                case WeatherType.Stormy:
                    // Reduced movement speed, animals get scared
                    ApplyStormEffects();
                    break;
                case WeatherType.Snowy:
                    // Slower crop growth, animals need more care
                    ApplySnowEffects();
                    break;
            }
        }
        
        private void ApplyRainEffects()
        {
            // Auto-water all crops
            CropManager cropManager = FindObjectOfType<CropManager>();
            if (cropManager != null)
            {
                // This would auto-water crops in a real implementation
            }
        }
        
        private void ApplyStormEffects()
        {
            // Reduce player movement speed
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                // This would modify movement speed in a real implementation
            }
        }
        
        private void ApplySnowEffects()
        {
            // Slow down crop growth
            // This would affect crop growth rates in a real implementation
        }
        
        public bool IsRaining()
        {
            return currentWeather == WeatherType.Rainy;
        }
        
        public bool IsStormy()
        {
            return currentWeather == WeatherType.Stormy;
        }
        
        public float GetGrowthMultiplier()
        {
            switch (currentWeather)
            {
                case WeatherType.Rainy:
                    return rainGrowthBonus;
                case WeatherType.Snowy:
                    return 0.5f;
                default:
                    return 1f;
            }
        }
    }
}

