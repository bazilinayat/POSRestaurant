using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace POSRestaurant.Service
{
    public class WindowsNotificationService
    {
        public void ShowToastNotification(string title, string message) 
        {
            // Define the toast notification XML template
            var toastXmlString = $@"
                <toast>
                    <visual>
                        <binding template='ToastGeneric'>
                            <text>{title}</text>Ē
                            <text>{message}</text>
                        </binding>
                    </visual>
                </toast>";

            // Load the XML string
            var toastXml = new XmlDocument();
            toastXml.LoadXml(toastXmlString);

            // Create the toast notification from the XML
            var toast = new ToastNotification(toastXml);

            // Show the notification
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
