
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
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;
    using System.Xml.Linq;

    public partial class EditorController : BaseController
    {
        private readonly IDraftDataRepository _draftDataRepository;
        private readonly IDraftPagesDataRepository _draftPagesDataRepository;
        private readonly ITemplateDataRepository _templateDataRepository;
        private readonly ITemplatePageDataRepository _templatePageDataRepository;
        private readonly IDeliveryScheduleDataRepository _deliveryScheduleDataRepository;
        private readonly IQueryDataRepository _draftQueryDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorController" /> class.
        /// </summary>
        /// <param name="draftDataRepository">The draft data repository.</param>
        /// <param name="draftPagesDataRepository">The draft pages data repository.</param>
        /// <param name="templatePageDataRepository">The template page data repository.</param>
        /// <param name="templateDataRepository">The template data repository.</param>
        /// <param name="deliveryScheduleDataRepository">The delivery schedule data repository.</param>
        public EditorController(IDraftDataRepository draftDataRepository,
            IDraftPagesDataRepository draftPagesDataRepository,
            ITemplatePageDataRepository templatePageDataRepository,
            ITemplateDataRepository templateDataRepository,
            IDeliveryScheduleDataRepository deliveryScheduleDataRepository)
        {
            this._draftDataRepository = draftDataRepository;
            this._draftPagesDataRepository = draftPagesDataRepository;
            this._templatePageDataRepository = templatePageDataRepository;
            this._templateDataRepository = templateDataRepository;
            this._deliveryScheduleDataRepository = deliveryScheduleDataRepository;
            this._draftQueryDataRepository = new QueryDataRepository<KeystoneDBEntities>();
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult Index()
        {
            TemplatePageModel selectedTemplatePage = null;
            try
            {
                var selectedTemplate = CommonUtility.GetSessionData<TemplateModel>
                    (SessionVariable.SelectedTemplate);

                var templatePages = selectedTemplate.TemplatePages
                    .Where(x => x.StatusId.Equals((int)StatusEnum.Active)).ToList();

                selectedTemplatePage = templatePages.FirstOrDefault();
                ViewBag.Pages = templatePages;
                ViewBag.PageId = selectedTemplatePage.TemplatePageId;
                ViewBag.SelectedTemplateId = selectedTemplate.TemplateId;
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
            return View(selectedTemplatePage);
        }

        /// <summary>
        /// Saveds the index of the image.
        /// </summary>
        /// <param name="draftId">The draft identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult SavedImageIndex(int draftId)
        {
            TemplateModel selectedTemplate = null;
            TemplatePageModel selectedTemplatePage = null;
            string message = "An unspecified error has been occured. Please try again later.";
            try
            {
                var userId = CommonUtility.GetSessionData<int?>(SessionVariable.UserId);
                if (userId.HasValue)
                {
                    CommonUtility.SetSessionData<TemplateModel>(SessionVariable.SelectedTemplate, null);

                    DraftModel savedDraft = this._draftDataRepository.GetList(x => x.DraftId.Equals(draftId)
                        && x.StatusId.Equals((int)StatusEnum.Active) && x.UserAccountId.Equals(userId.Value))
                        .FirstOrDefault();

                    if (savedDraft != null && selectedTemplate == null)
                    {
                        if (savedDraft.Template == null)
                        {
                            int draftTemplateId = savedDraft.TemplateId;
                            selectedTemplate = this._templateDataRepository.GetList(x => x.TemplateId.Equals(draftTemplateId)
                                && x.StatusId.Equals((int)StatusEnum.Active)).FirstOrDefault();
                        }
                        else
                        {
                            selectedTemplate = savedDraft.Template;
                        }

                        if (selectedTemplate != null)
                            CommonUtility.SetSessionData<TemplateModel>(SessionVariable.SelectedTemplate, selectedTemplate);
                    }

                    if (savedDraft != null && selectedTemplate != null)
                    {
                        foreach (var item in selectedTemplate.TemplatePages
                            .Where(x => x.StatusId.Equals((int)StatusEnum.Active)))
                        {
                            int templatePageId = item.TemplatePageId;
                            DraftPagesModel draftPage = savedDraft.DraftPages
                                .FirstOrDefault(x => x.TemplatePageId.Equals(templatePageId));

                            if (draftPage != null)
                            {
                                item.TemplatePageJson = draftPage.DraftJsonString;
                            }
                        }
                        ViewBag.DraftId = savedDraft.DraftId;
                        CommonUtility.SetSessionData<DraftModel>(SessionVariable.CurrentDraft, savedDraft);
                    }

                    selectedTemplatePage = selectedTemplate.TemplatePages.FirstOrDefault();
                    ViewBag.Pages = selectedTemplate.TemplatePages
                        .Where(x => x.StatusId.Equals((int)StatusEnum.Active));
                    ViewBag.PageId = selectedTemplatePage.TemplatePageId;
                    ViewBag.SelectedTemplateId = selectedTemplate.TemplateId;

                    ViewBag.DeliveryId = (savedDraft == null ? (int)DeliveryScheduleEnum.StandardTurnaround : savedDraft.DeliveryScheduleId);
                    ViewBag.Quantity = (savedDraft == null ? 0 : savedDraft.Quantity);
                    ViewBag.Price = (savedDraft == null ? 0 : savedDraft.Price);


                    return View("Index", selectedTemplatePage);
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
            return RedirectToAction("Index", "Error", new { errorMsg = message.ToBase64Encode() });
        }

        /// <summary>
        /// Pages the index.
        /// </summary>
        /// <param name="pageId">The pageid.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult PageIndex(int pageId, int draftId)
        {
            TemplatePageModel selectedTemplatePage = null;
            try
            {
                var selectedTemplate = CommonUtility.GetSessionData<TemplateModel>
                    (SessionVariable.SelectedTemplate);

                selectedTemplatePage = selectedTemplate.TemplatePages
                    .FirstOrDefault(x => x.TemplatePageId.Equals(pageId));

                ViewBag.DraftId = draftId;
                ViewBag.Pages = selectedTemplate.TemplatePages
                    .Where(x => x.StatusId.Equals((int)StatusEnum.Active));
                ViewBag.PageId = pageId;
                ViewBag.SelectedTemplateId = selectedTemplate.TemplateId;
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(pageId);
            }
            return View("Index", selectedTemplatePage);
        }

        /// <summary>
        /// Gets the save design.
        /// </summary>
        /// <param name="drafts">The drafts.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public PartialViewResult GetSaveDesign(IEnumerable<DraftPagesModel> drafts)
        {
            try
            {
                foreach (var item in drafts)
                {
                    if (item.TemplatePage == null)
                    {
                        int templatePageId = item.TemplatePageId;
                        item.TemplatePage = this._templatePageDataRepository.Get(templatePageId);
                    }
                }
                IEnumerable<DraftPagesModel> draftImages = drafts.OrderBy(x => x.TemplatePage.OrderIndex);
                ViewBag.DraftImages = draftImages.ToList();
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(drafts);
            }
            return PartialView("_SaveDesign");
        }

        /// <summary>
        /// Gets the order print.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public PartialViewResult GetOrderPrint(int templateId, int deliveryId, int quantity, decimal price)
        {
            TemplateModel template = this._templateDataRepository.Get(templateId);
            DraftModel currentDraft = CommonUtility.GetSessionData<DraftModel>(SessionVariable.CurrentDraft);

            var quantities = CommonUtility.GetQuantityByTemplateAndDeliverySchedule
                (templateId, deliveryId, template, quantity);

            ViewBag.AvailableQuantities = new SelectList(quantities, "Value", "Text",
                quantities.FirstOrDefault(x => x.Selected == true));

            ViewBag.AvailableDeliverySchedules = _deliveryScheduleDataRepository
                .GetList(x => x.StatusId.Equals((int)StatusEnum.Active)).ToList();

            ViewBag.DraftImage = currentDraft.DraftPages.FirstOrDefault().DraftPreviewUrl;

            ViewBag.TemplateId = templateId;
            ViewBag.DeliveryId = (deliveryId == 0 ? (int)DeliveryScheduleEnum.StandardTurnaround : deliveryId);
            ViewBag.Quantity = (quantity == 0 ? decimal.Parse(quantities.FirstOrDefault(x => x.Selected == true).Value) : quantity);
            ViewBag.Price = (price == 0 ? quantities.FirstOrDefault(x => x.Selected == true).Price : price); ;

            return PartialView("_OrderPrint", template);
        }

        /// <summary>
        /// Gets the proceed to cart.
        /// </summary>
        /// <param name="drafts">The drafts.</param>
        /// <param name="templateId">The template identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public PartialViewResult GetProceedToCart(IEnumerable<DraftPagesModel> drafts, int templateId)
        {
            try
            {
                foreach (var item in drafts)
                {
                    if (item.TemplatePage == null)
                    {
                        int templatePageId = item.TemplatePageId;
                        item.TemplatePage = this._templatePageDataRepository.Get(templatePageId);
                    }
                }

                IEnumerable<DraftPagesModel> draftImages = drafts.OrderBy(x => x.TemplatePage.OrderIndex);
                ViewBag.DraftImages = draftImages.ToList();
                ViewBag.TemplateId = templateId;
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(drafts, templateId);
            }
            return PartialView("_ProceedToCart");
        }

        /// <summary>
        /// Gets the upload window.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public PartialViewResult GetUploadWindow()
        {
            return PartialView("_ImageUploadWindow");
        }

        /// <summary>
        /// Uploads the image.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult UploadImage(IEnumerable<HttpPostedFileBase> files)
        {
            string status = "failed";
            IList<string> imageUrls = new List<string>();

            try
            {
                imageUrls = this.SaveFileToServer(files);
                if (imageUrls.Count() > 0)
                    status = "success";
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(files);
            }
            return Json(new { Status = status, url = imageUrls }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Replaces the image.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult ReplaceImage()
        {
            string status = "failed";
            string message = "";
            string imageUrl = "";
            try
            {
                if (Request.Files != null)
                {
                    HttpPostedFileBase file = Request.Files[0]; //assuming that's going to be the first file 
                    if (file.ContentLength > 0)
                    {
                        if (CommonUtility.IsImageSizeAppicableToUpload(file.InputStream))
                        {
                            imageUrl = this.SaveFileToServer(Enumerable.Repeat(file, 1)).FirstOrDefault();
                            if (imageUrl.Count() > 0)
                                status = "success";
                        }
                        else
                        {
                            message = "File size is too large. Maximum file size should be no more than 300 pixels. Please resize your image.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }

            var builder = new TagBuilder("input");
            builder.MergeAttributes<string, string>(new Dictionary<string, string> { { "id", "response" }, { "type", "hidden" }, { "value", "{0}" } });

            return Content(string.Format(builder.ToString(),
                Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    Status = status,
                    Message = message.ToBase64Encode(),
                    url = imageUrl
                },
                Newtonsoft.Json.Formatting.None, new Newtonsoft.Json.JsonSerializerSettings
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                }).ToBase64Encode()));
        }

        /// <summary>
        /// Sets the watermark.
        /// </summary>
        /// <param name="imageRawData">The image raw data.</param>
        /// <param name="templateId">The template identifier.</param>
        /// <param name="pageId">The page identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public JsonResult SetWatermark(string imageRawData, int templateId, int pageId)
        {
            try
            {

                string strSVG = imageRawData.ToBase64Decode();

                //byte[] data = Convert.FromBase64String(imageRawData);
                string watermarkText = ConfigurationManager.AppSettings["WatermarkText"].ToString();
                string rootDraftFolder = ConfigurationManager.AppSettings["DraftFolder"].ToString();
                string rootFinalFolder = ConfigurationManager.AppSettings["FinalFolder"].ToString();
                var userId = CommonUtility.GetSessionData<int?>(SessionVariable.UserId);
                string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

                string draftImage = "", finalImage = "";
                string svgFilePath = CreateSVGFileFromString(strSVG);
                if (!string.IsNullOrEmpty(svgFilePath))
                {
                    string draftPath = CommonUtility.GetUserFolder(rootDraftFolder, userId);
                    string tempImage = ConvertSVGToImage(svgFilePath, rootDraftFolder, userId);

                    draftImage = Utilities.CommonUtility.SaveImageWithWatermarkFromFile
                        (tempImage, templateId, pageId, userId, watermarkText, draftPath,
                        CommonUtility.GetAppSetting<int>("WatermarkTextOpacity"));


                    string finalPath = CommonUtility.GetUserFolder(rootFinalFolder, userId);
                    finalImage = string.Format("{0}_{1}_{2}.jpg", Session.SessionID, templateId, pageId);
                    string finelImagePath = Path.Combine(finalPath, finalImage);

                    if (System.IO.File.Exists(finelImagePath))
                    {
                        System.IO.File.Delete(finelImagePath);
                    }
                    System.IO.File.Move(tempImage, finelImagePath);
                    System.IO.File.Delete(tempImage);
                    System.IO.File.Delete(svgFilePath);
                }
                string draftImageUrl = CommonUtility.GetSavedImageUrl(rootDraftFolder, draftImage, userId);
                string finalImageUrl = CommonUtility.GetSavedImageUrl(rootFinalFolder, finalImage, userId);

                return Json(new
                {
                    Status = "success",
                    draftUrl = draftImageUrl.ToBase64Encode(),
                    finalUrl = finalImageUrl.ToBase64Encode()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(imageRawData);
            }
            return Json(new { Status = "failed" });
        }

        /// <summary>
        /// Prints the preview.
        /// </summary>
        /// <param name="printPreview">The print preview.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult PrintPreview(PrintPreview printPreview)
        {
            return View(printPreview);
        }

        /// <summary>
        /// Createds the preview of all page for A template.
        /// </summary>
        /// <param name="draftUrl">The draft URL.</param>
        /// <param name="templateId">The template id.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public JsonResult CreatedPreviewOfAllPageForATemplate(IEnumerable<string> draftUrl, int templateId)
        {
            IList<DraftUrlModel> result = new List<DraftUrlModel>();
            int firstTemplatePageId = 0;
            try
            {
                IList<string> decodedDraftUrl = new List<string>();
                var userId = CommonUtility.GetSessionData<int?>(SessionVariable.UserId);
                string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

                string watermarkText = ConfigurationManager.AppSettings["WatermarkText"].ToString();
                string rootDraftFolder = ConfigurationManager.AppSettings["DraftFolder"].ToString();
                string rootFinalFolder = ConfigurationManager.AppSettings["FinalFolder"].ToString();

                foreach (var item in draftUrl)
                    decodedDraftUrl.Add(item.ToBase64Decode());

                IList<int> excludedPages = decodedDraftUrl.Select(x => int.Parse(this.GetFilePart(x)[2])).ToList();

                var trmplate = this._templateDataRepository.GetList(x => x.StatusId.Equals((int)StatusEnum.Active)
                    && x.TemplateId.Equals(templateId)).FirstOrDefault();

                IEnumerable<TemplatePageModel> templatePages = trmplate.TemplatePages
                    .Where(x => !excludedPages.Contains(x.TemplatePageId) && x.StatusId.Equals((int)StatusEnum.Active));

                foreach (var item in templatePages)
                {
                    string sourcePath = Server.MapPath(string.Format("~/Templates/{0}/{1}", item.TemplateId, item.TemplatePageUrl));

                    string draftPath = CommonUtility.GetUserFolder(rootDraftFolder, userId);
                    string draftImage = Utilities.CommonUtility.SaveImageWithWatermarkFromFile
                        (sourcePath, item.TemplateId, item.TemplatePageId, userId, watermarkText, draftPath,
                        int.Parse(ConfigurationManager.AppSettings["WatermarkTextOpacity"].ToString()));

                    string finalPath = CommonUtility.GetUserFolder(rootFinalFolder, userId);
                    string finalImage = Utilities.CommonUtility.SaveImageWithoutWatermarkFromFile
                        (sourcePath, item.TemplateId, item.TemplatePageId, userId, finalPath);

                    string draftImageUrl = CommonUtility.GetSavedImageUrl(rootDraftFolder, draftImage, userId);
                    string finalImageUrl = CommonUtility.GetSavedImageUrl(rootFinalFolder, draftImage, userId);

                    result.Add(new DraftUrlModel
                    {
                        TemplateId = item.TemplateId,
                        TemplatePageId = item.TemplatePageId,
                        DraftImageJson = string.Empty,
                        DraftImageUrl = draftImageUrl.ToBase64Encode(),
                        FileImageUrl = finalImageUrl.ToBase64Encode()
                    });
                }

                firstTemplatePageId = trmplate.TemplatePages.FirstOrDefault().TemplatePageId;
                return Json(new
                {
                    Status = "success",
                    Drafts = result,
                    TemplateId = templateId,
                    TemplatePageId = firstTemplatePageId
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(draftUrl);
            }
            return Json(new { Status = "failed" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Sets the order items.
        /// </summary>
        /// <param name="templateId">The template id.</param>
        /// <param name="deliveryId">The delivery id.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="price">The price.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult SetOrderItems(int templateId, int deliveryId, int quantity, decimal price)
        {
            try
            {
                DraftModel draft = CommonUtility.GetSessionData<DraftModel>(SessionVariable.CurrentDraft);
                IList<OrderItemModel> orderItems = CommonUtility.GetSessionData<IList<OrderItemModel>>(SessionVariable.OrderItems);
                int userId = CommonUtility.GetSessionData<int>(SessionVariable.UserId);

                if (draft != null)
                {
                    if (orderItems == null) orderItems = new List<OrderItemModel>();

                    if (orderItems.Any(x => x.DraftId.Equals(draft.DraftId)))
                    {
                        try
                        {
                            orderItems.Remove(orderItems.FirstOrDefault(x => x.DraftId.Equals(draft.DraftId)));
                        }
                        catch (Exception ex) { }
                    }

                    orderItems.Add(new OrderItemModel
                    {
                        DraftId = draft.DraftId,
                        OrderItemIdentifier = CommonUtility.GenarateRandomString(5, 5).ToUpper(), //Guid.NewGuid().ToString("N"),
                        TemplateId = templateId,
                        DeliveryScheduleId = deliveryId,
                        DisplayUrl = draft.DraftPages.FirstOrDefault().DraftPreviewUrl,
                        Quantity = quantity,
                        Price = price,
                        Template = _templateDataRepository.Get(templateId)
                    });

                    CommonUtility.SetSessionData<IList<OrderItemModel>>(SessionVariable.OrderItems, orderItems);
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }

            return RedirectToAction("Index", "ShoppingCart");
        }

        /// <summary>
        /// Nullifies the session storage.
        /// </summary>
        /// <param name="draftId">The draft identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult NullifySessionStorage(int draftId)
        {
            string errorMessage = "Draft is not found. Access denied.";
            try
            {
                DraftModel draft = this._draftDataRepository.Get(draftId);

                return View(draft);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(draftId);
            }
            return RedirectToAction("Index", "Error", new { errorMsg = errorMessage });
        }

        #region Private Members
        /// <summary>
        /// Gets the file part.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        private string[] GetFilePart(string uri)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                string fileName = Path.GetFileNameWithoutExtension(Uri.UnescapeDataString(uri).Replace("/", "\\"));
                return fileName.Split('_');
            }
            return null;
        }

        /// <summary>
        /// Saves the file to server.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <returns></returns>
        private List<string> SaveFileToServer(IEnumerable<HttpPostedFileBase> files)
        {
            List<string> imageUrls = new List<string>();
            int? userId = CommonUtility.GetSessionData<int?>(SessionVariable.UserId);
            try
            {
                if (!files.IsEmptyCollection())
                {
                    files.ToList().ForEach(x =>
                    {
                        string fileName = string.Format("{0}-{1}-{2}{3}", Path.GetFileNameWithoutExtension(x.FileName),
                            Utilities.CommonUtility.ConvertToTimestamp(DateTime.Now), Session.SessionID.ToString(),
                            Path.GetExtension(x.FileName));

                        string uploadPath = CommonUtility.GetUserFolder(ConfigurationManager.AppSettings["UploadFolder"], userId);
                        string path = string.Format("{0}/{1}", uploadPath, fileName);

                        using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
                        {
                            x.InputStream.Seek(0, SeekOrigin.Begin);
                            x.InputStream.CopyTo(fileStream);
                            fileStream.Close();
                            fileStream.Dispose();
                        }

                        string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                        imageUrls.Add(string.Format("{0}{1}/{2}/{3}/{4}", baseUrl,
                            ConfigurationManager.AppSettings["VirtualDirectory"],
                            ConfigurationManager.AppSettings["UploadFolder"],
                            userId.HasValue ? userId.Value.ToString() : CommonUtility.AnonymousFolder, fileName));
                    });
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(files);
            }
            return imageUrls;
        }

        /// <summary>
        /// Creates the SVG file from string.
        /// </summary>
        /// <param name="strSVG">The STR SVG.</param>
        /// <returns></returns>
        private string CreateSVGFileFromString(string strSVG, bool useCMYK = false)
        {
            try
            {
                XNamespace xlink = "http://www.w3.org/1999/xlink";
                XElement xElm = XElement.Parse(strSVG);

                if (xElm != null)
                {
                    xElm.Descendants().Where(x => x.Name.LocalName.Equals("image"))
                        .ToList().ForEach(y =>
                        {
                            string linkAttr = y.Attribute(xlink + "href").Value
                                .Replace(CommonUtility.GetAppSetting<string>("DomainName"), "..");

                            if (useCMYK)
                            {
                                var filepart = linkAttr.Split('/');
                                if (filepart.Length > 0)
                                {
                                    string fileNameBG = filepart.LastOrDefault().AsString();
                                    if (fileNameBG != string.Empty && fileNameBG.StartsWith("BG"))
                                    {
                                        string fileBaseName = Path.GetFileNameWithoutExtension(fileNameBG);
                                        string fileExtensionName = Path.GetExtension(fileNameBG);
                                        linkAttr = string.Format("{0}/{1}-CMYK{2}", string.Join("/",
                                            filepart.Where(x => x != fileNameBG)), fileBaseName, fileExtensionName);
                                    }
                                }
                            }

                            y.SetAttributeValue(xlink + "href", string.Format("file:///{0}",
                                Server.MapPath(linkAttr).Replace("\\", "/")));
                            //y.Attribute(xlink + "href").Value = linkAttr;
                        });

                    string modifiedSVG = xElm.ToString();
                    string tempSVGFile = Server.MapPath(string.Format("~/{0}/{1}-{2}.svg",
                        CommonUtility.GetAppSetting<string>("TempFolder"),
                        Session.SessionID, CommonUtility.GenarateRandomString(10, 10)));

                    using (StreamWriter writer = new StreamWriter(tempSVGFile, false, System.Text.Encoding.UTF8))
                    {
                        writer.Write(modifiedSVG);
                        writer.Close();
                        writer.Dispose();

                        return tempSVGFile;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
            return string.Empty;
        }

        /// <summary>
        /// Converts the SVG to image.
        /// </summary>
        /// <param name="svgFileName">Name of the SVG file.</param>
        /// <param name="rootFolder">The root folder.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        private string ConvertSVGToImage(string svgFileName, string rootFolder, int? userId)
        {
            try
            {
                string tempSVGFile = Server.MapPath(string.Format("~/{0}/{1}-{2}.jpg",
                    CommonUtility.GetAppSetting<string>("TempFolder"),
                    Session.SessionID, CommonUtility.GenarateRandomString(10, 10)));

                ImageDimention dimention = GetFileDimentionFromSVG(svgFileName);

                string inkscapeArgs = string.Format(@"-f ""{0}"" -e ""{1}"" -d {2} -w {3} -h {4}",
                    svgFileName, tempSVGFile, CommonUtility.GetAppSetting<string>("SVGtoImageConversionDPI"),
                    dimention.Width, dimention.Height);
                string inkscapeExecutionPath = CommonUtility.GetAppSetting<string>("InkscapeExecutionPath");
                Process inkscape = Process.Start(new ProcessStartInfo(inkscapeExecutionPath, inkscapeArgs));

                //inkscape.WaitForExit(3000);
                inkscape.WaitForExit();

                while (!inkscape.HasExited)
                {
                    Console.WriteLine("Waiting...");
                    Thread.Sleep(1000);
                }

                inkscape.Dispose();
                return tempSVGFile;
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(svgFileName, userId);
            }
            return string.Empty;
        }

        private ImageDimention GetFileDimentionFromSVG(string svgFilePath)
        {
            ImageDimention dimention = new ImageDimention();
            try
            {
                if (System.IO.File.Exists(svgFilePath))
                {
                    XDocument xDoc = XDocument.Load(svgFilePath);
                    if (xDoc != null)
                    {
                        var width = xDoc.Root.Attribute("width").Value;
                        var height = xDoc.Root.Attribute("height").Value;

                        dimention.Width = Convert.ToDecimal(width);
                        dimention.Height = Convert.ToDecimal(height);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(svgFilePath);
            }
            return dimention;
        }
        #endregion
    }

    public class ImageDimention
    {
        public decimal Width { get; set; }
        public decimal Height { get; set; }
    }
}
