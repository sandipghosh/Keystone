

namespace Keystone.Web.Controllers
{
    using Keystone.Web.Controllers.Base;
    using Keystone.Web.Data.Context;
    using Keystone.Web.Data.Implementation.Base;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Data.Interface.Base;
    using Keystone.Web.Models;
    using Keystone.Web.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Transactions;
    using System.Web.Mvc;

    public class UserAccountController : BaseController
    {
        private readonly IUserAccountDataRepository _userAccountDataRepository;
        private readonly IQueryDataRepository _userAccountQueryDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccountController" /> class.
        /// </summary>
        /// <param name="userAccountDataRepository">The user account data repository.</param>
        public UserAccountController(IUserAccountDataRepository userAccountDataRepository)
        {
            this._userAccountDataRepository = userAccountDataRepository;
            this._userAccountQueryDataRepository = new QueryDataRepository<KeystoneDBEntities>();
        }

        /// <summary>
        /// Indexes the specified userid.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult Index(int userid)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(ex);
            }
            return null;
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="identity">The identity.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult ChangePasswordExternal(int userid, string expiration)
        {
            string message = "An unspecified error has been occured. Please try again later.";
            try
            {
                long expirationTime = long.Parse(expiration);
                long currentTimestamp = CommonUtility.ConvertToTimestamp(DateTime.Now);

                UserAccountModel user = this._userAccountDataRepository
                    .GetList(x => x.UserAccountId.Equals(userid)
                    && x.StatusId.Equals((int)StatusEnum.Active)).FirstOrDefault();

                if (user != null)
                {
                    if (currentTimestamp <= expirationTime)
                    {
                        user.IsNewUser = false;
                        this._userAccountDataRepository.Update(user);
                        ViewBag.ViewType = "ChangePasswordForNewUser";
                        return View("Index", user);
                    }
                    else
                    {
                        user.StatusId = (int)StatusEnum.Inactive;
                        this._userAccountDataRepository.Update(user);
                        message = "Activation link has been expired. Please create user again.";
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(ex);
            }
            return RedirectToAction("Index", "Error", new { errorMsg = message.ToBase64Encode() });
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="referalUrl">The referal URL.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult ChangePassword(int userid, string referalUrl)
        {
            string message = "An unspecified error has been occured. Please try again later.";
            try
            {
                UserAccountModel user = this._userAccountDataRepository
                    .GetList(x => x.UserAccountId.Equals(userid)
                        && x.StatusId.Equals((int)StatusEnum.Active)).FirstOrDefault();

                if (user != null)
                {
                    ViewBag.ReferalUrl = referalUrl;
                    ViewBag.ViewType = "ChangePassword";
                    return View("Index", user);
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(ex);
            }
            return RedirectToAction("Index", "Error", new { errorMsg = message.ToBase64Encode() });
        }

        /// <summary>
        /// Shows the sign in screen.
        /// </summary>
        /// <param name="referalUrl">The referal URL.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult ShowSignInScreen(string referalUrl)
        {
            UserAccountModel user = new UserAccountModel();
            ViewBag.ReferalUrl = referalUrl;
            ViewBag.ViewType = "SignIn";
            return View("Index", user);
        }

        /// <summary>
        /// Shows the forgot password screen.
        /// </summary>
        /// <param name="referalUrl">The referal URL.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult ShowForgotPasswordScreen(string referalUrl)
        {
            UserAccountModel user = new UserAccountModel();
            ViewBag.ReferalUrl = referalUrl;
            ViewBag.ViewType = "ForgotPassword";
            return View("Index", user);
        }


        /// <summary>
        /// Forgots the password.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult ForgotPassword(string UserId)
        {
            bool status = false;
            string message = string.Empty;
            try
            {
                var user = _userAccountDataRepository.GetList(x => (x.UserId.Equals(UserId)
                    || x.EmailId.Equals(UserId)) && x.StatusId.Equals((int)StatusEnum.Active)).FirstOrDefault();

                if (user != null)
                {
                    user.IsNewUser = true;
                    user.Password = CommonUtility.GenarateRandomString().ToString();

                    this._userAccountDataRepository.Update(user);
                    SendForgetPasswordMail(user);
                    message = "You password has been rest and send to your registered email id.";
                    status = true;
                }
                else
                    message = "User ID is invalid. Please try again.";
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(UserId);
            }
            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Shows the change profile screen.
        /// </summary>
        /// <param name="referalUrl">The referal URL.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get), SignInActionValidator(),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult ShowChangeProfileScreen(string referalUrl)
        {
            try
            {
                int userAccountId = CommonUtility.GetSessionData<int>(SessionVariable.UserId);
                UserAccountModel user = this._userAccountDataRepository.Get(userAccountId);

                ViewBag.ReferalUrl = referalUrl;
                ViewBag.ViewType = "ChangeProfile";
                return View("Index", user);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(referalUrl);
            }
            return RedirectToAction("Index", "Error");
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="userAccount">The user account.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public JsonResult CreateUser(UserAccountModel userAccount)
        {
            bool isUserCreated = false;
            string message = string.Empty;
            try
            {
                if (!IsUserExists(userAccount.EmailId))
                {
                    userAccount.StatusId = (int)StatusEnum.Active;
                    userAccount.CreatedOn = DateTime.Now;
                    userAccount.IsNewUser = true;
                    userAccount.IsAdmin = false;
                    userAccount.UserId = CommonUtility.GenarateRandomString().ToUpper();
                    userAccount.Password = CommonUtility.GenarateRandomString().ToString();
                    userAccount.RowId = Guid.NewGuid().ToString("N").ToUpper();

                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew,
                        new TransactionOptions()
                        {
                            IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                            Timeout = new TimeSpan(0, 1, 0)
                        }))
                    {
                        this._userAccountDataRepository.Insert(userAccount);

                        CreateUserSpecificFolders(userAccount);

                        scope.Complete();
                    }

                    if (userAccount.UserAccountId > 0)
                    {
                        CommonUtility.SetSessionData<int>(SessionVariable.UserId, userAccount.UserAccountId);
                        CommonUtility.SetSessionData<bool>(SessionVariable.IsAdminUser, userAccount.IsAdmin);
                        CommonUtility.SetSessionData<string>(SessionVariable.UserName, userAccount.ToString());

                        SendUserRegistrationMail(userAccount);
                        isUserCreated = true;
                    }
                }
                else
                    message = "User already exists. Please sign in into your account to proceed.";
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(userAccount);
            }
            return Json(new { Status = isUserCreated, Message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="userAccount">The user account.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult UpdateUser(UserAccountModel userAccount)
        {
            bool isUserUpdated = false;
            try
            {
                userAccount.UpdatedBy = userAccount.UserAccountId;
                userAccount.UpdatedOn = DateTime.Now;

                this._userAccountDataRepository.Update(userAccount);

                CommonUtility.SetSessionData<int>(SessionVariable.UserId, userAccount.UserAccountId);
                CommonUtility.SetSessionData<bool>(SessionVariable.IsAdminUser, userAccount.IsAdmin);
                CommonUtility.SetSessionData<string>(SessionVariable.UserName, userAccount.ToString());
                isUserUpdated = true;
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(userAccount);
            }
            return Json(new { IsUserCreated = isUserUpdated }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Signs the in user.
        /// </summary>
        /// <param name="userAccount">The user account.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public JsonResult SignInUser(UserAccountModel userAccount)
        {
            bool loginStatus = false;
            string message = string.Empty;
            try
            {
                string userId = userAccount.UserId;
                string userPassword = userAccount.Password;

                var user = _userAccountDataRepository.GetList(x => (x.EmailId.Equals(userId)
                    || x.UserId.Equals(userAccount.UserId)) && x.Password.Equals(userPassword)
                    && x.StatusId.Equals((int)StatusEnum.Active)).FirstOrDefault();

                if (user != null)
                {
                    CommonUtility.SetSessionData<int>(SessionVariable.UserId, user.UserAccountId);
                    CommonUtility.SetSessionData<bool>(SessionVariable.IsAdminUser, user.IsAdmin);
                    CommonUtility.SetSessionData<string>(SessionVariable.UserName, user.ToString());
                    loginStatus = true;
                }
                else
                    message = "User ID or Password is invalid. Please try again.";
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(userAccount);
            }
            return Json(new { Status = loginStatus, Message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Signs the out user.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult SignOutUser()
        {
            try
            {
                Session.Clear();
                Session.Abandon();
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Keepalives this instance.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ContentResult Keepalive()
        { return Content("OK"); }

        /// <summary>
        /// Determines whether [is user exists] [the specified user mail id].
        /// </summary>
        /// <param name="userMailId">The user mail id.</param>
        /// <returns></returns>
        public bool IsUserExists(string userMailId)
        {
            bool isDuplicateUser = false;
            try
            {
                bool returntype = false;
                string sql = "EXEC [dbo].[IsUserAlreadtExists] @UserMailId, @existingUser OUT";
                SqlParameter mailId = new SqlParameter { ParameterName = "UserMailId", Value = userMailId };
                SqlParameter existingUser = new SqlParameter { ParameterName = "existingUser", Value = false, Direction = ParameterDirection.Output };
                existingUser.Direction = ParameterDirection.Output;

                this._userAccountQueryDataRepository.ExecuteCommand(sql, mailId, existingUser);
                return Convert.ToBoolean(existingUser.Value);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(userMailId);
            }
            return isDuplicateUser;
        }

        /// <summary>
        /// Sends the user registration mail.
        /// </summary>
        /// <param name="userAccount">The user account.</param>
        private void SendUserRegistrationMail(UserAccountModel userAccount)
        {
            try
            {
                EmailSender email = new EmailSender
                {
                    SSL = Convert.ToBoolean(ConfigurationManager.AppSettings["MAIL_SERVER_SSL"].ToString()),
                    Subject = "Successfully registered to Keystone",
                    To = userAccount.EmailId
                };

                string changePasswordUrl = string.Format("{0}{1}", Request.Url.GetLeftPart(UriPartial.Authority),
                    Url.Action("ChangePasswordExternal", "UserAccount", new
                    {
                        userid = userAccount.UserAccountId,
                        expiration = CommonUtility.ConvertToTimestamp(DateTime.Now.AddDays(1)).ToString()
                    }));

                string mailBody = Utilities.CommonUtility.RenderViewToString
                    ("_UserRegistrationMailConfirmation", userAccount,
                    new UserAccountController(this._userAccountDataRepository),
                    new Dictionary<string, object>() { { "ChangePasswordUrl", changePasswordUrl } });

                email.SendMailAsync(mailBody);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(userAccount);
            }
        }

        /// <summary>
        /// Sends the forget password mail.
        /// </summary>
        /// <param name="userAccount">The user account.</param>
        private void SendForgetPasswordMail(UserAccountModel userAccount)
        {
            try
            {
                EmailSender email = new EmailSender
                {
                    SSL = Convert.ToBoolean(ConfigurationManager.AppSettings["MAIL_SERVER_SSL"].ToString()),
                    Subject = "Reset Password",
                    To = userAccount.EmailId
                };

                string changePasswordUrl = string.Format("{0}{1}", Request.Url.GetLeftPart(UriPartial.Authority),
                    Url.Action("ChangePasswordExternal", "UserAccount", new
                    {
                        userid = userAccount.UserAccountId,
                        expiration = CommonUtility.ConvertToTimestamp(DateTime.Now.AddDays(1)).ToString()
                    }));

                string mailBody = Utilities.CommonUtility.RenderViewToString
                    ("_ForgetPasswordMailConfirmation", userAccount,
                    new UserAccountController(this._userAccountDataRepository),
                    new Dictionary<string, object>() { { "ChangePasswordUrl", changePasswordUrl } });

                email.SendMailAsync(mailBody);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(userAccount);
            }
        }

        /// <summary>
        /// Creates the user specific folders.
        /// </summary>
        /// <param name="userAccount">The user account.</param>
        private void CreateUserSpecificFolders(UserAccountModel userAccount)
        {
            try
            {
                string userImageUploadPath = Server.MapPath(string.Format("~/{0}/{1}/",
                        ConfigurationManager.AppSettings["UploadFolder"].ToString(), userAccount.UserAccountId));

                string userDraftImagePath = Server.MapPath(string.Format("~/{0}/{1}/",
                    ConfigurationManager.AppSettings["DraftFolder"].ToString(), userAccount.UserAccountId));

                string userFinalImagePath = Server.MapPath(string.Format("~/{0}/{1}/",
                    ConfigurationManager.AppSettings["FinalFolder"].ToString(), userAccount.UserAccountId));

                string userOrderAttachmentPath = Server.MapPath(string.Format("~/{0}/{1}/",
                    ConfigurationManager.AppSettings["OrderAttachmentsFolder"].ToString(), userAccount.UserAccountId));

                if (!Directory.Exists(userImageUploadPath))
                    Directory.CreateDirectory(userImageUploadPath);

                if (!Directory.Exists(userDraftImagePath))
                    Directory.CreateDirectory(userDraftImagePath);

                if (!Directory.Exists(userFinalImagePath))
                    Directory.CreateDirectory(userFinalImagePath);

                if (!Directory.Exists(userOrderAttachmentPath))
                    Directory.CreateDirectory(userOrderAttachmentPath);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(userAccount);
            }
        }
    }
}
