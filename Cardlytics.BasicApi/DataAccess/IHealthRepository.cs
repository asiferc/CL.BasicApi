using Cardlytics.BasicApi.Models;

using Microsoft.EntityFrameworkCore;

namespace Cardlytics.BasicApi.DataAccess
{
    public interface IHealthRepository
    {
        DbContext Context { get; }

        Health VerifyDatabaseConnection();
    }
}