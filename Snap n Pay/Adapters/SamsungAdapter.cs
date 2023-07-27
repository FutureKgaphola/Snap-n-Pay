using Snap_n_Pay.Models;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using System;
using System.Collections.Generic;

namespace Snap_n_Pay.Adapters
{
    class SamsungAdapter : RecyclerView.Adapter
    {
        public event EventHandler<PhoneAdapterClickEventArgs> ItemClick;
        public event EventHandler<PhoneAdapterClickEventArgs> ItemLongClick;
        public event EventHandler<PhoneAdapterClickEventArgs> rmClick;

        List<SamsungModel> items;
        string uAcc;
        public SamsungAdapter(List<SamsungModel> data,string uAcc)
        {
            this.uAcc = uAcc;
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.samsung_item;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new PhoneAdapterViewHolder(itemView, OnClick, OnLongClick, OnrmClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as PhoneAdapterViewHolder;
            holder.brand.Text = items[position].StoreName.ToLower().Trim();

            if (this.uAcc.ToLower().Trim() != "admin".ToLower().Trim())
            {
                holder.rmImg.Visibility = ViewStates.Gone;
            }
            if (this.uAcc.ToLower().Trim() == "admin".ToLower().Trim())
            {
                holder.rmImg.Visibility = ViewStates.Visible;
            }
            getimage(items[position].img, holder.p_img);
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
            else { imageView.SetBackgroundResource(Resource.Mipmap.sharecards); }
        }

        public override int ItemCount => items.Count;

        void OnClick(PhoneAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(PhoneAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        void OnrmClick(PhoneAdapterClickEventArgs args) => rmClick?.Invoke(this, args);
    }

    public class PhoneAdapterViewHolder : RecyclerView.ViewHolder
    {

        public TextView brand { get; set; }

        public ImageView p_img { get; set; }

        public ImageView rmImg { get; set; }


        public PhoneAdapterViewHolder(View itemView, Action<PhoneAdapterClickEventArgs> clickListener,
                            Action<PhoneAdapterClickEventArgs> longClickListener,
                            Action<PhoneAdapterClickEventArgs> rmClickListener) : base(itemView)
        {
            p_img = itemView.FindViewById<ImageView>(Resource.Id.p_img);
            brand = itemView.FindViewById<TextView>(Resource.Id.brand);
            rmImg = itemView.FindViewById<ImageView>(Resource.Id.rmIm4);

            rmImg.Click += (sender, e) => rmClickListener(new PhoneAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.Click += (sender, e) => clickListener(new PhoneAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new PhoneAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class PhoneAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}