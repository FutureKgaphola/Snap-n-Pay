using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;
using Google.Android.Material.Button;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using Snap_n_Pay.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snap_n_Pay
{
    [Activity(Label = "login", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class login : Activity, IValueEventListener, IOnFailureListener, IOnSuccessListener
    {
        ProgressBar progBar;
        MaterialButton procced;
        TextInputEditText username,password;
        private FirebaseAuth auth;
        TextView resetpass,register;
        public void OnCancelled(DatabaseError error)
        {
            
        }
        string user, id, phone, account;
        public void OnDataChange(DataSnapshot snapshot)
        {
            if(snapshot.Exists())
            {
                user = snapshot.Child("email").Value.ToString();
                phone = snapshot.Child("phone").Value.ToString();
                account= snapshot.Child("AccountType").Value.ToString();

                if (user.ToLower().Trim() == username.Text.ToLower().Trim() && account.ToLower().Trim() == "customer".ToLower())
                {
                    progBar.Visibility = ViewStates.Gone;
                    password.Text = "";
                    db_listenRef.RemoveEventListener(this);
                    Intent intent = new Intent(this, typeof(MainActivity));
                    intent.PutExtra("user", id + "#" + account + "#" + phone + "#"+username.Text.ToLower().Trim());
                    StartActivity(intent);


                }
                
                else
                {
                    //Toast.MakeText(this, "Invalid login details..", ToastLength.Short).Show();
                    progBar.Visibility = ViewStates.Gone;
                }
                
            }
        }
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.login);
            resetpass= FindViewById<TextView>(Resource.Id.resetpass);
            register = FindViewById<TextView>(Resource.Id.register);
            register.Click += Register_Click;
            resetpass.Click += Resetpass_Click;
            progBar = FindViewById<ProgressBar>(Resource.Id.progBar);
            username = FindViewById<TextInputEditText>(Resource.Id.username);
            password = FindViewById<TextInputEditText>(Resource.Id.password);
            procced = FindViewById<MaterialButton>(Resource.Id.procced);
            procced.Click += Procced_Click;
            // Create your application here
        }

        private void Register_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(addUser));
            StartActivity(intent);
        }

        private void Resetpass_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(forgotpassword));
            StartActivity(intent);
        }

        private void Procced_Click(object sender, EventArgs e)
        {

            if (validfound()==true)
            {
                progBar.Visibility = ViewStates.Visible;
                auth = new firebase_Helper().GetFirebaseAuth();
                auth.SignInWithEmailAndPassword(username.Text, password.Text)
                    .AddOnSuccessListener(this)
                    .AddOnFailureListener(this);
            }
        }
        private bool validfound()
        {
            bool Result = true;

            if (string.IsNullOrEmpty(username.Text.Trim()))
            {
                Result = false;
                username.Error = "invalid input";
            }
            if (string.IsNullOrEmpty(password.Text.Trim()))
            {
                Result = false;
                password.Error = "invalid input";
            }

            return Result;
        }
        DatabaseReference db_listenRef;
        public void OnSuccess(Java.Lang.Object result)
        {
            if (username.Text.ToLower().Trim() == "admin@snp.co.za" && password.Text.Trim() == "admin@123")
            {

                progBar.Visibility = ViewStates.Gone;
                password.Text = "";

                Intent intent = new Intent(this, typeof(Dashboard));
                intent.PutExtra("userAdmin", "admin");
                StartActivity(intent);
            }else
            {
                id = auth.CurrentUser.Uid;
                db_listenRef = firebase_Helper.GetDatabase().GetReference("SUsers").Child(id);
                db_listenRef.AddValueEventListener(this);
            }
            
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            progBar.Visibility = ViewStates.Gone;
            Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this);
            builder.SetTitle("Error");
            builder.SetIcon(Resource.Mipmap.snapCard);
            builder.SetMessage("Something went wrong: " + e.Message.ToString());
            builder.SetNeutralButton("OK", delegate
            {
                builder.Dispose();
            });
            builder.Show();
        }
    }
}