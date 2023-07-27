using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Snap_n_Pay.Helpers;
using Snap_n_Pay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snap_n_Pay.EventListeners
{
    class CodesEvent: Java.Lang.Object, IValueEventListener
    {
        string MyID = "";
        private List<UniqueCodeModel> pPos = new List<UniqueCodeModel>();
        public event EventHandler<RetrivedCodesEventHandeler> RetriveOrders;
        public class RetrivedCodesEventHandeler : EventArgs
        {
            public List<UniqueCodeModel> CodesList { get; set; }
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
                        if (this.MyID.Trim() == data.Key.Trim())
                        {
                            UniqueCodeModel uM = new UniqueCodeModel
                            {
                                codeID = data.Key,
                                people = data.Child("people").Value.ToString(),
                                paymenteach = data.Child("paymenteach").Value.ToString(),
                                storeTopay= data.Child("store").Value.ToString(),
                                storeCopyEmail= data.Child("storemaill").Value.ToString()
                            };
                            pPos.Add(uM);
                        }
                        
                    }
                }
            }
            catch (System.Exception fetch)
            {
                Toast.MakeText(Application.Context, "Error: couldn't fetch data as expected: " + fetch.Message.ToString(), ToastLength.Long).Show();
            }

            RetriveOrders.Invoke(this, new RetrivedCodesEventHandeler
            {
                CodesList = pPos
            });
        }

        public void Retrieve_Codes(string MyID)
        {
            this.MyID = MyID;
            DatabaseReference dref = firebase_Helper.GetDatabase().GetReference("SPendings");
            dref.AddValueEventListener(this);
        }
    }
}