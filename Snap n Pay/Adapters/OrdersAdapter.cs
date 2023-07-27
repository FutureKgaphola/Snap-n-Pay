using Snap_n_Pay.Models;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace Snap_n_Pay.Adapters
{
    class OrdersAdapter : RecyclerView.Adapter
    {
        public event EventHandler<OrdersAdapterClickEventArgs> ItemClick;
        public event EventHandler<OrdersAdapterClickEventArgs> ItemLongClick; 
        public event EventHandler<OrdersAdapterClickEventArgs> uptdbtnClick;
        public event EventHandler<OrdersAdapterClickEventArgs> whatsappClick;
        public event EventHandler<OrdersAdapterClickEventArgs> smsClick; 
        public event EventHandler<OrdersAdapterClickEventArgs> newodericonClick;
        List<OrdersModel> items;
        string uAcc;
        public OrdersAdapter(List<OrdersModel> data,string uAcc)
        {
            this.uAcc = uAcc;
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.order_item;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new OrdersAdapterViewHolder(itemView, OnClick, OnLongClick, OnuptdbtnClick,
                OnwhatsappClick, OnsmsClick, OnnewodericonClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as OrdersAdapterViewHolder;
            holder.txtOrderId.Text = items[position].order_Id;
            holder.txtTime.Text = items[position].dateOrdered;
            holder.txtmycut.Text = items[position].maxCost;

            //holder.uptdbtn.Visibility = ViewStates.Visible;
            //string sysDate = DateTime.Today.ToString("D") + " " + DateTime.Now.ToString("HH:mm tt");
            //DateTime a = Convert.ToDateTime(sysDate);
            //DateTime b = Convert.ToDateTime(items[position].dateOrdered);
            if (items[position].status.Trim().ToLower()=="pending")
            {
                //show icon jst new order
                holder.deleterecord.SetImageResource(Resource.Mipmap.sharecards);

            }
            else if (items[position].status.Trim().ToLower() != "pending")
            {
                holder.deleterecord.SetImageResource(Resource.Mipmap.sharecards);
            }

        }

        public override int ItemCount => items.Count;

        void OnClick(OrdersAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(OrdersAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        void OnuptdbtnClick(OrdersAdapterClickEventArgs args) => uptdbtnClick?.Invoke(this, args);
        void OnwhatsappClick(OrdersAdapterClickEventArgs args) => whatsappClick?.Invoke(this, args);
        void OnsmsClick(OrdersAdapterClickEventArgs args) => smsClick?.Invoke(this, args);
        void OnnewodericonClick(OrdersAdapterClickEventArgs args) => newodericonClick?.Invoke(this, args);
    }

    public class OrdersAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView txtOrderId { get; set; }
        public TextView txtTime { get; set; }
        public Button uptdbtn { get; set; }
        public TextView txtmycut { get; set; }
        public ImageView newodericon { get; set; }
        public ImageView whatsapp { get; set; }
        public ImageView phonecall { get; set; }
        public ImageView deleterecord { get; set; }
        public OrdersAdapterViewHolder(View itemView, Action<OrdersAdapterClickEventArgs> clickListener,
                            Action<OrdersAdapterClickEventArgs> longClickListener,
                            Action<OrdersAdapterClickEventArgs> uptdbtnClickListener,
                            Action<OrdersAdapterClickEventArgs> whatsappClickListener,
                            Action<OrdersAdapterClickEventArgs> smsClickListener,
                            Action<OrdersAdapterClickEventArgs> newodericonClickListener) : base(itemView)
        {
            txtOrderId = itemView.FindViewById<TextView>(Resource.Id.txtOrderId);
            whatsapp = itemView.FindViewById<ImageView>(Resource.Id.whats_app);
            deleterecord = itemView.FindViewById<ImageView>(Resource.Id.newodericon);
            phonecall = itemView.FindViewById<ImageView>(Resource.Id.phonesms);
            txtTime = itemView.FindViewById<TextView>(Resource.Id.txtTime);
            uptdbtn= itemView.FindViewById<Button>(Resource.Id.uptdbtn);
            txtmycut = itemView.FindViewById<TextView>(Resource.Id.txtmycut);
            //newodericon= itemView.FindViewById<ImageView>(Resource.Id.newodericon);

            deleterecord.Click += (sender, e) => newodericonClickListener(new OrdersAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            whatsapp.Click += (sender, e) => whatsappClickListener(new OrdersAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            phonecall.Click += (sender, e) => smsClickListener(new OrdersAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            uptdbtn.Click+=(sender, e) => uptdbtnClickListener(new OrdersAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.Click += (sender, e) => clickListener(new OrdersAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new OrdersAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class OrdersAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}