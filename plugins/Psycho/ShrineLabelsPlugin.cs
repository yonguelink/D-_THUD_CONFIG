using Turbo.Plugins.Default;
using System.Collections.Generic;
using System.Linq;
using System;
namespace Turbo.Plugins.Psycho
{
    public class ShrineLabelsPlugin : BasePlugin, ICustomizer, IInGameWorldPainter
    {
        public Dictionary<ShrineType, WorldDecoratorCollection> ShrineDecorators { get; set; }
        public Dictionary<ShrineType, WorldDecoratorCollection> ShrineShortDecorators { get; set; }
        public Dictionary<ShrineType, string> ShrineCustomNames { get; set; }
        public Dictionary<ShrineType, string> ShrineCustomNamesShort { get; set;}
        public WorldDecoratorCollection PossibleRiftPylonDecorators { get; set; }
        public string PossibleRiftPylonName { get; set; }
        public bool ShowHealingWells { get; set;}
        public bool ShowPoolOfReflection { get; set;}

        public ShrineLabelsPlugin()
        {
            Enabled = true;
        }

        public WorldDecoratorCollection CreateMapDecorators(float size = 6f, int a = 192, int r = 255, int g = 255, int b = 55, float radiusOffset = 5f)
        {
            return new WorldDecoratorCollection(
                new MapLabelDecorator(Hud)
                {
                    LabelFont = Hud.Render.CreateFont("tahoma", size, a, r, g, b, false, false, 128, 0, 0, 0, true),
                    RadiusOffset = radiusOffset,
                });
        }

        public WorldDecoratorCollection CreateGroundLabelDecorators(float size = 6f, int a = 192, int r = 255, int g = 255, int b = 55,int bga = 255, int bgr = 0, int bgg = 0, int bgb = 0)
        {
            var grounLabelBackgroundBrush = Hud.Render.CreateBrush(bga, bgr, bgg, bgb, 0);
            return new WorldDecoratorCollection(
                new GroundLabelDecorator(Hud)
                {
                    BackgroundBrush = grounLabelBackgroundBrush,
                    BorderBrush = Hud.Render.CreateBrush(a, r, g, b, 1),
                    TextFont = Hud.Render.CreateFont("tahoma", size, a, r, g, b, false, false, 128, 0, 0, 0, true),
                });
        }
        
        public override void Load(IController hud)
        {
            base.Load(hud);

            ShrineDecorators = new Dictionary<ShrineType, WorldDecoratorCollection>();
            ShrineShortDecorators = new Dictionary<ShrineType, WorldDecoratorCollection>();
            ShrineCustomNames = new Dictionary<ShrineType, string>();
            ShrineCustomNamesShort = new Dictionary<ShrineType, string>();

            foreach (ShrineType shrine in Enum.GetValues(typeof(ShrineType)))
            {
                ShrineDecorators[shrine] = CreateMapDecorators();
                ShrineShortDecorators[shrine] = CreateGroundLabelDecorators();
                ShrineCustomNames[shrine] = string.Empty;
                ShrineCustomNamesShort[shrine] = string.Empty;
            }
            PossibleRiftPylonDecorators = CreateMapDecorators();
            PossibleRiftPylonName = string.Empty;
        }

        public void Customize()
        {
            Hud.TogglePlugin<ShrinePlugin>(false);
        }

        public void PaintWorld(WorldLayer layer)
        {
            var shrines = Hud.Game.Shrines.Where(s => s.DisplayOnOverlay);
            foreach (var shrine in shrines)
            {
                if (shrine.Type == ShrineType.HealingWell && ShowHealingWells == false) continue;
                if (shrine.Type == ShrineType.PoolOfReflection && ShowPoolOfReflection == false) continue;
                
                var shrineName = (ShrineCustomNames[shrine.Type] != string.Empty) ? ShrineCustomNames[shrine.Type] : shrine.SnoActor.NameLocalized;
                ShrineDecorators[shrine.Type].Paint(layer, shrine, shrine.FloorCoordinate, shrineName);

                var ShrineNameShort = (ShrineCustomNamesShort[shrine.Type] != string.Empty) ? ShrineCustomNamesShort[shrine.Type] : shrine.SnoActor.NameLocalized;
                ShrineShortDecorators[shrine.Type].Paint(layer, shrine, shrine.FloorCoordinate, ShrineNameShort);
            }

            var riftPylonSpawnPoints = Hud.Game.Actors.Where(x => x.SnoActor.Sno == 428690);
            foreach (var actor in riftPylonSpawnPoints)
            {
                PossibleRiftPylonDecorators.Paint(layer, actor, actor.FloorCoordinate, (PossibleRiftPylonName != string.Empty) ? PossibleRiftPylonName : "Pylon?");
            }
        }
    }
}