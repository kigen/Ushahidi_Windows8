using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Ushahidi.Library.Model;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Ushahidi.Library.Util
{
   public class LocalStorage
    {

        List<Deployment> Deployment;

        public LocalStorage()
        {
            Deployment = new List<Deployment>();
            read();
        }

        public async void read()
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync("Deployment");


                
                if (file == null) return;
                IRandomAccessStream inStream = await file.OpenReadAsync();

                // Deserialize the Session State.
                DataContractSerializer serializer = new DataContractSerializer(typeof(List<Deployment>));
                var StatsDetails = (List<Deployment>)serializer.ReadObject(inStream.AsStreamForRead());
                Deployment = StatsDetails;

                inStream.Dispose();
            }
            catch (Exception)
            {
                this.Deployment = new List<Deployment>();
            }
            finally
            {
                
            }


        }

        public List<Deployment> Deployments
        {
            get
            {
                if (this.Deployment == null)
                {
                    read();
                }
                return this.Deployment;
            }
            set
            {
                this.Deployment = value;
                write();

            }


        }

        public List<Deployment> Add(Deployment dep)
        {
            
            this.Deployment.Add(dep);
            write();

            return this.Deployment;
        }

        public async void write()
        {
            try
            {
                StorageFile userdetailsfile = await ApplicationData.Current.LocalFolder.CreateFileAsync("Deployment", CreationCollisionOption.ReplaceExisting);

                IRandomAccessStream raStream = await userdetailsfile.OpenAsync(FileAccessMode.ReadWrite);
                using (IOutputStream outStream = raStream.GetOutputStreamAt(0))
                {
                    // Serialize the Session State.
                    DataContractSerializer serializer = new DataContractSerializer(typeof(List<Deployment>));
                    serializer.WriteObject(outStream.AsStreamForWrite(), this.Deployment);
                    await outStream.FlushAsync();
                }
            }
            catch (Exception ex)
            {

            }
        }

    }
}
