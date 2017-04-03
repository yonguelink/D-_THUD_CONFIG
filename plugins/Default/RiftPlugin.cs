using System;
using System.Globalization;
using System.Linq;

namespace Turbo.Plugins.Default
{

    public class RiftPlugin : BasePlugin, IInGameTopPainter
	{

        public bool NephalemRiftPercentEnabled { get; set; }
        public IFont NephalemRiftPercentFont { get; set; }

        public bool GreaterRiftPercentEnabled { get; set; }
        public IFont GreaterRiftPercentFont { get; set; }

        public bool GreaterRiftTimerEnabled { get; set; }
        public IFont GreaterRiftTimerFont { get; set; }

        public bool NearMonsterProgressionEnabled { get; set; }
        public IFont NearMonsterProgressionFont { get; set; }
        public int NearMonsterProgressionRange { get; set; }

		public RiftPlugin()
		{
            Enabled = true;
            Order = 30000;
		}

        public override void Load(IController hud)
        {
            base.Load(hud);

            NephalemRiftPercentEnabled = true;
            NephalemRiftPercentFont = Hud.Render.CreateFont("tahoma", 7, 255, 255, 210, 150, true, false, 160, 0, 0, 0, true);

            GreaterRiftPercentEnabled = true;
            GreaterRiftPercentFont = Hud.Render.CreateFont("tahoma", 7, 255, 200, 100, 200, true, false, 160, 0, 0, 0, true);

            GreaterRiftTimerEnabled = true;
            GreaterRiftTimerFont = Hud.Render.CreateFont("tahoma", 7, 255, 180, 147, 109, true, false, 160, 0, 0, 0, true);

            NearMonsterProgressionEnabled = true;
            NearMonsterProgressionFont = Hud.Render.CreateFont("tahoma", 7, 255, 180, 147, 109, true, false, 160, 0, 0, 0, true);
            NearMonsterProgressionRange = 40;
		}

        public void PaintTopInGame(ClipState clipState)
        {
            if (clipState != ClipState.BeforeClip) return;

            if ((Hud.Game.SpecialArea != SpecialArea.Rift) && (Hud.Game.SpecialArea != SpecialArea.GreaterRift)) return;

            if (Hud.Game.SpecialArea == SpecialArea.Rift)
            {
                var percent = Hud.Game.RiftPercentage;
                var ui = Hud.Render.NephalemRiftBarUiElement;
                if (ui.Visible)
                {
                    var add = ui.Rectangle.Height / 10.0f;
                    if (NephalemRiftPercentEnabled)
                    {
                        var text = percent.ToString("F1", CultureInfo.InvariantCulture) + "%";
                        var textLayout = NephalemRiftPercentFont.GetTextLayout(text);
                        var x = (float)(ui.Rectangle.Left + ui.Rectangle.Width / 100.0f * percent) - textLayout.Metrics.Width / 2;
                        var y = ui.Rectangle.Top - ui.Rectangle.Height * 0.2f - textLayout.Metrics.Height;
                        NephalemRiftPercentFont.DrawText(textLayout, x, y);
                    }

                    if (NearMonsterProgressionEnabled && (Hud.Game.CurrentQuestProgress < Hud.Game.MaxQuestProgress))
                    {
                        var total = CalculateNearMonsterProgression(NearMonsterProgressionRange) / Hud.Game.MaxQuestProgress * 100d;
                        var text = total.ToString("F2", CultureInfo.InvariantCulture) + "% in " + NearMonsterProgressionRange.ToString("F0") + " yards";
                        var textLayout = NearMonsterProgressionFont.GetTextLayout(text);
                        var x = ui.Rectangle.Left + (ui.Rectangle.Width - textLayout.Metrics.Width) / 2;
                        var y = ui.Rectangle.Top + (ui.Rectangle.Height - textLayout.Metrics.Height) / 2 + 1;
                        NearMonsterProgressionFont.DrawText(textLayout, x, y);
                    }
                }
            }

            if (Hud.Game.SpecialArea == SpecialArea.GreaterRift)
            {
                var percent = Hud.Game.RiftPercentage;
                var ui = Hud.Render.GreaterRiftBarUiElement;
                if (ui.Visible)
                {
                    var secondsLeft = (Hud.Game.CurrentTimedEventEndTick - (double)Hud.Game.CurrentGameTick) / 60.0d;
                    if (GreaterRiftTimerEnabled && (secondsLeft > 0))
                    {
                        var text = ValueToString(((long)(Hud.Game.CurrentTimedEventEndTick - Hud.Game.CurrentGameTick) * 1000 * TimeSpan.TicksPerMillisecond / 60), ValueFormat.LongTime);
                        var textLayout = GreaterRiftTimerFont.GetTextLayout(text);
                        GreaterRiftTimerFont.DrawText(textLayout, ui.Rectangle.Right - (float)(ui.Rectangle.Width / 900.0f * secondsLeft) - textLayout.Metrics.Width / 2, ui.Rectangle.Bottom + ui.Rectangle.Height * 0.7f);
                    }

                    if (GreaterRiftPercentEnabled)
                    {
                        var text = percent.ToString("F1", CultureInfo.InvariantCulture) + "%";
                        var textLayout = GreaterRiftPercentFont.GetTextLayout(text);
                        var x = (float)(ui.Rectangle.Left + ui.Rectangle.Width / 100.0f * percent) - textLayout.Metrics.Width / 2;
                        var y = ui.Rectangle.Top - ui.Rectangle.Height * 0.7f - textLayout.Metrics.Height;
                        GreaterRiftPercentFont.DrawText(textLayout, x, y);
                    }

                    if (NearMonsterProgressionEnabled && (Hud.Game.CurrentQuestProgress < Hud.Game.MaxQuestProgress))
                    {
                        var total = CalculateNearMonsterProgression(NearMonsterProgressionRange) / Hud.Game.MaxQuestProgress * 100d;
                        var text = total.ToString("F2", CultureInfo.InvariantCulture) + "% in " + NearMonsterProgressionRange.ToString("F0") + " yards";
                        var textLayout = NearMonsterProgressionFont.GetTextLayout(text);
                        var x = ui.Rectangle.Left + (ui.Rectangle.Width - textLayout.Metrics.Width) / 2;
                        var y = ui.Rectangle.Top + (ui.Rectangle.Height - textLayout.Metrics.Height) / 2 + 1;
                        NearMonsterProgressionFont.DrawText(textLayout, x, y);
                    }
                }
            }
        }

        private float CalculateNearMonsterProgression(int range)
        {
            var monsters = Hud.Game.AliveMonsters.Where(x => (x.SnoMonster != null) && (x.NormalizedXyDistanceToMe <= range) && !((x.SummonerAcdDynamicId != 0) && (x.Rarity == ActorRarity.RareMinion)));
            return monsters.Sum(x => x.SnoMonster.RiftProgression);
        }

    }

}