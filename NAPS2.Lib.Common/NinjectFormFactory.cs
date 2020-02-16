﻿using NAPS2.Config;
using NAPS2.WinForms;
using Ninject;

namespace NAPS2
{
    public class NinjectFormFactory : IFormFactory
    {
        private readonly IKernel _kernel;

        public NinjectFormFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public T Create<T>() where T : FormBase
        {
            var form = _kernel.Get<T>();
            form.FormFactory = _kernel.Get<IFormFactory>();
            form.ConfigScopes = _kernel.Get<ConfigScopes>();
            form.ConfigProvider = _kernel.Get<ScopeSetConfigProvider<CommonConfig>>();
            return form;
        }
    }
}
