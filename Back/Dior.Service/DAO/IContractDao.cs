using Dior.Library.BO;

namespace Dior.Library.DAO
{
    public interface IContractDao
    {
        List<ContractBO> GetAll();
        List<ContractBO> GetByUserId(int userId);
        ContractBO? GetById(int id);
        void Create(ContractBO contract);
        void Delete(int id);
    }
}