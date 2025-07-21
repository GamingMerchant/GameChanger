using UnityEngine;
using System.Collections.Generic;

namespace Farmology
{
    public enum AnimalType
    {
        Chicken,
        Cow,
        Pig,
        Sheep,
        Goat,
        Duck
    }
    
    public enum AnimalState
    {
        Happy,
        Hungry,
        Sick,
        Sleeping,
        Producing
    }
    
    [System.Serializable]
    public class AnimalData
    {
        public AnimalType animalType;
        public string animalName;
        public int purchasePrice;
        public int dailyIncome;
        public int feedCost;
        public string productName;
        public float productionTime; // Hours
        public int experienceReward;
        public int maxHealth;
    }
    
    public class Animal : MonoBehaviour, IInteractable
    {
        [Header("Animal Properties")]
        public AnimalType animalType;
        public string animalName;
        public AnimalState currentState;
        
        [Header("Stats")]
        public int health = 100;
        public int happiness = 100;
        public float hunger = 0f; // 0 = full, 100 = starving
        public float productionProgress = 0f;
        
        [Header("Production")]
        public bool hasProduct = false;
        public float timeSinceLastFed = 0f;
        public float timeSinceLastProduced = 0f;
        
        [Header("Visual Components")]
        public SpriteRenderer animalRenderer;
        public GameObject productIndicator;
        public GameObject hungerIndicator;
        public GameObject happinessIndicator;
        
        [Header("Animation")]
        public Animator animalAnimator;
        
        private AnimalManager animalManager;
        private AnimalData animalData;
        
        private void Start()
        {
            animalManager = FindObjectOfType<AnimalManager>();
            animalData = animalManager.GetAnimalData(animalType);
            
            if (animalData != null)
            {
                health = animalData.maxHealth;
                animalName = animalData.animalName;
            }
            
            InvokeRepeating("UpdateAnimal", 1f, 1f);
        }
        
        private void UpdateAnimal()
        {
            if (GameManager.Instance != null && GameManager.Instance.isPaused)
                return;
                
            // Increase hunger over time
            timeSinceLastFed += Time.deltaTime;
            hunger = Mathf.Clamp(timeSinceLastFed / 3600f * 20f, 0f, 100f); // Gets hungry over 5 hours
            
            // Update happiness based on hunger and health
            if (hunger > 80f)
            {
                happiness = Mathf.Max(0, happiness - 1);
                currentState = AnimalState.Hungry;
            }
            else if (health < 30)
            {
                currentState = AnimalState.Sick;
                happiness = Mathf.Max(0, happiness - 2);
            }
            else if (GameManager.Instance.IsNight())
            {
                currentState = AnimalState.Sleeping;
            }
            else if (hasProduct)
            {
                currentState = AnimalState.Producing;
            }
            else
            {
                currentState = AnimalState.Happy;
                happiness = Mathf.Min(100, happiness + 1);
            }
            
            // Production logic
            if (!hasProduct && currentState != AnimalState.Sick && currentState != AnimalState.Hungry)
            {
                productionProgress += Time.deltaTime;
                
                if (animalData != null && productionProgress >= animalData.productionTime * 3600f)
                {
                    ProduceItem();
                }
            }
            
            UpdateVisuals();
        }
        
        private void ProduceItem()
        {
            if (animalData == null)
                return;
                
            hasProduct = true;
            productionProgress = 0f;
            timeSinceLastProduced = 0f;
            
            if (productIndicator != null)
                productIndicator.SetActive(true);
                
            Debug.Log($"{animalName} produced {animalData.productName}!");
        }
        
        public void FeedAnimal()
        {
            if (GameManager.Instance.SpendMoney(animalData.feedCost))
            {
                hunger = 0f;
                timeSinceLastFed = 0f;
                happiness = Mathf.Min(100, happiness + 20);
                health = Mathf.Min(animalData.maxHealth, health + 10);
                
                GameManager.Instance.AddExperience(5);
                
                Debug.Log($"Fed {animalName}!");
            }
            else
            {
                Debug.Log("Not enough money to feed animal!");
            }
        }
        
        public int CollectProduct()
        {
            if (!hasProduct || animalData == null)
                return 0;
                
            hasProduct = false;
            
            if (productIndicator != null)
                productIndicator.SetActive(false);
                
            int income = animalData.dailyIncome;
            
            // Bonus for happy animals
            if (happiness > 80)
                income = Mathf.RoundToInt(income * 1.2f);
                
            GameManager.Instance.AddExperience(animalData.experienceReward);
            
            Debug.Log($"Collected {animalData.productName} from {animalName} for ${income}!");
            return income;
        }
        
        public void Interact(PlayerController player)
        {
            if (hasProduct)
            {
                int income = CollectProduct();
                GameManager.Instance.AddMoney(income);
            }
            else if (hunger > 50f)
            {
                FeedAnimal();
            }
            else
            {
                // Pet the animal
                happiness = Mathf.Min(100, happiness + 10);
                GameManager.Instance.AddExperience(2);
                Debug.Log($"You petted {animalName}!");
            }
        }
        
        private void UpdateVisuals()
        {
            // Update hunger indicator
            if (hungerIndicator != null)
            {
                hungerIndicator.SetActive(hunger > 60f);
            }
            
            // Update happiness indicator
            if (happinessIndicator != null)
            {
                Color happinessColor = Color.Lerp(Color.red, Color.green, happiness / 100f);
                happinessIndicator.GetComponent<SpriteRenderer>().color = happinessColor;
            }
            
            // Update animations
            if (animalAnimator != null)
            {
                animalAnimator.SetInteger("State", (int)currentState);
                animalAnimator.SetFloat("Happiness", happiness);
                animalAnimator.SetFloat("Hunger", hunger);
            }
        }
        
