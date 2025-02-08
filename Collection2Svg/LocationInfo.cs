using System;
using System.Collections.Generic;

namespace Collection2Svg
{
    public class LocationDetails
    {
        public string locationName { get; set; }
        public string locationFullName { get; set; }
        public int backgroundYOffset { get; set; }
    }

    public class Background
    {
        public string backgroundName { get; set; }
        public string daytime { get; set; }
        public string puzzleEffectSlotBackgroundName { get; set; }
        public string puzzleGridBackgroundName { get; set; }
        public string puzzleGridBorderName { get; set; }
    }

    public class LocationInfo
    {
        public LocationDetails LocationDetails { get; set; }
        public List<Background> Backgrounds { get; set; }
    }
}
