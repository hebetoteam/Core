using Core.Infrastructure.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeCoreApp.Data.Entities
{
    [Table("SystemConfigs")]
    public class SystemConfig : DomainEntity<int>
    {
        public decimal TokenPrice { get; set; }
        public DateTime DateModified { get; set; }
    }
}