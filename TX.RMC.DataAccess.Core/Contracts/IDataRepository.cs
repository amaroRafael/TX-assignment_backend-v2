namespace TX.RMC.DataAccess.Core.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public interface IDataRepository<TModel> where TModel : IModel
    {
        ValueTask<TModel> GetByIdAsync(Guid id);
    }
}
