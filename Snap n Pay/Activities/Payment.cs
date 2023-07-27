using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Firebase.Database;
using Snap_n_Pay.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace Snap_n_Pay.Activities
{
    [Activity(Label = "Payment")]
    public class Payment : Activity, IValueEventListener
    {
        private int month = System.DateTime.Now.Month;
        private int day = System.DateTime.Now.Day;
        private int year = System.DateTime.Now.Year;
        string tot,feepay= "0.00", maxpeople,
            clientID="",
            purchaseDate="", selectedStore,
            clientEmail="",
            phone="",
            isfirstperson,
            uniquecode,
            selectedStoreEmail="";
        private WebView webViewpayment;
        string url = "";
        private Button btnPayoutside;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Payment);
            btnPayoutside = FindViewById<Button>(Resource.Id.btnPayoutside);
            btnPayoutside.Click += BtnPayoutside_Click;
            btnPayoutside.Visibility = ViewStates.Gone;
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
            DatabaseReference dref = firebase_Helper.GetDatabase().GetReference("externalweb");
            dref.AddValueEventListener(this);
            DeviceDisplay.KeepScreenOn = true;
            purchaseDate = day.ToString() + '/' + month.ToString() + '/' + year.ToString();
            tot = Intent.GetStringExtra("pdata");
            feepay = tot.Substring(0, tot.IndexOf('#'));
            tot = tot.Remove(0, tot.IndexOf('#') + 1);
            clientEmail = tot.Substring(0, tot.IndexOf('#'));
            tot = tot.Remove(0, tot.IndexOf('#') + 1);
            clientID = tot.Substring(0, tot.IndexOf('#'));
            tot = tot.Remove(0, tot.IndexOf('#') + 1);

            phone = tot.Substring(0, tot.IndexOf('#'));
            tot = tot.Remove(0, tot.IndexOf('#') + 1);
            selectedStore = tot.Substring(0, tot.IndexOf('#'));
            tot = tot.Remove(0, tot.IndexOf('#') + 1);
            maxpeople = tot.Substring(0, tot.IndexOf('#'));
            tot = tot.Remove(0, tot.IndexOf('#') + 1);
            isfirstperson = tot.Substring(0, tot.IndexOf('#'));
            tot = tot.Remove(0, tot.IndexOf('#') + 1);
            uniquecode = tot.Substring(0, tot.IndexOf('#'));
            tot = tot.Remove(0, tot.IndexOf('#') + 1);
            selectedStoreEmail = tot.Trim();

            url = "https://boardroomlcx.000webhostapp.com/yocoSnapPay/index.php?feepay=" +
                feepay.Replace(",", ".") +"&clientID="+ clientID+
                "&purchaseDate="+ purchaseDate+ "&clientEmail="+ clientEmail
                + "&phone="+ phone+ "&store=" + selectedStore + "&maxpeople=" + 
                maxpeople+ "&uniqcode=" + uniquecode + "&isfirsperson=" + isfirstperson+"&emstore="+ selectedStoreEmail;
            // Create your application here
            webViewpayment = FindViewById<WebView>(Resource.Id.webViewpayment);
            webViewpayment.Settings.JavaScriptEnabled = true;
            webViewpayment.SetWebChromeClient(new WebChromeClient());
            webViewpayment.LoadUrl(url);

        }

        private async void BtnPayoutside_Click(object sender, EventArgs e)
        {
            //open web browser
            url = "https://boardroomlcx.000webhostapp.com/yocoSnapPay/index.php?feepay=" +
                 feepay.Replace(",", ".") + "&clientID=" + clientID +
                 "&purchaseDate=" + purchaseDate + "&clientEmail=" + clientEmail
                 + "&phone=" + phone + "&store=" + selectedStore + "&maxpeople=" +
                 maxpeople + "&uniqcode=" + uniquecode + "&isfirsperson=" + isfirstperson + "&emstore=" + selectedStoreEmail;
            try
            {
                await Browser.OpenAsync(url, new BrowserLaunchOptions
                {
                    LaunchMode = BrowserLaunchMode.SystemPreferred,
                    TitleMode = BrowserTitleMode.Show,
                    PreferredToolbarColor = Color.Orange,
                    PreferredControlColor = Color.White
                });
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.ToString(), ToastLength.Long).Show();
                // An unexpected error occured. No browser may be installed on the device.
            }
        }

        public override void OnBackPressed() { Finish(); }

        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if(snapshot.Exists())
            {
                if(snapshot.Child("isallowed").Value.ToString()=="yes")
                {
                    btnPayoutside.Visibility = ViewStates.Visible;
                }
                else
                {
                    btnPayoutside.Visibility = ViewStates.Gone;
                }
            }
        }
    }

}