using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace TheSparkyStudios
{
    [Serializable]
    public class ConsumableItem
    {
        public string Id;
        public string Name;
        public string Description;
        public float Price;
    }

    [Serializable]
    public class NonConsumableItem
    {
        public string Id;
        public string Name;
        public string Description;
        public float Price;
    }

    public class ShopManager : MonoBehaviour, IDetailedStoreListener
    {
        [Header("For IAP Settings..")]
        public List<ConsumableItem> ConsumableItemList;
        public NonConsumableItem NonConsumableItem;
        private IStoreController m_storeController;

        [Header("Other Panels and GameObjects")]
        //public TextMeshProUGUI CoinText;
        //public TextMeshProUGUI CoinTextOfMainMenu;
        public GameObject PurchasedText;
        public GameObject PanelToDisappearAfterPurchasedNoAds;
        public GameObject PurchasedBlackScreenPanel;
        public GameObject PremiumIconInNoAdsPanel;

        public Image MainMenuNoAdsButton;
        public Sprite MainMenuAdsPurchasedFrame;
        //public TextMeshProUGUI NoAdsText;
        //public TextMeshProUGUI PurchasedTextMainMenu;

        // Initialize in Home......
        private void OnEnable()
        {
            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                SetupBuilder();
            }
        }

        #region ------------------------- IAP Helper Methods -----------------------------

        public void SetupBuilder()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            foreach (ConsumableItem ci in ConsumableItemList)
            {
                builder.AddProduct(ci.Id, ProductType.Consumable);
            }

            builder.AddProduct(NonConsumableItem.Id, ProductType.NonConsumable);

            UnityPurchasing.Initialize(this, builder);
        }

        private void CheckNonConsumable(string id)
        {
            if (m_storeController != null)
            {
                var product = m_storeController.products.WithID(id);
                if (product != null)
                {
                    if (product.hasReceipt)
                    {
                        RemoveAds();
                    }
                    else
                    {
                        ShowAds();
                    }
                }

            }
        }


        #endregion ---------------------------------------------------------------------

        #region ------------------------- Helpers Methods ------------------------

        //private void AddCoins(int coin)
        //{
        //    int newCoin = PlayerPrefs.GetInt(Constants.PlayerCoin);
        //    PlayerPrefs.SetInt(Constants.PlayerCoin, newCoin + coin);
        //    PlayerPrefs.Save();
        //    CoinText.SetText((newCoin + coin).ToString());
        //    CoinTextOfMainMenu.SetText((newCoin + coin).ToString());
        //    AudioManagerForButtons.Instance.Play(SoundNames.CoinCollect);
        //}

        private void RemoveAds()
        {
            //AudioManagerForButtons.Instance.Play(SoundNames.Claim);
            //PlayerPrefs.SetInt(Constants.HasBoughtNoAds, 1);
            PlayerPrefs.Save();
            PurchasedText.SetActive(true);
            PurchasedBlackScreenPanel.SetActive(true);
            PanelToDisappearAfterPurchasedNoAds.SetActive(false);
            PremiumIconInNoAdsPanel.SetActive(false);
            MainMenuNoAdsButton.sprite = MainMenuAdsPurchasedFrame;
            MainMenuNoAdsButton.GetComponent<Button>().enabled = false;
            //NoAdsText.gameObject.SetActive(false);
            //PurchasedTextMainMenu.gameObject.SetActive(true);
            Debug.Log("Removed Ads");
        }

        private void ShowAds()
        {
            //PlayerPrefs.SetInt(Constants.HasBoughtNoAds, 0);
            PlayerPrefs.Save();
            PurchasedText.SetActive(false);
            PurchasedBlackScreenPanel.SetActive(false);
            PanelToDisappearAfterPurchasedNoAds.SetActive(true);
            MainMenuNoAdsButton.GetComponent<Button>().enabled = true;
            //NoAdsText.gameObject.SetActive(true);
            //PurchasedTextMainMenu.gameObject.SetActive(false);
            Debug.Log("Show Ads");
        }

        #endregion ------------------------------------------------------ 

        #region ------------------------- Button CLicks ------------------------

        public void OnClick1000Coins()
        {
            m_storeController.InitiatePurchase(ConsumableItemList[0].Id);
            //FirebaseAnalytics.LogEvent(FirebaseAnalyticsConstants.ShopCoinClick, new Parameter(FirebaseAnalyticsConstants.ShopCoinAmount, 1000));
            //FirebaseAnalytics.LogEvent(FirebaseAnalyticsConstants.ShopCoinClick + " for " + "1000 Coins");
            Debug.Log("Clicked");
        }
        public void OnClick5000Coins()
        {
            m_storeController.InitiatePurchase(ConsumableItemList[1].Id);
            //FirebaseAnalytics.LogEvent(FirebaseAnalyticsConstants.ShopCoinClick, new Parameter(FirebaseAnalyticsConstants.ShopCoinAmount, 5000));
            //FirebaseAnalytics.LogEvent(FirebaseAnalyticsConstants.ShopCoinClick + " for " + "5000 Coins");
            Debug.Log("Clicked");
        }
        public void OnClick20000Coins()
        {
            m_storeController.InitiatePurchase(ConsumableItemList[2].Id);
            //FirebaseAnalytics.LogEvent(FirebaseAnalyticsConstants.ShopCoinClick, new Parameter(FirebaseAnalyticsConstants.ShopCoinAmount, 20000));
            //FirebaseAnalytics.LogEvent(FirebaseAnalyticsConstants.ShopCoinClick + " for " + "20000 Coins");
            Debug.Log("Clicked");
        }

        public void OnClickNoAds()
        {
            m_storeController.InitiatePurchase(NonConsumableItem.Id);
            //FirebaseAnalytics.LogEvent(FirebaseAnalyticsConstants.ShopNoAdsClick);
            Debug.Log("Clicked");
        }

        #endregion ------------------------------------------------------

        #region ------------------ IAP Callbacks ---------------------------------
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            m_storeController = controller;
            CheckNonConsumable(NonConsumableItem.Id);
            Debug.Log("Successfully Initialzed IAP");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            Debug.Log("Purchased Completed..");
            var product = purchaseEvent.purchasedProduct;
            if (product.definition.id == ConsumableItemList[0].Id) // 1000 coins
            {

            }
            else if (product.definition.id == ConsumableItemList[1].Id) // 5000 coins
            {

            }
            else if (product.definition.id == ConsumableItemList[2].Id) // 20000 coins
            {
                //AddCoins(20000);
            }
            else if (product.definition.id == NonConsumableItem.Id) // no ads
            {
                RemoveAds();
            }
            return PurchaseProcessingResult.Complete;
        }


        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.Log("Purchased failed: " + failureDescription.ToString());
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log("Initialize failed: " + error.ToString());
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.Log("Initialize failed: " + error.ToString());
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log("Purchased failed: " + failureReason.ToString());
        }
        #endregion --------------------------------------------------------------
    }
}