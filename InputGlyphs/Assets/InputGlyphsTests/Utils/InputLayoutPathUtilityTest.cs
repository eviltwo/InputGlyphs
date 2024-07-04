using InputGlyphs.Utils;
using NUnit.Framework;

public class InputLayoutPathUtilityTest
{
    [TestCase("XInputController/buttonSouth", "buttonSouth")]
    [TestCase("XInputController/dpad/left", "dpad/left")]
    [TestCase("<gamepad>/dpad/up", "dpad/up")]
    [TestCase("<gamepad>", "")]
    [TestCase("<gamepad>/", "")]
    public void GetLocalPath(string input, string expected)
    {
        var result = InputLayoutPathUtility.GetLocalPath(input);
        Assert.AreEqual(expected, result);
    }
}
