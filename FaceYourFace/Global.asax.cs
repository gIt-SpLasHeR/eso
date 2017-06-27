using FaceYourFace.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace FaceYourFace
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer<FaceYourFaceContext>(new DropCreateDatabaseIfModelChanges<FaceYourFaceContext>());
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
