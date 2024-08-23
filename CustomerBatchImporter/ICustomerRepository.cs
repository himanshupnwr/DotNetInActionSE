using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerBatchImporter
{
    public interface ICustomerRepository
    {
        Task CreateAsync(NewCustomerDto customer);

        Task UpdateAsync(UpdateCustomerDto customer);

        Task<Customer?> GetByEmailAsync(string email);
    }

    public record NewCustomerDto(string Email, string Name, string License)
    { }

    public record UpdateCustomerDto(int Id, string? NewName, string? NewLicense)
    { }
}