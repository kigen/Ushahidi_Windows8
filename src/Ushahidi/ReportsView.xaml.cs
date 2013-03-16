using Bing.Maps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ushahidi.Library.Model;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace Ushahidi
{
    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class ReportsView : Ushahidi.Common.LayoutAwarePage
    {
        private DataTransferManager dataTransferManager;

        public ReportsView()
        {
            this.InitializeComponent();
         
       
        }

        Incident incident;
        Deployment ActiveDeployment;

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // Allow saved page state to override the initial item to display

            App app = (App)Application.Current;
            incident= app.ActiveIncident;

            ActiveDeployment = app.ActiveMap;

            DataContext = incident;
            CategoryTextBlock.Text = " ";
            foreach (Category cat in incident.categories)
            {

                CategoryTextBlock.Text += cat.category.title + " | ";
            }


            Pushpin pushpin = new Pushpin();
            pushpin.Tag = incident.incident.incidenttitle;
           // pushpin.Text = incident.incident.incidentdescription;

            Location location = new Location()
            {
                Latitude = incident.incident.Latitude,
                Longitude = incident.incident.Longitude
            };

            ToolTipService.SetToolTip(pushpin, incident.incident.incidenttitle);           

            MapLayer.SetPosition(pushpin, location);
            ReportMap.Children.Add(pushpin);

            ReportMap.SetView(location, 14);


        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            
            // TODO: Derive a serializable navigation parameter and assign it to pageState["SelectedItem"]
        }
        
        private void CommentButton_Click_1(object sender, RoutedEventArgs e)
        {

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.dataTransferManager = DataTransferManager.GetForCurrentView();
            this.dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnDataRequested);


        }
     
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            this.dataTransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnDataRequested);

        }
       
    
        // When share is invoked (by the user or programatically) the event handler we registered will be called to populate the datapackage with the
        // data to be shared.
        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            // Call the scenario specific function to populate the datapackage with the data to be shared.
            if (GetShareContent(e.Request))
            {
                // Out of the datapackage properties, the title is required. If the scenario completed successfully, we need
                // to make sure the title is valid since the sample scenario gets the title from the user.
                if (String.IsNullOrEmpty(e.Request.Data.Properties.Title))
                {
                    e.Request.FailWithDisplayText("Missing Title");
                }
            }
        }

        private void ShowUIButton_Click(object sender, RoutedEventArgs e)
        {
            // If the user clicks the share button, invoke the share flow programatically.
            DataTransferManager.ShowShareUI();
        }


        private bool GetShareContent(DataRequest request)
        {
            bool succeeded = false;

            App app = (App)Application.Current;
            incident = app.ActiveIncident;

            ActiveDeployment = app.ActiveMap;

            Uri dataPackageUri = ValidateAndGetUri(ActiveDeployment.url + "reports/view/"+incident.incident.incidentid);
            if (dataPackageUri != null)
            {

                request.Data.SetUri(dataPackageUri);
                request.Data.Properties.Title = incident.incident.incidenttitle+" @ushahidi 4 #Windows8";
                request.Data.Properties.Description = incident.incident.incidentdescription;

                //request.Data.SetText(
                //            "@Ushahidi Deployment, " + ": " + incident.incident.incidentdescription.Substring(0, 50) + " "
                //   );


                succeeded = true;
            }
            else
            {
                request.FailWithDisplayText("Enter the link you would like to share and try again.");
            }
            return succeeded;
        }

        private Uri ValidateAndGetUri(string uriString)
        {
            Uri uri = null;
            try
            {
                uri = new Uri(uriString);
            }
            catch (FormatException)
            {

            }
            return uri;
        }


    }
}
