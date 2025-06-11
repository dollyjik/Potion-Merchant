using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MoneyManager : MonoBehaviour
{
    public int currentMoney;
    public TextMeshProUGUI moneyText;

    private void Start()
    {
        currentMoney = 0;
        UpdateMoneyText();
    }

    public void UpdateMoneyText()
    {
        moneyText.text = currentMoney.ToString() + "$";
    }

    public void AddMoney()
    {
        currentMoney += 1000;
        UpdateMoneyText();
    }

    public void SubtractMoney(int price)
    {
        currentMoney -= price;
        UpdateMoneyText();
    }
}
