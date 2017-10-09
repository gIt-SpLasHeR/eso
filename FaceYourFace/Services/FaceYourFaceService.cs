using FaceYourFace.Constants;
using FaceYourFace.DTO;
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
        const int PersonCount = 100;
        const int CallLimitPerSecond = 1;
        static Queue<DateTime> _timeStampQueue = new Queue<DateTime>(CallLimitPerSecond);
        FaceServiceClient faceServiceClient = new FaceServiceClient(ConstantsString.Key, ConstantsString.EndPoints);// apiroot means endpoints
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

        public async Task<bool> AddPerson(string GroupID, string eid, List<Stream> FaceStreamList)
        {
            bool success = false;
            PersonResultItem person = new PersonResultItem { EnterpriseID = eid, PersonGroupId = GroupID };

            try
            {
                var personList = await faceServiceClient.ListPersonsAsync(ConstantsString.GroupId);
                var responsePerson = personList.FirstOrDefault(t => t.Name == eid);
                if (responsePerson != null)
                {
                    person.PersonId = responsePerson.PersonId;
                }
                else
                {
                    CreatePersonResult result = await faceServiceClient.CreatePersonAsync(person.PersonGroupId, person.EnterpriseID);
                    person.PersonId = result.PersonId;
                }

                if (person.PersonId != Guid.Empty)
                {
                    foreach (var faceStream in FaceStreamList)
                    {
                        await WaitCallLimitPerSecondAsync();
                        AddPersistedFaceResult result = await faceServiceClient.AddPersonFaceAsync(ConstantsString.GroupId, person.PersonId, faceStream);
                        success = result.PersistedFaceId != Guid.Empty;
                    }
                }
                else
                {
                    success = false;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return success;
        }

        public async Task<VerifyResult> VerifyFace(string EnterpriseId, Stream FaceStream)
        {
            bool IsVerified = false;
            VerifyResult result = new VerifyResult { Confidence = 0.0, IsIdentical = false };
            PersonResultItem person = new PersonResultItem
            {
                EnterpriseID = EnterpriseId,
                PersonGroupId = ConstantsString.GroupId,
                // will assign new guid from service.
                PersonId = Guid.Empty
            };
            var personList = await faceServiceClient.ListPersonsAsync(ConstantsString.GroupId);
            var responsePerson = personList.FirstOrDefault(t => t.Name == EnterpriseId);
            if (responsePerson != null)
            {
                person.PersonId = responsePerson.PersonId;
            }

            Face[] TobeFace = await faceServiceClient.DetectAsync(FaceStream, returnFaceLandmarks: false, returnFaceAttributes: null);
            if ( TobeFace != null && TobeFace.Count() > 0)
            {
                Guid FaceTobeVerify = TobeFace.FirstOrDefault().FaceId;
                result = await faceServiceClient.VerifyAsync(FaceTobeVerify, ConstantsString.GroupId, person.PersonId);
                IsVerified = (result.IsIdentical || result.Confidence > 0.5) ? true : false; 
            }
            return result;
        }

        public async Task<bool> CheckPersonExist(string EnterpriseId)
        {
            try
            {
                Person[] PersonList = await faceServiceClient.ListPersonsAsync(ConstantsString.GroupId);
                bool exist = PersonList.Any(t => t.Name == EnterpriseId);
                return exist;
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
    }
}