
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
public sealed partial class BulletDisplayConfig : Luban.BeanBase
{
    public BulletDisplayConfig(JSONNode _buf) 
    {
        { if(!_buf["normalDragon"].IsObject) { throw new SerializationException(); }  NormalDragon = BulletConfig.DeserializeBulletConfig(_buf["normalDragon"]);  }
        { if(!_buf["displayTableId"].IsNumber) { throw new SerializationException(); }  DisplayTableId = _buf["displayTableId"]; }
    }

    public static BulletDisplayConfig DeserializeBulletDisplayConfig(JSONNode _buf)
    {
        return new BulletDisplayConfig(_buf);
    }

    public readonly BulletConfig NormalDragon;
    public readonly int DisplayTableId;
   
    public const int __ID__ = -2113096542;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
        NormalDragon?.ResolveRef(tables);
    }

    public override string ToString()
    {
        return "{ "
        + "normalDragon:" + NormalDragon + ","
        + "displayTableId:" + DisplayTableId + ","
        + "}";
    }
}

}

