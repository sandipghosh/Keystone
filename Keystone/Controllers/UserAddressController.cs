
namespace Keystone.Web.Controllers
{
    using Keystone.Web.Controllers.Base;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Models;
    using Keystone.Web.Utilities;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Transactions;
    using System.Web.Mvc;

    [SignInActionValidator()]
    public class UserAddressController : BaseController
    {
        private readonly IUserAccountDataRepository _userAccountDataRepository;
        private readonly IUserAddressDataRepository _userAddressDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAddressController"/> class.
        /// </summary>
        /// <param name="userAccountDataRepository">The user account data repository.</param>
        /// <param name="userAddressDataRepository">The user address data repository.</param>
        public UserAddressController(IUserAccountDataRepository userAccountDataRepository,
            IUserAddressDataRepository userAddressDataRepository)
        {
            this._userAccountDataRepository = userAccountDataRepository;
            this._userAddressDataRepository = userAddressDataRepository;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Saves the user address.
        /// </summary>
        /// <param name="userAddress">The user address.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public JsonResult SaveUserAddress(UserAddressModel userAddress)
        {
            bool status = false; string message = string.Empty;
            try
            {
                bool isSameAddressForBoth = Convert.ToBoolean(HttpContext.Request
                    .Form["IsSameAddressForBoth"].ToString().Split(',').FirstOrDefault());

                int userId = CommonUtility.GetSessionData<int>(SessionVariable.UserId);

                TransactionOptions options = new TransactionOptions()
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                    Timeout = new TimeSpan(0, 1, 0)
                };
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, options))
                {
                    if (userAddress.UserAddressId == 0)
                    {
                        userAddress.StatusId = (int)StatusEnum.Active;
                        userAddress.CreatedBy = userId;
                        userAddress.CreatedOn = DateTime.Now;
                        this._userAddressDataRepository.Insert(userAddress);
                    }
                    else
                    {
                        userAddress.UpdatedBy = userId;
                        userAddress.UpdatedOn = DateTime.Now;
                        this._userAddressDataRepository.Update(userAddress);
                    }

                    if (isSameAddressForBoth)
                    {
                        UserAddressModel oppAddress = (UserAddressModel)userAddress.Clone();
                        oppAddress.UserAddressId = 0;
                        userAddress.AddressTypeId = Convert.ToInt32(HttpContext.Request
                            .Form["AlternateAddressTypeId"].ToString());
                        userAddress.UpdatedBy = null;
                        userAddress.UpdatedOn = null;
                        this._userAddressDataRepository.Insert(userAddress);
                    }
                    scope.Complete();
                }
                status = true;
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(userAddress);
                message = "Unable to save address. An unconditional error has been occured.";
            }
            return Json(new { Status = status, Message = message });
        }

        /// <summary>
        /// Gets the change address popup.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult GetChangeAddressPopup(string type)
        {
            try
            {
                int? userId = CommonUtility.GetSessionData<int?>(SessionVariable.UserId);
                if (userId.HasValue)
                {
                    CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                    TextInfo textInfo = cultureInfo.TextInfo;

                    UserAddressModel userAddress = null;
                    var user = this._userAccountDataRepository.Get(userId.Value);

                    if (user != null)
                    {
                        userAddress = user.GetAddress((AddressTypeEnum)Enum.Parse(typeof(AddressTypeEnum), type));

                        if (userAddress == null)
                            userAddress = new UserAddressModel { UaserAccountId = userId.Value };
                    }

                    ViewBag.AddressTypeId = (int)Enum.Parse(typeof(AddressTypeEnum), type);
                    ViewBag.DialogTitle = textInfo.ToTitleCase(string.Format("{0} address", type));

                    return PartialView("_Address", userAddress);
                }
                else
                { return PartialView("_ResponseAlert", string.Format("Please sign in to change the {0} address. Access denied", type)); }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(type);
            }
            return null;
        }
    }
}
