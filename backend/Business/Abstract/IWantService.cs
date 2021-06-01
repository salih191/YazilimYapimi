using Core.Utilities.Results;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IWantService
    {
        IResult Want(Product product);
    }
}