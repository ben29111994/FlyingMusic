using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SonicBloom.Koreo
{
    public class ModelInfo : MonoBehaviour
    {
        public Text selectedText;
        public int shipNumber;
        public GameObject buttonBuyShip;
        // Use this for initialization
        void Start()
        {
            var unlockStatus = PlayerPrefs.GetInt("ship" + shipNumber.ToString());
            if (unlockStatus == 1 && shipNumber != 0)
            {
                buttonBuyShip.SetActive(false);
            }
            var currentShip = PlayerPrefs.GetInt("ship");
            if (currentShip == shipNumber)
            {
                selectedText.text = "SELECTED";
            }
            else
            {
                selectedText.text = "SELECT";
            }
        }

        public void OnSelectShip()
        {
            PlayerPrefs.SetInt("ship", shipNumber);
            foreach (var item in UIManager.instance.selectedText)
            {
                item.text = "SELECT";
            }
            selectedText.text = "SELECTED";
            MenuManager.MenuManagerInstance.OnButtonPress();
        }

        public void OnBuyShip()
        {
            var currentGem = PlayerPrefs.GetInt("gem");
            if (currentGem >= 200)
            {
                currentGem -= 200;
                PlayerPrefs.SetInt("gem", currentGem);
                PlayerPrefs.SetInt("ship" + shipNumber.ToString(), 1);
                UIManager.instance.gemText.text = currentGem.ToString();
                MenuManager.MenuManagerInstance.OnButtonPress();
                var button = EventSystem.current.currentSelectedGameObject.gameObject;
                button.SetActive(false);
            }
            else
                SoundManager.instance.PlaySound(SoundManager.instance.invalid);
        }
    }
}
