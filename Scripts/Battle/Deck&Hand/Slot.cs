using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public WeaponData weaponData;
    public Image image;
    public Sprite sprite
    {
        get { return image.sprite; }
    }

    public void Set(string weapon, Image image)
    {
        this.image = image;

        weaponData = DataManager.instance.AllWeaponDatas[weapon];
    }

    public void Set()
    {
        weaponData = new WeaponData();
    }
}