        private void OnMouseDown()
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                float distance = Vector2.Distance(transform.position, player.transform.position);
                if (distance <= player.interactionRange)
                {
                    Interact(player);
                }
            }
        }
    }
    
    public class AnimalManager : MonoBehaviour
    {
        [Header("Animal Database")]
        public AnimalData[] animalDatabase;
        
        [Header("Animal Prefabs")]
        public GameObject[] animalPrefabs;
        
        private List<Animal> allAnimals;
        
        private void Start()
        {
            InitializeAnimalDatabase();
            FindAllAnimals();
        }
        
        private void InitializeAnimalDatabase()
        {
            animalDatabase = new AnimalData[]
            {
                new AnimalData
                {
                    animalType = AnimalType.Chicken,
                    animalName = "Chicken",
                    purchasePrice = 50,
                    dailyIncome = 15,
                    feedCost = 5,
                    productName = "Egg",
                    productionTime = 4f, // 4 hours
                    experienceReward = 8,
                    maxHealth = 80
                },
                new AnimalData
                {
                    animalType = AnimalType.Cow,
                    animalName = "Cow",
                    purchasePrice = 200,
                    dailyIncome = 40,
                    feedCost = 15,
                    productName = "Milk",
                    productionTime = 8f, // 8 hours
                    experienceReward = 20,
                    maxHealth = 150
                },
                new AnimalData
                {
                    animalType = AnimalType.Pig,
                    animalName = "Pig",
                    purchasePrice = 100,
                    dailyIncome = 25,
                    feedCost = 10,
                    productName = "Truffle",
                    productionTime = 12f, // 12 hours
                    experienceReward = 15,
                    maxHealth = 120
                },
                new AnimalData
                {
                    animalType = AnimalType.Sheep,
                    animalName = "Sheep",
                    purchasePrice = 150,
                    dailyIncome = 30,
                    feedCost = 12,
                    productName = "Wool",
                    productionTime = 24f, // 24 hours
                    experienceReward = 18,
                    maxHealth = 100
                },
                new AnimalData
                {
                    animalType = AnimalType.Goat,
                    animalName = "Goat",
                    purchasePrice = 120,
                    dailyIncome = 28,
                    feedCost = 8,
                    productName = "Goat Milk",
                    productionTime = 6f, // 6 hours
                    experienceReward = 12,
                    maxHealth = 90
                },
                new AnimalData
                {
                    animalType = AnimalType.Duck,
                    animalName = "Duck",
                    purchasePrice = 40,
                    dailyIncome = 12,
                    feedCost = 4,
                    productName = "Duck Egg",
                    productionTime = 5f, // 5 hours
                    experienceReward = 6,
                    maxHealth = 70
                }
            };
        }
        
        private void FindAllAnimals()
        {
            allAnimals = new List<Animal>();
            Animal[] animals = FindObjectsOfType<Animal>();
            allAnimals.AddRange(animals);
        }
        
        public AnimalData GetAnimalData(AnimalType animalType)
        {
            foreach (AnimalData data in animalDatabase)
            {
                if (data.animalType == animalType)
                    return data;
            }
            return null;
        }
        
        public int GetDailyIncome()
        {
            int totalIncome = 0;
            foreach (Animal animal in allAnimals)
            {
                if (animal.hasProduct)
                {
                    AnimalData data = GetAnimalData(animal.animalType);
                    if (data != null)
                        totalIncome += data.dailyIncome;
                }
            }
            return totalIncome;
        }
        
        public void FeedAllAnimals()
        {
            int totalCost = 0;
            foreach (Animal animal in allAnimals)
            {
                AnimalData data = GetAnimalData(animal.animalType);
                if (data != null && animal.hunger > 50f)
                {
                    totalCost += data.feedCost;
                }
            }
            
            if (GameManager.Instance.SpendMoney(totalCost))
            {
                foreach (Animal animal in allAnimals)
                {
                    if (animal.hunger > 50f)
                    {
                        animal.FeedAnimal();
                    }
                }
                Debug.Log($"Fed all hungry animals for ${totalCost}!");
            }
        }
        
        public void CollectAllProducts()
        {
            int totalIncome = 0;
            foreach (Animal animal in allAnimals)
            {
                if (animal.hasProduct)
                {
                    totalIncome += animal.CollectProduct();
                }
            }
            
            if (totalIncome > 0)
            {
                GameManager.Instance.AddMoney(totalIncome);
                Debug.Log($"Collected all products for ${totalIncome}!");
            }
        }
        
        public GameObject PurchaseAnimal(AnimalType animalType, Vector3 position)
        {
            AnimalData data = GetAnimalData(animalType);
            if (data == null)
                return null;
                
            if (!GameManager.Instance.SpendMoney(data.purchasePrice))
            {
                Debug.Log("Not enough money to purchase animal!");
                return null;
            }
            
            // Find the appropriate prefab
            GameObject prefab = null;
            if (animalPrefabs != null && (int)animalType < animalPrefabs.Length)
            {
                prefab = animalPrefabs[(int)animalType];
            }
            
            if (prefab != null)
            {
                GameObject newAnimal = Instantiate(prefab, position, Quaternion.identity);
                Animal animalComponent = newAnimal.GetComponent<Animal>();
                if (animalComponent != null)
                {
                    animalComponent.animalType = animalType;
                    allAnimals.Add(animalComponent);
                }
                
                Debug.Log($"Purchased {data.animalName} for ${data.purchasePrice}!");
                return newAnimal;
            }
            
            return null;
        }
    }
}

