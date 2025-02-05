namespace TX.RMC.DataAccess.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using TX.RMC.DataAccess.Core.Contracts;

    public class User : IModel
    {
        public User(string name, string username, string secret, string salt)
        {
            this.Name = name;
            this.Username = username;
            this.Secret = secret;
            this.Salt = salt;
        }

        public User(Guid id, string name, string username, string secret, string salt)
            : this(name, username, secret, salt)
        {
            this.Id = id;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public string Secret { get; set; }

        public string Salt { get; set; }
    }
}
