using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Google.Android.Material.Button;
using Snap_n_Pay.Adapters;
using Snap_n_Pay.EventListeners;
using Snap_n_Pay.Helpers;
using Snap_n_Pay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xamarin.Essentials;

namespace Snap_n_Pay.Fragments
{
    public class HomeFragment : AndroidX.Fragment.App.Fragment, IValueEventListener
    {
        View view;
        SamsungAdapter vAdapter;
        private RecyclerView RecycleVotes;
        private List<SamsungModel> vmodel = new List<SamsungModel>();
        private samsungEvent userData = new samsungEvent();

        private List<UniqueCodeModel> ucodesmodel = new List<UniqueCodeModel>();
        private CodesEvent codesData = new CodesEvent();

        

        private ImageView isdecreasebtn,isincreasebtn;
        private TextView product_Count;
        private Button enquires, btnHistory, procced_pay;
        private MaterialButton btnStartover;
        private EditText costfield;
        double individualCost;
        string phone = "";
        string uAcc, myID, tot, emailfield;
        TextView selectedStore, selectedmsg;
        string selectedStoreEmail="";
        CardView cardBlock;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.Home, container, false);
            selectedStore= view.FindViewById<TextView>(Resource.Id.selectedStore);
            selectedmsg= view.FindViewById<TextView>(Resource.Id.selectedmsg);
            enquires = view.FindViewById<Button>(Resource.Id.btnEnquire);
            btnHistory = view.FindViewById<Button>(Resource.Id.btnHistory);
            btnStartover = view.FindViewById<MaterialButton>(Resource.Id.btnStartover);
            btnStartover.Click += BtnStartover_Click;
            procced_pay = view.FindViewById<Button>(Resource.Id.procced_pay);
            cardBlock= view.FindViewById<CardView>(Resource.Id.cardBlock);
            btnHistory.Click += BtnHistory_Click;
            enquires.Click += Enquires_Click;
            RecycleVotes= view.FindViewById<RecyclerView>(Resource.Id.R_Samsung);
            isdecreasebtn = view.FindViewById<ImageView>(Resource.Id.isdecreasebtn);
            isdecreasebtn.Click += Isdecreasebtn_Click;
            isincreasebtn = view.FindViewById<ImageView>(Resource.Id.isincreasebtn);
            isincreasebtn.Click += Isincreasebtn_Click;
            procced_pay.Click += Procced_pay_Click;
            product_Count = view.FindViewById<TextView>(Resource.Id.product_Count);
            costfield = view.FindViewById<EditText>(Resource.Id.amonut);
            tot = Arguments.GetString("MyDataTag");
            myID = tot.Substring(0, tot.IndexOf('#'));
            tot = tot.Remove(0, tot.IndexOf('#') + 1);
            uAcc = tot.Substring(0, tot.IndexOf('#'));
            tot = tot.Remove(0, tot.IndexOf('#') + 1);
            emailfield = tot.Substring(0, tot.IndexOf('#'));
            tot = tot.Remove(0, tot.IndexOf('#') + 1);
            phone = tot.Trim();

