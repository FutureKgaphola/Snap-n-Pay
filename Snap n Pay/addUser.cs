using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Java.Util;
using Snap_n_Pay.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace Snap_n_Pay
{
    [Activity(Label = "addUser", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class addUser : Activity, IOnSuccessListener, IOnFailureListener
    {
        private MaterialButton Addprofile;
        private TextInputEditText passTxt;
        private TextInputEditText preferdName, phone, emailTxt;
        FirebaseAuth auth;
        private ProgressBar progBar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddUser);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
            DeviceDisplay.KeepScreenOn = true;
            progBar = FindViewById<ProgressBar>(Resource.Id.progBar);
            Addprofile = FindViewById<MaterialButton>(Resource.Id.Addprofile);
            Addprofile.Click += Addprofile_Click;
            
            passTxt = FindViewById<TextInputEditText>(Resource.Id.passTxt);
            preferdName = FindViewById<TextInputEditText>(Resource.Id.preferdName);
            phone = FindViewById<TextInputEditText>(Resource.Id.phone);
            emailTxt= FindViewById<TextInputEditText>(Resource.Id.emailTxt);
            // Create your application here
        }

        private void Addprofile_Click(object sender, EventArgs e)
        {
            checkAndAdd();
        }
        
        private void register_User()
        {
            auth = new firebase_Helper().GetFirebaseAuth();
            auth.CreateUserWithEmailAndPassword(emailTxt.Text.Trim(), passTxt.Text.Trim())
                .AddOnFailureListener(this)
               .AddOnSuccessListener(this);
        }
        private void checkAndAdd()
        {
            try
            {
                //if auth succesfull then add
                if (validfound() == true)
                {
                    progBar.Visibility = ViewStates.Visible;
                    register_User();
                }
            }
            catch (Exception y)
            {

                Toast.MakeText(this, y.Message, ToastLength.Long).Show();
            }
        }

        private void Clean()
        {
            progBar.Visibility = ViewStates.Gone;
            auth = null;
            preferdName.Text = "";
            phone.Text = "";
            passTxt.Text = "";
            emailTxt.Text = "";
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
            builder.SetTitle("completed");
            builder.SetIcon(Resource.Mipmap.snapCard);
            builder.SetMessage("succsessfully registered");
            builder.SetPositiveButton("ok", delegate
            {
                builder.Dispose();
                Finish();

            });

            builder.Show();
        }

        private bool validfound()
        {
            bool Result = true;
            try
            {
                if (preferdName.Text.Trim().IndexOf(" ") > 0 || preferdName.Text.Trim().IndexOf("  ") > 0)
                {
                    Result = false;
                    preferdName.Error = "spaces are not allowed within your text";
                }

                if (phone.Text.Trim().IndexOf(" ") > 0 || phone.Text.Trim().IndexOf("  ") > 0)
                {
                    Result = false;
                    phone.Error = "spaces are not allowed within your text";
                }
                if (emailTxt.Text.Trim().IndexOf(" ") > 0 || emailTxt.Text.Trim().IndexOf("  ") > 0)
                {
                    Result = false;
                    emailTxt.Error = "spaces are not allowed within your text";
                }
                if (string.IsNullOrEmpty(passTxt.Text.Trim()) || passTxt.Text.Trim().Length < 6 
                    || passTxt.Text.Contains(" ") || passTxt.Text.Contains("    "))
                {
                    Result = false;
                    Toast.MakeText(this, "fields must not contain spaces", ToastLength.Long).Show();
                    passTxt.Error = "atlest 6 characters is required";
                }

                if (preferdName.Text.Trim().IndexOf(" ") == -1 && preferdName.Text.Trim().IndexOf("  ") == -1  &&
                    phone.Text.Trim().IndexOf(" ") == -1 && phone.Text.Trim().IndexOf("  ") == -1 &&
                    emailTxt.Text.Trim().IndexOf(" ") == -1 && emailTxt.Text.Trim().IndexOf("  ") == -1 )
                {

                    if (string.IsNullOrEmpty(preferdName.Text.Trim()))
                    {
                        Result = false;
                        preferdName.Error = "invalid input";
                    }
                    if (string.IsNullOrEmpty(phone.Text.Trim()) || phone.Text.Trim().Length<10)
                    {
                        Result = false;
                        phone.Error = "invalid input";
                    }
                    if (string.IsNullOrEmpty(emailTxt.Text.Trim()))
                    {
                        Result = false;
                        emailTxt.Error = "invalid input";
                    }
                    
                }

            }
            catch (Exception inv)
            {
                Toast.MakeText(this, inv.Message, ToastLength.Long).Show();
            }

            return Result;
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            progBar.Visibility = ViewStates.Gone;
            
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
            builder.SetTitle("Error");
            builder.SetIcon(Resource.Mipmap.snapCard);
            builder.SetMessage("Something went wrong: " + e.Message);
            builder.SetPositiveButton("ok", delegate
            {
                builder.Dispose();
                Finish();

            });

            builder.Show();
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            HashMap data = new HashMap();
            data.Put("username", preferdName.Text.Trim().ToLower());
            data.Put("phone", phone.Text.Trim());
            data.Put("AccountType", "customer");
            data.Put("email", emailTxt.Text);
            DatabaseReference databaseRef = firebase_Helper.GetDatabase().GetReference("SUsers").Child(auth.CurrentUser.Uid);
            databaseRef.SetValue(data);
            Clean();
        }
    }
}