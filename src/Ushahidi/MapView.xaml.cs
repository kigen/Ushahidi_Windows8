using Bing.Maps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ushahidi.Library.Model;
using Ushahidi.Library.Network;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Ushahidi.Common;


// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Ushahidi
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MapView : Ushahidi.Common.LayoutAwarePage
    {

        private DataTransferManager dataTransferManager;
        public MapView()
        {
            this.InitializeComponent();

        }

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
            if (pageState == null)
            {
                load();
            }
            else
            {
                reload();
            }

        }

        Deployment activeDeplyment;
        async void load()
        {

            WebProxy proxy = new WebProxy();
            App app = (App)Application.Current;

            activeDeplyment = app.ActiveMap;
            pageTitle.Text = activeDeplyment.name;

            await proxy.GetIncidents(activeDeplyment);
            ProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            var incidents = proxy.Incidents;

            app.Incidents = incidents;

            IncidentListview.ItemsSource = incidents;

            Location loc = new Location()
            {
                Latitude = activeDeplyment.Latitude,
                Longitude = activeDeplyment.Longitude
            };

            IncidentMap.SetView(loc, 8);

            foreach (Incident i in incidents)
            {
                Pushpin pushpin = new Pushpin();
                pushpin.Tag = i;

                Location location = new Location()
                {
                    Latitude = i.incident.Latitude,
                    Longitude = i.incident.Longitude
                };
                pushpin.Tapped += pushpin_Tapped;

                ToolTipService.SetToolTip(pushpin, i.incident.incidenttitle);

                MapLayer.SetPosition(pushpin, location);
                IncidentMap.Children.Add(pushpin);
            }

        }

        void pushpin_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Pushpin pushpin = sender as Pushpin;
            Incident incident = pushpin.Tag as Incident;
            App myapp = (App)Application.Current;
            myapp.ActiveIncident = incident;
            Frame.Navigate(typeof(ReportsView));
        }


        void reload()
        {
            ProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            App app = (App)Application.Current;
            Deployment activeDeplyment = app.ActiveMap;
            pageTitle.Text = activeDeplyment.name;
            IncidentListview.ItemsSource = app.Incidents;

            Location loc = new Location()
            {
                Latitude = activeDeplyment.Latitude,
                Longitude = activeDeplyment.Longitude
            };

            IncidentMap.SetView(loc, 8);

            foreach (Incident i in app.Incidents)
            {
                Pushpin pushpin = new Pushpin();

                Location location = new Location()
                {
                    Latitude = i.incident.Latitude,
                    Longitude = i.incident.Longitude
                };

                MapLayer.SetPosition(pushpin, location);
                IncidentMap.Children.Add(pushpin);
            }

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

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {

        }
        private void IncidentListview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App app = (App)Application.Current;
            app.ActiveIncident = app.Incidents[IncidentListview.SelectedIndex];
            Frame.Navigate(typeof(ReportsView));
        }


        private void RefreshButton_Click_1(object sender, RoutedEventArgs e)
        {
            load();
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

        protected void ShowUIButton_Click(object sender, RoutedEventArgs e)
        {
            // If the user clicks the share button, invoke the share flow programatically.
            DataTransferManager.ShowShareUI();
        }


        protected bool GetShareContent(DataRequest request)
        {
            bool succeeded = false;

            App app = (App)Application.Current;


            activeDeplyment = app.ActiveMap;

            Uri dataPackageUri = ValidateAndGetUri(activeDeplyment.url);
            if (dataPackageUri != null)
            {

                request.Data.SetUri(dataPackageUri);
                request.Data.Properties.Title = activeDeplyment.name;
                request.Data.Properties.Description = activeDeplyment.description;

                //request.Data.SetText(
                //           "@Ushahidi Deployment, "  + ": " + activeDeplyment.description.Substring(0, 50) + " "
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
