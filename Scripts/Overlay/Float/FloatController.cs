using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatController : MonoBehaviour
{
    public GameObject floatTextPrefab, floatDamagePrefab;
    public List<FloatingText> floatTextPool, floatDamagePool;

    public void Float(FloatData floatData)
    {
        // 풀에서 비활성화된 FloatingText를 찾음
        var floatingText = floatTextPool.FirstOrDefault(text => !text.gameObject.activeSelf);

        // 사용 가능한 FloatingText가 없으면 새로 생성
        if (floatingText == null)
        {
            GameObject newTextObj = Instantiate(floatTextPrefab, transform);
            floatingText = newTextObj.GetComponent<FloatingText>();
            floatTextPool.Add(floatingText);
        }

        // FloatingText 활성화 및 Float 메서드 호출
        floatingText.gameObject.SetActive(true);
        floatingText.Float(floatData);
    }
    public void FloatDamage(FloatData floatData)
    {
        // 풀에서 비활성화된 FloatingText를 찾음
        var floatingText = floatDamagePool.FirstOrDefault(text => !text.gameObject.activeSelf);

        // 사용 가능한 FloatingText가 없으면 새로 생성
        if (floatingText == null)
        {
            GameObject newTextObj = Instantiate(floatDamagePrefab, transform);
            floatingText = newTextObj.GetComponent<FloatingText>();
            floatDamagePool.Add(floatingText);
        }

        // FloatingText 활성화 및 Float 메서드 호출
        floatingText.gameObject.SetActive(true);
        floatingText.Float(floatData);
    }
}
