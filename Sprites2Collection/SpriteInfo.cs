using System;
using System.Drawing;

public class SpriteInfo
{
    private string _collectionName;
    private string _name;
    private int _uvIndex;
    private Rectangle _rect = new Rectangle();
    private bool _flipped;
    private bool _unnamed;

    public string CollectionName
    {
        get { return _collectionName; }
        set { _collectionName = value; }
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public int UVIndex
    {
        get { return _uvIndex; }
        set { _uvIndex = value; }
    }

    public Rectangle Rect
    {
        get { return _rect; }
        set { _rect = value; }
    }

    public bool Flipped
    {
        get { return _flipped; }
        set { _flipped = value; }
    }

    public bool Unnamed
    {
        get { return _unnamed; }
        set { _unnamed = value; }
    }

    public SpriteInfo()
    {
    }

    public SpriteInfo FromCsv(string csvLine)
    {
        string[] spriteInfo = csvLine.Split(',');

        this._collectionName = spriteInfo[0];
        this._name = spriteInfo[1];
        this._uvIndex = Convert.ToInt32(spriteInfo[2]);
        this._rect.X = Convert.ToInt32(spriteInfo[3]);
        this._rect.Y = Convert.ToInt32(spriteInfo[4]);
        this._rect.Width = Convert.ToInt32(spriteInfo[5]);
        this._rect.Height = Convert.ToInt32(spriteInfo[6]);
        this._flipped = Convert.ToBoolean(spriteInfo[7]);
        this._unnamed = false;

        spriteInfo = null;
        return this;
    }
}
