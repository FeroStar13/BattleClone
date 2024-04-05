using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _hpValueText;
    [SerializeField] Image[] _hotbarSlots;

    [SerializeField] Player _playerCharacter;

    [SerializeField] Image[] _hotbarBackgroundSlots;

    public static PlayerHUD instance;

    public Image[] HotbarSlots { get => _hotbarSlots; set => _hotbarSlots = value; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;

        }
    }

    private void Update()
    {
       
    }

    public void UpdateHP(float hpValue)
    {
        _hpValueText.text = hpValue.ToString();
    }

    public void SelecterSlotHotbar()
    {
        for (int i = 0; i < _hotbarBackgroundSlots.Length; i++)
        {
            Vector3 normalScale = new Vector3(0.5f, 0.5f, 0.5f);
            _hotbarBackgroundSlots[i].rectTransform.localScale = normalScale;
        }
        Vector3 bigScale = new Vector3(0.65f, 0.65f, 0.65f);
        _hotbarBackgroundSlots[_playerCharacter.CurrentSlot].rectTransform.localScale = bigScale;


    }
}
