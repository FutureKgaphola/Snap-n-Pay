
using Snap_n_Pay.Models;
using Android.App;
using Android.Widget;
using Firebase.Database;
using Snap_n_Pay.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Snap_n_Pay.EventListeners
{
    public class OrdersEvent : Java.Lang.Object, IValueEventListener
    {
        string MyID = "";
        private List<OrdersModel> pPos = new List<OrdersModel>();
        public event EventHandler<RetrivedOrdersEventHandeler> RetriveOrders;
        public class RetrivedOrdersEventHandeler : EventArgs
        {
            public List<OrdersModel> OrdersList { get; set; }
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
                        if(this.MyID.Trim().ToLower()=="admin".ToLower())
                        {
                            OrdersModel uM = new OrdersModel
                            {
                                order_Id = data.Key,
                                dateOrdered = data.Child("date").Value.ToString(),
                                email = data.Child("email").Value.ToString(),
                                phone = data.Child("phone").Value.ToString(),
                                status = data.Child("status").Value.ToString(),
                                maxCost = data.Child("maxcost").Value.ToString(),
                                customerId = data.Child("clientID").Value.ToString(),
                                uniquecode = data.Child("secretKey").Value.ToString()

                            };
                            pPos.Add(uM);
                        }
                        else
                        {
                            if (this.MyID.Trim() == data.Child("clientID").Value.ToString().Trim())
                            {
                                OrdersModel uM = new OrdersModel
                                {
                                    order_Id = data.Key,
                                    dateOrdered = data.Child("date").Value.ToString(),
                                    email = data.Child("email").Value.ToString(),
                                    phone = data.Child("phone").Value.ToString(),
                                    status = data.Child("status").Value.ToString(),
                                    maxCost = data.Child("maxcost").Value.ToString(),
                                    customerId = data.Child("clientID").Value.ToString(),
                                    uniquecode = data.Child("secretKey").Value.ToString()

                                };
                                pPos.Add(uM);
                            }
                        }

                    }
                }
            }
            catch (System.Exception fetch)
            {
                Toast.MakeText(Application.Context, "Error: couldn't fetch data as expected: " + fetch.Message.ToString(), ToastLength.Long).Show();
            }

            RetriveOrders.Invoke(this, new RetrivedOrdersEventHandeler
            {
                OrdersList = pPos
            });
        }

        public void Retrieve_orders(string MyID)
        {
            this.MyID = MyID;
            DatabaseReference dref = firebase_Helper.GetDatabase().GetReference("SPayments");
            dref.AddValueEventListener(this);
        }
    }
}