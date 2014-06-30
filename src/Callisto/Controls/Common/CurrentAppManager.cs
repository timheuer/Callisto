using System;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Storage;

namespace Callisto.Controls.Common
{
    public sealed partial class CurrentAppManager
    {
        #region events
        #endregion

        #region fields
        private static volatile CurrentAppManager _instance;
        private static readonly object SyncRoot = new object();
        #endregion

        #region constructors
        private CurrentAppManager()
        {
#if !DEBUG
            CurrentAppManager.UseSimulator = false;
#else
            CurrentAppManager.UseSimulator = true;
#endif
        }
        #endregion

        #region properties

        #region Current
        /// <summary>
        /// Provides access to the app settings contract.
        /// </summary>
        /// <value>The app settings contract.</value>
        public static CurrentAppManager Current
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new CurrentAppManager();
                        }
                    }
                }


                return _instance;
            }
        }
        #endregion

        public static bool UseSimulator
        {
            get;
            set;
        }

        #region AppId
        /// <summary>
        /// Gets the app ID.
        /// </summary>
        /// <value>The <see cref="Guid"/> that identifies the app.</value>
        public Guid AppId
        {
            get
            {
                return UseSimulator ? CurrentAppSimulator.AppId : CurrentApp.AppId;
            }
        }
        #endregion

        #region LicenseInformation
        /// <summary>
        /// Gets the license metadata for the current app.
        /// </summary>
        /// <value>The license metadata for the current app.</value>
        public LicenseInformation LicenseInformation
        {
            get
            {
                return UseSimulator ? CurrentAppSimulator.LicenseInformation : CurrentApp.LicenseInformation;
            }
        }
        #endregion

        #region LinkUri
        /// <summary>
        /// Gets the Uniform Resource Identifier (URI) that represents a Windows
        ///  Store listing page for the current app.
        /// </summary>
        /// <value>The <see cref="Uri"/> of the listing page for the current app.</value>
        public Uri LinkUri
        {
            get
            {
                return UseSimulator ? CurrentAppSimulator.LinkUri : CurrentApp.LinkUri;
            }
        }
        #endregion

        #endregion

        #region methods

        #region GetAppReceiptAsync
        /// <summary>
        /// Requests all receipts for this app and any in-app purchases.
        /// </summary>
        /// <returns>An XML-formatted string that contains all receipts for 
        /// this app and any in-app purchases.</returns>
        public IAsyncOperation<string> GetAppReceiptAsync()
        {
            return UseSimulator ? CurrentAppSimulator.GetAppReceiptAsync() : CurrentApp.GetAppReceiptAsync();
        }
        #endregion

        #region GetProductReceiptAsync
        /// <summary>
        /// Requests the receipt for the <paramref name="productId"/> specified.
        /// </summary>
        /// <param name="productId">The unique identifier for the product
        /// that you specified this identifier when you submitted the app
        /// to the Store.</param>
        /// <returns>An XML-formatted string that contains the receipt for
        /// the specified <paramref name="productId"/>.</returns>
        public IAsyncOperation<string> GetProductReceiptAsync(string productId)
        {
            return UseSimulator ? CurrentAppSimulator.GetProductReceiptAsync(productId) : CurrentApp.GetProductReceiptAsync(productId);
        }
        #endregion

        #region LoadListingInformationAsync
        /// <summary>
        /// Loads the app's listing information asynchronously.
        /// </summary>
        /// <returns>The app listing information. If the method fails, it
        /// returns an HRESULT error code.</returns>
        public IAsyncOperation<ListingInformation> LoadListingInformationAsync()
        {
            return UseSimulator ? CurrentAppSimulator.LoadListingInformationAsync() : CurrentApp.LoadListingInformationAsync();
        }
        #endregion

        #region ReloadSimulatorAsync
        /// <summary>
        /// Reloads the simulator using a <see cref="StorageFile"/> containing
        /// the WindowsStoreProxy.xml file.
        /// </summary>
        /// <param name="simulatorSettingsFile">The WindowsStoreProxy.xml file
        /// that the simulator uses. For more information, see 
        /// <see cref="CurrentAppSimulator"/>.</param>
        /// <returns>The async operation that reloads the simulator.</returns>
        /// <exception cref="System.NotSupportedException">
        /// This method is called when <see cref="UseSimulator"/> is 
        /// <see langword="false"/>.</exception>
        public IAsyncAction ReloadSimulatorAsync(StorageFile simulatorSettingsFile)
        {
            if (UseSimulator)
            {
                return CurrentAppSimulator.ReloadSimulatorAsync(simulatorSettingsFile);
            }

            throw new NotSupportedException();
        }
        #endregion

        #region RequestAppPurchaseAsync
        /// <summary>
        /// Creates the async operation that enables the user to buy a full
        /// license for the current app.
        /// </summary>
        /// <param name="includeReceipt">Determines if the method should 
        /// return the receipts for this app.</param>
        /// <returns>If <paramref name="includeReceipt"/> is set to
        /// <see langword="true"/>, this string contains XML that represents
        /// all receipts for the app and any in-app purchases; otherwise, this
        /// string is empty.
        /// </returns>
        public IAsyncOperation<string> RequestAppPurchaseAsync(bool includeReceipt)
        {
            return UseSimulator ? CurrentAppSimulator.RequestAppPurchaseAsync(includeReceipt) : CurrentApp.RequestAppPurchaseAsync(includeReceipt);
        }
        #endregion

        #region RequestProductPurchaseAsync
        /// <summary>
        /// Creates the async operation that will display the UI that is used to make
        /// an in-app purchase from the Windows Store.
        /// </summary>
        /// <param name="productId">Specifies the id of the product or feature to purchase.</param>
        /// <param name="includeReceipt">Determines if the method should return the receipts for the specified productId.</param>
        /// <returns>If <paramref name="includeReceipt"/> is set to
        /// <see langword="true"/>, this string contains XML that represents
        /// all receipts for the specified <paramref name="productId"/>; otherwise, this
        /// string is empty.
        /// </returns>
        public IAsyncOperation<string> RequestProductPurchaseAsync(string productId, bool includeReceipt)
        {
            return UseSimulator ? CurrentAppSimulator.RequestProductPurchaseAsync(productId, includeReceipt) : CurrentApp.RequestProductPurchaseAsync(productId, includeReceipt);
        }
        #endregion

        #endregion
    }
}