using System;
using System.Collections;
using System.Collections.Generic;
using Forge.Editor.Messages;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Forge.Editor.Tests
{
	public class MessagePropertyTests
	{
		[Test]
		public void SetPropertyName_ShouldBeEqual()
		{
			string name = "ForgeNetworking-Alloy";
			MessageProperty property = new MessageProperty();
			property.Name = name;
			Assert.AreEqual(name, property.Name);
		}

		[Test]
		public void SetPropertyType_ShouldBeEqual()
		{
			Type type = typeof(int);
			MessageProperty property = new MessageProperty();
			property.PropertyType = type;
			Assert.AreEqual(type, property.PropertyType);
		}
	}
}
