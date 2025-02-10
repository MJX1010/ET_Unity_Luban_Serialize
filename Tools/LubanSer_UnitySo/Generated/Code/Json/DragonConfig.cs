
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using SimpleJSON;


namespace cfg
{
public sealed partial class DragonConfig : Luban.BeanBase
{
    public DragonConfig(JSONNode _buf) 
    {
        { if(!_buf["normalBullet"].IsObject) { throw new SerializationException(); }  NormalBullet = BulletConfig.DeserializeBulletConfig(_buf["normalBullet"]);  }
        { if(!_buf["damage"].IsNumber) { throw new SerializationException(); }  Damage = _buf["damage"]; }
    }

    public static DragonConfig DeserializeDragonConfig(JSONNode _buf)
    {
        return new DragonConfig(_buf);
    }

    public readonly BulletConfig NormalBullet;
    public readonly float Damage;
   
    public const int __ID__ = -828872875;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
        NormalBullet?.ResolveRef(tables);
    }

    public override string ToString()
    {
        return "{ "
        + "normalBullet:" + NormalBullet + ","
        + "damage:" + Damage + ","
        + "}";
    }
}

}

