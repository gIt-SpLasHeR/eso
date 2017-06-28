using FaceYourFace.Constants;
using FaceYourFace.Services;
using Microsoft.ProjectOxford.Face.Contract;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace FaceYourFace.Controllers
{
    public class FaceYourFaceController : ApiController
    {
        // GET: api/FaceYourFace
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
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
            var sync = await faceservice.AddPerson(ConstantsString.GroupId,  EnterpriseId, FaceStreamList);
            return sync.ToString();
        }
        [HttpPost]
        public async Task<VerifyResult> VerifyPerson(string EnterpriseId, HttpRequestMessage request)
        {
            FaceYourFaceService faceservice = new FaceYourFaceService();
            //string personImageDir = @"/Image/test" ;
            //List<byte[]> FaceStreamList = new List<byte[]>();
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
            Stream stream = await request.Content.ReadAsStreamAsync();
            stream.Position = 0;
            //byte[] buffer = new byte[st.Length];
            //using (FileStream fs = new FileStream(@"C:\Image\test\input.jpg",
            //                    FileMode.Create, FileAccess.Write, FileShare.None))
            //{
            //    int read;
            //    while ((read = st.Read(buffer, 0, buffer.Length)) > 0)
            //    {
            //        fs.Write(buffer, 0, read);
            //    }
            //}
            var sync = await faceservice.VerifyFace(EnterpriseId, stream);
            return sync;
        }

        // PUT: api/FaceYourFace/5
        //public async void PostTest(HttpRequestMessage request)
        //{
        //   var t = request;
        //   Stream st =await request.Content.ReadAsStreamAsync();
        //    st.Position = 0;
        //    byte[] buffer = new byte[st.Length];
        //    using (FileStream fs = new FileStream(@"C:\Image\test\input.jpg",
        //                        FileMode.Create, FileAccess.Write, FileShare.None))
        //    {
        //        int read;
        //        while ((read = st.Read(buffer, 0, buffer.Length)) > 0)
        //        {
        //            fs.Write(buffer, 0, read);
        //        }
        //    }
        //}

        // DELETE: api/FaceYourFace/5
        public void Delete(int id)
        {
        }
    }
}
