using AssemblyMgr.Core.DataModel;
using NUnit.Framework;

namespace AssemblyMgr.Core.Tests.DataModel
{
    [TestFixture]
    internal class DirectionEnumTests
    {
        [SetUp] public void Setup() { }

        [TearDown] public void Teardown() { }

        [TestCase(Direction3d.N | Direction3d.E | Direction3d.Down, ViewCubeCorner.BottomFrontRight)]
        [TestCase(Direction3d.N | Direction3d.E | Direction3d.Up, ViewCubeCorner.TopFrontRight)]
        [TestCase(Direction3d.N | Direction3d.W | Direction3d.Down, ViewCubeCorner.BottomFrontLeft)]
        [TestCase(Direction3d.N | Direction3d.W | Direction3d.Up, ViewCubeCorner.TopFrontLeft)]
        [TestCase(Direction3d.S | Direction3d.E | Direction3d.Down, ViewCubeCorner.BottomBackRight)]
        [TestCase(Direction3d.S | Direction3d.E | Direction3d.Up, ViewCubeCorner.TopBackRight)]
        [TestCase(Direction3d.S | Direction3d.W | Direction3d.Down, ViewCubeCorner.BottomBackLeft)]
        [TestCase(Direction3d.S | Direction3d.W | Direction3d.Up, ViewCubeCorner.TopBackLeft)]
        public void Direction_ViewCube(Direction3d direction3D, ViewCubeCorner viewCube)
        {
            Assert.That(direction3D, Is.EqualTo((Direction3d)viewCube));
        }
        [TestCase(Direction3d.E, ElevationOrientation.Right)]
        [TestCase(Direction3d.N, ElevationOrientation.Front)]
        [TestCase(Direction3d.S, ElevationOrientation.Back)]
        [TestCase(Direction3d.W, ElevationOrientation.Left)]
        public void Direction_ElevationOrientation(Direction3d direction3D, object elevationOrientation)
        {
            Assert.That(direction3D, Is.EqualTo((Direction3d)elevationOrientation));
        }
        [TestCase(Direction3d.N | Direction3d.E, Quadrant.TopRight)]
        [TestCase(Direction3d.N | Direction3d.W, Quadrant.TopLeft)]
        [TestCase(Direction3d.S | Direction3d.E, Quadrant.BottomRight)]
        [TestCase(Direction3d.S | Direction3d.W, Quadrant.BottomLeft)]
        public void Direction_Quadrant(Direction3d direction3D, object elevationOrientation)
        {
            Assert.That(direction3D, Is.EqualTo((Direction3d)elevationOrientation));
        }
        [TestCase(Direction3d.N | Direction3d.E, Box2dLocation.TopRight)]
        [TestCase(Direction3d.N | Direction3d.W, Box2dLocation.TopLeft)]
        [TestCase(Direction3d.S | Direction3d.E, Box2dLocation.BottomRight)]
        [TestCase(Direction3d.S | Direction3d.W, Box2dLocation.BottomLeft)]
        [TestCase(Direction3d.Center, Box2dLocation.Center)]
        public void Direction_Box2dLocation(Direction3d direction3D, object elevationOrientation)
        {
            Assert.That(direction3D, Is.EqualTo((Direction3d)elevationOrientation));
        }

    }
}
