
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
public sealed partial class EmptyData : Luban.BeanBase
{
    public EmptyData(JSONNode _buf) 
    {
    }

    public static EmptyData DeserializeEmptyData(JSONNode _buf)
    {
        return new EmptyData(_buf);
    }

   
    public const int __ID__ = 583506551;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "}";
    }
}

}

