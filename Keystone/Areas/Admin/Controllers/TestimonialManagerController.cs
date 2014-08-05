
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
    public class TestimonialManagerController : BaseController
    {
        private readonly ITestimonialDataRepository _testimonialDataRepository;
        /// <summary>
        /// Initializes a new instance of the <see cref="TestimonialManagerController"/> class.
        /// </summary>
        /// <param name="testimonialDataRepository">The testimonial data repository.</param>
        public TestimonialManagerController(ITestimonialDataRepository testimonialDataRepository)
        {
            this._testimonialDataRepository = testimonialDataRepository;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult Index()
        {
            ViewBag.SchemaData = GetTestimonialModelSchema();
            return View();
        }

        /// <summary>
        /// Gets the testimonial.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult GetTestimonial()
        {
            try
            {
                GridSearchDataModel requestSearchData = new GridSearchDataModel();
                requestSearchData.SetPropertiesFromContext<GridSearchDataModel>(System.Web.HttpContext.Current);

                if (requestSearchData != null)
                {
                    List<TestimonialModel> testimonials = new List<TestimonialModel>();
                    int totalRecords = 0;

                    if (requestSearchData._search)
                    {
                        string criteria = requestSearchData.filters.ToString();
                        var searchCriteria = CommonUtility.GetLamdaExpressionFromFilter<TestimonialModel>(criteria);
                        totalRecords = this._testimonialDataRepository.GetCount(searchCriteria);

                        testimonials = this._testimonialDataRepository
                            .GetList(requestSearchData.page, requestSearchData.rows, searchCriteria,
                            x => x.CreatedOn, false).ToList();
                    }
                    else
                    {
                        totalRecords = this._testimonialDataRepository.GetCount(x => x.StatusId.Equals((int)StatusEnum.Active));
                        testimonials = this._testimonialDataRepository
                            .GetList(requestSearchData.page, requestSearchData.rows, x => x.StatusId.Equals((int)StatusEnum.Active),
                            x => x.CreatedOn, false).ToList();
                    }

                    return new JSONActionResult(new GridDataModel()
                    {
                        currpage = requestSearchData.page,
                        totalpages = (int)Math.Ceiling((float)totalRecords / (float)requestSearchData.rows),
                        totalrecords = totalRecords,
                        invdata = testimonials.Select(x => new
                        {
                            x.TestimonialId,
                            x.TestimonialContent,
                            x.WriterName,
                            x.PostedOn,
                            x.DisplayOrder,
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
        /// Sets the testimonial.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult SetTestimonial()
        {
            try
            {
                if (HttpContext.Request["oper"].ToString() == "edit")
                {
                    TestimonialModel testimonial = new TestimonialModel();

                    if (HttpContext.Request["GridMode"].ToString() == "insert")
                    {
                        testimonial.SetPropertiesFromContext<TestimonialModel>(System.Web.HttpContext.Current);
                        testimonial.StatusId = (int)StatusEnum.Active;
                        testimonial.CreatedBy = CommonUtility.GetSessionData<int>(SessionVariable.UserId);
                        testimonial.CreatedOn = DateTime.Now;
                        this._testimonialDataRepository.Insert(testimonial);

                        if (testimonial.TestimonialId > 0)
                        {
                            return Json(true);
                        }
                    }
                    else if (HttpContext.Request["GridMode"].ToString() == "edit")
                    {
                        testimonial.SetPropertiesFromContext(System.Web.HttpContext.Current);
                        TestimonialModel existingTestimonial = this._testimonialDataRepository
                            .Get(testimonial.TestimonialId);

                        existingTestimonial.TestimonialContent = testimonial.TestimonialContent;
                        existingTestimonial.StatusId = testimonial.StatusId;
                        existingTestimonial.WriterName = testimonial.WriterName;
                        existingTestimonial.PostedOn = testimonial.PostedOn;
                        existingTestimonial.UpdatedBy = CommonUtility.GetSessionData<int>(SessionVariable.UserId);
                        existingTestimonial.UpdatedOn = DateTime.Now;
                        this._testimonialDataRepository.Update(existingTestimonial);
                        return Json(true);
                    }
                    else if (HttpContext.Request["GridMode"].ToString() == "delete") { }
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

                CommonFuntionality.ExportToCSV<TestimonialModel, object>
                (this._testimonialDataRepository.GetList,
                x => x.CreatedOn, false, x => new
                {
                    x.TestimonialContent,
                    x.WriterName,
                    x.PostedOn,
                    x.DisplayOrder
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
        /// Gets the testimonial model schema.
        /// </summary>
        /// <returns></returns>
        private string GetTestimonialModelSchema()
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
                    name = CommonUtility.GetDisplayName((TestimonialModel x) => x.TestimonialId),
                    editable = true,
                    hidden = true,
                    edittype = Edittype.custom.ToString(),
                });
                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((TestimonialModel x) => x.TestimonialContent),
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
                    name = CommonUtility.GetDisplayName((TestimonialModel x) => x.WriterName),
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
                    name = CommonUtility.GetDisplayName((TestimonialModel x) => x.PostedOn),
                    editable = true,
                    formatoptions = new formatoptions { newformat = "m-d-Y" },
                    edittype = Edittype.text.ToString(),
                    width = 50,
                    align = Align.center.ToString(),
                    formatter = "date",
                    editrules = new editrules { required = true },
                    editoptions = new editoptions { dataInit = "initDatepickerOnDateEdit" },
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
                    name = CommonUtility.GetDisplayName((TestimonialModel x) => x.DisplayOrder),
                    editable = true,
                    align = Align.right.ToString(),
                    width = 50,
                    edittype = Edittype.text.ToString(),
                    editrules = new editrules { required = true, number = true },
                    search = false
                });

                columnModel.Add(new colModel()
                {
                    name = CommonUtility.GetDisplayName((TestimonialModel x) => x.StatusId),
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
