using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Player : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    [SerializeField] float _movementSpeed;

    [SerializeField] float _jumpForce;
    bool _isGrounded;
    [SerializeField] float _groundDistance;
    [SerializeField] LayerMask _groundMask;

    [Header("References")]
    Rigidbody _rigidBody;
    [SerializeField] GameObject _weaponHandPosition;
    [SerializeField] PlacementSystem _placementSystem;

    [Header("Life")]
    [SerializeField] float _currentHP;
    [SerializeField] float _maxHP;

    [Header("Attack")]
    [SerializeField] float _currentDamage;
    [SerializeField] float _baseDamage;

    [SerializeField] float _currentRange;
    [SerializeField] float _baseRange;

    [Header("Inventory")]

    [SerializeField] GameObject[] _hotbar;
    [SerializeField] int _currentSlot = 0;
    bool hotbarIsFull;

    [Header("Item")]
    [SerializeField] GameObject _currentItem;

    [Header("Building")]
   [SerializeField] bool _isBuildingMode;

    public float CurrentDamage { get => _currentDamage; set => _currentDamage = value; }
    public float CurrentRange { get => _currentRange; set => _currentRange = value; }
    public GameObject[] Hotbar { get => _hotbar; set => _hotbar = value; }
    public int CurrentSlot { get => _currentSlot; set => _currentSlot = value; }
    public bool HotbarIsFull { get => hotbarIsFull; set => hotbarIsFull = value; }
    public bool IsBuildingMode { get => _isBuildingMode; set => _isBuildingMode = value; }


    // Start is called before the first frame update
    void Awake()
    {
        _rigidBody = GetComponentInChildren<Rigidbody>();

        _currentHP = _maxHP;
        _currentDamage = _baseDamage;
        CurrentRange = _baseRange;
 

        PlayerHUD.instance.UpdateHP(_currentHP);

    }

    void Update()
    {
       if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(IsBuildingMode == false)
            {
            GiveDamage();
            }
           
        }
       if(Input.GetKey(KeyCode.Mouse0))
        {
            
            if (IsBuildingMode == true)
            {
                _placementSystem.Build();
            } 
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GroundCheck();
        }

       if(Input.GetKeyDown(KeyCode.E))
        {
            Grab();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropItem();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            OnAndOffBuildingMode(true);
            _placementSystem._buildingBlocks = PlacementSystem.BuildingBlocksEnum.Floor;
            _placementSystem.ChangePlacementBlock();

        }
        if (Input.GetKeyDown(KeyCode.Mouse4))
        {
            OnAndOffBuildingMode(true);

            _placementSystem._buildingBlocks = PlacementSystem.BuildingBlocksEnum.Wall;
            _placementSystem.ChangePlacementBlock();

        }
        if (Input.GetKeyDown(KeyCode.Mouse3))
        {
            OnAndOffBuildingMode(true);

            _placementSystem._buildingBlocks = PlacementSystem.BuildingBlocksEnum.Ramp;
            _placementSystem.ChangePlacementBlock();

        }


        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if(IsBuildingMode == false)
        {
            if (scrollInput > 0f)
            {
                //scroll up
                SwitchSlot(true);
            } 
            else if (scrollInput < 0f)
            {
                //scroll down
                SwitchSlot(false);
            }
        }
        // Check if any number key (1 to 9) is pressed
        if (Input.anyKeyDown && Input.inputString.Length == 1 && Input.inputString[0] >= '1' && Input.inputString[0] <= '9')
            {
                //Get the pressed number as an integer
                int keyPressed = int.Parse(Input.inputString);

                //Changes item slot
                CurrentSlot = keyPressed - 1;

                //Make item apear on hand
                 ItemHandUpdate();
                PlayerHUD.instance.SelecterSlotHotbar();

                //Deactivates building mode
                OnAndOffBuildingMode(false);
            }
       
    }

    #region Player Movement
    private void FixedUpdate()
    {
        MoveInDirection(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
    }

    private void MoveInDirection(Vector2 direction)
    {
        Vector3 finalVelocity = (direction.x * transform.right + direction.y * transform.forward).normalized * _movementSpeed;

        finalVelocity.y = _rigidBody.velocity.y;
        _rigidBody.velocity = finalVelocity;
    }

    void GroundCheck()
    {
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, _groundDistance, _groundMask);

        // Jump input
        if (_isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        _rigidBody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }

    #endregion

    #region Player Actions
    void Grab()
    {
       

        RaycastHit hit; 

            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, _baseRange))
            {
                IInteractable interacted = hit.transform.GetComponent<IInteractable>();

                if (interacted != null)
                {
                    interacted.Interaction(this);
                }
            }
        
    }

    public void GrabedItem(GameObject grabedItem)
    {
      
            //place weapon in hand
            grabedItem.transform.position = _weaponHandPosition.transform.position;
            grabedItem.transform.rotation = _weaponHandPosition.transform.rotation;
            grabedItem.transform.SetParent(this.transform);

            //freeze weapon position in hand place
            Rigidbody grabedWeaponRB = grabedItem.GetComponent<Rigidbody>();
            grabedWeaponRB.constraints = RigidbodyConstraints.FreezeAll;

            //disable collision on hand
            BoxCollider grabedItemCollider = grabedItem.GetComponent<BoxCollider>();
            grabedItemCollider.enabled = false;

        if (_currentItem == null)
        {

            _currentItem = grabedItem;

            //Add item to the hotbar
            Hotbar[CurrentSlot] = _currentItem;

            //Give to the player the item information needed
            GetItemInfo(grabedItem, CurrentSlot);
        }
        else
        {
            for (int i = 0; i < Hotbar.Length; i++)
            {
                if (Hotbar[i] == null)
                {
                    Hotbar[i] = grabedItem;
                    GetItemInfo(grabedItem, i);
                    
                return;
                }
            }
        }

        
    }

    void DropItem()
    {
        if(_currentItem != null)
        {
        //Remove current weapon from the hotbar
        Hotbar[CurrentSlot] = null;

        //unfreeze weapon position 
        Rigidbody currentWeaponRB = _currentItem.GetComponent<Rigidbody>();
        currentWeaponRB.constraints = RigidbodyConstraints.None;

        //able weapon collision 
        BoxCollider currentWeaponCollider = _currentItem.GetComponent<BoxCollider>();
        currentWeaponCollider.enabled = true;

        //removes from player
        _currentItem.transform.SetParent(null);
        _currentItem = null;

        //Resets Damage and range to base values
        _currentDamage = _baseDamage;
        _currentRange = _baseRange;

        //Reset slot Icon 
        PlayerHUD.instance.HotbarSlots[CurrentSlot].sprite = null;
        PlayerHUD.instance.HotbarSlots[CurrentSlot].enabled = false;
        }
    }

    void GiveDamage()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, CurrentRange))
        {
            IDamageable characterHit = hit.transform.GetComponent<IDamageable>();
            if (characterHit != null)
            {
                characterHit.TakeDamage(CurrentDamage); //Gives your damage to those that can be damaged (has idamageable)
            }
        }
    }

    #endregion

    public void TakeDamage(float DamageToTake)
    {
        _currentHP = _currentHP - DamageToTake;
        PlayerHUD.instance.UpdateHP(_currentHP);

        if(_maxHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        //Death Logic
    }

    public void SwitchSlot(bool scrollUp)
    {
        //changes to the next slot using scroll

        if(scrollUp == false)
        {

            if(CurrentSlot < Hotbar.Length - 1)
            {
                CurrentSlot++;
            }
            else
            {
                CurrentSlot = 0;
            }
        }
        else
        {
            if (CurrentSlot > 0)
            {
                CurrentSlot--;
            }
            else
            {
                CurrentSlot = Hotbar.Length - 1;
            }
        }
        PlayerHUD.instance.SelecterSlotHotbar();
        ItemHandUpdate();
    }

    public void ItemHandUpdate()
    {
        //deactivates current item
        if (_currentItem != null)
        {
            _currentItem.SetActive(false);
            _currentItem = null;

            //Reset player values to normal 
            ResetInfo();
        }

        //activates next slot item
        if (Hotbar[CurrentSlot] != null)
        {
            Hotbar[CurrentSlot].SetActive(true);
            _currentItem = Hotbar[CurrentSlot];

            //Get On hand item their information
            GetItemInfo(_currentItem, CurrentSlot);
        }
    }

    public void GetItemInfo(GameObject collectedWeapon, int itemPosToUpdateUI)
    {
       Weapon weapon = collectedWeapon.GetComponent<Weapon>();

        PlayerHUD.instance.HotbarSlots[itemPosToUpdateUI].sprite = weapon.WeaponSprite;
        PlayerHUD.instance.HotbarSlots[itemPosToUpdateUI].enabled = true;
    }

    public void ResetInfo()
    {
        _currentRange = _baseRange;
        CurrentDamage = _baseDamage;
    }

    void OnAndOffBuildingMode(bool allowBuild)
    {
        if(IsBuildingMode == true && allowBuild == false)
        {
            IsBuildingMode = false;

            _placementSystem._buildingBlocks = PlacementSystem.BuildingBlocksEnum.Null;
            _placementSystem.CellIndicator.SetActive(false);
   
        }
        else if(IsBuildingMode == false && allowBuild == true) 
        {
            IsBuildingMode = true;
        }
    }

   
}
