using Bing.Maps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ushahidi.Library.Model;
using Ushahidi.Library.Network;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Ushahidi.Library.Util;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace Ushahidi
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class MainPage : Ushahidi.Common.LayoutAwarePage
    {
        public MainPage()
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
            
                load();
            
        }
        
        async void load()
        {
            WebProxy proxy = new WebProxy();
          
            try
            {

                App myapp = (App)Application.Current;
                
                if (myapp.Deployments == null)
                {
                    ProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    LocalStorage local = new LocalStorage();
                    var maps = await proxy.GetDeployments();                  
                    myapp.Deployments = maps;

                    if (local.Deployments.Count > 0)
                    {
                        myapp.Deployments.AddRange(local.Deployments);
                    }
                   
                }
                ProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                DeploymentListView.ItemsSource = myapp.Deployments;
              
             
                foreach (Deployment deploy in myapp.Deployments)
                {
                      

                    Pushpin pushpin = new Pushpin();
                    //pushpin.Text = deploy.name;

                    Location location = new Location();
                    double lat ,lon;
                    double.TryParse(deploy.latitude, out lat);
                    double.TryParse(deploy.longitude,out lon);
                    location.Longitude = lon;
                    location.Latitude = lat;
                    pushpin.Tapped+=pushpin_Tapped;
                    pushpin.Tag = deploy;

                    MapLayer.SetPosition(pushpin, location);

                    ToolTipService.SetToolTip(pushpin, deploy.name);

                    DeploymentMap.Children.Add(pushpin);
                }


            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message+ ex.StackTrace);
                msg.ShowAsync();
            }

        }

        void pushpin_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Pushpin pushpin = sender as Pushpin;
            Deployment deploy = pushpin.Tag as Deployment;
            App myapp = (App)Application.Current;
            myapp.ActiveMap = deploy;
            Frame.Navigate(typeof(MapView));
            
        }

        private void DeploymentListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App myapp = (App)Application.Current;
            myapp.ActiveMap = myapp.Deployments[DeploymentListView.SelectedIndex];
            Frame.Navigate(typeof(MapView));
        }

        private void AddButton_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddDeployment));
        }

        private void RefreshButton_Click_1(object sender, RoutedEventArgs e)
        {
            App myapp = (App)Application.Current;
            myapp.Deployments = null;
            load();
            
        }
    }
}
