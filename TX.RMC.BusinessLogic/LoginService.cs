namespace TX.RMC.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LoginService
{
    public async Task<string> ValidateCredentialsAsync(string? username, string? password)
    {
        // TODO: Implement this method
        return await Task.FromResult(Guid.NewGuid().ToString());
    }
}
