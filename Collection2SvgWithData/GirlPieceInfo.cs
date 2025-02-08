using System;
using System.Collections.Generic;

namespace Collection2SvgWithData
{
    public class GirlDetails
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string favoriteColor { get; set; }
        public string defaultExpression { get; set; }
        public string defaultFootwear { get; set; }
        public string defaultHairstyle { get; set; }
        public string defaultOutfit { get; set; }
        public string bonusRoundExpression { get; set; }
        public string bonusRoundHairstyle { get; set; }
    }

    public class PieceArt
    {
        public string name { get; set; }
        public string spriteName { get; set; }
        public int x { get; set; }
        public int y { get; set; }
    }

    public class Art
    {
        public string spriteName { get; set; }
        public int x { get; set; }
        public int y { get; set; }
    }

    public class PiecePreset
    {
        public int index { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string layer { get; set; }
        public string expressionType { get; set; }
        public bool hideOnDates { get; set; }
        public string limitToOutfits { get; set; }
        public double showChance { get; set; }
        public bool underwear { get; set; }
        public List<Art> art { get; set; }
    }

    public class GirlPieceInfo
    {
        public GirlDetails GirlDetails { get; set; }
        public List<PieceArt> PieceArt { get; set; }
        public List<PiecePreset> PiecePresets { get; set; }
    }
}
