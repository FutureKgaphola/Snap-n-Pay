using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snap_n_Pay.Models
{
    public class OrdersModel
    {
        public string order_Id { get; set; }
        public string dateOrdered { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string status { get; set; }
        public string maxCost { get; set; }
        public string customerId { get; set; }
        public string uniquecode { get; set; }
    }
}