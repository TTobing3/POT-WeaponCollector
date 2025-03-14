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
        // Ǯ���� ��Ȱ��ȭ�� FloatingText�� ã��
        var floatingText = floatTextPool.FirstOrDefault(text => !text.gameObject.activeSelf);

        // ��� ������ FloatingText�� ������ ���� ����
        if (floatingText == null)
        {
            GameObject newTextObj = Instantiate(floatTextPrefab, transform);
            floatingText = newTextObj.GetComponent<FloatingText>();
            floatTextPool.Add(floatingText);
        }

        // FloatingText Ȱ��ȭ �� Float �޼��� ȣ��
        floatingText.gameObject.SetActive(true);
        floatingText.Float(floatData);
    }
    public void FloatDamage(FloatData floatData)
    {
        // Ǯ���� ��Ȱ��ȭ�� FloatingText�� ã��
        var floatingText = floatDamagePool.FirstOrDefault(text => !text.gameObject.activeSelf);

        // ��� ������ FloatingText�� ������ ���� ����
        if (floatingText == null)
        {
            GameObject newTextObj = Instantiate(floatDamagePrefab, transform);
            floatingText = newTextObj.GetComponent<FloatingText>();
            floatDamagePool.Add(floatingText);
        }

        // FloatingText Ȱ��ȭ �� Float �޼��� ȣ��
        floatingText.gameObject.SetActive(true);
        floatingText.Float(floatData);
    }
}
