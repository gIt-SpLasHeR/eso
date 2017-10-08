using FaceYourFace.Constants;
using FaceYourFace.Services;
using Microsoft.ProjectOxford.Face.Contract;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace FaceYourFace.Controllers
{
    public class FaceYourFaceController : ApiController
    {
        // GET: api/FaceYourFace
        [HttpGet]
        public IHttpActionResult Get(string token)
        {
            List<dynamic> heroes;

            if (token.Equals("Booker", System.StringComparison.InvariantCultureIgnoreCase))
            {
                heroes = new List<dynamic>()
                {
                    new { id = 0,  name = "Zero" },
                    new { id = 11, name = "Mr. Nice" },
                    new { id = 12, name = "Narco" },
                    new { id = 13, name = "Bombasto" },
                    new { id = 14, name = "Celeritas" },
                    new { id = 15, name = "Magneta" },
                    new { id = 16, name = "RubberMan" },
                    new { id = 17, name = "Dynama" },
                    new { id = 18, name = "Dr IQ" },
                    new { id = 19, name = "Magma" },
                    new { id = 20, name = "Tornado" }
                };
                return Ok(heroes);
            }
            else
            {
                return NotFound();
            }
        }

        // GET: api/FaceYourFace/5
        [HttpGet]
        public async Task<bool> CheckPersonExist(string EnterpriseId)
        {
            FaceYourFaceService faceservice = new FaceYourFaceService();
            //var sync = await faceservice.AddPerson(groupId.ToString(),"jipeng.zhang");
            ////var str = await sync.Content.ReadAsStringAsync();
            //return sync.ToString();
            bool result = await faceservice.CheckPersonExist(EnterpriseId);
            return result;
        }

        // POST: api/FaceYourFace
        [HttpPost]
        public async Task<string> AddPerson(string EnterpriseId, HttpRequestMessage request)
        {
            FaceYourFaceService faceservice = new FaceYourFaceService();
            //string personImageDir = @"/Image/"+ EnterpriseId;
            List<Stream> FaceStreamList = new List<Stream>();
            Stream stream = await request.Content.ReadAsStreamAsync();
            stream.Position = 0;
            FaceStreamList.Add(stream);
            //foreach (string imagePath in Directory.GetFiles(personImageDir, "*.jpg"))
            //{
            //    using (Stream stream = File.OpenRead(imagePath))
            //    {
            //        byte[] bytes = new byte[stream.Length];
            //        stream.Read(bytes, 0, bytes.Length);
            //        stream.Seek(0, SeekOrigin.Begin);
            //        FaceStreamList.Add(bytes);
            //    }
            //}
            var sync = await faceservice.AddPerson(ConstantsString.GroupId, EnterpriseId, FaceStreamList);
            return sync.ToString();
        }
        [HttpPost]
        public async Task<VerifyResult> VerifyPerson(string EnterpriseId, HttpRequestMessage request)
        {
            FaceYourFaceService faceservice = new FaceYourFaceService();
            Stream stream = await request.Content.ReadAsStreamAsync();
            stream.Position = 0;
            var sync = await faceservice.VerifyFace(EnterpriseId, stream);
            return sync;
        }

        [HttpOptions]
        public async Task<HttpResponseMessage> MakeCreateGroupRequest(string personGroupId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ConstantsString.Key);
            string uri = ConstantsString.EndPoints + "/persongroups/" + personGroupId;
            // Here "name" is for display and doesn't have to be unique. Also, "userData" is optional.
            string json = "{\"name\":\"My Group\", \"userData\":\"Some data related to my group.\"}";
            HttpContent content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.PutAsync(uri, content);
            // If the group was created successfully, you'll see "OK".
            // Otherwise, if a group with the same personGroupId has been created before, you'll see "Conflict".
            //Console.WriteLine("Response status: " + response.StatusCode);
            return response;
        }


        // DELETE: api/FaceYourFace/5
        public void Delete(int id)
        {
        }
    }
}
