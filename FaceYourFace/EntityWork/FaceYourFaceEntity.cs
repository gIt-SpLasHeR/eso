using FaceYourFace.Constants;
using FaceYourFace.DTO;
using FaceYourFace.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace FaceYourFace.EntityWork
{
    public class FaceYourFaceEntity
    {
        private FaceYourFaceContext db = new FaceYourFaceContext();
        public PersonResultItem GetPerson(string EnterpriseId)
        {
            PersonResultItem Person = new PersonResultItem();
            var Query = from person in db.Persons
                        where person.EnterpriseID == EnterpriseId
                        select new PersonResultItem
                        {
                            EnterpriseID = person.EnterpriseID,
                            UserData = person.PersonUserData,
                            PersonGroupId = person.PersonGroupID,
                            PersonId = (Guid)person.PersonID
                        };
            Person = Query.Count() > 0 ? Query.FirstOrDefault() : Person;
            return Person;
        }

        public string InsertPerson(PersonResultItem person)
        {
            string result = "0";
            Person personFace = new Person
            {
                EnterpriseID = person.EnterpriseID,
                PersonGroupID = person.PersonGroupId,
                PersonID = person.PersonId,
                PersonUserData = String.Empty
            };
            db.Persons.Add(personFace);
            try
            {
                db.SaveChanges();
                result = "1";
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }
            return result;
        }
    }
}