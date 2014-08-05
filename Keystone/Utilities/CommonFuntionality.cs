

namespace Keystone.Web.Utilities
{
    using Keystone.Web.Data.Implementation;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Models;
    using Keystone.Web.Utilities.PaymentGetway;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;

    public class CommonFuntionality
    {
        /// <summary>
        /// Payments the error redirection.
        /// </summary>
        /// <param name="paymentResponse">The payment response.</param>
        /// <param name="shoppingCart">The shopping cart.</param>
        /// <returns></returns>
        public string PaymentErrorRedirection(PaypalCheckoutResponse paymentResponse,
            ShoppingCartModel shoppingCart)
        {
            try
            {
                IPaymentInfoDataRepository _paymentInfoDataRepositor
                    = DependencyResolver.Current.GetService<PaymentInfoDataRepository>();
                CommonFuntionality commonFunc = new CommonFuntionality();
                PaymentInfoModel paymentInfo = PreparePaimentInfo(paymentResponse, shoppingCart);
                _paymentInfoDataRepositor.Insert(paymentInfo);
                return paymentInfo.ToString();
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(paymentResponse, shoppingCart);
            }
            return string.Empty;
        }

        public string PaymentErrorRedirection(PaypalCreditCardCheckoutResponse paymentResponse,
            ShoppingCartModel shoppingCart)
        {
            try
            {
                IPaymentInfoDataRepository _paymentInfoDataRepositor
                    = DependencyResolver.Current.GetService<PaymentInfoDataRepository>();
                CommonFuntionality commonFunc = new CommonFuntionality();
                PaymentInfoModel paymentInfo = PreparePaimentInfo(paymentResponse, shoppingCart);
                _paymentInfoDataRepositor.Insert(paymentInfo);
                return paymentInfo.ToString();
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(paymentResponse, shoppingCart);
            }
            return string.Empty;
        }

        /// <summary>
        /// Prepares the paiment information.
        /// </summary>
        /// <param name="paymentResponse">The payment response.</param>
        /// <param name="shoppingCart">The shopping cart.</param>
        /// <returns></returns>
        public PaymentInfoModel PreparePaimentInfo(PaypalCheckoutResponse paymentResponse,
            ShoppingCartModel shoppingCart)
        {
            PaymentInfoModel paymentInfo = new PaymentInfoModel()
            {
                OrderId = shoppingCart.OrderId,
                RequistedIp = CommonUtility.GetClientIPAddress(),
                Acknowledgement = paymentResponse.Decoder["ACK"].ToUpper().AsString(),
                StatusId = (int)StatusEnum.Active,
                TransactionId = paymentResponse.Decoder[PaypalAttributes.PAYMENTINFO_n_TRANSACTIONID].AsString().ToUpper(),
                TransactionType = paymentResponse.Decoder[PaypalAttributes.PAYMENTINFO_n_PAYMENTTYPE].AsString(),
                TransactionErrorCode = Convert.ToString(paymentResponse.Decoder[PaypalAttributes.L_ERRORCODEn]).AsString(),
                TransactionShortMessage = Convert.ToString(paymentResponse.Decoder[PaypalAttributes.L_SHORTMESSAGEn]).AsString("Success"),
                TransactionLongMessage = Convert.ToString(paymentResponse.Decoder[PaypalAttributes.L_LONGMESSAGEn]).AsString("Payment successfully made."),
                TransactionTime = DateTime.Parse(paymentResponse.Decoder[PaypalAttributes.PAYMENTINFO_n_ORDERTIME].AsString(DateTime.Now.ToString())),
                TransactionAmount = decimal.Parse(paymentResponse.Decoder[PaypalAttributes.PAYMENTINFO_n_AMT].AsString("0.00")),
                CreatedBy = shoppingCart.UserAccountId,
                CreatedOn = DateTime.Now
            };

            return paymentInfo;
        }

