/****************************************************************************
 * Copyright (c) 2017 ouyanggongping@putao.com
 * Copyright (c) 2017 liqingyun@putao.com
****************************************************************************/

using PTGame.Core;

namespace PTGame.Framework
{
	using System.Collections.Generic;

    public class AbstractModuleMgr : AbstractActor 
    {
        private Dictionary<string, IModule> mModuleMgrMap = new Dictionary<string,IModule>();

        public IModule GetModule(string name)
        {
            IModule ret;
            if (mModuleMgrMap.TryGetValue(name, out ret))
            {
                return ret;
            }
            return null;
        }

        protected override void OnAddComponent(IActorComponent actorComponent)
        {
            if (actorComponent is IModule)
            {
                IModule module = actorComponent as IModule;
                string name = module.GetType().Name;
                if (mModuleMgrMap.ContainsKey(name))
                {
                    PTDebug.LogError("ModuleMgr Already Add Module:" + name);
                    return;
                }
                mModuleMgrMap.Add(name, module);

                OnModuleRegister(module);
            }
        }

        protected virtual void OnModuleRegister(IModule module)
        {

        }
    }
}
