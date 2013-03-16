using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Ushahidi.Library.Model
{

    public class IncidentObj
    {
        public string incidentid { get; set; }
        public string incidenttitle { get; set; }
        public string incidentdescription { get; set; }
        public string incidentdate { get; set; }
        public string incidentmode { get; set; }
        public string incidentactive { get; set; }
        public string incidentverified { get; set; }
        public string locationid { get; set; }
        public string locationname { get; set; }
        public string locationlatitude { get; set; }
        public string locationlongitude { get; set; }
        public double Latitude
        {
            get
            {
                double d;
                double.TryParse(this.locationlatitude, out d);
                return d;
            }
        }
        public double Longitude
        {
            get
            {
                double d;
                double.TryParse(this.locationlongitude, out d);
                return d;
            }
        }
    }

    public class Medium
    {
        public string id { get; set; }
        public string type { get; set; }
        public string link { get; set; }
        public string thumb { get; set; }
    }

    public class Incident
    {
        public IncidentObj incident { get; set; }
        public List<Category> categories { get; set; }
        public List<Medium> media { get; set; }
        public List<object> comments { get; set; }
    }

    public class Payload
    {
        public string domain { get; set; }
        public List<Incident> incidents { get; set; }
    }

    public class Error
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class RootObject
    {
        public Payload payload { get; set; }
        public Error error { get; set; }
    }

    public class Deployment
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public string category_id { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string discovery_date { get; set; }
        public string ss1 { get; set; }
        public double Latitude
        {
            get
            {
                double d;
                double.TryParse(this.latitude, out d);
                return d;
            }
        }
        public double Longitude
        {
            get
            {
                double d;
                double.TryParse(this.longitude, out d);
                return d;
            }
        }
    }

    #region "Category"

    public class CategoryObj
    {
        public string id { get; set; }
        public string parent_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string color { get; set; }
        public string position { get; set; }
        public string icon { get; set; }
    }

    public class Category
    {
        public CategoryObj category { get; set; }
    }

    public class PayloadCategory
    {
        public string domain { get; set; }
        public List<Category> categories { get; set; }
    }

    public class RootObjectCategory
    {
        public PayloadCategory payload { get; set; }
        public Error error { get; set; }
    }

    #endregion
}
