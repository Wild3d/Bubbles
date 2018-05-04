using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Bubbles.Model
{
    public class Feed
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }

    public class Item
    {
        public string Title { get; set; }
        public string PubDate { get; set; }
        public string Link { get; set; }
        public string Guid { get; set; }
        public string Author { get; set; }
        public string Thumbnail { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public object Enclosure { get; set; }
        public List<string> Categories { get; set; }
    }

    public class RssObject
    {
        public string Status { get; set; }
        public Feed Feed { get; set; }
        public List<Item> Items { get; set; }
    }
}