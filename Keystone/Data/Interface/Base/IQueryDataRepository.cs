
namespace Keystone.Web.Data.Interface.Base
{
    #region Required Namespace(s)
    using Keystone.Web.Models.Base;
    using System.Collections.Generic;
    #endregion

    public interface IQueryDataRepository
    {
        IEnumerable<TModel> ExecuteQuery<TModel>(string sqlQuery, params object[] parameters)
             where TModel : BaseModel;

        int ExecuteCommand(string sqlCommand, params object[] parameters);
    }
}
