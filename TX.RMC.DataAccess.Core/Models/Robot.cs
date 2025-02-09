namespace TX.RMC.DataAccess.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using TX.RMC.DataAccess.Core.Contracts;

    public class Robot : IModel
    {
        public object Id { get; set; }

        public string NameIdentity { get; set; } = string.Empty;
    }
}
