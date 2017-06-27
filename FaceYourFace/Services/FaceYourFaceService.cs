using FaceYourFace.Constants;
using FaceYourFace.DTO;
using FaceYourFace.EntityWork;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FaceYourFace.Services
{
    public class FaceYourFaceService
    {
        const int PersonCount = 10000;
        const int CallLimitPerSecond = 10;
        static Queue<DateTime> _timeStampQueue = new Queue<DateTime>(CallLimitPerSecond);
        FaceServiceClient faceServiceClient = new FaceServiceClient(ConstantsString.Key, ConstantsString.EndPoints);// apiroot means endpoints
        FaceYourFaceEntity faceEntity = new FaceYourFaceEntity();
        public static async Task WaitCallLimitPerSecondAsync()
        {
            Monitor.Enter(_timeStampQueue);
            try
            {
                if (_timeStampQueue.Count >= CallLimitPerSecond)
                {
                    TimeSpan timeInterval = DateTime.UtcNow - _timeStampQueue.Peek();
                    if (timeInterval < TimeSpan.FromSeconds(1))
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1) - timeInterval);
                    }
                    _timeStampQueue.Dequeue();
                }
                _timeStampQueue.Enqueue(DateTime.UtcNow);
            }
            finally
            {
                Monitor.Exit(_timeStampQueue);
            }
        }
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

        public async Task<bool> AddPerson(string GroupID, string eid, List<Stream> FaceStreamList)
        {
            string personGroupId = GroupID;
            string personGroupName = GroupID;
            string personName = eid;
            bool success = false;
            PersonResultItem person = faceEntity.GetPerson(eid);
            if (person.EnterpriseID == null)// 
            {
                _timeStampQueue.Enqueue(DateTime.UtcNow);
                //CreatePersonResult person = new CreatePersonResult();
                var syncPersonResult = await faceServiceClient.CreatePersonAsync(personGroupId, personName);
                //add
                person.EnterpriseID = eid;
                person.PersonGroupId = ConstantsString.GroupId;
                person.PersonId = syncPersonResult.PersonId;
                faceEntity.InsertPerson(person);
            }
            try
            {
                foreach (var faceStream in FaceStreamList)
                {
                    //Stream face = new MemoryStream(faceStream);

                    await WaitCallLimitPerSecondAsync();
                    AddPersistedFaceResult result = await faceServiceClient.AddPersonFaceAsync(person.PersonGroupId, person.PersonId, faceStream);
                    success = result.PersistedFaceId != Guid.Empty;
                    //person.PersonFaceId = new List<Guid>();
                    //person.PersonFaceId.Add(result.PersistedFaceId);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            //add face to db
            //var insertResult = faceEntity.InsertPersonFace(person);
            return success;
        }

        public async Task<VerifyResult> VerifyFace(string EnterpriseId, Stream FaceStream)
        {
            bool IsVerified = false;
            //get person details by eid             
            //from db.userinfo
            //where db.userinfo.EnterpriseID = eid
            //select new PersonItem { EnterpriseID, PersonGroupId, UserData, PersonId, PersonFaceId }
            PersonResultItem person = faceEntity.GetPerson(EnterpriseId);
            //faceServiceClient.VerifyAsync(Guid facei, ConstantsString.GroupId, personId);
            //Stream FaceStream = new MemoryStream(FaceStreamList.FirstOrDefault());
            Face[] TobeFace = await faceServiceClient.DetectAsync(FaceStream, returnFaceLandmarks:false, returnFaceAttributes:null);
            Guid FaceTobeVerify = TobeFace.FirstOrDefault().FaceId;
            VerifyResult result = await faceServiceClient.VerifyAsync(FaceTobeVerify, ConstantsString.GroupId, person.PersonId);
            IsVerified = (result.IsIdentical || result.Confidence > 0.5) ? true : false;
            return result;
        }
    }
}