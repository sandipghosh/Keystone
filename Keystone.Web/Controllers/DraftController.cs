
namespace Keystone.Web.Controllers
{
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
    using System.Net.Mime;
    using System.Transactions;
    using System.Web.Mvc;

    [SignInActionValidator()]
    public class DraftController : Controller
    {
        private readonly IDraftDataRepository _draftDataRepository;
        private readonly IDraftPagesDataRepository _draftPagesDataRepository;
        private readonly IQueryDataRepository _draftQueryDataRepository;
        private readonly ITemplateDataRepository _templateDataRepository;
        private readonly IOrderItemDataRepository _orderItemDataRepository;
        private readonly IOrderDataRepository _orderDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DraftController"/> class.
        /// </summary>
        /// <param name="draftDataRepository">The draft data repository.</param>
        /// <param name="draftPagesDataRepository">The draft pages data repository.</param>
        /// <param name="templateDataRepository">The template data repository.</param>
        public DraftController(IDraftDataRepository draftDataRepository,
            IDraftPagesDataRepository draftPagesDataRepository,
            ITemplateDataRepository templateDataRepository,
            IOrderItemDataRepository orderItemDataRepository,
            IOrderDataRepository orderDataRepository)
        {
            this._draftDataRepository = draftDataRepository;
            this._draftPagesDataRepository = draftPagesDataRepository;
            this._templateDataRepository = templateDataRepository;
            this._orderItemDataRepository = orderItemDataRepository;
            this._orderDataRepository = orderDataRepository;
            this._draftQueryDataRepository = new QueryDataRepository<KeystoneDBEntities>();
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
        /// Saves the draft.
        /// </summary>
        /// <param name="draft">The draft.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public JsonResult SaveDraft(DraftModel draft)
        {
            bool status = false;
            string message = string.Empty;
            int draftId = 0;
            try
            {
                var userId = CommonUtility.GetSessionData<int?>(SessionVariable.UserId);
                if (userId.HasValue)
                {
                    if (!IsDraftAlreadyExists(draft))
                    {
                        TransactionOptions options = new TransactionOptions()
                        {
                            IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                            Timeout = new TimeSpan(0, 1, 0)
                        };
                        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, options))
                        {
                            draft.CreatedBy = draft.UserAccountId = userId.Value;
                            draft.CreatedOn = DateTime.Now;
                            draft.StatusId = (int)StatusEnum.Active;

                            foreach (var page in draft.DraftPages)
                            {
                                page.DraftJsonString = page.DraftJsonString ?? string.Empty;
                                page.DraftPreviewUrl = page.DraftPreviewUrl.ToBase64Decode();
                                page.FinalImageUrl = page.FinalImageUrl.ToBase64Decode();
                                page.CreatedBy = userId.Value;
                                draft.CreatedOn = DateTime.Now;
                                draft.StatusId = (int)StatusEnum.Active;
                            }

                            MoveDraftImagesFromAnonymousToUserFolder(draft.DraftPages);
                            _draftDataRepository.Insert(draft);

                            CommonUtility.SetSessionData<DraftModel>(SessionVariable.CurrentDraft, draft);
                            draftId = draft.DraftId;
                            scope.Complete();
                        }
                        return Json(new
                        {
                            Status = true,
                            DraftId = draft.DraftId,
                            TemplateId = draft.TemplateId,
                            Message = "Draft is successfully saved"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        message = "Draft name should be unique. Please try again.";
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(draft);
            }
            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Updates the draft.
        /// </summary>
        /// <param name="newDraft">The draft.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public JsonResult UpdateDraft(DraftModel newDraft)
        {
            bool status = false;
            string message = "An Unspecified error has been occured. Please try again latter.";
            int draftId = newDraft.DraftId;
            try
            {
                var userId = CommonUtility.GetSessionData<int?>(SessionVariable.UserId);
                if (userId.HasValue)
                {
                    DraftModel existingDraft = this._draftDataRepository.GetList(x => x.DraftId.Equals(draftId)
                        && x.UserAccountId.Equals(userId.Value) && x.StatusId.Equals((int)StatusEnum.Active)).FirstOrDefault();

                    if (existingDraft != null)
                    {
                        TransactionOptions options = new TransactionOptions()
                        {
                            IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                            Timeout = new TimeSpan(0, 1, 0)
                        };
                        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, options))
                        {
                            existingDraft.UpdatedBy = userId.Value;
                            existingDraft.UpdatedOn = DateTime.Now;

                            foreach (var page in existingDraft.DraftPages)
                            {
                                int currentTemplateId = page.TemplatePageId;
                                DraftPagesModel newDraftPage = newDraft.DraftPages
                                    .FirstOrDefault(x => x.TemplatePageId.Equals(currentTemplateId));

                                if (newDraftPage != null)
                                {
                                    page.DraftJsonString = newDraftPage.DraftJsonString ?? string.Empty;
                                    page.DraftPreviewUrl = newDraftPage.DraftPreviewUrl.ToBase64Decode();
                                    page.FinalImageUrl = newDraftPage.FinalImageUrl.ToBase64Decode();
                                    page.UpdatedBy = userId.Value;
                                    page.UpdatedOn = DateTime.Now;
                                }
                            }

                            MoveDraftImagesFromAnonymousToUserFolder(existingDraft.DraftPages);
                            this._draftDataRepository.Update(existingDraft);
                            this._draftPagesDataRepository.Update(existingDraft.DraftPages);

                            CommonUtility.SetSessionData<DraftModel>(SessionVariable.CurrentDraft, existingDraft);
                            draftId = existingDraft.DraftId;
                            scope.Complete();
                        }
                        var quantity = CommonUtility.GetQuantityByTemplateAndDeliverySchedule
                            (existingDraft.TemplateId, (int)DeliveryScheduleEnum.StandardTurnaround, existingDraft.Template);

                        if (quantity != null)
                        {
                            return Json(new
                            {
                                Status = true,
                                DraftId = existingDraft.DraftId,
                                TemplateId = existingDraft.TemplateId,
                                DeliveryId = existingDraft.DeliveryScheduleId == 0 ?
                                    (int)DeliveryScheduleEnum.StandardTurnaround : existingDraft.DeliveryScheduleId,
                                Quantity = existingDraft.Quantity == 0 ?
                                    quantity.SingleOrDefault(x => x.Selected == true).Value : existingDraft.Quantity,
                                Price = existingDraft.Price == 0 ?
                                    quantity.FirstOrDefault(x => x.Selected == true).Price : existingDraft.Price,
                                Message = "Draft is successfully updated."
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(newDraft);
            }
            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Deletes the draft.
        /// </summary>
        /// <param name="draftId">The draft identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public JsonResult DeleteDraft(int draftId)
        {
            bool status = false;
            string message = "An Unspecified error has been occured. Please try again latter.";
            try
            {
                var userId = CommonUtility.GetSessionData<int>(SessionVariable.UserId);
                DraftModel draft = this._draftDataRepository.Get(draftId);
                if (draft != null)
                {
                    //int orderItems = this._orderItemDataRepository.GetCount(x => x.DraftId == draftId);

                    //if (orderItems > 0)
                    //    message = "Unable to delete the draft. This draft is associated with some order.";
                    //else
                    //{
                    //    draft.UpdatedBy = userId;
                    //    draft.UpdatedOn = DateTime.Now;
                    //    draft.StatusId = (int)StatusEnum.Inactive;

                    //    this._draftDataRepository.Update(draft);
                    //    status = true;
                    //}

                    draft.UpdatedBy = userId;
                    draft.UpdatedOn = DateTime.Now;
                    draft.StatusId = (int)StatusEnum.Inactive;

                    this._draftDataRepository.Update(draft);
                    status = true;
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(draftId);
            }
            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the draft items.
        /// </summary>
        /// <param name="userAccountId">The user account identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        ChildActionOnly()]
        public PartialViewResult GetDraftItems(int userAccountId)
        {
            try
            {
                var drafts = this._draftDataRepository
                    .GetList(x => x.UserAccountId.Equals(userAccountId)
                    && x.StatusId.Equals((int)StatusEnum.Active)
                    && !Utilities.CommonFuntionality.IsDraftOrdered(x.DraftId)).ToList();

                if (!drafts.IsEmptyCollection())
                {
                    return PartialView("_DraftList", drafts);
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(userAccountId);
            }
            return null;
        }

        /// <summary>
        /// Gets the order items.
        /// </summary>
        /// <param name="userAccountId">The user account identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        ChildActionOnly()]
        public PartialViewResult GetOrderItems(int userAccountId)
        {
            try
            {
                List<OrderModel> orders = this._orderDataRepository
                    .GetList(x => x.UserAccountId.Equals(userAccountId)
                    && x.StatusId.Equals((int)StatusEnum.Active)).ToList();

                if (!orders.IsEmptyCollection())
                {
                    return PartialView("_SavedOrderList", orders);
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(userAccountId);
            }
            return null;
        }

        /// <summary>
        /// Shares the draft.
        /// </summary>
        /// <param name="imagePaths">The image paths.</param>
        /// <param name="templateId">The template identifier.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="emailId">The email identifier.</param>
        /// <param name="comment">The comment.</param>
        /// <returns></returns>
        [AllowAnonymousAccess]
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public JsonResult ShareDraft(List<string> imagePaths, int templateId,
            string recipient, string emailId, string comment)
        {
            bool status = false;
            try
            {
                EmailSender email = new EmailSender
                {
                    SSL = bool.Parse(ConfigurationManager.AppSettings["MAIL_SERVER_SSL"].ToString()),
                    Subject = "Draft sample",
                    To = emailId
                };

                var draftPDF = CommonUtility.CreatePdfStream(imagePaths);
                email.Attachment = new System.Net.Mail.Attachment(draftPDF,
                    string.Format("{0}.pdf", CommonUtility.ConvertToTimestamp(DateTime.Now)),
                    MediaTypeNames.Application.Pdf);

                var template = _templateDataRepository.Get(templateId);

                string mailBody = Utilities.CommonUtility.RenderViewToString
                    ("_MailShareTemplate", template,
                    this, new Dictionary<string, object>() { 
                    { "SampleImage", imagePaths.FirstOrDefault().ToBase64Decode() },
                    {"Recipient", recipient}, {"Comment", comment}});

                email.SendMailAsync(mailBody, () =>
                {
                    draftPDF.Close();
                    draftPDF.Dispose();
                });

                status = true;
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(imagePaths, templateId, recipient, emailId, comment);
            }
            return Json(new { Status = status }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Determines whether [is draft already exists] [the specified draft].
        /// </summary>
        /// <param name="draft">The draft.</param>
        /// <returns></returns>
        private bool IsDraftAlreadyExists(DraftModel draft)
        {
            bool status = false;
            try
            {
                int currentLoggedInUser = CommonUtility.GetSessionData<int>(SessionVariable.UserId);
                bool returntype = false;
                string sql = "EXEC [dbo].[IsDraftAlreadyExists] @DraftName, @UserId, @existingDraft OUT";
                SqlParameter userId = new SqlParameter { ParameterName = "DraftName", Value = draft.DraftName };
                SqlParameter draftName = new SqlParameter { ParameterName = "UserId", Value = currentLoggedInUser };
                SqlParameter totalDraft = new SqlParameter { ParameterName = "existingDraft", Value = false, Direction = ParameterDirection.Output };
                totalDraft.Direction = ParameterDirection.Output;

                this._draftQueryDataRepository.ExecuteCommand(sql, userId, draftName, totalDraft);
                return Convert.ToBoolean(totalDraft.Value);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(draft);
            }
            return status;
        }

        /// <summary>
        /// Moves the draft images from anonymous to user folder.
        /// </summary>
        /// <param name="draftPages">The draft pages.</param>
        private void MoveDraftImagesFromAnonymousToUserFolder(IEnumerable<DraftPagesModel> draftPages)
        {
            try
            {
                int userId = CommonUtility.GetSessionData<int>(SessionVariable.UserId);
                foreach (var item in draftPages)
                {
                    Uri draftImageUri = new Uri(item.DraftPreviewUrl);
                    if (item.DraftPreviewUrl.Contains(CommonUtility.AnonymousFolder))
                    {
                        string sourceDraftImagePath = draftImageUri.AbsolutePath;
                        string targetDraftImagePath = CommonUtility.GetSavedImageUrl(ConfigurationManager.AppSettings["DraftFolder"],
                            Path.GetFileName(draftImageUri.LocalPath), userId, false);
                        item.DraftPreviewUrl = targetDraftImagePath;

                        string sourceDraftAbsoluteImagePath = Server.MapPath(string.Format("~/{0}", sourceDraftImagePath));
                        string targetDraftAbsoluteImagePath = Server.MapPath(string.Format("~/{0}", targetDraftImagePath));

                        if (System.IO.File.Exists(targetDraftAbsoluteImagePath))
                        {
                            System.IO.File.Delete(targetDraftAbsoluteImagePath);
                        }
                        System.IO.File.Move(sourceDraftAbsoluteImagePath, targetDraftAbsoluteImagePath);
                    }
                    else { item.DraftPreviewUrl = draftImageUri.AbsolutePath; }

                    Uri finalImageUri = new Uri(item.FinalImageUrl);
                    if (item.FinalImageUrl.Contains(CommonUtility.AnonymousFolder))
                    {
                        string sourceFinalImagePath = finalImageUri.AbsolutePath;
                        string targetFinalImagePath = CommonUtility.GetSavedImageUrl(ConfigurationManager.AppSettings["FinalFolder"],
                            Path.GetFileName(finalImageUri.LocalPath), userId, false);
                        item.FinalImageUrl = targetFinalImagePath;


                        string sourceFinalAbsoluteImagePath = Server.MapPath(string.Format("~/{0}", sourceFinalImagePath));
                        string targetFinalAbsoluteImagePath = Server.MapPath(string.Format("~/{0}", targetFinalImagePath));

                        if (System.IO.File.Exists(targetFinalAbsoluteImagePath))
                        {
                            System.IO.File.Delete(targetFinalAbsoluteImagePath);
                        }
                        System.IO.File.Move(sourceFinalAbsoluteImagePath, targetFinalAbsoluteImagePath);
                    }
                    else
                    { item.FinalImageUrl = finalImageUri.AbsolutePath; }
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(draftPages);
            }
        }
    }
}
