using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.BottomNavigation;
using Snap_n_Pay.Fragments;
using System.Collections.Generic;
using Xamarin.Essentials;

namespace Snap_n_Pay
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        private AndroidX.Fragment.App.Fragment Currentfragment;
        private HomeFragment homeFragment;
        private ProfileFragment profile_Fragment;
        private Stack<AndroidX.Fragment.App.Fragment> mstackFragment;
        string tot, uid, account, phone, clientEmail;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
            DeviceDisplay.KeepScreenOn = true;
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);

            tot = Intent.GetStringExtra("user");
            //Toast.MakeText(this, tot, ToastLength.Short).Show();
            uid = tot.Substring(0, tot.IndexOf('#'));
            tot = tot.Remove(0, tot.IndexOf('#') + 1);
            account = tot.Substring(0, tot.IndexOf('#'));
            tot = tot.Remove(0, tot.IndexOf('#') + 1);
            phone = tot.Substring(0, tot.IndexOf('#'));
            tot = tot.Remove(0, tot.IndexOf('#') + 1);
            clientEmail = tot.Trim();

            homeFragment = new HomeFragment();
            profile_Fragment = new ProfileFragment();
 
            mstackFragment = new Stack<AndroidX.Fragment.App.Fragment>();

            var trans = SupportFragmentManager.BeginTransaction();
            
            trans.Add(Resource.Id.fragmentContainer, profile_Fragment, "Fragment2");
            trans.Hide(profile_Fragment);
            trans.Add(Resource.Id.fragmentContainer, homeFragment, "Fragment1");
            trans.Commit();
            Currentfragment = homeFragment;

            Bundle mybundle = new Bundle();
            mybundle.PutString("MyDataTag", uid + "#" + account + "#" + clientEmail+"#"+ phone);
            homeFragment.Arguments = mybundle;
            profile_Fragment.Arguments = mybundle;

        }
        private void ShowFragment(AndroidX.Fragment.App.Fragment fragment)
        {
            var trans = SupportFragmentManager.BeginTransaction();
            trans.Hide(Currentfragment);
            trans.Show(fragment);
            trans.AddToBackStack(null);
            trans.Commit();
            mstackFragment.Push(Currentfragment);
            Currentfragment = fragment;
        }
        public override void OnBackPressed() { }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    ShowFragment(homeFragment);
                    return true;
                case Resource.Id.navigation_dashboard:
                    ShowFragment(profile_Fragment);
                    return true;
                case Resource.Id.navigation_notifications:
                    Toast.MakeText(this,"Bye, hope we snap soon.",ToastLength.Long).Show();
                    Finish();
                    return true;
            }
            return false;
        }
    }
}

