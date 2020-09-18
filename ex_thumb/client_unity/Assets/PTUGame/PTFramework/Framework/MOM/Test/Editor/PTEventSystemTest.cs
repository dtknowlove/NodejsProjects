/****************************************************************************
 * Copyright (c) 2017 yuanhuibin@putao.com
 * Copyright (c) 2018.3 liqingyun@putao.com
 ****************************************************************************/

namespace PTGame.Framework.Test.Core
{
    using NUnit.Framework;
    using PTGame.Core;
    
    public class EventSystemTest
    {
        [Test]
        public void EventSystemTest_Register()
        {
            var key = 10000;
            var registerCount = 0;
            var rigisters1 = PTEventSystem.Instance.Register(key, delegate
            {
                registerCount++;
            });
            PTEventSystem.Instance.Send(key);
            Assert.AreEqual(1, registerCount);
            var rigisters2 = PTEventSystem.Instance.Register(key, delegate
            {
                registerCount++;
            });
            PTEventSystem.Instance.Send(key);
            Assert.AreEqual(3, registerCount);
        }

        [Test]
        public void EventSystemTest_UnRegister()
        {
            var key = 20000;
            var registerCount = 0;
            var rigisters = PTEventSystem.Instance.Register(key, delegate
            {
                registerCount++;
            });
            PTEventSystem.Instance.Send(key);
            Assert.AreEqual(1, registerCount);
            PTEventSystem.Instance.UnRegister(key, delegate
            {
                registerCount--;
            });
            Assert.AreEqual(1, registerCount);
        }

        [Test]
        public void EventSystemTest_Send()
        {
            var key = 30000;
            var registerCount = 0;
            var rigisters = PTEventSystem.Instance.Register(key, delegate
            {
                registerCount++;
            });
            var sendValue = true;
            var backValue = PTEventSystem.Instance.Send(key);
            Assert.AreEqual(sendValue, backValue);
        }
    }
}