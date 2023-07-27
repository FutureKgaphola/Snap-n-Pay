using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Firebase.Storage;
using Java.Util;
using Snap_n_Pay.Adapters;
using Snap_n_Pay.EventListeners;
using Snap_n_Pay.Helpers;
using Snap_n_Pay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace Snap_n_Pay
{
    [Activity(Label = "Dashboard")]
    public class Dashboard : Activity, IOnSuccessListener, IOnFailureListener, IValueEventListener
    {
        private LinearLayout linearStoredetails;
        private VideoAdapter vidAdapter;
        private RecyclerView recycleradds;
        private List<VideoModel> vids_model = new List<VideoModel>();
        private VideoEvent vidsData = new VideoEvent();

        string uAcc, IDstring;
        private ImageView add_video_btn;
        private ProgressBar progBar_vid;
        string messageText;
        private byte[] fileArray;
        private Button uploadbtn;
        StorageReference storageReference;
        private FrameLayout frameAddVids;
        private EditText u_storeEmail, u_storename, u_storreg;
        private Button procced_updateStore;

        OrdersAdapter order_Adapter;
        private RecyclerView Recycleorders;
        private List<OrdersModel> order_model = new List<OrdersModel>();
        private OrdersEvent order_Data = new OrdersEvent();
        private EditText storeEmail, storename, storreg;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AdminDashboard);
            linearStoredetails= FindViewById<LinearLayout>(Resource.Id.linearStoredetails);
            linearStoredetails.Visibility = ViewStates.Gone;
            uploadbtn =FindViewById<Button>(Resource.Id.upload_btn);
            uploadbtn.Click += Uploadbtn_Click;
            add_video_btn =FindViewById<ImageView>(Resource.Id.add_video_btn);
            progBar_vid = FindViewById<ProgressBar>(Resource.Id.progBar_vid);//show when trying to prepare video for upload and while uploading
            add_video_btn.Click += Add_video_btn_Click;

            storeEmail = FindViewById<EditText>(Resource.Id.storeEmail);
            storename = FindViewById<EditText>(Resource.Id.storename);
            storreg = FindViewById<EditText>(Resource.Id.storreg);

            add_video_btn.Visibility = ViewStates.Visible;
            uploadbtn.Visibility = ViewStates.Gone;
            frameAddVids = FindViewById<FrameLayout>(Resource.Id.frameAddVids);//samsung
            recycleradds =FindViewById<RecyclerView>(Resource.Id.recycleradds);//recycler adds
            Recycleorders = FindViewById<RecyclerView>(Resource.Id.Recycleorders);
            uAcc=Intent.GetStringExtra("userAdmin");
            Retrieve_orders();
            Retrieve_videos();
            
        }

        private Android.App.AlertDialog dialogs;
        private Android.App.AlertDialog.Builder dialogBuilders;
        private void showDescproduct()
        {

            dialogBuilders = new Android.App.AlertDialog.Builder(this);
            LayoutInflater inflater = LayoutInflater.From(this);
            View dialogView = inflater.Inflate(Resource.Layout.Profile, null);
            procced_updateStore = dialogView.FindViewById<Button>(Resource.Id.procced_updateStore);
            procced_updateStore.Click += Procced_updateStore_Click;
            u_storeEmail = dialogView.FindViewById<EditText>(Resource.Id.u_storeEmail);
            u_storename = dialogView.FindViewById<EditText>(Resource.Id.u_storename);
            u_storreg = dialogView.FindViewById<EditText>(Resource.Id.u_storreg);
            dialogBuilders.SetView(dialogView);
            dialogBuilders.SetCancelable(true);
            dialogs = dialogBuilders.Create();
            dialogs.Show();

        }
        private bool isUpdtable()
        {
            bool result = true;
            if (string.IsNullOrEmpty(u_storeEmail.Text.Trim()))
            {
                result = false;
                u_storeEmail.Error = "invalid input";
            }
            if (string.IsNullOrEmpty(u_storename.Text.Trim()))
            {
                result = false;
                u_storename.Error = "invalid input";
            }
            if (string.IsNullOrEmpty(u_storreg.Text.Trim()))
            {
                result = false;
                u_storreg.Error = "invalid input";
            }

            return result;
        }
        private void Procced_updateStore_Click(object sender, EventArgs e)
        {
            if(isUpdtable()==true)
            {
                try
                {
                    firebase_Helper.GetDatabase().GetReference("Stores").Child(IDstring).Child("storemaill").SetValue(u_storeEmail.Text.Trim());
                    firebase_Helper.GetDatabase().GetReference("Stores").Child(IDstring).Child("Name").SetValue(u_storename.Text.Trim());
                    firebase_Helper.GetDatabase().GetReference("Stores").Child(IDstring).Child("companyReg").SetValue(u_storreg.Text.Trim());

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
                    Toast.MakeText(Application.Context, "error: " + er.Message.ToString(), ToastLength.Long).Show();
                }
            }
        }

        public void Retrieve_orders()
        {
            if (uAcc == "admin")
            {
                order_Data.Retrieve_orders(uAcc);
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


        public void Retrieve_videos()
        {
            vidsData.Retrive_videos();
            vidsData.RetriveUsers += VidsData_RetriveUsers;

        }

        private void VidsData_RetriveUsers(object sender, VideoEvent.RetrivedUsersEventHandeler e)
        {
            vids_model = e.videoList;
            set_Recycle_videos();
        }
        public void set_Recycle_videos()
        {
            LinearLayoutManager linMan = new LinearLayoutManager(Application.Context, LinearLayoutManager.Horizontal, false);
            vidAdapter = new VideoAdapter(vids_model, uAcc.Trim());
            recycleradds.SetLayoutManager(linMan);
            recycleradds.SetAdapter(vidAdapter);
            vidAdapter.rmClick += VidAdapter_rmClick;
            vidAdapter.ItemClick += VidAdapter_ItemClick;
        }

        private void VidAdapter_ItemClick(object sender, VideoAdapterClickEventArgs e)
        {
            IDstring = vids_model[e.Position].vId.Trim();
            DatabaseReference dref = firebase_Helper.GetDatabase().GetReference("Stores").Child(vids_model[e.Position].vId.Trim());
            dref.AddValueEventListener(this);
            showDescproduct();
        }

        private void VidAdapter_rmClick(object sender, VideoAdapterClickEventArgs e)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                DatabaseReference database;
                database = firebase_Helper.GetDatabase().GetReference("Stores");
                database.Child(vids_model[e.Position].vId.Trim()).RemoveValue();
                vids_model.Remove(vids_model[e.Position]);
                vidAdapter.NotifyDataSetChanged();

                //view = (View).RootView;
                Toast.MakeText(this,"Removed",ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, "Lost Internet connection", ToastLength.Long).Show();

            }
        }

        private void Uploadbtn_Click(object sender, EventArgs e)
        {
            upload();
        }

        private void Add_video_btn_Click(object sender, EventArgs e)
        {
            Getfile();
        }
        string filename;
        private async void Getfile()
        {
            try
            {
                var customFileType =
                new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {

                      { DevicePlatform.Android, new[] { "image/*" } },

                });


                var file = await FilePicker.PickAsync(new PickOptions { FileTypes = customFileType });

                if (file != null)
                {
                    filename = file.FileName;

                    fileArray = System.IO.File.ReadAllBytes(file.FullPath);

                    ///Bitmap bitmap = BitmapFactory.DecodeByteArray(fileArray, 0, fileArray.Length);
                    if (fileArray != null)
                    {
                        add_video_btn.Visibility = ViewStates.Gone;
                        uploadbtn.Visibility = ViewStates.Visible;
                        linearStoredetails.Visibility = ViewStates.Visible;

                        Toast.MakeText(this, "ready for upload", ToastLength.Long).Show();

                    }
                    else
                    {
                        add_video_btn.Visibility = ViewStates.Visible;
                        uploadbtn.Visibility = ViewStates.Gone;
                        linearStoredetails.Visibility = ViewStates.Gone;
                    }


                    return;
                }
            }
            catch (Exception fl)
            {
                Toast.MakeText(Application.Context, fl.Message, ToastLength.Long).Show();

            }
        }
        private bool storeready()
        {
            bool result = true;
            if (string.IsNullOrEmpty(storeEmail.Text.Trim()))
            {
                result = false;
                storeEmail.Error = "invalid input";
            }
            if (string.IsNullOrEmpty(storename.Text.Trim()))
            {
                result = false;
                storename.Error = "invalid input";
            }
            if (string.IsNullOrEmpty(storreg.Text.Trim()))
            {
                result = false;
                storreg.Error = "invalid input";
            }

            return result;

        }
        void upload()
        {
            if(storeready()==true)
            {
                if (fileArray != null)
                {
                    progressIndicatorStart();

                    storageReference = FirebaseStorage.Instance.GetReference("SnapImages").Child(filename);
                    storageReference.PutBytes(fileArray)
                        .AddOnSuccessListener(this)
                        .AddOnFailureListener(this);
                }
            }

        }

        private void Seturl(string link)
        {
            progressIndicatorStop();

            if (link.Contains("http")) 
            {
                HashMap data_all = new HashMap();
                data_all.Put("Name", storename.Text.Trim().ToLower());
                data_all.Put("companyReg", storreg.Text.Trim().ToLower());
                data_all.Put("storeImage", link.Trim());
                //data_all.Put("storemaill", DateTime.Today.ToString("D") + " " + DateTime.Now.ToString("HH:mm tt"));
                data_all.Put("storemaill", storeEmail.Text.Trim().ToLower());
                DatabaseReference datab = firebase_Helper.GetDatabase().GetReference("Stores").Push();
                datab.SetValue(data_all);
                
                Toast.MakeText(Application.Context, "Store added", ToastLength.Long).Show();
                resetupload();

            }
        }
        private void resetupload()
        {
            progressIndicatorStop();
            fileArray = null;
            filename = "";
            storreg.Text = "";
            storename.Text = "";
            storeEmail.Text = "";
            linearStoredetails.Visibility = ViewStates.Gone;
            add_video_btn.Visibility = ViewStates.Visible;
            uploadbtn.Visibility = ViewStates.Gone;

        }
        private void progressIndicatorStart()
        {
            progBar_vid.Visibility = ViewStates.Visible;
        }
        private void progressIndicatorStop()
        {
            progBar_vid.Visibility = ViewStates.Gone;
        }

        public async void OnSuccess(Java.Lang.Object result)
        {
            if (storageReference != null)
            {
                var link = await storageReference.GetDownloadUrlAsync();
                Seturl(link.ToString());

            }
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            progressIndicatorStop();
            resetupload();
            linearStoredetails.Visibility = ViewStates.Gone;
            Toast.MakeText(Application.Context, "error: " + e.Message, ToastLength.Long).Show();
        }

        public void OnCancelled(DatabaseError error)
        {
           
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists())
            {
                u_storeEmail.Text = snapshot.Child("storemaill").Value.ToString();
                u_storename.Text = snapshot.Child("Name").Value.ToString();
                u_storreg.Text = snapshot.Child("companyReg").Value.ToString();
            }
        }
    }
}