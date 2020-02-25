using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.WebApi;
using Umbraco.Web.Models;
using Umbraco.Core.Models;
using System.Net.Http;
using System.Net;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;
using System.Web.Http;
using Umbraco.Core;
using System.Web.Http.Cors;

namespace UmbracCMSBackend.Controllers
{
    [EnableCors(origins: "http://localhost:3000/", headers: "*", methods: "*")]
    public class EndUserController : UmbracoApiController
    {
        public EndUserController() { }
        // GET: EndUsers
        public HttpResponseMessage GetEndUsers()
        {
            try
            {
                var result = Umbraco.ContentAtRoot().
                    FirstOrDefault().
                    Children.
                    ToList().
                    Where(content => content.UrlSegment.Equals("end-users-listed")).
                    ToList().
                    FirstOrDefault()
                    .Children
                    .Select(x =>
                    new EndUser
                    {
                        Id = x.Id,
                        Name = x.Name,
                        PhoneNumber = x.GetProperty("PhoneNumber").GetValue().ToString(),
                        EmailAddress = x.GetProperty("EmailAddress").GetValue().ToString(),
                        StartDate = DateTime.Parse(x.GetProperty("StartDate").GetValue().ToString()),
                        EndDate = DateTime.Parse(x.GetProperty("EndDate").GetValue().ToString())
                    }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed);
            }
        }
        // POST: EndUser
        public HttpResponseMessage PostEndUser([FromBody] EndUser endUser) //
        {
            try
            {
                var endUsersListContainer = endUsersList();
                var numberOfEndUsers = endUsersList().Children.Count();
                IContentService contentService = Services.ContentService;
                GuidUdi endUserUdi = new GuidUdi(endUsersListContainer.ContentType.ItemType.ToString(), endUsersListContainer.Key);
                IContent newEndUserToAdd = contentService.CreateContent(endUser.Name, endUserUdi, "endUser", numberOfEndUsers++);
                newEndUserToAdd = SaveAndPublishEndUserRecord(newEndUserToAdd, endUser, contentService);
                return Request.CreateResponse(HttpStatusCode.Created, newEndUserToAdd);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, message);
            }
        }
        // GET: EndUserById
        public HttpResponseMessage GetEndUserById(int id)
        {
            try
            {
                var result = Umbraco.ContentAtRoot().
                    FirstOrDefault().
                    Children.
                    ToList().
                    Where(content => content.UrlSegment.Equals("end-users-listed")).
                    ToList().
                    FirstOrDefault()
                    .Children
                    .Select(x =>
                    new EndUser
                    {
                        Id = x.Id,
                        Name = x.Name,
                        PhoneNumber = x.GetProperty("PhoneNumber").GetValue().ToString(),
                        EmailAddress = x.GetProperty("EmailAddress").GetValue().ToString(),
                        StartDate = DateTime.Parse(x.GetProperty("StartDate").GetValue().ToString()),
                        EndDate = DateTime.Parse(x.GetProperty("EndDate").GetValue().ToString())
                    })
                    .Where(user => user.Id == id)
                    .FirstOrDefault();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed);
            }
        }
        private IContent SaveAndPublishEndUserRecord(IContent newEndUserToAdd, EndUser endUser, IContentService contentService)
        {
            newEndUserToAdd.SetValue("userName", endUser.Name);
            newEndUserToAdd.SetValue("emailAddress", endUser.EmailAddress);
            newEndUserToAdd.SetValue("phoneNumber", endUser.PhoneNumber);
            newEndUserToAdd.SetValue("startDate", endUser.StartDate);
            newEndUserToAdd.SetValue("endDate", endUser.EndDate);
            contentService.SaveAndPublish(newEndUserToAdd);
            return newEndUserToAdd;
        }
        private IPublishedContent endUsersList(string aliasName = "end-users-listed")
        {
            return Umbraco.ContentAtRoot().
                    FirstOrDefault().
                    Children.
                    ToList().
                    Where(content => content.UrlSegment.Equals(aliasName))
                    .FirstOrDefault();
        }
        public class EndUser
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string EmailAddress { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
    }
}