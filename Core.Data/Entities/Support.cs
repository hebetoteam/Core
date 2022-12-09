using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Data.Enums;
using Core.Data.Interfaces;
using Core.Infrastructure.SharedKernel;

namespace Core.Data.Entities
{
    [Table("Supports")]
    public class Support : DomainEntity<int>, IDateTracking
    {
        [Required]
        public string Name { set; get; }

        public string RequestContent { set; get; }
        public string ResponseContent { set; get; }
        public SupportType Type { get; set; }
        public DateTime DateCreated { set; get; }
        public DateTime DateModified { set; get; }

        public Guid AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public virtual AppUser AppUser { set; get; }
    }
}
