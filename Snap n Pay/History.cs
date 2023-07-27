using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Widget;
using Firebase.Database;
using Snap_n_Pay.Adapters;
using Snap_n_Pay.EventListeners;
using Snap_n_Pay.Helpers;
using Snap_n_Pay.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Snap_n_Pay
{
    [Activity(Label = "History")]
    public class History : Activity
    {
        string uAcc;
        string myID;
        string tot;
        OrdersAdapter order_Adapter;
        private RecyclerView Recycleorders;
        private List<OrdersModel> order_model = new List<OrdersModel>();
        private OrdersEvent order_Data = new OrdersEvent();
        string messageText;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.History);
            Recycleorders = FindViewById<RecyclerView>(Resource.Id.Recycleorders);
            tot = Intent.GetStringExtra("historydata");
            myID = tot.Substring(0, tot.IndexOf('#'));
            tot = tot.Remove(0, tot.IndexOf('#') + 1);
            uAcc = tot.Trim();

            Retrieve_orders();
        }
        public void Retrieve_orders()
        {
            if (uAcc == "admin")
            {
                order_Data.Retrieve_orders(uAcc);
                order_Data.RetriveOrders += Order_Data_RetriveOrders;
            }
            else
            {
                order_Data.Retrieve_orders(myID);
                order_Data.RetriveOrders += Order_Data_RetriveOrders;
            }

        }
        private void Order_Data_RetriveOrders(object sender, OrdersEvent.RetrivedOrdersEventHandeler e)
        {
            order_model = e.OrdersList;
            set_Recycle_orders();
        }
        private void set_Recycle_orders()
        {
            LinearLayoutManager linMan = new LinearLayoutManager(Application.Context, LinearLayoutManager.Vertical, false);
            order_Adapter = new OrdersAdapter(order_model, uAcc);
            Recycleorders.SetLayoutManager(linMan);
            Recycleorders.SetAdapter(order_Adapter);
            order_Adapter.whatsappClick += Order_Adapter_whatsappClick;
            order_Adapter.smsClick += Order_Adapter_smsClickAsync;
            order_Adapter.newodericonClick += DeleteRecord;
            order_Adapter.ItemLongClick += Order_Adapter_ItemLongClick;
        }

        private async void Order_Adapter_ItemLongClick(object sender, OrdersAdapterClickEventArgs e)
        {
            //copy key n share with other ways excluding whatsap n message(your chice)
            await Clipboard.SetTextAsync(order_model[e.Position].uniquecode.Trim());
            Toast.MakeText(this, "unique code copied", ToastLength.Long).Show();
        }

        private void DeleteRecord(object sender, OrdersAdapterClickEventArgs e)
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
            builder.SetTitle("Removal notification");
            builder.SetIcon(Resource.Mipmap.sharecards);
            builder.SetMessage("You are about to delete this record");
            builder.SetPositiveButton("ok", delegate
            {
                builder.Dispose();
                var current = Connectivity.NetworkAccess;
                if (current == NetworkAccess.Internet)
                {
                    DatabaseReference database;
                    database = firebase_Helper.GetDatabase().GetReference("SPayments");
                    database.Child(order_model[e.Position].order_Id.Trim()).RemoveValue();
                    order_model.Remove(order_model[e.Position]);
                    order_Adapter.NotifyDataSetChanged();

                    //view = (View).RootView;
                    Toast.MakeText(this, "Removed", ToastLength.Long).Show();
                }
                else
                {
                    Toast.MakeText(this, "Lost Internet connection", ToastLength.Long).Show();
                }
            });
            builder.SetNeutralButton("no, cancel", delegate
            {
                builder.Dispose();

            });

            builder.Show();
            //just hide from this user by updating the key that allows them to see this record to something random
            //just hide from this user by updating the key that allows them to see this record to something random
            
        }

        private async void Order_Adapter_smsClickAsync(object sender, OrdersAdapterClickEventArgs e)
        {
            messageText = "Hi, Kindly find payment details shared below\n" + " We wil each paying as described.\nR " +
                 order_model[e.Position].maxCost.Trim() + "\n Key Reference(code) : " + order_model[e.Position].uniquecode.Trim() + "\n" +
                 "if you suspect this text of anything or beleive"
                 + " was not me" +
                 " please ignore it.";

            if (!string.IsNullOrEmpty(messageText.Trim()))
            {
                try
                {
                    var message = new SmsMessage(messageText, "");
                    await Sms.ComposeAsync(message);
                }
                catch (FeatureNotSupportedException ex)
                {
                    // Sms is not supported on this device.
                    Toast.MakeText(Application.Context, ex.Message.ToString(), ToastLength.Long).Show();
                }
                catch (Exception ex)
                {
                    // Other error has occurred.
                    Toast.MakeText(Application.Context, ex.Message.ToString(), ToastLength.Long).Show();
                }
            }
            else
            {
                Toast.MakeText(Application.Context, "fill in the payment details first", ToastLength.Long).Show();
            }
        }

        private void Order_Adapter_whatsappClick(object sender, OrdersAdapterClickEventArgs e)
        {
            messageText = "Hi, Kindly find payment details shared below\n" + " We wil each paying as described.\nR " +
                 order_model[e.Position].maxCost.Trim() + "\n Key Reference(code) : " + order_model[e.Position].uniquecode.Trim() + "\n" +
                 "if you suspect this text of anything or beleive"
                 + " was not me" +
                 " please ignore it.";
            Intent share = new Intent(Intent.ActionSend);
            share.SetType("text/plain");
            share.PutExtra(Intent.ExtraText, "Snap and pay" + "\n " + messageText);
            share.SetPackage("com.whatsapp");
            StartActivity(share);
        }
    }
}