using BeCoreApp.Application.ViewModels.System;

namespace BeCoreApp.Application.Interfaces
{
    public interface ISystemConfigService
    {
        SystemConfigViewModel Get();

        void Update(SystemConfigViewModel Model);

        void Save();
    }
}
