using Core.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Entities
{
    [Table("Configs")]
    public class Config : DomainEntity<int>
    {
        public string Name { get;set;}

        public string Value { get;set;}

        public string Remarks { get;set;}

    }
}
