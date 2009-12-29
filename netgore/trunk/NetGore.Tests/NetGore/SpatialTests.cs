﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace NetGore.Tests.NetGore
{
    [TestFixture]
    public class SpatialTests
    {
        static readonly Vector2 SpatialSize = new Vector2(1024, 512);

        [Test]
        public void AddTest()
        {
            foreach (var spatial in GetSpatials())
            {
                var entity = new TestEntity();
                spatial.Add(entity);
                Assert.IsTrue(spatial.Contains(entity), "Current spatial: " + spatial);
            }
        }

        static IEnumerable<Entity> CreateEntities(int amount, Vector2 minPos, Vector2 maxPos)
        {
            Entity[] ret = new Entity[amount];
            for (int i = 0; i < amount; i++)
            {
                ret[i] = new TestEntity { Position = RandomHelperXna.NextVector2(minPos, maxPos) };
            }

            return ret;
        }

        [Test]
        public void GetEntitiesTest()
        {
            const int count = 25;
            Vector2 min = new Vector2(32, 64);
            Vector2 max = new Vector2(256, 128);
            Vector2 diff = max - min;

            foreach (var spatial in GetSpatials())
            {
                var entities = CreateEntities(count, min, max);
                spatial.Add(entities);

                foreach (var entity in entities)
                {
                    Assert.IsTrue(spatial.Contains(entity), "Current spatial: " + spatial);
                }

                var found = spatial.GetEntities(new Rectangle((int)min.X, (int)min.Y, (int)diff.X, (int)diff.Y));

                Assert.AreEqual(count, found.Count());
            }
        }

        static IEnumerable<ISpatialCollection> GetSpatials()
        {
            // TODO: !! Add tests back again when I re-add the good spatials
            var a = new LinearSpatialCollection();
            a.SetAreaSize(SpatialSize);

            var b = new LinearSpatialCollection();
            b.SetAreaSize(SpatialSize);

            /*
            var a = new DynamicEntitySpatial();
            a.SetMapSize(SpatialSize);

            var b = new StaticEntitySpatial();
            b.SetMapSize(SpatialSize);
            */

            return new ISpatialCollection[] { a, b };
        }

        [Test]
        public void MoveTest()
        {
            foreach (var spatial in GetSpatials())
            {
                var entity = new TestEntity();
                spatial.Add(entity);
                Assert.IsTrue(spatial.Contains(entity), "Current spatial: " + spatial);

                entity.Teleport(new Vector2(128, 128));
                Assert.IsTrue(spatial.ContainsEntities(new Vector2(128, 128)), "Current spatial: " + spatial);
                Assert.IsFalse(spatial.ContainsEntities(new Vector2(256, 128)), "Current spatial: " + spatial);
                Assert.IsFalse(spatial.ContainsEntities(new Vector2(128, 256)), "Current spatial: " + spatial);
            }
        }

        [Test]
        public void RemoveTest()
        {
            foreach (var spatial in GetSpatials())
            {
                var entity = new TestEntity();
                spatial.Add(entity);
                Assert.IsTrue(spatial.Contains(entity), "Current spatial: " + spatial);

                spatial.Remove(entity);
                Assert.IsFalse(spatial.Contains(entity), "Current spatial: " + spatial);
            }
        }

        class TestEntity : Entity
        {
            /// <summary>
            /// When overridden in the derived class, gets if this <see cref="Entity"/> will collide against
            /// walls. If false, this <see cref="Entity"/> will pass through walls and completely ignore them.
            /// </summary>
            public override bool CollidesAgainstWalls
            {
                get { return true; }
            }
        }
    }
}