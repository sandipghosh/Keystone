
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
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;

    [SignInActionValidator(true)]
    public class PaymentManagerController : BaseController
    {
        private readonly IPaymentInfoDataRepository _paymentInfoDataRepository;
        private readonly IOrderDataRepository _orderDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentManagerController"/> class.
        /// </summary>
        /// <param name="paymentInfoDataRepository">The payment information data repository.</param>
        /// <param name="orderDataRepository">The order data repository.</param>
        public PaymentManagerController(IPaymentInfoDataRepository paymentInfoDataRepository,
            IOrderDataRepository orderDataRepository)
        {
            this._paymentInfoDataRepository = paymentInfoDataRepository;
            this._orderDataRepository = orderDataRepository;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult Index()
        {
            ViewBag.SchemaData = GetPaymentInfoModelSchema();
            return View();
        }

        /// <summary>
        /// Gets the payment infos.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult GetPaymentInfos()
        {
            try
            {
                GridSearchDataModel requestSearchData = new GridSearchDataModel();
                requestSearchData.SetPropertiesFromContext<GridSearchDataModel>(System.Web.HttpContext.Current);

                if (requestSearchData != null)
                {
                    List<PaymentInfoModel> payments = new List<PaymentInfoModel>();
                    int totalRecords = 0;

                    if (requestSearchData._search)
                    {
                        string criteria = requestSearchData.filters.ToString();
                        var searchCriteria = CommonUtility.GetLamdaExpressionFromFilter<PaymentInfoModel>(criteria);
                        totalRecords = this._paymentInfoDataRepository.GetCount(searchCriteria);

                        payments = this._paymentInfoDataRepository
                            .GetList(requestSearchData.page, requestSearchData.rows, searchCriteria,
                            x => x.CreatedOn, false).ToList();
                    }
                    else
                    {
                        totalRecords = this._paymentInfoDataRepository.GetCount(x => x.StatusId.Equals((int)StatusEnum.Active));
                        payments = this._paymentInfoDataRepository
                            .GetList(requestSearchData.page, requestSearchData.rows, x => x.StatusId.Equals((int)StatusEnum.Active),
                            x => x.CreatedOn, false).ToList();
                    }

                    return new JSONActionResult(new GridDataModel()
                    {
                        currpage = requestSearchData.page,
                        totalpages = (int)Math.Ceiling((float)totalRecords / (float)requestSearchData.rows),
                        totalrecords = totalRecords,
                        invdata = payments.Select(x => new
                        {
                            x.PaymentInfoId,
                            x.PaymentMonth,
                            x.Order.OrderReferance,
                            x.TransactionTime,
                            x.Acknowledgement,
                            x.TransactionType,
                            x.TransactionErrorCode,
                            x.TransactionShortMessage,
                            x.TransactionAmount,
                            x.StatusId,
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
        /// Exports the specified filters.
        /// </summary>
        /// <param name="filters">The filters.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
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

                CommonFuntionality.ExportToCSV<PaymentInfoModel, object>
                (this._paymentInfoDataRepository.GetList,
                x => x.CreatedOn, false, x => new
                {
                    x.Order.OrderReferance,
                    x.TransactionTime,
                    x.Acknowledgement,
                    x.TransactionType,
                    x.TransactionErrorCode,
                    x.TransactionShortMessage,
                    x.TransactionAmount
                },
                filters ?? "", "Airport_Search_List", fileName);

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
        /// Gets the payment information model schema.
        /// </summary>
        /// <returns></returns>
        private string GetPaymentInfoModelSchema()
        {
            string strSchema = string.Empty;
            GridModelSchema gridModelSchema = null;
            try
            {
                List<colModel> columnModel = new List<colModel>();

                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((PaymentInfoModel x) => x.PaymentInfoId),
                    editable = true,
                    hidden = true,
                    edittype = Edittype.custom.ToString(),
                });
                columnModel.Add(new colModel()
                {
                    name = "PaymentMonth",
                    //formatter = "fn:MonthFormatter"
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((PaymentInfoModel x) => x.Order.OrderReferance),
                    summaryType = SummaryType.count.ToString(),
                    summaryTpl = "Total payments ({0})",
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
                    name = CommonUtility.GetDisplayName((PaymentInfoModel x) => x.TransactionTime),
                    label = "Transaction Date",
                    align = Align.center.ToString(),
                    width = 80,
                    formatoptions = new formatoptions { newformat = "m-d-Y" },
                    formatter = "date",
                    searchoptions = new SearchOptions
                    {
                        sopt = new string[] 
                        {
                            SearchOperator.eq.ToString(), 
                            SearchOperator.ne.ToString(),
                            SearchOperator.le.ToString(),
                            SearchOperator.lt.ToString(),
                            SearchOperator.gt.ToString(),
                            SearchOperator.ge.ToString()
                        }
                    }
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((PaymentInfoModel x) => x.Acknowledgement),
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
                    name = CommonUtility.GetDisplayName((PaymentInfoModel x) => x.TransactionType),
                    width = 70,
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
                    name = CommonUtility.GetDisplayName((PaymentInfoModel x) => x.TransactionErrorCode),
                    align = Align.center.ToString(),
                    width = 70,
                    label = "Error Code",
                    search = false
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((PaymentInfoModel x) => x.TransactionShortMessage),
                    label = "Error Message",
                    search = false
                });

                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((PaymentInfoModel x) => x.TransactionAmount),
                    label="Transaction Amt.",
                    align = Align.right.ToString(),
                    width = 100,
                    formatter = Formatter.currency.ToString(),
                    formatoptions = new formatoptions
                    {
                        decimalSeparator = ".",
                        thousandsSeparator = ",",
                        decimalPlaces = 2,
                        prefix = "$"
                    },
                    searchoptions = new SearchOptions
                    {
                        sopt = new string[] 
                        {
                            SearchOperator.eq.ToString(), 
                            SearchOperator.ne.ToString(),
                            SearchOperator.le.ToString(),
                            SearchOperator.lt.ToString(),
                            SearchOperator.gt.ToString(),
                            SearchOperator.ge.ToString()
                        }
                    },
                    summaryType = SummaryType.sum.ToString()
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((PaymentInfoModel x) => x.StatusId),
                    editable = true,
                    hidden = true,
                    edittype = Edittype.custom.ToString(),
                });
                gridModelSchema = new GridModelSchema()
                {
                    colModel = columnModel,
                    groupingView = new groupingView
                    {
                        groupField = new string[] { "PaymentMonth" },
                        groupColumnShow = new bool[] { false },
                        groupText = new string[] { "<b>{0}</b>" },
                        groupCollapse = false,
                        groupOrder = new string[] { "asc" },
                        groupSummary = new bool[] { true }
                    }
                };
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
