using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IInteractable
{
    [SerializeField] float _weaponDamage;
    [SerializeField] float _weaponRange;
    [SerializeField] Sprite _weaponSprite;

    public float WeaponDamage { get => _weaponDamage;}
    public float WeaponRange { get => _weaponRange;}
    public Sprite WeaponSprite { get => _weaponSprite;}

    public void Interaction(Player playerCharacter)
    {
        playerCharacter.GrabedItem(this.gameObject);
    }
}
