namespace TX.RMC.BusinessLogic;

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TX.RMC.BusinessLogic.Security.Crypt;
using TX.RMC.DataAccess.Core.Contracts;
using TX.RMC.DataAccess.Core.Models;

public class UserService(IUserDataRepository userDataRepository)
{
    private readonly IUserDataRepository userDataRepository = userDataRepository;

    /// <summary>
    /// Validate user credentials
    /// </summary>
    /// <exception cref="ArgumentException">Parameter (username or password) is missing.</exception>
    /// <exception cref="ApplicationException">Username and/or password doesn't match data from database.</exception>
    public async Task<string?> ValidateCredentialsAsync(string? username, string? password)
    {
        if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username is required.", nameof(username));
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password is required.", nameof(password));

        User? user = await this.userDataRepository.GetByUsernameAsync(username);

        if (user is User userFound)
        {
            byte[] salt = Convert.FromBase64String(userFound.Salt);
            string secret = Pbkdf2Hasher.ComputeHash(password, salt);

            if (secret == userFound.Secret)
            {
                return userFound.Id.ToString();
            }
        }

        throw new ApplicationException("Username and/or password are invalid.");
    }

    /// <summary>
    /// Add a user to the database.
    /// </summary>
    /// <exception cref="ArgumentException">Parameter (name, username or password) is missing.</exception>
    /// <exception cref="InvalidCastException">User retrieved from data with the same username. Username must be unique.</exception>
    public async ValueTask<User> AddAsync(string name, string username, string password)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username is required.", nameof(username));
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password is required.", nameof(password));

        User? user = await this.userDataRepository.GetByUsernameAsync(username);

        if (user is User userFound) throw new InvalidCastException("Username is invalid. Already used.");

        byte[] salt = Pbkdf2Hasher.GenerateRandomSalt();
        string secret = Pbkdf2Hasher.ComputeHash(password, salt);
        string saltBase64 = Convert.ToBase64String(salt);

        return await userDataRepository.AddAsync(new User { Name = name, Username = username, Secret = secret, Salt = saltBase64 });
    }

    /// <summary>
    /// Retrieves user from database.
    /// </summary>
    /// <param name="id">User identity.</param>
    /// <returns>Returns the user.</returns>
    public async Task<User?> GetAsync(object id)
    {
        User? user = await this.userDataRepository.GetByIdAsync(id);
        return user;

    }
}
