using ET;
using UnityEngine;

namespace TEST_LUBAN_SER {
    [LubanSer]
    public partial class EmptyData {
        
    }
    
    [LubanSer]
    public partial class FP {
        public long m_rawValue;
    }

    [LubanSer]
    public partial class FVector2 {
        public FP x;
        public FP y;
    }
    
    [LubanSer]
    public partial class FVector3 {
        public FP x;
        public FP y;
        public FP z;
    }
    
    [LubanSer]
    public partial class FVector4 {
        public FP x;
        public FP y;
        public FP z;
        public FP w;
    }

    [LubanSer]
    public partial class FCurve {
        public FP value;
        public string name;
    }

    [LubanSer]
    [LubanSerTable(dataPath = "Bullet1")]
    public partial class BulletConfig {
        public MoveConfig move1;
        public DmgConfig dmg2;
        public ShootStyle shootStyle3;
        
        //[LubanSerIgnore]
        //public GameObject bulletPrefab;
        //[LubanSerUnityObject(type = typeof(GameObject))]
        //public string bulletPrefabPath;
        
        //[LubanSerIgnore]
        //public BulletConfigSo2 config;
        //[LubanSerLubanSo]
        //public string configGuid;
    }
    
    [LubanSer]
    //[LubanSerTable(dataPath = "Bullet2")]
    public partial class BulletConfig2 {
        public MoveConfig move1;
    }

    [LubanSer]
    //[LubanSerTable(dataPath = "Move")]
    public partial class MoveConfig {
        public FP startSpd;
        public FVector2 spdRange;
        public FP acce;
        public FCurve spdCurve;
        public FP spdCurveTime;
    }

    [LubanSer]
    public partial class DmgConfig {
        public FP dmgMulti;
        public EDmgSource dmgSource;
        public EDmgElement dmgElement;
    }

    [LubanSer]
    public partial class ShootStyle {
        public EArrangeStyle arrange;
        public FVector2 spread;
    }

    //不进表枚举
    [LubanSer]
    public enum EArrangeStyle {
        Equal = 0,
        Random,
    }

    //进表枚举，应该报错
    [LubanSer]
    public enum EDmgSource {
        Normal = 0,
        Counter = 1,
    }

    //进表枚举，应该报错
    [LubanSer]
    public enum EDmgElement {
        Fire = 0,
        Ice = 1,
    }
    
    [LubanSer]
    //[LubanSerTable(dataPath = "Dragon")]
    public partial class DragonConfig  {
        public BulletConfig normalBullet;
        public float damage;
    }

    [LubanSer]
    public partial class DragonDisplayConfig {
        public DragonConfig normalDragon;
        public int displayTableId;
    }
    
    [LubanSer]
    public partial class BulletDisplayConfig {
        public BulletConfig normalDragon;
        public int displayTableId;
    }
}