            firebase_Helper.GetDatabase().GetReference("SPendings").AddValueEventListener(this);
            AreyouInitiator();
            Retrieve_Samsungs();
            return view;
        }

        public void OnCancelled(DatabaseError error)
        {

        }
        List<string> ClientsKeys = new List<string>();
        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists())
            {
                ClientsKeys.Clear();
                for (int i = 0; i < Convert.ToInt32(snapshot.ChildrenCount.ToString()); i++)
                {
                    var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                    //Toast.MakeText(Application.Context,(child.ToList())[i].Key.ToString(),ToastLength.Long).Show();
                    ClientsKeys.Add((child.ToList())[i].Key.ToString());
                }

            }
            else
            {
                ClientsKeys.Clear();
            }
        }


        public void Retrieve_Samsungs()
        {
            userData.Retrive_phones();
            userData.RetrivePhones += UserData_RetrivePhones;
        }
        private void UserData_RetrivePhones(object sender, samsungEvent.RetrivedPhonesEventHandeler e)
        {
            vmodel = e.phoneList;
            setSamsungRecycler();
        }
        private void setSamsungRecycler()
        {
            LinearLayoutManager linMan = new LinearLayoutManager(Application.Context, LinearLayoutManager.Horizontal, false);
            vAdapter = new SamsungAdapter(vmodel, uAcc);
            RecycleVotes.SetLayoutManager(linMan);
            RecycleVotes.SetAdapter(vAdapter);
            vAdapter.rmClick += VAdapter_rmClick;
            vAdapter.ItemClick += VAdapter_ItemClick;

        }

        private void VAdapter_ItemClick(object sender, PhoneAdapterClickEventArgs e)
        {
            selectedStore.Text = vmodel[e.Position].StoreName;
            selectedStoreEmail= vmodel[e.Position].selectedStoreEmail;
            if (!string.IsNullOrEmpty(selectedStore.Text.Trim()))
            {
                selectedmsg.Visibility = ViewStates.Visible;
                selectedStore.Visibility = ViewStates.Visible;
            }
            else
            {
                selectedmsg.Visibility = ViewStates.Gone;
                selectedStore.Visibility = ViewStates.Gone;
            }
        }

        private void VAdapter_rmClick(object sender, PhoneAdapterClickEventArgs e)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                DatabaseReference database;
                database = firebase_Helper.GetDatabase().GetReference("Stores");
                database.Child(vmodel[e.Position].Idstore.Trim()).RemoveValue();
                vmodel.Remove(vmodel[e.Position]);
                vAdapter.NotifyDataSetChanged();

                view = (View).RootView;
                Snackbar.Make(view, "removed", Snackbar.LengthLong)
                    .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
            }
            else
            {
                view = (View)sender;
                Snackbar.Make(view, "Lost Internet connection", Snackbar.LengthLong)
                    .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
            }
        }
        private void Isincreasebtn_Click(object sender, EventArgs e)
        {
            if (count > 0 && count<20 &&!string.IsNullOrEmpty(costfield.Text))
            {
                count++;
                product_Count.Text = count.ToString();
                individualCost = (Convert.ToDouble(costfield.Text.Replace('.',',')) / Convert.ToDouble(count));
                //Toast.MakeText(Application.Context, individualCost.ToString(), ToastLength.Long).Show();
            }
            else
            {
                if(count==20)
                {
                    Toast.MakeText(Application.Context, "You can split the cost to a maximum people of 20", ToastLength.Long).Show();
                }
                else
                {
                    costfield.Error = "input a value";
                }
                
            }
        }
        int count=1;
        private void Isdecreasebtn_Click(object sender, EventArgs e)
        {
            if (count > 0 && count != 1 && !string.IsNullOrEmpty(costfield.Text))
            {
                count--;
                product_Count.Text = count.ToString();
                individualCost = (Convert.ToDouble(costfield.Text.Replace('.', ',')) /Convert.ToDouble(count));
            }
            else
            {
                if(string.IsNullOrEmpty(costfield.Text))
                {
                    costfield.Error = "input a value";
                }
                
            }
        }
        bool checkfields()
        {
            bool Result=true;
            if (string.IsNullOrEmpty(emailfield.Trim()))
            {
                Result = false;
                Toast.MakeText(Application.Context,"Email is not set",ToastLength.Long).Show();
            }
            if (string.IsNullOrEmpty(costfield.Text.Trim()))
            {
                Result = false;
                costfield.Error = "required";
            }
            if(count==1 && Result==true)
            {
                individualCost = Convert.ToDouble(costfield.Text.Replace('.', ','));
            }
            return Result;
        }

        void paydetails()
        {
            if(!string.IsNullOrEmpty(selectedStore.Text.Trim()))
            {
                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(RequireActivity());
                builder.SetTitle("Payment approval");
                builder.SetIcon(Resource.Mipmap.sharecards);
                builder.SetMessage("Based on the details provided, your cost will be R" + individualCost.ToString() + " as devided from the given maximum. a copy recept will be mailed to " + emailfield.Trim());
                builder.SetPositiveButton("ok", delegate
                {
                    builder.Dispose();
                    Intent i = new Intent(Application.Context, typeof(Activities.Payment));
                    i.PutExtra("pdata", individualCost.ToString() + "#" + emailfield.Trim() + "#" +
                        myID + "#" + phone + "#" + selectedStore.Text.Trim() + "#" +
                        product_Count.Text.Trim() + "#" + "yes" + "#" + "nocode"+"#"+ selectedStoreEmail);
                    costfield.Text = null;
                    StartActivity(i);
                });
                builder.SetNeutralButton("no, cancel", delegate
                {
                    builder.Dispose();

                });

                builder.Show();
            }
            else
            {
                Toast.MakeText(Application.Context,"Please select a store",ToastLength.Long).Show();
            }
            
        }

        private Android.App.AlertDialog dialogs;
        private Android.App.AlertDialog.Builder dialogBuilders;
        private Button procced_checkcode;
        private EditText uniquecode;
        void AreyouInitiator()
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(RequireActivity());
            builder.SetTitle("Heads up");
            builder.SetIcon(Resource.Mipmap.sharecards);
            builder.SetMessage("Are you the initiator (the first person to pay.)");
            builder.SetPositiveButton("yes", delegate
            {
                builder.Dispose();
                cardBlock.Visibility = ViewStates.Visible;

                CheckIsAllowed();

            });
            builder.SetNeutralButton("just paying my half", delegate
            {
                builder.Dispose();
                //call reference box, then pay
                enterCode();
                //display the call icons only if valid code after checking it

            });

            builder.Show();
        }


        private void enterCode()
        {
            dialogBuilders = new Android.App.AlertDialog.Builder(Activity);
            LayoutInflater inflater = LayoutInflater.From(Activity);
            View dialogView = inflater.Inflate(Resource.Layout.dialogCode, null);
            procced_checkcode = dialogView.FindViewById<Button>(Resource.Id.procced_checkcode);

            uniquecode = dialogView.FindViewById<EditText>(Resource.Id.uniquecode);

            procced_checkcode.Click += Procced_checkcode_Click;
            dialogBuilders.SetView(dialogView);
            dialogBuilders.SetCancelable(true);
            dialogs = dialogBuilders.Create();
            dialogs.Show();

        }
        private void CheckIsAllowed()
        {
            if(ClientsKeys.Contains(myID.Trim()))
            {
                cardBlock.Visibility = ViewStates.Gone;
                Toast.MakeText(Application.Context,"You have a payment hanging...please complete it.",ToastLength.Long).Show();
                enterCode();
            }
        }
        private bool isCodevalid()
        {
            bool result = false;
            if (ClientsKeys.Contains(uniquecode.Text.Trim()))
            {
                RetriveCodeDetails();
                cardBlock.Visibility = ViewStates.Gone;
                result = true;
            }
            return result;
        }

        private void BtnStartover_Click(object sender, EventArgs e)
        {
            cardBlock.Visibility = ViewStates.Gone;
            AreyouInitiator();
        }

        private void Procced_checkcode_Click(object sender, EventArgs e)
        {
            if(isCodevalid()==true)
            {
                dialogs.Dismiss();
    

            }
        }
        void RetriveCodeDetails()
        {
            codesData.Retrieve_Codes(uniquecode.Text.Trim());
            codesData.RetriveOrders += CodesData_RetriveOrders;
        }
        string maxparticipants="0", costTopay=null;
        string storeTopay=null, storeCopyEmail=null;
        bool Ready = false;
        private void CodesData_RetriveOrders(object sender, CodesEvent.RetrivedCodesEventHandeler e)
        {
            ucodesmodel = e.CodesList;
            if(ucodesmodel.Count>0)
            {
                maxparticipants = ucodesmodel[0].people;
                costTopay = ucodesmodel[0].paymenteach;
                storeTopay = ucodesmodel[0].storeTopay;
                storeCopyEmail = ucodesmodel[0].storeCopyEmail;
                Ready = true;
            }
            if (Ready == true && costTopay!=null && storeCopyEmail!=null && storeTopay!=null)
            {
                Intent i = new Intent(Application.Context, typeof(Activities.Payment));//email must come from user s profile
                i.PutExtra("pdata", costTopay.Trim() + "#" + emailfield.Trim() + "#" +
                    myID + "#" + phone + "#" + storeTopay.Trim() + "#" +
                    maxparticipants.Trim() + "#" + "no" + "#" + uniquecode.Text.Trim()+"#"+ storeCopyEmail);
                costfield.Text = null;
                costTopay = null;
                Ready = false;
                StartActivity(i);
            }

            //Toast.MakeText(Application.Context, maxparticipants, ToastLength.Long).Show();
            //Toast.MakeText(Application.Context, costTopay, ToastLength.Long).Show();
        }

        private void Procced_pay_Click(object sender, EventArgs e)
        {
            if(checkfields())
            {
                paydetails();
                cardBlock.Visibility = ViewStates.Gone;
            }
        }
        private void BtnHistory_Click(object sender, EventArgs e)
        {
            Intent i = new Intent(Application.Context, typeof(History));
            i.PutExtra("historydata", myID+"#"+uAcc);
            StartActivity(i);
        }

        private void Enquires_Click(object sender, System.EventArgs e)
        {
            try
            {
                PhoneDialer.Open("071232400");
            }
            catch (ArgumentNullException anEx)
            {
                // Number was null or white space
                Toast.MakeText(Application.Context, anEx.Message, ToastLength.Long).Show();
            }
            catch (FeatureNotSupportedException ex)
            {
                // Phone Dialer is not supported on this device.
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
            catch (Exception ex)
            {
                // Other error has occurred.
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }
    }
}