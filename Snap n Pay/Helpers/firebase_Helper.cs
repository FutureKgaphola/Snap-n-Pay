
using Android.App;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

namespace Snap_n_Pay.Helpers
{
    class firebase_Helper
    {
        public static FirebaseDatabase GetDatabase()
        {
            FirebaseDatabase database;
            var app = FirebaseApp.InitializeApp(Application.Context);
            if (app == null)
            {
                var option = new FirebaseOptions.Builder()
                    .SetApiKey("AIzaSyDqwVFAYJeTqyGChxZ7tl45PV1GSPoQ17E")
                    .SetApplicationId("ledet-lab")
                    .SetDatabaseUrl("https://ledet-lab-default-rtdb.firebaseio.com")
                    .SetProjectId("ledet-lab")
                    .SetStorageBucket("ledet-lab.appspot.com")
                    .Build();
                app = FirebaseApp.InitializeApp(Application.Context, option);
                database = FirebaseDatabase.GetInstance(app);
                return database;

            }
            else
            {
                database = FirebaseDatabase.GetInstance(app);
                return database;

            }
        }
        public FirebaseAuth GetFirebaseAuth()
        {
            FirebaseAuth firebase;
            var app = FirebaseApp.InitializeApp(Application.Context);
            if (app == null)
            {
                var option = new FirebaseOptions.Builder()
                    .SetApiKey("AIzaSyDqwVFAYJeTqyGChxZ7tl45PV1GSPoQ17E")
                    .SetApplicationId("ledet-lab")
                    .SetDatabaseUrl("https://ledet-lab-default-rtdb.firebaseio.com")
                    .SetProjectId("ledet-lab")
                    .SetStorageBucket("ledet-lab.appspot.com")
                    .Build();
                app = FirebaseApp.InitializeApp(Application.Context, option);
                firebase = FirebaseAuth.GetInstance(app);
                return firebase;

            }
            else
            {

                firebase = FirebaseAuth.GetInstance(app);
                return firebase;
            }

        }

    }
}