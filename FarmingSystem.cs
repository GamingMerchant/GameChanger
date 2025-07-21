using UnityEngine;
using System.Collections.Generic;

namespace Farmology
{
    public enum CropType
    {
        Wheat,
        Corn,
        Tomato,
        Carrot,
        Potato,
        Strawberry,
        Pumpkin,
        Lettuce
    }
    
    public enum CropGrowthStage
    {
        Seed,
        Sprout,
        Growing,
        Mature,
        ReadyToHarvest
    }
    
    [System.Serializable]
    public class CropData
    {
        public CropType cropType;
        public string cropName;
        public int growthDays;
        public int sellPrice;
        public int buyPrice;
        public Sprite[] growthSprites;
        public bool needsWater;
        public int experienceReward;
    }
    
    public class FarmTile : MonoBehaviour
    {
        [Header("Tile State")]
        public bool isTilled = false;
        public bool isWatered = false;
        public bool hasCrop = false;
        
        [Header("Crop Information")]
        public CropType currentCropType;
        public CropGrowthStage growthStage;
        public int daysPlanted = 0;
        public bool isReadyToHarvest = false;
        
        [Header("Visual Components")]
        public SpriteRenderer tileRenderer;
        public SpriteRenderer cropRenderer;
        public GameObject waterEffect;
        
        [Header("Sprites")]
        public Sprite grassSprite;
        public Sprite tilledSprite;
        public Sprite wateredSprite;
        
        private CropManager cropManager;
        
        private void Start()
        {
            cropManager = FindObjectOfType<CropManager>();
            UpdateVisuals();
        }
        
        public bool CanBeTilled()
        {
            return !isTilled && !hasCrop;
        }
        
        public void TillSoil()
        {
            if (CanBeTilled())
            {
                isTilled = true;
                UpdateVisuals();
                Debug.Log("Soil tilled!");
            }
        }
        
        public bool IsTilled()
        {
            return isTilled;
        }
        
        public bool HasCrop()
        {
            return hasCrop;
        }
        
        public bool IsWatered()
        {
            return isWatered;
        }
        
        public void WaterCrop()
        {
            if (hasCrop && !isWatered)
            {
                isWatered = true;
                UpdateVisuals();
                
                if (waterEffect != null)
                {
                    waterEffect.SetActive(true);
                    Invoke("HideWaterEffect", 2f);
                }
                
                Debug.Log("Crop watered!");
            }
        }
        
        private void HideWaterEffect()
        {
            if (waterEffect != null)
                waterEffect.SetActive(false);
        }
        
        public void PlantCrop(CropType cropType)
        {
            if (isTilled && !hasCrop)
            {
                hasCrop = true;
                currentCropType = cropType;
                growthStage = CropGrowthStage.Seed;
                daysPlanted = 0;
                isReadyToHarvest = false;
                
                UpdateVisuals();
                Debug.Log($"Planted {cropType}!");
            }
        }
        
        public void GrowCrop()
        {
            if (!hasCrop)
                return;
                
            daysPlanted++;
            
            CropData cropData = cropManager.GetCropData(currentCropType);
            if (cropData == null)
                return;
            
            // Determine growth stage based on days planted
            float growthProgress = (float)daysPlanted / cropData.growthDays;
            
            if (growthProgress >= 1f)
            {
                growthStage = CropGrowthStage.ReadyToHarvest;
                isReadyToHarvest = true;
            }
            else if (growthProgress >= 0.75f)
            {
                growthStage = CropGrowthStage.Mature;
            }
            else if (growthProgress >= 0.5f)
            {
                growthStage = CropGrowthStage.Growing;
            }
            else if (growthProgress >= 0.25f)
            {
                growthStage = CropGrowthStage.Sprout;
            }
            
            // Crops grow faster when watered
            if (isWatered)
            {
                daysPlanted += 0.5f; // Bonus growth
                isWatered = false; // Reset water status
            }
            
            UpdateVisuals();
        }
        
        public int HarvestCrop()
        {
            if (!isReadyToHarvest)
                return 0;
                
            CropData cropData = cropManager.GetCropData(currentCropType);
            if (cropData == null)
                return 0;
            
            int harvestValue = cropData.sellPrice;
            
            // Bonus for well-cared crops
            if (isWatered)
                harvestValue = Mathf.RoundToInt(harvestValue * 1.2f);
            
            // Reset tile
            hasCrop = false;
            isReadyToHarvest = false;
            currentCropType = CropType.Wheat;
            growthStage = CropGrowthStage.Seed;
            daysPlanted = 0;
            isTilled = false; // Need to till again
            
            UpdateVisuals();
            
            // Add experience
            GameManager.Instance.AddExperience(cropData.experienceReward);
            
            Debug.log($"Harvested crop for ${harvestValue}!");
            return harvestValue;
        }
        
