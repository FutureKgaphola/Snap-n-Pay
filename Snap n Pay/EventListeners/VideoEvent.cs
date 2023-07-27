using Snap_n_Pay.Helpers;
using Snap_n_Pay.Models;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snap_n_Pay.EventListeners
{
    public class VideoEvent : Java.Lang.Object, IValueEventListener
    {
        private List<VideoModel> UserPos = new List<VideoModel>();

        public event EventHandler<RetrivedUsersEventHandeler> RetriveUsers;
        public class RetrivedUsersEventHandeler : EventArgs
        {
            public List<VideoModel> videoList { get; set; }
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
                    UserPos.Clear();
                    var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                    foreach (DataSnapshot data in child)
                    {
                        VideoModel uM = new VideoModel
                        {
                            vId = data.Key,
                            companyReg = data.Child("companyReg").Value.ToString(),
                            storemaill = data.Child("storemaill").Value.ToString(),
                            Msg = data.Child("Name").Value.ToString(),
                            vlink = data.Child("storeImage").Value.ToString()

                        };
                        UserPos.Add(uM);
                    }
                    RetriveUsers.Invoke(this, new RetrivedUsersEventHandeler
                    {
                        videoList = UserPos
                    });
                }
                else
                {

                }
            }
            catch (System.Exception fetch)
            {
                Toast.MakeText(Application.Context, "Error: couldn't fetch data as expected: " + fetch.Message.ToString(), ToastLength.Long).Show();
            }
        }
        public void Retrive_videos()
        {
            DatabaseReference dref = firebase_Helper.GetDatabase().GetReference("Stores");
            dref.AddValueEventListener(this);
        }
    }
}