using System;
using System.Collections.Generic;

namespace Pangu.Frame.Core
{
    public class ModuleContainer
    {
        private Dictionary<Type, object> _modules = new Dictionary<Type, object>();

        private static ModuleContainer _instance;

        public static ModuleContainer Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ModuleContainer();
                return _instance;
            }
        }

        public ModuleContainer RegisterModule<Module,IModule>() where Module : IModule
        {
            var type = typeof(IModule);
            
            if (!_modules.ContainsKey(type))
            {
                object obj = Activator.CreateInstance(typeof(Module));
                RegisterModule<IModule>((IModule)obj);
            }

            return Instance;
        }

        public IMoudle Query<IMoudle>()
        {
            object obj = null;

            if (_modules.TryGetValue(typeof(IMoudle), out obj))
            {
                return (IMoudle)obj;
            }
            
            return default(IMoudle);
        }

        public void Tick(float deltaTime)
        {

        }

        public void LateTick(float deltaTime)
        {

        }

        private ModuleContainer RegisterModule<IModule>(object obj)
        {
            if (obj == null)
                return Instance;

            var type = typeof(IModule);

            if (!_modules.ContainsKey(type))
            {
                _modules.Add(type, obj);
            }

            return Instance;
        }
    }
}

