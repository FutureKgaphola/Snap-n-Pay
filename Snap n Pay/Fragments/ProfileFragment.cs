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
using Snap_n_Pay.Helpers;
using System;



namespace Snap_n_Pay.Fragments
{
    public class ProfileFragment : AndroidX.Fragment.App.Fragment, IValueEventListener
    {
        View view;
        MaterialButton updateprofile;
        TextInputEditText phone, preferdName;
        TextView UseName, AccountType, UserMail;
        string phonestr = "";
        string uAcc, myID, tot, emailfield;

        string IDstring;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
              
            view= inflater.Inflate(Resource.Layout.uProfile, container, false);
            updateprofile = view.FindViewById<MaterialButton>(Resource.Id.updateprofile);
            UseName = view.FindViewById<TextView>(Resource.Id.UseName);
            AccountType = view.FindViewById<TextView>(Resource.Id.AccountType);
            UserMail = view.FindViewById<TextView>(Resource.Id.UserMail);
            phone = view.FindViewById<TextInputEditText>(Resource.Id.phone);
            preferdName = view.FindViewById<TextInputEditText>(Resource.Id.preferdName);
            updateprofile.Click += Updateprofile_Click;

            tot = Arguments.GetString("MyDataTag");
            myID = tot.Substring(0, tot.IndexOf('#'));
            tot = tot.Remove(0, tot.IndexOf('#') + 1);
            uAcc = tot.Substring(0, tot.IndexOf('#'));
            tot = tot.Remove(0, tot.IndexOf('#') + 1);
            emailfield = tot.Substring(0, tot.IndexOf('#'));
            tot = tot.Remove(0, tot.IndexOf('#') + 1);
            phonestr = tot.Trim();
            DatabaseReference dref = firebase_Helper.GetDatabase().GetReference("SUsers").Child(myID);
            dref.AddValueEventListener(this);
            return view;

            
        }

        private void Updateprofile_Click(object sender, EventArgs e)
        {
            if (isNotNull() == true)
            {
                try
                {

                    firebase_Helper.GetDatabase().GetReference("SUsers").Child(IDstring).Child("username").SetValue(preferdName.Text.Trim());
                    firebase_Helper.GetDatabase().GetReference("SUsers").Child(IDstring).Child("phone").SetValue(phone.Text.Trim());

                    AlertDialog.Builder builder = new AlertDialog.Builder(RequireActivity());
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
                    Toast.MakeText(Application.Context, "error: " + er.Message.ToString(), ToastLength.Long).Show();
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

            if (string.IsNullOrEmpty(phone.Text.Trim()) || phone.Text.Trim().Length < 10)
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
            if (snapshot.Exists())
            {
                preferdName.Text = snapshot.Child("username").Value.ToString();
                UseName.Text = preferdName.Text;
                phone.Text = snapshot.Child("phone").Value.ToString();
                UserMail.Text = snapshot.Child("email").Value.ToString();
                AccountType.Text = snapshot.Child("AccountType").Value.ToString();
            }

        }
    }
}