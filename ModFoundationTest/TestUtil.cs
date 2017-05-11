using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModFoundationTest
{
public static class TestUtil
{
    public static void AreClose(float us, float them, float part = 0.00001f)
    {
        if (Math.Abs(us - them) > Math.Abs(part * us))
        {
            Assert.Fail("AreClose failed. Expected<" + us + ">. Actual<" + them + ">");
        }
    }
}
}
