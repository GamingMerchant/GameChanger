using UnityEngine;

namespace Farmology
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float runSpeedMultiplier = 1.5f;
        
        [Header("Animation")]
        public Animator animator;
        
        [Header("Interaction")]
        public float interactionRange = 1.5f;
        public LayerMask interactableLayer = 1;
        
        [Header("Tools")]
        public GameObject currentTool;
        public Transform toolHolder;
        
        private Rigidbody2D rb;
        private Vector2 movement;
        private Vector2 lastMovement;
        private bool isRunning;
        private bool isUsingTool;
        
        // Tool types
        public enum ToolType
        {
            None,
            Hoe,
            WateringCan,
            Seeds,
            Axe,
            Pickaxe
        }
        
        public ToolType currentToolType = ToolType.None;
        
        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            
            if (animator == null)
                animator = GetComponent<Animator>();
        }
        
        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.isPaused)
                return;
                
            HandleInput();
            HandleInteraction();
            UpdateAnimation();
        }
        
        private void FixedUpdate()
        {
            if (GameManager.Instance != null && GameManager.Instance.isPaused)
                return;
                
            MovePlayer();
        }
        
        private void HandleInput()
        {
            // WASD movement (user preference)
            movement.x = 0f;
            movement.y = 0f;
            
            if (Input.GetKey(KeyCode.W))
                movement.y = 1f;
            if (Input.GetKey(KeyCode.S))
                movement.y = -1f;
            if (Input.GetKey(KeyCode.A))
                movement.x = -1f;
            if (Input.GetKey(KeyCode.D))
                movement.x = 1f;
            
            // Normalize diagonal movement
            movement = movement.normalized;
            
            // Running with Shift
            isRunning = Input.GetKey(KeyCode.LeftShift);
            
            // Tool usage
            if (Input.GetMouseButtonDown(0) && !isUsingTool)
            {
                UseTool();
            }
            
            // Tool switching (1-5 keys)
            if (Input.GetKeyDown(KeyCode.Alpha1))
                SwitchTool(ToolType.Hoe);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                SwitchTool(ToolType.WateringCan);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                SwitchTool(ToolType.Seeds);
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                SwitchTool(ToolType.Axe);
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                SwitchTool(ToolType.Pickaxe);
            
            // Store last movement for idle direction
            if (movement != Vector2.zero)
                lastMovement = movement;
        }
        
        private void MovePlayer()
        {
            if (isUsingTool)
                return;
                
            float currentSpeed = moveSpeed;
            if (isRunning)
                currentSpeed *= runSpeedMultiplier;
                
            rb.MovePosition(rb.position + movement * currentSpeed * Time.fixedDeltaTime);
        }
        
        private void HandleInteraction()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                InteractWithNearbyObjects();
            }
        }
        
        private void InteractWithNearbyObjects()
        {
            Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactableLayer);
            
            foreach (Collider2D obj in nearbyObjects)
            {
                IInteractable interactable = obj.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact(this);
                    break; // Interact with the first found object
                }
            }
        }
        
        private void UseTool()
        {
            if (currentToolType == ToolType.None)
                return;
                
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            
            Vector2 direction = (mousePos - transform.position).normalized;
            Vector3 targetPosition = transform.position + (Vector3)direction * interactionRange;
            
            switch (currentToolType)
            {
                case ToolType.Hoe:
                    TillSoil(targetPosition);
                    break;
                case ToolType.WateringCan:
                    WaterCrops(targetPosition);
                    break;
                case ToolType.Seeds:
                    PlantSeeds(targetPosition);
                    break;
                case ToolType.Axe:
                    ChopTree(targetPosition);
                    break;
                case ToolType.Pickaxe:
                    MineRock(targetPosition);
                    break;
            }
            
            StartCoroutine(ToolUsageAnimation());
        }
        
        private System.Collections.IEnumerator ToolUsageAnimation()
        {
            isUsingTool = true;
            
            if (animator != null)
                animator.SetTrigger("UseTool");
                
            yield return new WaitForSeconds(0.5f);
            
            isUsingTool = false;
        }
        
        private void TillSoil(Vector3 position)
        {
            FarmTile tile = GetFarmTileAtPosition(position);
            if (tile != null && tile.CanBeTilled())
            {
                tile.TillSoil();
                GameManager.Instance.AddExperience(5);
            }
        }
        
        private void WaterCrops(Vector3 position)
        {
            FarmTile tile = GetFarmTileAtPosition(position);
            if (tile != null && tile.HasCrop() && !tile.IsWatered())
            {
                tile.WaterCrop();
                GameManager.Instance.AddExperience(3);
            }
        }
        
        private void PlantSeeds(Vector3 position)
        {
            FarmTile tile = GetFarmTileAtPosition(position);
            if (tile != null && tile.IsTilled() && !tile.HasCrop())
            {
                // Check if player has seeds and money
                if (GameManager.Instance.SpendMoney(10))
                {
                    tile.PlantCrop(CropType.Wheat); // Default crop
                    GameManager.Instance.AddExperience(10);
                }
            }
        }
        
        private void ChopTree(Vector3 position)
        {
            Tree tree = GetTreeAtPosition(position);
            if (tree != null)
            {
                tree.Chop();
                GameManager.Instance.AddMoney(20);
                GameManager.Instance.AddExperience(15);
            }
        }
        
        private void MineRock(Vector3 position)
        {
            Rock rock = GetRockAtPosition(position);
            if (rock != null)
            {
                rock.Mine();
                GameManager.Instance.AddMoney(15);
                GameManager.Instance.AddExperience(12);
            }
        }
        
        private FarmTile GetFarmTileAtPosition(Vector3 position)
        {
            Collider2D collider = Physics2D.OverlapPoint(position);
            return collider?.GetComponent<FarmTile>();
        }
        
        private Tree GetTreeAtPosition(Vector3 position)
        {
            Collider2D collider = Physics2D.OverlapPoint(position);
            return collider?.GetComponent<Tree>();
        }
        
        private Rock GetRockAtPosition(Vector3 position)
        {
            Collider2D collider = Physics2D.OverlapPoint(position);
            return collider?.GetComponent<Rock>();
        }
        
        private void SwitchTool(ToolType newTool)
        {
            currentToolType = newTool;
            
            // Update tool visual
            if (currentTool != null)
                Destroy(currentTool);
                
            // Instantiate new tool (would need tool prefabs)
            // currentTool = Instantiate(toolPrefabs[(int)newTool], toolHolder);
            
            Debug.Log($"Switched to tool: {newTool}");
        }
        
        private void UpdateAnimation()
        {
            if (animator == null)
                return;
                
            // Set animation parameters
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
            animator.SetFloat("Speed", movement.magnitude);
            animator.SetBool("IsRunning", isRunning && movement.magnitude > 0);
            animator.SetBool("IsUsingTool", isUsingTool);
            
            // Set idle direction
            if (movement.magnitude == 0)
            {
                animator.SetFloat("LastMoveX", lastMovement.x);
                animator.SetFloat("LastMoveY", lastMovement.y);
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            // Draw interaction range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCircle(transform.position, interactionRange);
        }
    }
    
    // Interface for interactable objects
    public interface IInteractable
    {
        void Interact(PlayerController player);
    }
}

