using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackViewModel : MonoBehaviour
{
    private int comboIndex = 0;
    private float comboTimer = 0f;
    private bool isAttacking = false;

    public List<AttackModel> comboList;
    public WeaponController weaponView;

}