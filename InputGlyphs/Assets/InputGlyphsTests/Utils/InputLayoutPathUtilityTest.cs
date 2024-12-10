using InputGlyphs.Utils;
using NUnit.Framework;

namespace InputGlyphs.Tests
{
    public class InputLayoutPathUtilityTest
    {
        [TestCase("/XInputController/buttonSouth", "buttonSouth")]
        [TestCase("/XInputController/dpad/right", "dpad/right")]
        [TestCase("/XInputController/<Button>", "<Button>")]
        [TestCase("/XInputController/{Submit}", "{Submit}")]
        [TestCase("/XInputController/#(a)", "#(a)")]
        [TestCase("/<Gamepad>/buttonSouth", "buttonSouth")]
        [TestCase("/<Gamepad>/buttonSouth", "buttonSouth")]
        [TestCase("XInputController/buttonSouth", "buttonSouth")]
        [TestCase("*/buttonSouth", "buttonSouth")]
        public void RemoveRoot(string input, string expected)
        {
            var result = InputLayoutPathUtility.RemoveRoot(input);
            Assert.AreEqual(expected, result);
        }

        [TestCase("/leftStick/x", "/leftStick")]
        [TestCase("/leftStick", "")]
        [TestCase("/leftStick/x/y", "/leftStick/x")]
        [TestCase("leftStick/x", "leftStick")]
        [TestCase("leftStick", "")]
        [TestCase("leftStick/x/y", "leftStick/x")]
        [TestCase("", "")]
        public void GetParent(string input, string expected)
        {
            var result = InputLayoutPathUtility.GetParent(input);
            Assert.AreEqual(expected, result);
        }
    }
}
