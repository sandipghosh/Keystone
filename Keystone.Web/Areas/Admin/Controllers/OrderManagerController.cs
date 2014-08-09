
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
    public class OrderManagerController : BaseController
    {
        private readonly IOrderDataRepository _orderDataRepository;
        private readonly IOrderItemDataRepository _orderItemDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderManagerController"/> class.
        /// </summary>
        /// <param name="orderDataRepository">The order data repository.</param>
        public OrderManagerController(IOrderDataRepository orderDataRepository,
            IOrderItemDataRepository orderItemDataRepository)
        {
            this._orderDataRepository = orderDataRepository;
            this._orderItemDataRepository = orderItemDataRepository;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult Index()
        {
            ViewBag.SchemaData = GetOrderModelSchema();
            ViewBag.SchemaSubData = GetOrderItemModelSchema();
            return View();
        }

        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult GetOrders()
        {
            try
            {
                GridSearchDataModel requestSearchData = new GridSearchDataModel();
                requestSearchData.SetPropertiesFromContext<GridSearchDataModel>(System.Web.HttpContext.Current);

                if (requestSearchData != null)
                {
                    List<OrderModel> orders = new List<OrderModel>();
                    int totalRecords = 0;

                    if (requestSearchData._search)
                    {
                        string criteria = requestSearchData.filters.ToString();
                        var searchCriteria = CommonUtility.GetLamdaExpressionFromFilter<OrderModel>(criteria);
                        totalRecords = this._orderDataRepository.GetCount(searchCriteria);

                        orders = this._orderDataRepository
                            .GetList(requestSearchData.page, requestSearchData.rows, searchCriteria,
                            x => x.CreatedOn, false).ToList();
                    }
                    else
                    {
                        totalRecords = this._orderDataRepository.GetCount(x => x.StatusId.Equals((int)StatusEnum.Active));
                        orders = this._orderDataRepository
                            .GetList(requestSearchData.page, requestSearchData.rows, x => x.StatusId.Equals((int)StatusEnum.Active),
                            x => x.CreatedOn, false).ToList();
                    }

                    return new JSONActionResult(new GridDataModel()
                    {
                        currpage = requestSearchData.page,
                        totalpages = (int)Math.Ceiling((float)totalRecords / (float)requestSearchData.rows),
                        totalrecords = totalRecords,
                        invdata = orders.Select(x => new
                        {
                            x.OrderId,
                            x.OrderReferance,
                            x.OrderDate,
                            OrderMonth = x.OrderDate.ToString("MMM yy", CultureInfo.InvariantCulture),
                            PromoCode = x.OrderAppliedPromoes.FirstOrDefault() != null ?
                                x.OrderAppliedPromoes.FirstOrDefault().PromoCode.PromoCodeName : "",
                            x.SubTotal,
                            x.DiscountAmount,
                            x.TotalAmount,
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
        /// Gets the order items.
        /// </summary>
        /// <param name="OrderId">The order identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult GetOrderItems(int OrderId)
        {
            try
            {
                var orderItems = this._orderItemDataRepository
                    .GetList(x => x.OrderId.Equals(OrderId) && x.StatusId.Equals((int)StatusEnum.Active)).ToList();

                var dipalyOrderItems = orderItems.Select(x => new
                {
                    x.OrderItemId,
                    Template = x.Template.ToString(),
                    x.Quantity,
                    x.Price,
                    ExpectedDelivery = CommonUtility.AddBusinessDays(x.Order.OrderDate, x.DeliverySchedule.DeliveryTo),
                    x.StatusId
                });

                return new JSONActionResult(dipalyOrderItems);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
            return RedirectToAction("Index", "Error", new { area = "" });
        }

        /// <summary>
        /// Sets the testimonial.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult SetOrder()
        {
            try
            {
                if (HttpContext.Request["oper"].ToString() == "edit")
                {
                    OrderModel order = new OrderModel();

                    if (HttpContext.Request["GridMode"].ToString() == "edit")
                    {
                        order.SetPropertiesFromContext(System.Web.HttpContext.Current);
                        OrderModel existingOreder = this._orderDataRepository.Get(order.OrderId);

                        existingOreder.StatusId = order.StatusId;
                        existingOreder.UpdatedBy = CommonUtility.GetSessionData<int>(SessionVariable.UserId);
                        existingOreder.UpdatedOn = DateTime.Now;
                        this._orderDataRepository.Update(existingOreder);
                        return Json(true);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
            return Json(false);
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

                CommonFuntionality.ExportToCSV<OrderModel, object>
                (this._orderDataRepository.GetList,
                x => x.CreatedOn, false, x => new
                {
                    x.OrderReferance,
                    x.OrderDate,
                    x.PromoCode,
                    x.SubTotal,
                    x.DiscountAmount,
                    x.TotalAmount
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
        /// Downloads the ordered file.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="drownloadType">Type of the drownload.</param>
        /// <returns></returns>
        public FileResult DownloadOrderedFile(int orderId, string drownloadType)
        {
            try
            {
                OrderModel selectedOrder = this._orderDataRepository.Get(orderId);

                if (selectedOrder != null)
                {
                    System.IO.MemoryStream stream = new System.IO.MemoryStream();
                    switch (drownloadType)
                    {
                        case "Archive":
                            var imagePaths = selectedOrder.OrderItems.Select(x =>
                                new OrderedImageModel
                                {
                                    OrderedItemCode = x.OrderItemId,
                                    OrderedImages = x.Draft.DraftPages
                                        .OrderBy(z => z.TemplateId)
                                        .ThenBy(z => z.TemplatePage.OrderIndex)
                                        .Select(y => y.FinalImageUrl.ToBase64Encode()).ToList()
                                }).ToList<OrderedImageModel>();

                            if (imagePaths != null)
                            {
                                stream = CommonUtility.CreateArchiveStream(imagePaths);

                                return File(stream, "application/zip", string.Format("{0}.zip",
                                    CommonUtility.GenarateRandomString(10, 10).ToUpper()));
                            }
                            break;

                        case "PDF":
                            var imagePreviewPaths = selectedOrder.OrderItems.SelectMany(x => x.Draft.DraftPages)
                                    .Select(x => new PrintableOrderViewModel
                                    {
                                        DraftId = x.DraftId,
                                        TemplateId = x.TemplateId,
                                        TemplatePageId = x.TemplatePageId,
                                        TemplateTitle = x.Template.TemplateTitle,
                                        OrderIndex = x.TemplatePage.OrderIndex,
                                        FinalImageUrl = x.FinalImageUrl.ToBase64Encode()
                                    }).ToList();

                            if (imagePreviewPaths != null)
                            {
                                stream = CommonUtility.CreateArchivePdfStream(imagePreviewPaths);

                                return File(stream, "application/zip", string.Format("{0}.zip",
                                    CommonUtility.GenarateRandomString(10, 10).ToUpper()));
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(orderId, drownloadType);
            }
            return null;
        }

        /// <summary>
        /// Gets the order model schema.
        /// </summary>
        /// <returns></returns>
        private string GetOrderModelSchema()
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
                    name = CommonUtility.GetDisplayName((OrderModel x) => x.OrderId),
                    editable = true,
                    hidden = true,
                    edittype = Edittype.custom.ToString(),
                });
                columnModel.Add(new colModel()
                {
                    name = "OrderMonth",
                    //formatter = "fn:MonthFormatter"
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((OrderModel x) => x.OrderReferance),
                    width = 100,
                    summaryType = SummaryType.count.ToString(),
                    summaryTpl = "Total orders ({0})",
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
                    name = CommonUtility.GetDisplayName((OrderModel x) => x.OrderDate),
                    formatoptions = new formatoptions { newformat = "m-d-Y" },
                    formatter = Formatter.date.ToString(),
                    align = Align.center.ToString(),
                    width = 50,
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
                    name = "PromoCode",
                    align = Align.center.ToString(),
                    width = 50,
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
                    name = CommonUtility.GetDisplayName((OrderModel x) => x.SubTotal),
                    align = Align.right.ToString(),
                    width = 50,
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
                    }
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((OrderModel x) => x.DiscountAmount),
                    align = Align.right.ToString(),
                    label = "Discount Amt.",
                    width = 50,
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
                    }
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((OrderModel x) => x.TotalAmount),
                    label = "Total Amt.",
                    align = Align.right.ToString(),
                    width = 50,
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
                    name = CommonUtility.GetDisplayName((OrderModel x) => x.StatusId),
                    editable = true,
                    hidden = true,
                    edittype = Edittype.custom.ToString(),
                });
                columnModel.Add(new colModel()
                {
                    name = "Status",
                    editable = true,
                    width = 50,
                    targetFieldName = CommonUtility.GetDisplayName((TestimonialModel x) => x.StatusId),
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
                    name = "Download",
                    index = "dwn",
                    width = 30,
                    sortable = false,
                    search = false
                });
                columnModel.Add(new colModel()
                {
                    name = "Actions",
                    index = "act",
                    width = 30,
                    sortable = false,
                    search = false
                });

                gridModelSchema = new GridModelSchema()
                {
                    colModel = columnModel,
                    groupingView = new groupingView
                    {
                        groupField = new string[] { "OrderMonth" },
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

        /// <summary>
        /// Gets the order item model schema.
        /// </summary>
        /// <returns></returns>
        private string GetOrderItemModelSchema()
        {
            string strSchema = string.Empty;
            GridModelSchema gridModelSchema = null;
            try
            {
                List<colModel> columnModel = new List<colModel>();

                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((OrderItemModel x) => x.OrderItemId),
                    editable = true,
                    hidden = true,
                    edittype = Edittype.custom.ToString(),
                });
                columnModel.Add(new colModel()
                {
                    name = "Template",
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((OrderItemModel x) => x.Quantity),
                    align = Align.right.ToString(),
                    width = 50
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((OrderItemModel x) => x.Price),
                    align = Align.right.ToString(),
                    width = 50,
                    formatter = Formatter.currency.ToString(),
                    formatoptions = new formatoptions
                    {
                        decimalSeparator = ".",
                        thousandsSeparator = ",",
                        decimalPlaces = 2,
                        prefix = "$"
                    }
                });
                columnModel.Add(new colModel()
                {
                    name = "ExpectedDelivery",
                    align = Align.center.ToString(),
                    width = 50,
                    formatoptions = new formatoptions { newformat = "m-d-Y" },
                    formatter = "date",
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((OrderItemModel x) => x.StatusId),
                    editable = true,
                    hidden = true,
                    edittype = Edittype.custom.ToString(),
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
