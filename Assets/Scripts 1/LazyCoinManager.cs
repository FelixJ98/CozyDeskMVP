using System;
using TMPro;
using UnityEngine;

public class LazyCoinManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
        [SerializeField] TextMeshProUGUI money;
        private int npcCount = 0;
        private int houseCount = 0;
        private int libraryCount = 0;
        private int bowlingCount = 0;
        private int theaterCount = 0;
        public int coinValue = 100000; // Public getter

        private float coinUpdateTimer = 0f;
        private float updateInterval = 1f;
        
        
        void Start()
        {
            // Optionally initialize
 
        }

        public void Update()
        {
            coinUpdateTimer += Time.deltaTime;
            if (coinUpdateTimer >= updateInterval)
            {
                UpdateCoins();
                coinUpdateTimer = 0f;
            }
        }

        public void RegisterHouse()
        {
            houseCount++;
        }

        public void RegisterLibrary()
        {
            libraryCount++;
        }

        public void RegisterTheater()
        {
            theaterCount++;
        }

        public void RegisterBowling()
        {
            bowlingCount++;
        }

        public void RegisterNPC()
        {
            npcCount++;
            UpdateCoins();
        }

        private void UpdateCoins()
        {
            coinValue += (npcCount * 1) + (houseCount * 2) + (theaterCount * 5) + (bowlingCount * 8) + (libraryCount * 13); 
            money.text = $"Bank: {coinValue}";
            Debug.Log("Coins: " + coinValue);
        }

        public bool TrySpendCoins(int amount)
        {
            if (coinValue >= amount)
            {
                coinValue -= amount;
                Debug.Log($"Spent {amount} coins. Remaining: {coinValue}");
                return true;
            }
            else
            {
                Debug.Log("Not enough coins!");
                return false;
            }
        }
    }
