using System;
using System.Collections.Generic;

namespace Pangu.Frame.Core
{
    public class ModuleContainer
    {
        private Dictionary<Type, Type> _unActivationTypes = new Dictionary<Type, Type>();

        private Dictionary<Type, object> _activationModules = new Dictionary<Type, object>();

        private List<IModuleTick> _ticks = new List<IModuleTick>();

        private List<IModuleLateTick> _lateTicks = new List<IModuleLateTick>();

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

        public ModuleContainer RegisterModule<Module, IModule>(bool loadimm) where Module : IModule
        {
            var type = typeof(IModule);

            if (HasRegisterModule(type))
            {
                return Instance;
            }
            
            var objType = typeof(Module);

            if (loadimm)
            {
                MakeModule(type, objType);
            }
            else
            {
                _unActivationTypes.Add(type, objType);
            }

            return Instance;
        }

        public IModule Query<IModule>()
        {
            var itype = typeof(IModule);
            
            object obj = null;

            if (_activationModules.TryGetValue(itype, out obj))
            {
                return (IModule)obj;
            }
            
            Type objType = null;
            
            if(_unActivationTypes.TryGetValue(itype, out objType))
            {
                obj = MakeModule(itype, objType);
                return (IModule)obj;
            }
            
            return default(IModule);
        }

        private object MakeModule(Type type, Type objType) 
        {
            object obj = Activator.CreateInstance(objType);

            if(obj != null)
            {
                RegisterObjectModule(type, obj);
                
                if(obj is IModuleTick)
                {
                    _ticks.Add((IModuleTick)obj);
                }

                if(obj is IModuleLateTick)
                {
                    _lateTicks.Add((IModuleLateTick)obj);
                }
            }
            
            return obj;
        }

        private ModuleContainer RegisterObjectModule(Type type,object obj)
        {
            if (obj == null)
                return Instance;

            if (!_activationModules.ContainsKey(type))
            {
                _activationModules.Add(type, obj);
            }

            return Instance;
        }

        private bool HasRegisterModule(Type type)
        {
            if(_activationModules.ContainsKey(type))
            {
                return true;
            }
            else if(_unActivationTypes.ContainsKey(type))
            {
                return true;
            }
            return false;
        }
        
        internal void Tick(float deltaTime)
        {
            foreach (var t in _ticks)
            {
                t.Tick(deltaTime);
            }
        }

        internal void LateTick(float deltaTime)
        {
            foreach (var t in _lateTicks)
            {
                t.LateTick(deltaTime);
            }
        }

    }
}
