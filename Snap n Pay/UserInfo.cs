using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Snap_n_Pay.EventListeners;
using Snap_n_Pay.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace Snap_n_Pay
{
    [Activity(Label = "UserInfo", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class UserInfo : Activity, IValueEventListener
    {
        MaterialButton updateprofile;
        TextInputEditText phone, preferdName;
        TextView UseName, AccountType, UserMail;

        string IDstring;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.uProfile);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
            DeviceDisplay.KeepScreenOn = true;
            updateprofile = FindViewById<MaterialButton>(Resource.Id.updateprofile);
            UseName = FindViewById<TextView>(Resource.Id.UseName);
            AccountType = FindViewById<TextView>(Resource.Id.AccountType);
            UserMail = FindViewById<TextView>(Resource.Id.UserMail);
            phone = FindViewById<TextInputEditText>(Resource.Id.phone);
            preferdName = FindViewById<TextInputEditText>(Resource.Id.preferdName);
            updateprofile.Click += Updateprofile_Click;

            IDstring = Intent.GetStringExtra("ID");
            DatabaseReference dref = firebase_Helper.GetDatabase().GetReference("SUsers").Child(IDstring);
            dref.AddValueEventListener(this);
        }

        public override void OnBackPressed()
        {
            Finish();
        }

        private void Updateprofile_Click(object sender, EventArgs e)
        {
            if(isNotNull()==true)
            {
                try
                {
                    
                    firebase_Helper.GetDatabase().GetReference("SUsers").Child(IDstring).Child("username").SetValue(preferdName.Text.Trim());
                    firebase_Helper.GetDatabase().GetReference("SUsers").Child(IDstring).Child("phone").SetValue(phone.Text.Trim());

                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.SetTitle("notification");
                    builder.SetMessage("successfully updated");
                    builder.SetPositiveButton("ok", delegate
                    {
                        builder.Dispose();
                        
                    });
                    builder.Show();

                }
                catch (System.Exception er)
                {
                    Toast.MakeText(this, "error: " + er.Message.ToString(), ToastLength.Long).Show();
                }
            }
        }

        private bool isNotNull()
        {
            bool result = true;
            if (preferdName.Text.Trim().IndexOf(" ") > 0 || preferdName.Text.Trim().IndexOf("  ") > 0)
            {
                result = false;
                preferdName.Error = "spaces are not allowed within your name";
            }
            if (string.IsNullOrEmpty(UseName.Text.Trim()))
            {
                UseName.Error = "required";
                result = false;
            }
            if (string.IsNullOrEmpty(preferdName.Text.Trim()))
            {
                preferdName.Error = "required";
                result = false;
            }

            if (string.IsNullOrEmpty(UserMail.Text.Trim()))
            {
                UserMail.Error = "Badly formated email";
                result = false;
            }
            
            if (string.IsNullOrEmpty(phone.Text.Trim()) || phone.Text.Trim().Length<10)
            {
                phone.Error = "Badly formated phone number";
                result = false;
            }

            return result;
        }

        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if(snapshot.Exists())
            {
                preferdName.Text = snapshot.Child("username").Value.ToString();
                UseName.Text = preferdName.Text;
                phone.Text =snapshot.Child("phone").Value.ToString();
                UserMail.Text= snapshot.Child("email").Value.ToString();
                AccountType.Text = snapshot.Child("AccountType").Value.ToString();
            }
            
        }
    }
}