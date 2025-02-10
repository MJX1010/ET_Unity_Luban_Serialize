using ET;
using UnityEngine;

namespace TEST_LUBAN_SER {
    //Test
    //[LubanSerEditor(resPath = "Config/Editor/Bullet1")]
    [CreateAssetMenu(menuName = "ET/GameConfig/Bullet", fileName = "Bullet1", order = 0)]
    public partial class BulletConfigSo : LubanSo<BulletConfig> {
    }

    //Test
    //[LubanSerEditor(resPath = "Config/Editor/Bullet2")]
    [CreateAssetMenu(menuName = "ET/GameConfig/Bullet2", fileName = "Bullet2", order = 1)]
    public partial class BulletConfigSo2 : LubanSo<BulletConfig2> {
    }

    //Test
    //[LubanSerEditor(resPath = "Config/Editor/Dragon")]
    [CreateAssetMenu(menuName = "ET/GameConfig/Dragon", fileName = "Dragon", order = 2)]
    public partial class DragonConfigSo : LubanSo<DragonConfig> {
    }

    //Test
    //[LubanSerEditor(resPath = "Config/Editor/Move")]
    [CreateAssetMenu(menuName = "ET/GameConfig/Move", fileName = "Move", order = 3)]
    public partial class MoveConfigSo : LubanSo<MoveConfig> {
    }   
}