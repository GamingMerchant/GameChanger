using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace Farmology
{
    public class UIManager : MonoBehaviour
    {
        [Header("Main HUD")]
        public GameObject mainHUD;
        public TextMeshProUGUI moneyText;
        public TextMeshProUGUI dayText;
        public TextMeshProUGUI timeText;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI experienceText;
        public Slider experienceSlider;
        
        [Header("Tool Bar")]
        public GameObject toolBar;
        public Button[] toolButtons;
        public Image[] toolIcons;
        public TextMeshProUGUI currentToolText;
        
        [Header("Menus")]
        public GameObject pauseMenu;
        public GameObject shopMenu;
        public GameObject inventoryMenu;
        public GameObject settingsMenu;
        public GameObject helpMenu;
        
        [Header("Shop UI")]
        public Transform seedShopContent;
        public Transform animalShopContent;
        public GameObject shopItemPrefab;
        
        [Header("Inventory UI")]
        public Transform inventoryContent;
        public GameObject inventorySlotPrefab;
        public TextMeshProUGUI inventoryTitle;
        
        [Header("Notifications")]
        public GameObject notificationPanel;
        public TextMeshProUGUI notificationText;
        public float notificationDuration = 3f;
        
        [Header("Controls Help")]
        public GameObject controlsPanel;
        public TextMeshProUGUI controlsText;
        
        private PlayerController playerController;
        private CropManager cropManager;
        private AnimalManager animalManager;
        private Queue<string> notificationQueue;
        
        private void Start()
        {
            playerController = FindObjectOfType<PlayerController>();
            cropManager = FindObjectOfType<CropManager>();
            animalManager = FindObjectOfType<AnimalManager>();
            notificationQueue = new Queue<string>();
            
            InitializeUI();
            SetupControlsText();
        }
        
        private void Update()
        {
            UpdateHUD();
            HandleUIInput();
        }
        
        private void InitializeUI()
        {
            // Initialize all menus as closed
            if (pauseMenu != null) pauseMenu.SetActive(false);
            if (shopMenu != null) shopMenu.SetActive(false);
            if (inventoryMenu != null) inventoryMenu.SetActive(false);
            if (settingsMenu != null) settingsMenu.SetActive(false);
            if (helpMenu != null) helpMenu.SetActive(false);
            if (controlsPanel != null) controlsPanel.SetActive(false);
            
            // Setup shop
            PopulateShop();
            
            // Setup tool buttons
            SetupToolButtons();
        }
        
        private void HandleUIInput()
        {
            if (GameManager.Instance.isPaused)
                return;
                
            // Shop menu (B key)
            if (Input.GetKeyDown(KeyCode.B))
            {
                ToggleShop();
            }
            
            // Inventory menu (I key)
            if (Input.GetKeyDown(KeyCode.I))
            {
                ToggleInventory();
            }
            
            // Help menu (H key)
            if (Input.GetKeyDown(KeyCode.H))
            {
                ToggleHelp();
            }
            
            // Controls help (C key)
            if (Input.GetKeyDown(KeyCode.C))
            {
                ToggleControls();
            }
        }
        
        private void UpdateHUD()
        {
            if (GameManager.Instance == null)
                return;
                
            // Update money
            if (moneyText != null)
                moneyText.text = $"${GameManager.Instance.playerMoney}";
                
            // Update day
            if (dayText != null)
                dayText.text = $"Day {GameManager.Instance.currentDay}";
                
            // Update time
            if (timeText != null)
            {
                float timeOfDay = GameManager.Instance.GetTimeOfDay();
                int hours = Mathf.FloorToInt(timeOfDay * 24);
                int minutes = Mathf.FloorToInt((timeOfDay * 24 - hours) * 60);
                timeText.text = $"{hours:00}:{minutes:00}";
            }
            
            // Update level
            if (levelText != null)
                levelText.text = $"Level {GameManager.Instance.playerLevel}";
                
            // Update experience
            if (experienceText != null)
                experienceText.text = $"XP: {GameManager.Instance.playerExperience}/{GameManager.Instance.experienceToNextLevel}";
                
            if (experienceSlider != null)
            {
                experienceSlider.value = (float)GameManager.Instance.playerExperience / GameManager.Instance.experienceToNextLevel;
            }
            
            // Update current tool
            if (currentToolText != null && playerController != null)
            {
                currentToolText.text = $"Tool: {playerController.currentToolType}";
            }
        }
        
        private void SetupToolButtons()
        {
            if (toolButtons == null || playerController == null)
                return;
                
            for (int i = 0; i < toolButtons.Length; i++)
            {
                int toolIndex = i;
                toolButtons[i].onClick.AddListener(() => SwitchTool(toolIndex));
            }
        }
        
        private void SwitchTool(int toolIndex)
        {
            if (playerController == null)
                return;
                
            PlayerController.ToolType[] tools = {
                PlayerController.ToolType.Hoe,
                PlayerController.ToolType.WateringCan,
                PlayerController.ToolType.Seeds,
                PlayerController.ToolType.Axe,
                PlayerController.ToolType.Pickaxe
            };
            
            if (toolIndex < tools.Length)
            {
                playerController.currentToolType = tools[toolIndex];
                ShowNotification($"Switched to {tools[toolIndex]}");
            }
        }
        
        public void TogglePause()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TogglePause();
            }
        }
        
        public void ToggleShop()
        {
            if (shopMenu != null)
            {
                bool isActive = shopMenu.activeSelf;
                shopMenu.SetActive(!isActive);
                
                if (!isActive)
                {
                    PopulateShop();
                }
            }
        }
        
        public void ToggleInventory()
        {
            if (inventoryMenu != null)
            {
                bool isActive = inventoryMenu.activeSelf;
                inventoryMenu.SetActive(!isActive);
                
                if (!isActive)
                {
                    PopulateInventory();
                }
            }
        }
        
        public void ToggleHelp()
        {
            if (helpMenu != null)
            {
                helpMenu.SetActive(!helpMenu.activeSelf);
            }
        }
        
        public void ToggleControls()
        {
            if (controlsPanel != null)
            {
                controlsPanel.SetActive(!controlsPanel.activeSelf);
            }
        }
        
        private void PopulateShop()
        {
            if (seedShopContent == null || animalShopContent == null || shopItemPrefab == null)
                return;
                
            // Clear existing items
            foreach (Transform child in seedShopContent)
            {
                Destroy(child.gameObject);
            }
            
            foreach (Transform child in animalShopContent)
            {
                Destroy(child.gameObject);
            }
            
            // Add seed items
            if (cropManager != null && cropManager.cropDatabase != null)
            {
                foreach (CropData crop in cropManager.cropDatabase)
                {
                    CreateShopItem(seedShopContent, crop.cropName + " Seeds", crop.buyPrice, () => BuySeed(crop.cropType));
                }
            }
            
            // Add animal items
            if (animalManager != null && animalManager.animalDatabase != null)
            {
                foreach (AnimalData animal in animalManager.animalDatabase)
                {
                    CreateShopItem(animalShopContent, animal.animalName, animal.purchasePrice, () => BuyAnimal(animal.animalType));
                }
            }
        }
        
        private void CreateShopItem(Transform parent, string itemName, int price, System.Action onPurchase)
        {
            GameObject item = Instantiate(shopItemPrefab, parent);
            
            TextMeshProUGUI nameText = item.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI priceText = item.transform.Find("PriceText")?.GetComponent<TextMeshProUGUI>();
            Button buyButton = item.transform.Find("BuyButton")?.GetComponent<Button>();
            
            if (nameText != null) nameText.text = itemName;
            if (priceText != null) priceText.text = $"${price}";
            if (buyButton != null) buyButton.onClick.AddListener(() => onPurchase());
        }
        
        private void BuySeed(CropType cropType)
        {
            CropData cropData = cropManager.GetCropData(cropType);
            if (cropData != null && GameManager.Instance.SpendMoney(cropData.buyPrice))
            {
                // Add to inventory (simplified - would need proper inventory system)
                ShowNotification($"Purchased {cropData.cropName} seeds!");
            }
            else
            {
                ShowNotification("Not enough money!");
            }
        }
        
        private void BuyAnimal(AnimalType animalType)
        {
            // Find a suitable location to place the animal
            Vector3 playerPos = playerController != null ? playerController.transform.position : Vector3.zero;
            Vector3 spawnPos = playerPos + Vector3.right * 2f;
            
            GameObject animal = animalManager.PurchaseAnimal(animalType, spawnPos);
            if (animal != null)
            {
                ShowNotification($"Purchased {animalType}!");
            }
            else
            {
                ShowNotification("Not enough money or failed to purchase!");
            }
        }
        
        private void PopulateInventory()
        {
            if (inventoryContent == null || inventorySlotPrefab == null)
                return;
                
            // Clear existing items
            foreach (Transform child in inventoryContent)
            {
                Destroy(child.gameObject);
            }
            
            // This would be expanded with a proper inventory system
            // For now, show basic stats
            CreateInventoryItem("Money", $"${GameManager.Instance.playerMoney}");
            CreateInventoryItem("Level", GameManager.Instance.playerLevel.ToString());
            CreateInventoryItem("Experience", $"{GameManager.Instance.playerExperience}/{GameManager.Instance.experienceToNextLevel}");
            
            if (cropManager != null)
            {
                int readyCrops = cropManager.GetTotalCropsReady();
                CreateInventoryItem("Ready Crops", readyCrops.ToString());
            }
        }
        
        private void CreateInventoryItem(string itemName, string value)
        {
            GameObject item = Instantiate(inventorySlotPrefab, inventoryContent);
            
            TextMeshProUGUI nameText = item.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI valueText = item.transform.Find("ValueText")?.GetComponent<TextMeshProUGUI>();
            
            if (nameText != null) nameText.text = itemName;
            if (valueText != null) valueText.text = value;
        }
        
        private void SetupControlsText()
        {
            if (controlsText == null)
                return;
                
            controlsText.text = @"FARMOLOGY CONTROLS

MOVEMENT:
• W, A, S, D - Move around
• Left Shift - Run

TOOLS:
• 1 - Hoe (till soil)
• 2 - Watering Can
• 3 - Seeds
• 4 - Axe (chop trees)
• 5 - Pickaxe (mine rocks)
• Left Click - Use tool

INTERACTION:
• E - Interact with objects
• Left Click - Harvest crops/collect from animals

MENUS:
• Space - Pause/Unpause
• F5 - Restart game
• B - Shop
• I - Inventory
• H - Help
• C - Controls
• ESC - Pause menu

FARMING TIPS:
• Till soil before planting
• Water crops daily for faster growth
• Feed animals to keep them happy
• Collect products regularly
• Upgrade tools as you level up";
        }
        
        public void ShowNotification(string message)
        {
            notificationQueue.Enqueue(message);
            
            if (notificationPanel != null && !notificationPanel.activeSelf)
            {
                StartCoroutine(ShowNextNotification());
            }
        }
        
        private System.Collections.IEnumerator ShowNextNotification()
        {
            while (notificationQueue.Count > 0)
            {
                string message = notificationQueue.Dequeue();
                
                if (notificationPanel != null && notificationText != null)
                {
                    notificationText.text = message;
                    notificationPanel.SetActive(true);
                    
                    yield return new WaitForSeconds(notificationDuration);
                    
                    notificationPanel.SetActive(false);
                }
                
                yield return new WaitForSeconds(0.5f);
            }
        }
        
        // Button event handlers
        public void OnResumeGame()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TogglePause();
            }
        }
        
        public void OnRestartGame()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartGame();
            }
        }
        
        public void OnQuitGame()
        {
            Application.Quit();
        }
        
        public void OnAutoHarvest()
        {
            if (cropManager != null)
            {
                cropManager.HarvestAllReadyCrops();
                ShowNotification("Auto-harvested all ready crops!");
            }
        }
        
        public void OnAutoFeedAnimals()
        {
            if (animalManager != null)
            {
                animalManager.FeedAllAnimals();
                ShowNotification("Fed all hungry animals!");
            }
        }
        
        public void OnAutoCollectProducts()
        {
            if (animalManager != null)
            {
                animalManager.CollectAllProducts();
                ShowNotification("Collected all animal products!");
            }
        }
    }
    
    // Shop item component
    public class ShopItem : MonoBehaviour
    {
        public string itemName;
        public int price;
        public System.Action onPurchase;
        
        public void Purchase()
        {
            onPurchase?.Invoke();
        }
    }
    
    // Inventory slot component
    public class InventorySlot : MonoBehaviour
    {
        public string itemName;
        public int quantity;
        public Sprite itemIcon;
        
        [Header("UI Components")]
        public Image iconImage;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI quantityText;
        
        public void UpdateSlot()
        {
            if (iconImage != null) iconImage.sprite = itemIcon;
            if (nameText != null) nameText.text = itemName;
            if (quantityText != null) quantityText.text = quantity.ToString();
        }
    }
}

