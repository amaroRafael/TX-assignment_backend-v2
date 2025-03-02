namespace TX.RMC.DataAccess.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using TX.RMC.DataAccess.Core.Contracts;

    public class User : IModel
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Secret { get; set; } = null!;

        public string Salt { get; set; } = null!;
    }
}