        private void UpdateVisuals()
        {
            if (tileRenderer == null)
                return;
                
            // Update tile sprite
            if (isWatered && wateredSprite != null)
            {
                tileRenderer.sprite = wateredSprite;
            }
            else if (isTilled && tilledSprite != null)
            {
                tileRenderer.sprite = tilledSprite;
            }
            else if (grassSprite != null)
            {
                tileRenderer.sprite = grassSprite;
            }
            
            // Update crop sprite
            if (cropRenderer != null)
            {
                if (hasCrop)
                {
                    CropData cropData = cropManager?.GetCropData(currentCropType);
                    if (cropData != null && cropData.growthSprites != null && cropData.growthSprites.Length > 0)
                    {
                        int spriteIndex = Mathf.Clamp((int)growthStage, 0, cropData.growthSprites.Length - 1);
                        cropRenderer.sprite = cropData.growthSprites[spriteIndex];
                        cropRenderer.enabled = true;
                        
                        // Highlight ready crops
                        if (isReadyToHarvest)
                        {
                            cropRenderer.color = Color.yellow;
                        }
                        else
                        {
                            cropRenderer.color = Color.white;
                        }
                    }
                }
                else
                {
                    cropRenderer.enabled = false;
                }
            }
        }
        
        private void OnMouseDown()
        {
            if (isReadyToHarvest)
            {
                int harvestValue = HarvestCrop();
                GameManager.Instance.AddMoney(harvestValue);
            }
        }
    }
    
    public class CropManager : MonoBehaviour
    {
        [Header("Crop Database")]
        public CropData[] cropDatabase;
        
        private List<FarmTile> allFarmTiles;
        
        private void Start()
        {
            InitializeCropDatabase();
            FindAllFarmTiles();
        }
        
        private void InitializeCropDatabase()
        {
            cropDatabase = new CropData[]
            {
                new CropData
                {
                    cropType = CropType.Wheat,
                    cropName = "Wheat",
                    growthDays = 3,
                    sellPrice = 25,
                    buyPrice = 10,
                    needsWater = true,
                    experienceReward = 15
                },
                new CropData
                {
                    cropType = CropType.Corn,
                    cropName = "Corn",
                    growthDays = 5,
                    sellPrice = 40,
                    buyPrice = 15,
                    needsWater = true,
                    experienceReward = 25
                },
                new CropData
                {
                    cropType = CropType.Tomato,
                    cropName = "Tomato",
                    growthDays = 4,
                    sellPrice = 35,
                    buyPrice = 12,
                    needsWater = true,
                    experienceReward = 20
                },
                new CropData
                {
                    cropType = CropType.Carrot,
                    cropName = "Carrot",
                    growthDays = 3,
                    sellPrice = 20,
                    buyPrice = 8,
                    needsWater = true,
                    experienceReward = 12
                },
                new CropData
                {
                    cropType = CropType.Potato,
                    cropName = "Potato",
                    growthDays = 4,
                    sellPrice = 30,
                    buyPrice = 10,
                    needsWater = true,
                    experienceReward = 18
                },
                new CropData
                {
                    cropType = CropType.Strawberry,
                    cropName = "Strawberry",
                    growthDays = 6,
                    sellPrice = 50,
                    buyPrice = 20,
                    needsWater = true,
                    experienceReward = 30
                },
                new CropData
                {
                    cropType = CropType.Pumpkin,
                    cropName = "Pumpkin",
                    growthDays = 7,
                    sellPrice = 60,
                    buyPrice = 25,
                    needsWater = true,
                    experienceReward = 35
                },
                new CropData
                {
                    cropType = CropType.Lettuce,
                    cropName = "Lettuce",
                    growthDays = 2,
                    sellPrice = 15,
                    buyPrice = 5,
                    needsWater = true,
                    experienceReward = 10
                }
            };
        }
        
        private void FindAllFarmTiles()
        {
            allFarmTiles = new List<FarmTile>();
            FarmTile[] tiles = FindObjectsOfType<FarmTile>();
            allFarmTiles.AddRange(tiles);
        }
        
        public CropData GetCropData(CropType cropType)
        {
            foreach (CropData data in cropDatabase)
            {
                if (data.cropType == cropType)
                    return data;
            }
            return null;
        }
        
        public void GrowAllCrops()
        {
            foreach (FarmTile tile in allFarmTiles)
            {
                if (tile.HasCrop())
                {
                    tile.GrowCrop();
                }
            }
            
            Debug.Log("All crops have grown!");
        }
        
        public int GetTotalCropsReady()
        {
            int count = 0;
            foreach (FarmTile tile in allFarmTiles)
            {
                if (tile.isReadyToHarvest)
                    count++;
            }
            return count;
        }
        
        public void HarvestAllReadyCrops()
        {
            int totalValue = 0;
            foreach (FarmTile tile in allFarmTiles)
            {
                if (tile.isReadyToHarvest)
                {
                    totalValue += tile.HarvestCrop();
                }
            }
            
            if (totalValue > 0)
            {
                GameManager.Instance.AddMoney(totalValue);
                Debug.Log($"Auto-harvested crops worth ${totalValue}!");
            }
        }
    }
}

