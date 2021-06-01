using Core.Utilities.Results;
using Entities.Abstract;

namespace Business.Abstract
{
    public interface IReportService
    {
        IDataResult<IReport> Report();
    }
}