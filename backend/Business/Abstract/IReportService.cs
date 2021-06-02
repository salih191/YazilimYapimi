using Core.Utilities.Results;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IReportService
    {
        IResult Report(ReportInfo reportInfo);
    }
}