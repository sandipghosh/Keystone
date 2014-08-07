
namespace Keystone.Web.Areas.Admin.Controllers
{
    using Keystone.Web.Controllers.Base;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Models;
    using Keystone.Web.Models.Base;
    using Keystone.Web.Utilities;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    [SignInActionValidator(true)]
    public class CustomerManagerController : BaseController
    {
        private readonly IUserAccountDataRepository _userAccountDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerManagerController"/> class.
        /// </summary>
        /// <param name="userAccountDataRepository">The user account data repository.</param>
        public CustomerManagerController(IUserAccountDataRepository userAccountDataRepository)
        {
            this._userAccountDataRepository = userAccountDataRepository;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult Index()
        {
            ViewBag.SchemaData = GetCustomerModelSchema();
            return View();
        }

        /// <summary>
        /// Gets the customer.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult GetCustomer()
        {
            try
            {
                GridSearchDataModel requestSearchData = new GridSearchDataModel();
                requestSearchData.SetPropertiesFromContext<GridSearchDataModel>(System.Web.HttpContext.Current);

                if (requestSearchData != null)
                {
                    List<UserAccountModel> users = new List<UserAccountModel>();
                    int totalRecords = 0;

                    if (requestSearchData._search)
                    {
                        string criteria = requestSearchData.filters.ToString();
                        var searchCriteria = CommonUtility.GetLamdaExpressionFromFilter<UserAccountModel>(criteria);
                        totalRecords = this._userAccountDataRepository.GetCount(searchCriteria);

                        users = this._userAccountDataRepository
                            .GetList(requestSearchData.page, requestSearchData.rows, searchCriteria,
                            x => x.CreatedOn, false).ToList();
                    }
                    else
                    {
                        totalRecords = this._userAccountDataRepository.GetCount(x => x.StatusId.Equals((int)StatusEnum.Active));
                        users = this._userAccountDataRepository
                            .GetList(requestSearchData.page, requestSearchData.rows, x => x.StatusId.Equals((int)StatusEnum.Active),
                            x => x.CreatedOn, false).ToList();
                    }

                    return new JSONActionResult(new GridDataModel()
                    {
                        currpage = requestSearchData.page,
                        totalpages = (int)Math.Ceiling((float)totalRecords / (float)requestSearchData.rows),
                        totalrecords = totalRecords,
                        invdata = users.Select(x => new
                        {
                            x.UserAccountId,
                            x.UserId,
                            x.FirstName,
                            x.LastName,
                            x.EmailId,
                            x.PrimaryContact,
                            x.Address1,
                            x.Address2,
                            x.State,
                            x.City,
                            x.Pin,
                            x.Country,
                            Status = ((StatusEnum)x.StatusId).ToString()
                        })
                    });
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
            return RedirectToAction("Index", "Error", new { area = "" });
        }

        /// <summary>
        /// Sets the customer.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult SetCustomer()
        {
            try
            {
                if (HttpContext.Request["oper"].ToString() == "edit")
                {
                    UserAccountModel customer = new UserAccountModel();

                    if (HttpContext.Request["GridMode"].ToString() == "insert")
                    {
                        customer.SetPropertiesFromContext<UserAccountModel>(System.Web.HttpContext.Current);
                        customer.StatusId = (int)StatusEnum.Active;
                        customer.CreatedBy = CommonUtility.GetSessionData<int>(SessionVariable.UserId);
                        customer.CreatedOn = DateTime.Now;
                        this._userAccountDataRepository.Insert(customer);

                        if (customer.UserAccountId > 0)
                        {
                            return Json(true);
                        }
                    }
                    if (HttpContext.Request["GridMode"].ToString() == "edit")
                    {
                        customer.SetPropertiesFromContext(System.Web.HttpContext.Current);
                        UserAccountModel existingCustomer = this._userAccountDataRepository
                            .Get(customer.UserAccountId);

                        existingCustomer.FirstName = customer.FirstName;
                        existingCustomer.LastName = customer.LastName;
                        existingCustomer.EmailId = customer.EmailId;
                        existingCustomer.PrimaryContact = customer.PrimaryContact;
                        existingCustomer.Address1 = customer.Address1;
                        existingCustomer.Address2 = customer.Address2;
                        existingCustomer.State = customer.State;
                        existingCustomer.City = customer.City;
                        existingCustomer.Pin = customer.Pin;
                        existingCustomer.Country = customer.Country;
                        existingCustomer.StatusId = customer.StatusId;
                        existingCustomer.UpdatedBy = CommonUtility.GetSessionData<int>(SessionVariable.UserId);
                        existingCustomer.UpdatedOn = DateTime.Now;
                        this._userAccountDataRepository.Update(existingCustomer);
                        return Json(true);
                    }
                    if (HttpContext.Request["GridMode"].ToString() == "delete") { }
                }
                else
                {

                }

            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
            return Json(false);
        }

        /// <summary>
        /// Exports the specified criteria.
        /// </summary>
        /// <param name="filters">The criteria.</param>
        /// <returns></returns>
        public FileResult Export(string filters)
        {
            try
            {
                GridSearchDataModel requestSearchData = new GridSearchDataModel();
                requestSearchData.SetPropertiesFromContext<GridSearchDataModel>(System.Web.HttpContext.Current);
                if (requestSearchData != null)
                {
                    filters = requestSearchData.filters == null ? "" : requestSearchData.filters.ToString();
                }

                string fileName = CommonUtility.GenarateRandomString(20, 20) + ".csv";

                CommonFuntionality.ExportToCSV<UserAccountModel, object>
                (_userAccountDataRepository.GetList,
                x => x.CreatedOn, false, x => new
                {
                    x.UserId,
                    x.FirstName,
                    x.LastName,
                    x.EmailId,
                    x.PrimaryContact,
                    x.Address1,
                    x.Address2,
                    x.State,
                    x.City,
                    x.Pin,
                    x.Country
                },
                filters ?? "", "Airport_Search_List", fileName);

                //string fileName = "Airport_List_Export" + "_" + DateTime.Now.ToString("dd-MMMM-yyyy") + ".csv";
                string fullPath = System.IO.Path.Combine(Server.MapPath(string
                    .Format("~/{0}", CommonUtility.GetAppSetting<string>("ExportStorageFolder"))), fileName);
                byte[] fileStream = System.IO.File.ReadAllBytes(fullPath);
                System.IO.File.Delete(fullPath);
                return File(fileStream, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
            return null;
        }

        /// <summary>
        /// Gets the customer model schema.
        /// </summary>
        /// <returns></returns>
        private string GetCustomerModelSchema()
        {
            string strSchema = string.Empty;
            GridModelSchema gridModelSchema = null;
            try
            {
                List<colModel> columnModel = new List<colModel>();
                var status = Enum.GetValues(typeof(StatusEnum)).OfType<StatusEnum>()
                    .Select(x => new { Key = (int)x, Value = x.ToString() });

                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((UserAccountModel x) => x.UserAccountId),
                    editable = true,
                    hidden = true,
                    edittype = Edittype.custom.ToString(),
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((UserAccountModel x) => x.UserId),
                    edittype = Edittype.textarea.ToString(),
                    editrules = new editrules { required = true },
                    searchoptions = new SearchOptions
                    {
                        sopt = new string[] 
                        {
                            SearchOperator.eq.ToString(), 
                            SearchOperator.ne.ToString(),
                            SearchOperator.bw.ToString(),
                            SearchOperator.bn.ToString(),
                            SearchOperator.cn.ToString(),
                            SearchOperator.nc.ToString(),
                            SearchOperator.ew.ToString(),
                            SearchOperator.en.ToString(),
                        }
                    }
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((UserAccountModel x) => x.FirstName),
                    width = 50,
                    editable = true,
                    edittype = Edittype.text.ToString(),
                    editrules = new editrules { required = true },
                    searchoptions = new SearchOptions
                    {
                        sopt = new string[] 
                        {
                            SearchOperator.eq.ToString(), 
                            SearchOperator.ne.ToString(),
                            SearchOperator.bw.ToString(),
                            SearchOperator.bn.ToString(),
                            SearchOperator.cn.ToString(),
                            SearchOperator.nc.ToString(),
                            SearchOperator.ew.ToString(),
                            SearchOperator.en.ToString(),
                        }
                    }
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((UserAccountModel x) => x.LastName),
                    width = 50,
                    editable = true,
                    edittype = Edittype.text.ToString(),
                    editrules = new editrules { required = true },
                    searchoptions = new SearchOptions
                    {
                        sopt = new string[] 
                        {
                            SearchOperator.eq.ToString(), 
                            SearchOperator.ne.ToString(),
                            SearchOperator.bw.ToString(),
                            SearchOperator.bn.ToString(),
                            SearchOperator.cn.ToString(),
                            SearchOperator.nc.ToString(),
                            SearchOperator.ew.ToString(),
                            SearchOperator.en.ToString(),
                        }
                    }
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((UserAccountModel x) => x.EmailId),
                    width = 50,
                    editable = true,
                    edittype = Edittype.text.ToString(),
                    editrules = new editrules { required = true, email = true },
                    searchoptions = new SearchOptions
                    {
                        sopt = new string[] 
                        {
                            SearchOperator.eq.ToString(), 
                            SearchOperator.ne.ToString(),
                            SearchOperator.bw.ToString(),
                            SearchOperator.bn.ToString(),
                            SearchOperator.cn.ToString(),
                            SearchOperator.nc.ToString(),
                            SearchOperator.ew.ToString(),
                            SearchOperator.en.ToString(),
                        }
                    }
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((UserAccountModel x) => x.PrimaryContact),
                    width = 50,
                    editable = true,
                    edittype = Edittype.text.ToString(),
                    editrules = new editrules { required = false },
                    searchoptions = new SearchOptions
                    {
                        sopt = new string[] 
                        {
                            SearchOperator.eq.ToString(), 
                            SearchOperator.ne.ToString(),
                            SearchOperator.bw.ToString(),
                            SearchOperator.bn.ToString(),
                            SearchOperator.cn.ToString(),
                            SearchOperator.nc.ToString(),
                            SearchOperator.ew.ToString(),
                            SearchOperator.en.ToString(),
                        }
                    }
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((UserAccountModel x) => x.Address1),
                    width = 50,
                    editable = true,
                    edittype = Edittype.textarea.ToString(),
                    editrules = new editrules { required = true },
                    searchoptions = new SearchOptions
                    {
                        sopt = new string[] 
                        {
                            SearchOperator.eq.ToString(), 
                            SearchOperator.ne.ToString(),
                            SearchOperator.bw.ToString(),
                            SearchOperator.bn.ToString(),
                            SearchOperator.cn.ToString(),
                            SearchOperator.nc.ToString(),
                            SearchOperator.ew.ToString(),
                            SearchOperator.en.ToString(),
                        }
                    }
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((UserAccountModel x) => x.Address2),
                    width = 50,
                    editable = true,
                    edittype = Edittype.textarea.ToString(),
                    editrules = new editrules { required = false },
                    searchoptions = new SearchOptions
                    {
                        sopt = new string[] 
                        {
                            SearchOperator.eq.ToString(), 
                            SearchOperator.ne.ToString(),
                            SearchOperator.bw.ToString(),
                            SearchOperator.bn.ToString(),
                            SearchOperator.cn.ToString(),
                            SearchOperator.nc.ToString(),
                            SearchOperator.ew.ToString(),
                            SearchOperator.en.ToString(),
                        }
                    }
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((UserAccountModel x) => x.State),
                    width = 50,
                    editable = true,
                    edittype = Edittype.text.ToString(),
                    editrules = new editrules { required = true },
                    searchoptions = new SearchOptions
                    {
                        sopt = new string[] 
                        {
                            SearchOperator.eq.ToString(), 
                            SearchOperator.ne.ToString(),
                            SearchOperator.bw.ToString(),
                            SearchOperator.bn.ToString(),
                            SearchOperator.cn.ToString(),
                            SearchOperator.nc.ToString(),
                            SearchOperator.ew.ToString(),
                            SearchOperator.en.ToString(),
                        }
                    }
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((UserAccountModel x) => x.City),
                    width = 50,
                    editable = true,
                    edittype = Edittype.text.ToString(),
                    editrules = new editrules { required = true },
                    searchoptions = new SearchOptions
                    {
                        sopt = new string[] 
                        {
                            SearchOperator.eq.ToString(), 
                            SearchOperator.ne.ToString(),
                            SearchOperator.bw.ToString(),
                            SearchOperator.bn.ToString(),
                            SearchOperator.cn.ToString(),
                            SearchOperator.nc.ToString(),
                            SearchOperator.ew.ToString(),
                            SearchOperator.en.ToString(),
                        }
                    }
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((UserAccountModel x) => x.Pin),
                    label = "Zip Code",
                    editable = true,
                    width = 50,
                    edittype = Edittype.text.ToString(),
                    editrules = new editrules { required = true, number = true },
                    searchoptions = new SearchOptions
                    {
                        sopt = new string[] 
                        {
                            SearchOperator.eq.ToString(), 
                            SearchOperator.ne.ToString(),
                            SearchOperator.bw.ToString(),
                            SearchOperator.bn.ToString(),
                            SearchOperator.cn.ToString(),
                            SearchOperator.nc.ToString(),
                            SearchOperator.ew.ToString(),
                            SearchOperator.en.ToString(),
                        }
                    }
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((UserAccountModel x) => x.Country),
                    editable = true,
                    width = 50,
                    edittype = Edittype.text.ToString(),
                    editrules = new editrules { required = true },
                    searchoptions = new SearchOptions
                    {
                        sopt = new string[] 
                        {
                            SearchOperator.eq.ToString(), 
                            SearchOperator.ne.ToString(),
                            SearchOperator.bw.ToString(),
                            SearchOperator.bn.ToString(),
                            SearchOperator.cn.ToString(),
                            SearchOperator.nc.ToString(),
                            SearchOperator.ew.ToString(),
                            SearchOperator.en.ToString(),
                        }
                    }
                });

                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((UserAccountModel x) => x.StatusId),
                    editable = true,
                    hidden = true,
                    edittype = Edittype.custom.ToString(),
                });
                columnModel.Add(new colModel()
                {
                    name = "Status",
                    editable = true,
                    width = 50,
                    targetFieldName = CommonUtility.GetDisplayName((UserAccountModel x) => x.StatusId),
                    edittype = Edittype.select.ToString(),
                    align = Align.center.ToString(),
                    editrules = new editrules { required = true },
                    editoptions = new editoptions { value = CommonUtility.GetGridDropdownData(status, "Key", "Value") },
                    searchoptions = new SearchOptions
                    {
                        sopt = new string[] 
                        {
                            SearchOperator.eq.ToString(), 
                            SearchOperator.ne.ToString(),
                            SearchOperator.bw.ToString(),
                            SearchOperator.bn.ToString(),
                            SearchOperator.cn.ToString(),
                            SearchOperator.nc.ToString(),
                            SearchOperator.ew.ToString(),
                            SearchOperator.en.ToString(),
                        }
                    }
                });
                columnModel.Add(new colModel()
                {
                    name = "Actions",
                    index = "act",
                    width = 30,
                    sortable = false,
                    search = false
                });

                gridModelSchema = new GridModelSchema(columnModel);
                return JsonConvert.SerializeObject(gridModelSchema, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                }).ToBase64Encode();
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
            return strSchema;
        }
    }
}
