// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Input.Bindings;
using osu.Game.Overlays.Settings;

namespace osu.Game.Overlays.KeyBinding
{
    public class GlobalKeyBindingsSection : SettingsSection
    {
        public override Drawable CreateIcon() => new SpriteIcon
        {
            Icon = FontAwesome.Solid.Globe
        };

        public override string Header => "Global";

        public GlobalKeyBindingsSection(GlobalActionContainer manager)
        {
            Add(new DefaultBindingsSubsection(manager));
            Add(new AudioControlKeyBindingsSubsection(manager));
            Add(new SongSelectKeyBindingSubsection(manager));
            Add(new InGameKeyBindingsSubsection(manager));
            Add(new EditorKeyBindingsSubsection(manager));
        }

        private class DefaultBindingsSubsection : KeyBindingsSubsection
        {
            protected override LocalisableString Header => string.Empty;

            public DefaultBindingsSubsection(GlobalActionContainer manager)
                : base(null)
            {
                Defaults = manager.GlobalKeyBindings;
            }
        }

        private class SongSelectKeyBindingSubsection : KeyBindingsSubsection
        {
            protected override LocalisableString Header => "Song Select";

            public SongSelectKeyBindingSubsection(GlobalActionContainer manager)
                : base(null)
            {
                Defaults = manager.SongSelectKeyBindings;
            }
        }

        private class InGameKeyBindingsSubsection : KeyBindingsSubsection
        {
            protected override LocalisableString Header => "In Game";

            public InGameKeyBindingsSubsection(GlobalActionContainer manager)
                : base(null)
            {
                Defaults = manager.InGameKeyBindings;
            }
        }

        private class AudioControlKeyBindingsSubsection : KeyBindingsSubsection
        {
            protected override LocalisableString Header => "Audio";

            public AudioControlKeyBindingsSubsection(GlobalActionContainer manager)
                : base(null)
            {
                Defaults = manager.AudioControlKeyBindings;
            }
        }

        private class EditorKeyBindingsSubsection : KeyBindingsSubsection
        {
            protected override LocalisableString Header => "Editor";

            public EditorKeyBindingsSubsection(GlobalActionContainer manager)
                : base(null)
            {
                Defaults = manager.EditorKeyBindings;
            }
        }
    }
}
