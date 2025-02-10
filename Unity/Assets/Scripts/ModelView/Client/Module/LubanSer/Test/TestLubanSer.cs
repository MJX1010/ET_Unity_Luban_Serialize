using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Luban;
using UnityEngine;
using YooAsset;
using ET;

namespace TEST_LUBAN_SER
{
    [EnableClass]
    public class TestLoadLubanSerTable
    {
        public cfg.Tables LubanTables => _lubanTables;
        private cfg.Tables _lubanTables = null;

        public void LoadLubanTables() {
            _lubanTables = new cfg.Tables(LoadTables);
            var item = LubanTables.TbBulletConfigSo.Get(0);
            Debug.Log("LoadLubanTables: " + item.ToString());
        }

        private ByteBuf LoadTables(string tableName) {
            AssetHandle handle = YooAssets.LoadAssetSync<TextAsset>($"Assets/Bundles/Config/TableData/{tableName}.bytes");
            if (handle != null && handle.AssetObject != null && handle.AssetObject is TextAsset textAsset) {
                return new Luban.ByteBuf(textAsset.bytes);
            }
            return null;
        }
    }

}
