using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class WeaponSprite : MonoBehaviour
{

    [SerializeField] GameObject weaponPrefab;

    List<GameObject> weaponPool = new List<GameObject>();

    GameObject GetObjectFromPool()
    {
        GameObject weapon = null;

        // 남는 오브젝트가 있으면 가져오기
        foreach(GameObject i in weaponPool)
        {
            if (i.GetComponent<SpriteRenderer>().enabled) continue;

            i.GetComponent<SpriteRenderer>().enabled = true;
            weapon = i;

            break;
        }

        // 없으면 만들기
        if(weapon == null)
        {
            weapon = Instantiate(weaponPrefab);
            weapon.transform.SetParent(transform);

            weaponPool.Add(weapon);
        }

        return weapon;
    }

    GameObject GetReadyGameObject(string weaponName)
    {
        var weapon = GetObjectFromPool().transform;

        weapon.localScale = new Vector3(-1, 1, 1);
        weapon.localPosition = new Vector2(-1, 0);
        weapon.localRotation = Quaternion.Euler(0, 0, 0);
        weapon.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        weapon.GetComponent<SpriteResolver>().SetCategoryAndLabel("Weapon", weaponName);

        return weapon.gameObject;
    }

    public void Attack(DeckSlot deckSlot)
    {
        var weaponAnimator = GetReadyGameObject(deckSlot.GetWeaponData.name).GetComponent<Animator>();

        var type = (int)DataManager.instance.GetWeaponType(deckSlot.GetWeaponData.type);
        weaponAnimator.SetInteger("type", type);
        weaponAnimator.SetTrigger("act");
    }

}
