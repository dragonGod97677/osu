// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Game.Online.Rooms;
using osu.Game.Rulesets.Catch;
using osu.Game.Rulesets.Osu;
using osu.Game.Screens.OnlinePlay.Lounge.Components;
using osu.Game.Tests.Visual.OnlinePlay;
using osuTK.Input;

namespace osu.Game.Tests.Visual.Multiplayer
{
    public class TestSceneLoungeRoomsContainer : OnlinePlayTestScene
    {
        protected new BasicTestRoomManager RoomManager => (BasicTestRoomManager)base.RoomManager;

        private RoomsContainer container;

        [SetUp]
        public new void Setup() => Schedule(() =>
        {
            Child = container = new RoomsContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Width = 0.5f,
            };
        });

        [Test]
        public void TestBasicListChanges()
        {
            AddStep("add rooms", () => RoomManager.AddRooms(3));

            AddAssert("has 3 rooms", () => container.Rooms.Count == 3);
            AddStep("remove first room", () => RoomManager.Rooms.Remove(RoomManager.Rooms.FirstOrDefault()));
            AddAssert("has 2 rooms", () => container.Rooms.Count == 2);
            AddAssert("first room removed", () => container.Rooms.All(r => r.Room.RoomID.Value != 0));

            AddStep("select first room", () => container.Rooms.First().Click());
            AddAssert("first room selected", () => checkRoomSelected(RoomManager.Rooms.First()));
        }

        [Test]
        public void TestKeyboardNavigation()
        {
            AddStep("add rooms", () => RoomManager.AddRooms(3));

            AddAssert("no selection", () => checkRoomSelected(null));

            press(Key.Down);
            AddAssert("first room selected", () => checkRoomSelected(RoomManager.Rooms.First()));

            press(Key.Up);
            AddAssert("first room selected", () => checkRoomSelected(RoomManager.Rooms.First()));

            press(Key.Down);
            press(Key.Down);
            AddAssert("last room selected", () => checkRoomSelected(RoomManager.Rooms.Last()));
        }

        [Test]
        public void TestClickDeselection()
        {
            AddStep("add room", () => RoomManager.AddRooms(1));

            AddAssert("no selection", () => checkRoomSelected(null));

            press(Key.Down);
            AddAssert("first room selected", () => checkRoomSelected(RoomManager.Rooms.First()));

            AddStep("click away", () => InputManager.Click(MouseButton.Left));
            AddAssert("no selection", () => checkRoomSelected(null));
        }

        private void press(Key down)
        {
            AddStep($"press {down}", () => InputManager.Key(down));
        }

        [Test]
        public void TestStringFiltering()
        {
            AddStep("add rooms", () => RoomManager.AddRooms(4));

            AddUntilStep("4 rooms visible", () => container.Rooms.Count(r => r.IsPresent) == 4);

            AddStep("filter one room", () => container.Filter(new FilterCriteria { SearchString = "1" }));

            AddUntilStep("1 rooms visible", () => container.Rooms.Count(r => r.IsPresent) == 1);

            AddStep("remove filter", () => container.Filter(null));

            AddUntilStep("4 rooms visible", () => container.Rooms.Count(r => r.IsPresent) == 4);
        }

        [Test]
        public void TestRulesetFiltering()
        {
            AddStep("add rooms", () => RoomManager.AddRooms(2, new OsuRuleset().RulesetInfo));
            AddStep("add rooms", () => RoomManager.AddRooms(3, new CatchRuleset().RulesetInfo));

            // Todo: What even is this case...?
            AddStep("set empty filter criteria", () => container.Filter(null));
            AddUntilStep("5 rooms visible", () => container.Rooms.Count(r => r.IsPresent) == 5);

            AddStep("filter osu! rooms", () => container.Filter(new FilterCriteria { Ruleset = new OsuRuleset().RulesetInfo }));
            AddUntilStep("2 rooms visible", () => container.Rooms.Count(r => r.IsPresent) == 2);

            AddStep("filter catch rooms", () => container.Filter(new FilterCriteria { Ruleset = new CatchRuleset().RulesetInfo }));
            AddUntilStep("3 rooms visible", () => container.Rooms.Count(r => r.IsPresent) == 3);
        }

        [Test]
        public void TestPasswordProtectedRooms()
        {
            AddStep("add rooms", () => RoomManager.AddRooms(3, withPassword: true));
        }

        private bool checkRoomSelected(Room room) => SelectedRoom.Value == room;
    }
}
