using Snap_n_Pay.Models;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using FFImageLoading;

namespace Snap_n_Pay.Adapters
{
    class VideoAdapter : RecyclerView.Adapter
    {
        public event EventHandler<VideoAdapterClickEventArgs> ItemClick;
        public event EventHandler<VideoAdapterClickEventArgs> ItemLongClick;
        public event EventHandler<VideoAdapterClickEventArgs> rmClick;
        List<VideoModel> items;
        private string account = "";

        public VideoAdapter(List<VideoModel> data,string account)
        {
            
            items = data;
            this.account = account.ToLower();
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.videoAds;
            itemView = LayoutInflater.From(parent.Context).
                  Inflate(id, parent, false);

            var vh = new VideoAdapterViewHolder(itemView, OnClick, OnLongClick, OnrmClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as VideoAdapterViewHolder;
            holder.msg.Text = items[position].Msg;
            getimage(items[position].vlink, holder.vid);
            if (this.account.ToLower().Trim()=="admin")
            {
                holder.rmImgvid.Visibility = ViewStates.Visible;
            }
            else
            {
                holder.rmImgvid.Visibility = ViewStates.Gone;
            }
            
        }
        void getimage(string imageUrl, ImageView imageView)
        {
            if (imageUrl.ToLower().Contains("http") == true)
            {
                ImageService.Instance.LoadUrl(imageUrl)
                .Retry(3, 200)
                .DownSample(350, 350)
                .Into(imageView);

            }
            else { imageView.SetBackgroundResource(Resource.Mipmap.snapCard); }

        }

        public override int ItemCount => items.Count;

        void OnClick(VideoAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(VideoAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        void OnrmClick(VideoAdapterClickEventArgs args) => rmClick?.Invoke(this, args);
    }

    public class VideoAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView msg { get; set; }
        public ImageView vid { get; set; }
        public ImageView rmImgvid { get; set; }

        public VideoAdapterViewHolder(View itemView, Action<VideoAdapterClickEventArgs> clickListener,
                            Action<VideoAdapterClickEventArgs> longClickListener,
                            Action<VideoAdapterClickEventArgs> rmClickListener) : base(itemView)
        {
            //TextView = v;
            vid = itemView.FindViewById<ImageView>(Resource.Id.vid);
            msg = itemView.FindViewById<TextView>(Resource.Id.msg);
            rmImgvid = itemView.FindViewById<ImageView>(Resource.Id.rmImgvid);
            rmImgvid.Click += (sender, e) => rmClickListener(new VideoAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.Click += (sender, e) => clickListener(new VideoAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new VideoAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class VideoAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}