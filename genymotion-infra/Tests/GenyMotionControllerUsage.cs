using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Webdriver_infra.Infra;

namespace genymotion_infra.Tests
{
    [TestClass]
    public class GenyMotionControllerUsage
    {
        private GenyMotionController controller;

        private string vmName = "s5";

        [TestInitialize]
        public void Setup()
        {
            controller = new GenyMotionController();
        }

        [TestMethod]
        public void TestCloneLaunchKill()
        {            
            string newName = vmName + "_" + new Random().Next(1, 10000);            
            controller.DeleteVm(newName);
            controller.CloneVm(vmName, newName);
            controller.StartVM(newName);
            Thread.Sleep(20000);
            controller.StopVm(newName);
            if (!controller.DeleteVm(newName))
            {
                throw new Exception("Failed to delete cloned machine");
            }
        }
    }
}
