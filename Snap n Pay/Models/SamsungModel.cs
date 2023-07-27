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
    public class SamsungModel
    {
        public string Idstore { get; set; }
        public string StoreName { get; set; }
        public string img { get; set; }
        public string companyReg { get; set; }
        public string selectedStoreEmail { get; set; }
    }
}