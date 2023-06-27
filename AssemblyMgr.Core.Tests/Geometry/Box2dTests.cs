using AssemblyMgr.Core.Extensions;
using AssemblyMgr.Core.Geometry;
using NUnit.Framework;
using System;
using System.Linq;
using System.Numerics;

namespace AssemblyMgr.Core.Tests.Geometry
{
    [TestFixture]
    public class Box2dTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test] 
        public void TestBoxGeometry()
        {
            // arrange
            var height = 11;
            var width = 17;

            var expectedTopLeft = new Vector2(0, height);
            var expectedTopRight = new Vector2(width, height);
            var expectedBottomLeft = new Vector2(0, 0);
            var expectedBottomRight = new Vector2(width, 0);
            var expectedCenter = new Vector2(width / 2.0f, height / 2.0f);

            // act
            var box = new Box2d((0, 0), (width, height));

            // assert
            Assert.That(box.TopLeft, Is.EqualTo(expectedTopLeft));
            Assert.That(box.TopRight, Is.EqualTo(expectedTopRight));
            Assert.That(box.BottomLeft, Is.EqualTo(expectedBottomLeft));
            Assert.That(box.BottomRight, Is.EqualTo(expectedBottomRight));
            Assert.That(box.Center, Is.EqualTo(expectedCenter));
            Assert.That(box.Height, Is.EqualTo(height));
            Assert.That(box.Width, Is.EqualTo(width));
        }

        [Test]
        public void TestDivision()
        {
            // arrange
            float height = 11;
            float width = 17;
            float scale = 2;
            float scaledHeight = height / scale;
            float scaledWidth = width / scale;
            var box = new Box2d((0, 0), (width, height));

            var expectedTopLeft = new Vector2(0, scaledHeight);
            var expectedTopRight = new Vector2(scaledWidth, scaledHeight);
            var expectedBottomLeft = new Vector2(0, 0);
            var expectedBottomRight = new Vector2(scaledWidth, 0);
            var expectedCenter = new Vector2(scaledWidth / 2.0f, scaledHeight / 2.0f);

            // act
            var scaledBox = box / scale;

            // assert
            Assert.That(scaledBox.TopLeft, Is.EqualTo(expectedTopLeft));
            Assert.That(scaledBox.TopRight, Is.EqualTo(expectedTopRight));
            Assert.That(scaledBox.BottomLeft, Is.EqualTo(expectedBottomLeft));
            Assert.That(scaledBox.BottomRight, Is.EqualTo(expectedBottomRight));
            Assert.That(scaledBox.Center, Is.EqualTo(expectedCenter));
            Assert.That(scaledBox.Height, Is.EqualTo(scaledHeight));
            Assert.That(scaledBox.Width, Is.EqualTo(scaledWidth));
        }

        [Test]
        public void TestMultiplication()
        {
            // arrange
            float height = 11;
            float width = 17;
            float scale = 2;
            float scaledHeight = scale * height;
            float scaledWidth = scale * width;
            var box = new Box2d((0, 0), (width, height));

            var expectedTopLeft = new Vector2(0, scaledHeight);
            var expectedTopRight = new Vector2(scaledWidth, scaledHeight);
            var expectedBottomLeft = new Vector2(0, 0);
            var expectedBottomRight = new Vector2(scaledWidth, 0);
            var expectedCenter = new Vector2(scaledWidth / 2.0f, scaledHeight / 2.0f);

            // act
            var scaledBox = box * scale;

            // assert
            Assert.That(scaledBox.TopLeft, Is.EqualTo(expectedTopLeft));
            Assert.That(scaledBox.TopRight, Is.EqualTo(expectedTopRight));
            Assert.That(scaledBox.BottomLeft, Is.EqualTo(expectedBottomLeft));
            Assert.That(scaledBox.BottomRight, Is.EqualTo(expectedBottomRight));
            Assert.That(scaledBox.Center, Is.EqualTo(expectedCenter));
            Assert.That(scaledBox.Height, Is.EqualTo(scaledHeight));
            Assert.That(scaledBox.Width, Is.EqualTo(scaledWidth));
        }

        [Test]
        public void TestAddition()
        {
            // arrange
            float height = 11;
            float width = 17;
            var translation = new Vector2(3,5);
            var box = new Box2d((0, 0), (width, height));

            var expectedTopLeft = box.TopLeft + translation;
            var expectedTopRight = box.TopRight + translation;
            var expectedBottomLeft = box.BottomLeft + translation;
            var expectedBottomRight = box.BottomRight + translation;
            var expectedCenter = box.Center + translation;

            // act
            var translatedBox = box + translation;

            // assert
            Assert.That(translatedBox.TopLeft, Is.EqualTo(expectedTopLeft));
            Assert.That(translatedBox.TopRight, Is.EqualTo(expectedTopRight));
            Assert.That(translatedBox.BottomLeft, Is.EqualTo(expectedBottomLeft));
            Assert.That(translatedBox.BottomRight, Is.EqualTo(expectedBottomRight));
            Assert.That(translatedBox.Center, Is.EqualTo(expectedCenter));
            Assert.That(translatedBox.Height, Is.EqualTo(height));
            Assert.That(translatedBox.Width, Is.EqualTo(width));
        }

    }
}