### LubanSer_UnitySo目录文件说明

---



#### Defines

* 用于Luban生成的Bean定义

* 来源

  > 1. 根据识别到全部带[LubanSer]特性的语法树，生成的中间xml文件
  
* Xml_Code

  > 存放根据来源生成的中间Xml文件，供Luban.dll生成代码使用

* Xml_Data

  > 存放根据来源生成的中间Xml文件，供Luban.dll生成数据使用



---



#### Env

* 默认环境为Windows 11

* 默认dotnet 版本为

  > dotnet-runtime-8.0.10-win-x64
  >
  > 本地地址: ./LubanSer_UnitySo/Env/dotnet/dotnet.exe
  >
  > 下载地址：https://dotnet.microsoft.com/en-us/download/dotnet/8.0
  >
  > 需要指定shared 

#### env.conf

* 用于配置自定义dotnet环境变量

  > 如：MacOs下，指定为dotnet



---



#### Generated

* Data/Bytes

  > 根据Luban.dll，指定生成数据目标bin，生成的以csharp/bin解析对应的数据文件

* Data/Json

  > 根据Luban.dll，指定生成数据目标json，生成的以csharp/json解析对应的数据文件

* Code/CSharp

  > 根据Luban.dll，指定生成代码目标cs-bin，生成的解析csharp代码文件
  
* Code/Json

  > 根据Luban.dll，指定生成代码目标cs-simple-json，生成的解析json代码文件



---



#### luban.conf

> Luban.dll依赖的配置

* luban_code.conf

  > Luban.dll 生成序列化代码，所依赖的配置

* luban_data.conf

  > Luban.dll 生成序列化代码 和 数据，所依赖的配置



---



#### luban_gen.bat

> 手动执行Luban生成的脚本

* luban_all_gen_to_copy.bat / luban_all_gen_to_copy.sh

  > 生成csharp / bin + csharp / json 两套 代码+表格数据

* luban_code_gen.bat / luban_code_gen.sh

  > 生成csharp / bin 一份代码数据

* luban_data_gen_to_unity.bat / luban_data_gen_to_unity.sh

  > 生成csharp / bin 一份代码+表格数据（表格数据会生成到Unity/Bundles/Config/TableData下）



