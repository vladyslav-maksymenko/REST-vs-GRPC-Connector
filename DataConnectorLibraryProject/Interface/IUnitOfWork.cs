using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConnectorLibraryProject.Interface
{
    internal interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>(Type entryType) where TEntity : class;
        Task SaveAsync();
    }
}
