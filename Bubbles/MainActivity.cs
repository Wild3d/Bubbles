using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.Widget;
using Bubbles.Model;
using Bubbles.Common;
using Newtonsoft.Json;
using Bubbles.Adapter;
using System.Text;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using System.Collections.Generic;
using Bubbles;

namespace Bubble
{
    [Activity(Label = "Bubble", MainLauncher = true, Icon = "@drawable/Logo", Theme = "@style/MyTheme")]
    public class MainActivity : AppCompatActivity
    {
        RecyclerView recyclerView;
        RssObject rssObject;
        private const string RSS_link = "http://rss.nytimes.com/services/xml/rss/nyt/Science.xml";
        private const string RSS_to_json = "https://api.rss2json.com/v1/api.json?rss_url=";
        private SupportToolbar mToolbar;
        private Bubbles.MyActionBarDrawerToggle mDrawerToggle;
        private DrawerLayout mDrawerLayout;
        private ListView mLeftDrawer;
        private ListView mRightDrawer;
        private ArrayAdapter mLeftAdapter;
        private ArrayAdapter mRightAdapter;
        private List<string> mLeftDataSet;
        private List<string> mRightDataSet;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);
            mRightDrawer = FindViewById<ListView>(Resource.Id.right_drawer);

            mLeftDrawer.Tag = 0;
            mRightDrawer.Tag = 1;

            SetSupportActionBar(mToolbar);

            mLeftDataSet = new List<string>();
            mLeftDataSet.Add("Profil");
            mLeftDataSet.Add("Kontakte");
            mLeftDataSet.Add("Tasks");
            mLeftDataSet.Add("Arbeitsplan");
            mLeftDataSet.Add("Chat");
            mLeftDataSet.Add("Suche");
            mLeftAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mLeftDataSet);
            mLeftDrawer.Adapter = mLeftAdapter;

            mRightDataSet = new List<string>();
            mRightDataSet.Add("Einstellungen");
            mRightDataSet.Add("Benachrichtigungen");
            mRightAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mRightDataSet);
            mRightDrawer.Adapter = mRightAdapter;

            mDrawerToggle = new Bubbles.MyActionBarDrawerToggle(
                this,                           //Host Activity
                mDrawerLayout,                  //DrawerLayout
                Resource.String.openDrawer,     //Opened Message
                Resource.String.closeDrawer     //Closed Message
            );

            mDrawerLayout.AddDrawerListener(mDrawerToggle);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            mDrawerToggle.SyncState();

            if (bundle != null)
            {
                if (bundle.GetString("DrawerState") == "Opened")
                {
                    SupportActionBar.SetTitle(Resource.String.openDrawer);
                }

                else
                {
                    SupportActionBar.SetTitle(Resource.String.closeDrawer);
                }
            }

            else
            {
                //This is the first the time the activity is ran
                SupportActionBar.SetTitle(Resource.String.closeDrawer);
            }
            //NewsFeed
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            recyclerView.SetLayoutManager(linearLayoutManager);
            LoadData();
        }
        private void LoadData()
        {
            StringBuilder strBuilder = new StringBuilder(RSS_to_json);
            strBuilder.Append(RSS_link);

            new LoadDataAsync(this).Execute(strBuilder.ToString());
        }
        class LoadDataAsync : AsyncTask<string, string, string>
        {

            MainActivity mainActivity;
        //    ProgressDialog mDialog;

            public LoadDataAsync(MainActivity mainActivity)
            {
                this.mainActivity = mainActivity;
            }

        /*    protected override void OnPreExecute()
            {
                mDialog = new ProgressDialog(mainActivity);
                mDialog.Window.SetType(Android.Views.WindowManagerTypes.SystemAlert);
                mDialog.SetMessage("Please wait...");
                mDialog.Show();
            }*/
            protected override string RunInBackground(params string[] @params)
            {
                string result = new HTTPDataHandler().GetHTTPData(@params[0]);
                return result;
            }

            protected override void OnPostExecute(string result)
            {
                RssObject data = JsonConvert.DeserializeObject<RssObject>(result);
           //     mDialog.Dismiss();
                FeedAdapter adapter = new FeedAdapter(data, mainActivity);
                mainActivity.recyclerView.SetAdapter(adapter);
                adapter.NotifyDataSetChanged();
            }
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {

                case Android.Resource.Id.Home:
                    //The hamburger icon was clicked which means the drawer toggle will handle the event
                    //all we need to do is ensure the right drawer is closed so the don't overlap
                    mDrawerLayout.CloseDrawer(mRightDrawer);
                    mDrawerToggle.OnOptionsItemSelected(item);
                    return true;

                case Resource.Id.action_refresh:
                //Refresh
                LoadData();
                return true;

                case Resource.Id.action_help:
                    if (mDrawerLayout.IsDrawerOpen(mRightDrawer))
                    {
                        //Right Drawer is already open, close it
                        mDrawerLayout.CloseDrawer(mRightDrawer);
                    }

                    else
                    {
                        //Right Drawer is closed, open it and just in case close left drawer
                        mDrawerLayout.OpenDrawer(mRightDrawer);
                        mDrawerLayout.CloseDrawer(mLeftDrawer);
                    }

                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.action_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (mDrawerLayout.IsDrawerOpen((int)GravityFlags.Left))
            {
                outState.PutString("DrawerState", "Opened");
            }

            else
            {
                outState.PutString("DrawerState", "Closed");
            }

            base.OnSaveInstanceState(outState);
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            mDrawerToggle.SyncState();
        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            mDrawerToggle.OnConfigurationChanged(newConfig);
        }
             

        
    }
}

