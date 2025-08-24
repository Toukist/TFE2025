using Dior.Library.BO;

namespace Dior.Library.DAO
{
    public interface IContractDao
    {
        List<ContractBO> GetAll();
        List<ContractBO> GetByUserId(long userId);
        ContractBO? GetById(long id);
        void Create(ContractBO contract);
        void Delete(long id);
    }
}