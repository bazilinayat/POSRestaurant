using CommunityToolkit.Mvvm.Messaging;
using POSRestaurant.ChangedMessages;
using POSRestaurant.DBO;

namespace POSRestaurant.Service
{
    /// <summary>
    /// To calculate teh tax of any order given the sub total
    /// This service should be working independently
    /// Tax laws could change or different countries can use the software
    /// </summary>
    public class TaxService : IRecipient<TaxChangedMessage>
    {
        /// <summary>
        /// To handle tax calculation as per Indian laws
        /// </summary>
        public TaxServiceIndia IndianTaxService;

        /// <summary>
        /// Class constructor, to generate initialize the different tax services
        /// </summary>
        /// <param name="databaseService">DIed Database Service</param>
        public TaxService(DatabaseService databaseService)
        {
            IndianTaxService = new TaxServiceIndia(databaseService);

            // Registering for listetning to the WeakReferenceMessenger for item change
            WeakReferenceMessenger.Default.Register<TaxChangedMessage>(this);
        }

        /// <summary>
        /// To know that there are some changes in tax details
        /// </summary>
        /// <param name="taxChangedMessage">Tax Details are changed</param>
        public void Receive(TaxChangedMessage taxChangedMessage)
        {
            IndianTaxService.InitializeData();
        }
    }
}
