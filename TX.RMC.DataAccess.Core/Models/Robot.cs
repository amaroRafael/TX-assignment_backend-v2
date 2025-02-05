namespace TX.RMC.DataAccess.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using TX.RMC.DataAccess.Core.Contracts;

    public class Robot : IModel
    {
        public Robot(string nameIdentity)
        {
            this.NameIdentity = nameIdentity;
        }

        public Robot(Guid id, string nameIdentity)
            : this(nameIdentity)
        {
            this.Id = id;
        }

        public Guid Id { get; set; }

        public string NameIdentity { get; set; }
    }
}
