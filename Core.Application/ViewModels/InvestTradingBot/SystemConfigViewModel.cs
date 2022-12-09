using System;

namespace BeCoreApp.Application.ViewModels.System
{
    public class SystemConfigViewModel
    {
        public SystemConfigViewModel()
        {
        }
        public int Id { get; set; }
        public decimal TokenPrice { get; set; }
        public DateTime DateModified { get; set; }
    }
}
