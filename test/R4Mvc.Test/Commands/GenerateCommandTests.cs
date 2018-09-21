using R4Mvc.Tools;
using R4Mvc.Tools.Commands;
using Xunit;

namespace R4Mvc.Test.Commands
{
    public class GenerateCommandTests
    {
        [Fact]
        public void AreaMap_NoAreas()
        {
            var controllers = new[]
            {
                new ControllerDefinition { Name = "Home", Area = "" },
                new ControllerDefinition { Name = "User", Area = "" },
            };
            var command = new GenerateCommand.Runner(null, null, null, null, null, new Settings(), null, null);

            var areaMap = command.GenerateAreaMap(controllers);
            Assert.Equal(0, areaMap.Count);
        }

        [Fact]
        public void AreaMap_NoClashes()
        {
            var controllers = new[]
            {
                new ControllerDefinition { Name = "Home", Area = "Admin" },
                new ControllerDefinition { Name = "Home", Area = "" },
                new ControllerDefinition { Name = "User", Area = "" },
            };
            var command = new GenerateCommand.Runner(null, null, null, null, null, new Settings(), null, null);

            var areaMap = command.GenerateAreaMap(controllers);
            Assert.Collection(areaMap,
                k => { Assert.Equal("Admin", k.Key); Assert.Equal("Admin", k.Value); });
        }

        [Fact]
        public void AreaMap_Clash()
        {
            var controllers = new[]
            {
                new ControllerDefinition { Name = "Home", Area = "Admin" },
                new ControllerDefinition { Name = "Home", Area = "User" },
                new ControllerDefinition { Name = "Home", Area = "" },
                new ControllerDefinition { Name = "User", Area = "" },
            };
            var command = new GenerateCommand.Runner(null, null, null, null, null, new Settings(), null, null);

            var areaMap = command.GenerateAreaMap(controllers);
            Assert.Collection(areaMap,
                k => { Assert.Equal("Admin", k.Key); Assert.Equal("Admin", k.Value); },
                k => { Assert.Equal("User", k.Key); Assert.Equal("UserArea", k.Value); });
        }

        [Fact]
        public void AreaMap_Clash_Double_Controller()
        {
            /// Currently unhandled, although why would you have a controller called "{X}Area" if you already have a "{X}" area?
            /// This will cause a duplicate property/field on the MVC class, with an area shortcut MVC.UserArea and a controller shortcut MVC.UserArea
            var controllers = new[]
            {
                new ControllerDefinition { Name = "Home", Area = "Admin" },
                new ControllerDefinition { Name = "Home", Area = "User" },
                new ControllerDefinition { Name = "Home", Area = "" },
                new ControllerDefinition { Name = "User", Area = "" },
                new ControllerDefinition { Name = "UserArea", Area = "" },
            };
            var command = new GenerateCommand.Runner(null, null, null, null, null, new Settings(), null, null);

            var areaMap = command.GenerateAreaMap(controllers);
            Assert.Collection(areaMap,
                k => { Assert.Equal("Admin", k.Key); Assert.Equal("Admin", k.Value); },
                k => { Assert.Equal("User", k.Key); Assert.Equal("UserArea", k.Value); });
        }

        [Fact]
        public void AreaMap_Clash_Double_Area()
        {
            /// Currently unhandled, although why would you have an area called "{X}Area" if you already have a "{X}" area?
            /// This will cause a duplicate property/field on the MVC class, with two areas being called MVC.UserArea
            var controllers = new[]
            {
                new ControllerDefinition { Name = "Home", Area = "Admin" },
                new ControllerDefinition { Name = "Home", Area = "User" },
                new ControllerDefinition { Name = "Home", Area = "" },
                new ControllerDefinition { Name = "User", Area = "" },
                new ControllerDefinition { Name = "User", Area = "UserArea" },
            };
            var command = new GenerateCommand.Runner(null, null, null, null, null, new Settings(), null, null);

            var areaMap = command.GenerateAreaMap(controllers);
            Assert.Collection(areaMap,
                k => { Assert.Equal("Admin", k.Key); Assert.Equal("Admin", k.Value); },
                k => { Assert.Equal("User", k.Key); Assert.Equal("UserArea", k.Value); },
                k => { Assert.Equal("UserArea", k.Key); Assert.Equal("UserArea", k.Value); });
        }
    }
}
