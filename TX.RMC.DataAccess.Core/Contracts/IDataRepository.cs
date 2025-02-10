namespace TX.RMC.DataAccess.Core.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using TX.RMC.DataAccess.Core.Models;

    public interface IDataRepository<TModel> where TModel : class, IModel
    {
        ValueTask<TModel> AddAsync(TModel model, CancellationToken cancellationToken = default);
    }
}
