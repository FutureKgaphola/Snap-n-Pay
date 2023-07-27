
using Android.App;
using Firebase.Storage;
using Firebase;

namespace Snap_n_Pay.Helpers
{
    class storageHelper
    {
        public static FirebaseStorage GetFirebaseStorage()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseStorage storage;
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
                storage = FirebaseStorage.GetInstance(app);
                return storage;
            }
            else
            {
                storage = FirebaseStorage.GetInstance(app);
                return storage;
            }
        }
    }
}