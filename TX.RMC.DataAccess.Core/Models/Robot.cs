namespace TX.RMC.DataAccess.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using TX.RMC.DataAccess.Core.Contracts;

    public class Robot : IModel
    {
        public string Id { get; set; } = null!;

        public string NameIdentity { get; set; } = null!;
    }
}
