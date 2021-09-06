using UnityEngine.Events;
using TMPro;

namespace UnityEngine.Purchasing
{
    /// <summary>
    /// A GUI component for exposing the current price and allow purchasing of In-App Purchases. Exposes configurable
    /// elements through the Inspector.
    /// </summary>
    /// <seealso cref="CodelessIAPStoreListener"/>
    //[RequireComponent(typeof(BoxCollider))]
#if ENABLE_EDITOR_GAME_SERVICES
    [AddComponentMenu("In-App Purchasing/IAP Button")]
#else
    [AddComponentMenu("Unity IAP/Custom IAP Button")]
#endif
    [HelpURL("https://docs.unity3d.com/Manual/UnityIAP.html")]
    public class CustomIAPButton : MonoBehaviour
    {
        /// <summary>
        /// The type of this button, can be either a purchase or a restore button.
        /// </summary>
        public enum ButtonType
        {
            /// <summary>
            /// This button will display localized product title and price. Clicking will trigger a purchase.
            /// </summary>
            Purchase,
            /// <summary>
            /// This button will display a static string for restoring previously purchased non-consumable
            /// and subscriptions. Clicking will trigger this restoration process, on supported app stores.
            /// </summary>
            Restore
        }

        /// <summary>
        /// Type of event fired after a successful purchase of a product.
        /// </summary>
        [System.Serializable]
        public class OnPurchaseCompletedEvent : UnityEvent<Product>
        {
        };

        /// <summary>
        /// Type of event fired after a failed purchase of a product.
        /// </summary>
        [System.Serializable]
        public class OnPurchaseFailedEvent : UnityEvent<Product, PurchaseFailureReason>
        {
        };

        /// <summary>
        /// Which product identifier to represent. Note this is not a store-specific identifier.
        /// </summary>
        [HideInInspector]
        public string productId;

        /// <summary>
        /// The type of this button, can be either a purchase or a restore button.
        /// </summary>
        [Tooltip("The type of this button, can be either a purchase or a restore button.")]
        public ButtonType buttonType = ButtonType.Purchase;

        // Text for this button
        TextMeshPro tmp;
        bool toggle;

        void Start()
        {
            tmp = GetComponentInChildren<TextMeshPro>();
        }

        void Update()
        {
            if (!toggle && buttonType == ButtonType.Purchase)
            {
                if (CodelessIAPStoreListener.initializationComplete)
                {
                    UpdateText();
                    toggle = true;
                }
            }
        }

        public void OnClick()
        {
            if (buttonType == ButtonType.Purchase)
            {
                PurchaseProduct();
            }
            else
            {
                Restore();
            }
        }

        void PurchaseProduct()
        {
            if (buttonType == ButtonType.Purchase)
            {
                CodelessIAPStoreListener.Instance.InitiatePurchase(productId);
            }
        }

        void Restore()
        {
            if (buttonType == ButtonType.Restore)
            {
                if (Application.platform == RuntimePlatform.WSAPlayerX86 ||
                    Application.platform == RuntimePlatform.WSAPlayerX64 ||
                    Application.platform == RuntimePlatform.WSAPlayerARM)
                {
                    CodelessIAPStoreListener.Instance.GetStoreExtensions<IMicrosoftExtensions>()
                        .RestoreTransactions();
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer ||
                         Application.platform == RuntimePlatform.OSXPlayer ||
                         Application.platform == RuntimePlatform.tvOS)
                {
                    CodelessIAPStoreListener.Instance.GetStoreExtensions<IAppleExtensions>()
                        .RestoreTransactions(OnTransactionsRestored);
                }
                else if (Application.platform == RuntimePlatform.Android &&
                    StandardPurchasingModule.Instance().appStore == AppStore.GooglePlay)
                {
                    CodelessIAPStoreListener.Instance.GetStoreExtensions<IGooglePlayStoreExtensions>()
                        .RestoreTransactions(OnTransactionsRestored);
                }
                else
                {
                    Debug.LogWarning(Application.platform.ToString() +
                                     " is not a supported platform for the Codeless IAP restore button");
                }
            }
        }

        void OnTransactionsRestored(bool success)
        {
            //TODO: Add an invocation hook here for developers.
        }

        internal void UpdateText()
        {
            var product = CodelessIAPStoreListener.Instance.GetProduct(productId);
            if (product != null)
            {
                var pointTitle = product.metadata.localizedTitle.Split();
                tmp.text = pointTitle[0] + " " + pointTitle[1] + " - " + product.metadata.localizedPriceString;
            }
        }
    }
}
