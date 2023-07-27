using Snap_n_Pay.Helpers;
using Snap_n_Pay.Models;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snap_n_Pay.EventListeners
{
    public class samsungEvent : Java.Lang.Object, IValueEventListener
    {
        private List<SamsungModel> pPos = new List<SamsungModel>();
        public event EventHandler<RetrivedPhonesEventHandeler> RetrivePhones;
        public class RetrivedPhonesEventHandeler : EventArgs
        {
            public List<SamsungModel> phoneList { get; set; }
        }
        public void OnCancelled(DatabaseError error)
        {

        }
        public void OnDataChange(DataSnapshot snapshot)
        {
            try
            {
                if (snapshot.Value != null)
                {
                    pPos.Clear();
                    var child = snapshot.Children.ToEnumerable<DataSnapshot>();

                    foreach (DataSnapshot data in child)
                    {
                        
                        SamsungModel uM = new SamsungModel
                        {
                            Idstore = data.Key,
                            StoreName = data.Child("Name").Value.ToString(),
                            img = data.Child("storeImage").Value.ToString(),
                            companyReg = data.Child("companyReg").Value.ToString(),
                            selectedStoreEmail= data.Child("storemaill").Value.ToString()
                        };
                        pPos.Add(uM);
                        
                    }
                }
            }
            catch (System.Exception fetch)
            {
                Toast.MakeText(Application.Context, "Error: couldn't fetch data as expected: " + fetch.Message.ToString(), ToastLength.Long).Show();
            }

            RetrivePhones.Invoke(this, new RetrivedPhonesEventHandeler
            {
                phoneList = pPos
            });
        }

        public void Retrive_phones()
        {
            DatabaseReference dref = firebase_Helper.GetDatabase().GetReference("Stores");
            dref.AddValueEventListener(this);
        }
    }
    
}