        /// <summary>
        /// Prepares the paiment information.
        /// </summary>
        /// <param name="paymentResponse">The payment response.</param>
        /// <param name="shoppingCart">The shopping cart.</param>
        /// <returns></returns>
        public PaymentInfoModel PreparePaimentInfo(PaypalCreditCardCheckoutResponse paymentResponse,
            ShoppingCartModel shoppingCart)
        {
            PaymentInfoModel paymentInfo = new PaymentInfoModel()
            {
                OrderId = shoppingCart.OrderId,
                RequistedIp = CommonUtility.GetClientIPAddress(),
                Acknowledgement = paymentResponse.Acknowledgement,
                StatusId = (int)StatusEnum.Active,
                TransactionId = paymentResponse.Decoder[PaypalAttributes.PPREF].AsString().ToUpper(),
                TransactionType = "card",
                TransactionErrorCode = Convert.ToString(paymentResponse.Decoder[PaypalAttributes.RESULT]).AsString(),
                TransactionShortMessage = Convert.ToString(paymentResponse.Decoder[PaypalAttributes.RESPMSG]).AsString("Success"),
                TransactionLongMessage = Convert.ToString(paymentResponse.Decoder[PaypalAttributes.RESPMSG]).AsString("Payment successfully made."),
                TransactionTime = DateTime.Parse(paymentResponse.Decoder[PaypalAttributes.TRANSTIME].AsString(DateTime.Now.ToString())),
                TransactionAmount = decimal.Parse(paymentResponse.Decoder[PaypalAttributes.AMT].AsString("0.00")),
                CreatedBy = shoppingCart.UserAccountId,
                CreatedOn = DateTime.Now
            };

            return paymentInfo;
        }

        /// <summary>
        /// Exports to CSV.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="KProperty">The type of the property.</typeparam>
        /// <param name="dateRepository">The date repository.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="asc">if set to <c>true</c> [asc].</param>
        /// <param name="fieldNameExp">The field name exp.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="reportHeader">The report header.</param>
        public static void ExportToCSV<TModel, KProperty>(Func<System.Linq.Expressions.Expression<Func<TModel, bool>>,
            System.Linq.Expressions.Expression<Func<TModel, KProperty>>, bool, IEnumerable<TModel>> dateRepository,
            System.Linq.Expressions.Expression<Func<TModel, KProperty>> sortOrder, bool asc,
            System.Linq.Expressions.Expression<Func<TModel, KProperty>> fieldNameExp, string criteria, 
            string reportHeader, string fileName)
        {
            try
            {
                var filter = CommonUtility.GetLamdaExpressionFromFilter<TModel>(criteria);
                IEnumerable<TModel> result = dateRepository(filter, sortOrder, asc);

                IList<string> propList = new List<string>();
                System.Linq.Expressions.NewExpression newExp = fieldNameExp.Body as System.Linq.Expressions.NewExpression;

                foreach (System.Linq.Expressions.Expression arg in newExp.Arguments)
                {
                    System.Linq.Expressions.MemberExpression me = arg as System.Linq.Expressions.MemberExpression;
                    propList.Add(me.Member.Name);
                }
                string exportPath = System.IO.Path.Combine(System.Web.HttpContext.Current.Server
                    .MapPath(string.Format("~/{0}", CommonUtility
                    .GetAppSetting<string>("ExportStorageFolder"))), fileName);

                using (CsvFileWriter writer = new CsvFileWriter(exportPath))
                {
                    IList<PropertyInfo> props = new List<PropertyInfo>();//typeof(TModel).GetProperties().Where(x => propList.Contains(x.Name)).ToList();
                    foreach (var item in propList)
                    {
                        props.Add(typeof(TModel).GetProperties().Where(x => x.Name == item).SingleOrDefault());
                    }
                    CsvRow row = new CsvRow();
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    foreach (var item in props)
                    {
                        DisplayAttribute attribute = item.GetCustomAttributes(typeof(DisplayAttribute), true)
                            .Select(prop => (DisplayAttribute)prop).FirstOrDefault();

                        if (attribute != null)
                            row.Add(String.Format("{0}", attribute.GetType().GetProperty(DisplayProperty.Name.ToString())
                               .GetValue(attribute, null).ToString()));
                        else
                            row.Add(String.Format("{0}", item.Name));
                    }
                    writer.WriteRow(row);

                    result.ToList().ForEach(y =>
                    {
                        row = new CsvRow();
                        sb = new System.Text.StringBuilder();
                        IList<PropertyInfo> props1 = new List<PropertyInfo>();
                        foreach (var item in propList)
                        {
                            props1.Add(typeof(TModel).GetProperties().Where(x => x.Name == item).SingleOrDefault());
                        }
                        foreach (var item in props1)
                        {
                            row.Add(String.Format("{0}", (item.GetValue(y, null) ?? "").ToString()));
                        }
                        writer.WriteRow(row);
                    });
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(dateRepository, sortOrder, asc, fieldNameExp, criteria);
            }
        }
    }
}