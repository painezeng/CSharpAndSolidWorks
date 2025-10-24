# CSharpAndSolidWorks 项目使用说明文档

## 📋 目录
- [项目简介](#项目简介)
- [功能模块](#功能模块)
- [环境准备](#环境准备)
- [代码获取](#代码获取)
- [项目构建](#项目构建)
- [运行说明](#运行说明)
- [功能详细说明](#功能详细说明)
- [常见问题](#常见问题)
- [附录](#附录)

---

## 项目简介

**CSharpAndSolidWorks** 是一个基于 C# 和 VB.NET 开发的 SolidWorks 二次开发资料库。该项目提供了丰富的 SolidWorks API 使用示例，帮助开发者快速掌握 SolidWorks 二次开发技术。

### 项目特点
- ✅ 完整的 SolidWorks API 调用示例
- ✅ 独立应用程序和插件（Add-in）两种开发模式
- ✅ 覆盖零件、装配体、工程图多种文档类型
- ✅ 提供实用的工具和特征创建功能
- ✅ 中文注释，易于理解和学习

---

## 功能模块

本项目包含以下主要功能模块：

### 1. **CSharpAndSolidWorks**（主项目 - 独立应用程序）
独立运行的 Windows Forms 应用程序，通过 COM 接口连接到 SolidWorks。

**核心功能：**
- 连接 SolidWorks
- 打开和创建新零件
- 读取和修改零件属性
- 遍历零件特征、装配体、工程图
- 导出文件（x_t/dwg）
- 插入库特征
- 选择过滤和高级选择
- 包围框生成
- 测量与质量属性获取
- 材质和颜色管理
- 打包文件（Pack and Go）
- 工程图操作

**实用工具：**
- 创建草图中心点
- 自增 Note 插入
- 球标位置自动优化
- 批量导出实体，生成新装配体
- 随机上色
- 屏幕 1:1 显示实物

### 2. **PaineTool**（SolidWorks 插件）
作为 SolidWorks 插件运行，在 SolidWorks 启动时自动加载。

**核心功能：**
- Add-in 自动注册和更新
- Property Manager Page（PMP）自定义页面
- 自定义特征（MacroFeature）
- 事件处理机制
- 命令组和工具栏集成

### 3. **ComparePart**（零件对比工具）
用于比较不同版本的 SolidWorks 零件文件。

### 4. **InsertDrawingNote**（工程图注释工具）
在工程图中批量插入和管理注释。

**功能特点：**
- 自定义 Note 内容
- 鼠标交互插入
- 自动编号

### 5. **GetRayIntersectionWithBody**（射线求交工具）
计算射线与实体的交点，用于空间分析。

**应用场景：**
- 碰撞检测
- 路径分析
- 干涉检查

### 6. **MyeDrawing**（eDrawing 集成）
集成 eDrawing 查看器，实现轻量化模型预览。

---

## 环境准备

### 系统要求
- **操作系统：** Windows 10/11（64位推荐）
- **SolidWorks：** SolidWorks 2018 或更高版本
- **开发工具：** Visual Studio 2017 或更高版本（推荐 Visual Studio 2022）
- **.NET Framework：** .NET Framework 4.6.1 或更高版本

### 必需软件安装

#### 1. 安装 SolidWorks
确保已安装 SolidWorks，并且安装了 SolidWorks API SDK。

**验证方法：**
- 打开 SolidWorks
- 菜单：工具 → 选项 → 系统选项 → 常规
- 确认 SolidWorks 正常运行

#### 2. 安装 Visual Studio
下载并安装 Visual Studio（Community 版本免费）：
- 下载地址：https://visualstudio.microsoft.com/zh-hans/downloads/

**必需工作负载：**
- ✅ .NET 桌面开发
- ✅ Visual Basic（用于 VB.NET 项目）

**可选组件：**
- NuGet 包管理器
- Git 版本控制工具

#### 3. 配置 SolidWorks API DLL 引用
本项目的 `dll` 文件夹已包含必需的 SolidWorks API DLL 文件：
- SolidWorks.Interop.sldworks.dll
- SolidWorks.Interop.swconst.dll
- SolidWorks.Interop.swcommands.dll
- SolidWorks.Interop.swpublished.dll
- SolidWorks.Interop.swdocumentmgr.dll
- SolidWorks.Interop.cosworks.dll
- SolidWorks.Interop.swdimxpert.dll
- SolidWorksTools.dll
- 等

**注意：** 如果您的 SolidWorks 版本与项目不匹配，可能需要更新这些 DLL 文件。

**查找 SolidWorks API DLL 位置：**
```
C:\Program Files\SolidWorks Corp\SolidWorks\api\redist
```

---

## 代码获取

### 方法 1：Git 克隆（推荐）

```powershell
# 打开 PowerShell 或命令提示符
cd E:\01_Work\22_Gitee

# 克隆仓库
git clone https://gitee.com/your-username/CSharpAndSolidWorks.git

# 进入项目目录
cd CSharpAndSolidWorks
```

### 方法 2：直接下载

1. 访问 Gitee 仓库页面
2. 点击"克隆/下载"按钮
3. 选择"下载 ZIP"
4. 解压到本地目录（如：`E:\01_Work\22_Gitee\CSharpAndSolidWorks`）

---

## 项目构建

### 第一步：打开解决方案

1. 使用 Visual Studio 打开项目文件：
   ```
   E:\01_Work\22_Gitee\CSharpAndSolidWorks\CSharpAndSolidWorks.sln
   ```

2. Visual Studio 会自动加载以下项目：
   - CSharpAndSolidWorks（主项目）
   - PaineTool（插件项目）
   - InsertDrawingNote（工具项目）
   - MyeDrawing（VB.NET 项目）
   - ComparePart（对比工具）
   - GetRayIntersectionWithBody（射线工具）

### 第二步：还原 NuGet 包

Visual Studio 会自动检测并还原 NuGet 包。如果没有自动还原，请手动执行：

1. 在"解决方案资源管理器"中右键点击解决方案
2. 选择"还原 NuGet 程序包"
3. 等待包下载完成

### 第三步：检查项目引用

确保所有项目的引用都正确：

1. 展开项目 → "引用"节点
2. 检查是否有黄色警告图标
3. 如果有缺失的引用，需要重新添加

**关键引用：**
- SolidWorks.Interop.* 系列 DLL（位于 `dll` 文件夹）
- System.Windows.Forms
- System.Drawing

**添加引用步骤：**
1. 右键"引用" → "添加引用"
2. 点击"浏览"
3. 导航到项目的 `dll` 文件夹
4. 选择所需的 DLL 文件
5. 点击"确定"

### 第四步：配置生成平台

1. 顶部菜单：生成 → 配置管理器
2. 选择"活动解决方案配置"：**Debug** 或 **Release**
3. 选择"活动解决方案平台"：
   - **Any CPU**（推荐）
   - **x64**（如果 SolidWorks 是 64 位）

### 第五步：生成解决方案

1. 顶部菜单：生成 → 生成解决方案（或按 `Ctrl+Shift+B`）
2. 查看"输出"窗口，确认生成成功
3. 生成成功后，可执行文件位于：
   ```
   CSharpAndSolidWorks\bin\Debug\CSharpAndSolidWorks.exe
   PaineTool\bin\Debug\PaineTool.dll
   ```

**常见生成错误及解决方法：**

| 错误提示 | 原因 | 解决方法 |
|---------|------|---------|
| 找不到类型或命名空间 | DLL 引用缺失 | 重新添加 SolidWorks API DLL 引用 |
| 平台不匹配 | 目标平台设置错误 | 统一设置为 Any CPU 或 x64 |
| 文件访问被拒绝 | 文件被占用 | 关闭 SolidWorks 和其他进程 |

---

## 运行说明

### A. 运行独立应用程序（CSharpAndSolidWorks）

#### 启动步骤

1. **启动 SolidWorks**
   - 先启动 SolidWorks 应用程序
   - 确保 SolidWorks 完全加载完成

2. **运行应用程序**
   - 方式 1：在 Visual Studio 中按 `F5`（调试模式）或 `Ctrl+F5`（非调试模式）
   - 方式 2：直接运行生成的 exe 文件：
     ```
     CSharpAndSolidWorks\bin\Debug\CSharpAndSolidWorks.exe
     ```

3. **连接 SolidWorks**
   - 在应用程序主窗口点击"连接 SolidWorks"按钮
   - 成功后会显示 SolidWorks 版本信息

#### 功能测试

**测试 1：创建新零件**
1. 点击"打开和创建新零件"按钮
2. 程序会自动创建一个新零件，绘制一条直线
3. 保存到 `C:\myNewPart.SLDPRT`
4. 关闭后重新打开

**测试 2：读取零件属性**
1. 在 SolidWorks 中打开示例零件（如项目中的 `TMYTest.SLDPRT`）
2. 点击"读取零件属性"按钮
3. 查看输出窗口中的属性信息

**测试 3：遍历特征**
1. 打开包含多个特征的零件
2. 点击"遍历特征"按钮
3. 在 Visual Studio 的"输出"窗口查看特征树

### B. 运行 SolidWorks 插件（PaineTool）

#### 插件注册

**方法 1：使用 Visual Studio 自动注册（开发模式）**

1. 在 Visual Studio 中右键点击 `PaineTool` 项目
2. 选择"属性"
3. 进入"生成事件" → "生成后事件命令行"
4. 添加以下命令（用于注册 COM DLL）：
   ```cmd
   "%windir%\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe" "$(TargetPath)" /codebase
   ```

5. 生成项目（这会自动注册插件）

**方法 2：手动注册（生产模式）**

打开**管理员权限**的命令提示符或 PowerShell：

```powershell
# 注册插件
cd E:\01_Work\22_Gitee\CSharpAndSolidWorks\PaineTool\bin\Debug
%windir%\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe PaineTool.dll /codebase

# 如果是 64 位系统，使用：
%windir%\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe PaineTool.dll /codebase
```

#### 启动插件

1. **重启 SolidWorks**
   - 完全关闭 SolidWorks
   - 重新启动 SolidWorks

2. **验证插件加载**
   - 菜单：工具 → 插件
   - 在插件列表中找到"PaineTool"
   - 确保勾选了"启动时加载"和"活动"

3. **使用插件功能**
   - 在 SolidWorks 菜单栏或工具栏会出现"Paine Addin(C#)"
   - 点击菜单项测试功能：
     - **CreateCube**：创建一个立方体
     - **Show PMP**：显示自定义属性管理器页面
     - **NewFeature**：创建一个自定义特征

#### 卸载插件

```powershell
# 卸载插件
%windir%\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe PaineTool.dll /u

# 或 64 位：
%windir%\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe PaineTool.dll /u
```

### C. 运行子工具项目

#### InsertDrawingNote（工程图注释工具）

1. 将项目设置为启动项目：
   - 右键"InsertDrawingNote"项目
   - 选择"设为启动项目"

2. 启动 SolidWorks 并打开一个工程图文件

3. 运行项目（F5）

4. 在工具窗口中：
   - 输入注释内容
   - 点击鼠标插入位置
   - 注释会自动编号

#### ComparePart（零件对比工具）

1. 设置为启动项目

2. 运行后选择两个 SLDPRT 文件

3. 查看对比结果

#### GetRayIntersectionWithBody（射线求交工具）

1. 设置为启动项目

2. 在 SolidWorks 中打开零件

3. 运行工具，选择起点和方向

4. 查看交点信息

---

## 功能详细说明

### 核心 API 功能

#### 1. 连接 SolidWorks

```csharp
// 连接到已运行的 SolidWorks 实例
SldWorks swApp = Utility.ConnectToSolidWorks();

// 获取版本信息
swApp.GetBuildNumbers2(out string baseV, out string _, out string _);
string version = swApp.RevisionNumber();
```

#### 2. 文档操作

```csharp
// 创建新零件
string partTemplate = swApp.GetDocumentTemplate((int)swDocumentTypes_e.swDocPART, "", 0, 0, 0);
var newDoc = swApp.NewDocument(partTemplate, 0, 0, 0);

// 打开文件
swApp.OpenDoc(filePath, (int)swDocumentTypes_e.swDocPART);

// 保存文件
ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
swModel.SaveAs3(savePath, 0, 1);
```

#### 3. 属性管理

```csharp
// 读取自定义属性
string value = swModel.GetCustomInfoValue("", "PropertyName");

// 添加自定义属性
swModel.AddCustomInfo3("", "PropertyName", 30, "PropertyValue");

// 删除自定义属性
swModel.DeleteCustomInfo2("", "PropertyName");

// 配置特定属性
var manager = swModel.Extension.CustomPropertyManager["ConfigName"];
manager.Add3("Code", (int)swCustomInfoType_e.swCustomInfoText, "Value", 
             (int)swCustomPropertyAddOption_e.swCustomPropertyReplaceValue);
```

#### 4. 特征操作

```csharp
// 遍历特征
Feature swFeat = (Feature)swModel.FirstFeature();
while (swFeat != null)
{
    Debug.Print(swFeat.Name);
    swFeat = (Feature)swFeat.GetNextFeature();
}

// 添加特征（圆角）
swModel.Extension.SelectByID2("", "EDGE", x, y, z, false, 1, null, 0);
Feature fillet = (Feature)swModel.FeatureManager.FeatureFillet3(
    195, 0.000508, 0.01, 0, 0, 0, 0, null, null, null, null, null, null, null);

// 压缩特征
fillet.Select(false);
swModel.EditSuppress();

// 删除特征
fillet.Select(false);
swModel.EditDelete();
```

#### 5. 尺寸修改

```csharp
// 获取尺寸
Dimension dimension = (Dimension)swModel.Parameter("D1@Boss-Extrude1");

// 修改尺寸值（系统单位：米）
dimension.SystemValue = 0.1; // 100mm

// 重建模型
swModel.EditRebuild3();
```

#### 6. 装配体遍历

```csharp
// 获取顶层组件
Component2 rootComp = (Component2)swModel.ConfigurationManager.ActiveConfiguration.GetRootComponent3(true);

// 递归遍历子组件
void TraverseComponent(Component2 comp)
{
    object[] children = (object[])comp.GetChildren();
    foreach (Component2 child in children)
    {
        Debug.Print(child.Name2);
        TraverseComponent(child);
    }
}
```

#### 7. 工程图操作

```csharp
// 获取工程图视图
DrawingDoc drawDoc = (DrawingDoc)swModel;
View view = (View)drawDoc.GetFirstView();
while (view != null)
{
    Debug.Print(view.Name);
    view = (View)view.GetNextView();
}

// 插入注释
Note note = swModel.Extension.InsertNote("Note Text");
Annotation annotation = note.GetAnnotation();
annotation.SetPosition(x, y, z);
```

### 实用工具功能

#### 1. 批量导出

- 支持导出格式：STEP、IGES、DWG、DXF、STL、PDF
- 批量处理多个文件
- 自定义导出选项

#### 2. 材质和颜色

```csharp
// 设置材质
swModel.SetMaterialPropertyName2("", "", "AISI 1020");

// 设置颜色
double[] matProps = (double[])swModel.MaterialPropertyValues;
matProps[0] = 1.0; // R
matProps[1] = 0.0; // G
matProps[2] = 0.0; // B
swModel.MaterialPropertyValues = matProps;
```

#### 3. 包围框

```csharp
// 获取包围框
double[] boundBox = (double[])swModel.Extension.GetBox((int)swBoundingBoxOptions_e.swBoundingBoxIncludeRefPlanes);
// boundBox: [xMin, yMin, zMin, xMax, yMax, zMax]
```

---

## 常见问题

### Q1：无法连接到 SolidWorks
**原因：**
- SolidWorks 未启动
- COM 组件未正确注册

**解决方法：**
1. 确保 SolidWorks 已启动
2. 尝试以管理员身份运行应用程序
3. 重新安装 SolidWorks API SDK

### Q2：插件未显示在 SolidWorks 中
**原因：**
- 插件未正确注册
- 注册表权限不足

**解决方法：**
1. 以管理员身份运行 RegAsm.exe
2. 检查注册表：
   ```
   HKEY_LOCAL_MACHINE\SOFTWARE\SolidWorks\Addins\{GUID}
   ```
3. 确认"启动时加载"选项已勾选

### Q3：生成时提示找不到 DLL
**原因：**
- SolidWorks API DLL 路径不正确
- DLL 版本不匹配

**解决方法：**
1. 检查 `dll` 文件夹中的 DLL 文件
2. 更新为您 SolidWorks 版本对应的 DLL
3. 重新添加引用

### Q4：运行时提示"未将对象引用设置到对象的实例"
**原因：**
- SolidWorks 文档未打开
- 对象未正确初始化

**解决方法：**
1. 确保在操作前打开了相应类型的文档
2. 添加空值检查：
   ```csharp
   if (swModel != null)
   {
       // 执行操作
   }
   ```

### Q5：插件在 SolidWorks 2024 中不工作
**原因：**
- API 版本兼容性问题

**解决方法：**
1. 更新项目中的 SolidWorks API DLL 到对应版本
2. 重新编译项目
3. 重新注册插件

---

## 附录

### A. 项目结构说明

```
CSharpAndSolidWorks/
├── CSharpAndSolidWorks/          # 主项目（独立应用程序）
│   ├── FormMain.cs               # 主窗体（核心功能演示）
│   ├── Utility.cs                # 工具类（连接、遍历等）
│   ├── BodyHelper.cs             # 实体操作帮助类
│   ├── ThumbnailHelper.cs        # 缩略图工具
│   ├── ComparePart/              # 零件对比子项目
│   ├── InsertDrawingNote/        # 工程图注释子项目
│   ├── GetRayIntersectionWithBody/ # 射线求交子项目
│   ├── eDrawingDLL/              # eDrawing 集成（VB.NET）
│   └── screenClass/              # 屏幕显示工具
│
├── PaineTool/                    # SolidWorks 插件项目
│   ├── SwAddin.cs                # 插件主类（入口）
│   ├── UserPMPage.cs             # 自定义属性页
│   ├── PMPHandler.cs             # PMP 处理器
│   ├── EventHandling.cs          # 事件处理
│   └── NewFeature/               # 自定义特征模块
│       ├── NewFeaturePMPage.cs   # 特征属性页
│       └── NewFeatureHandler.cs  # 特征处理器
│
├── dll/                          # SolidWorks API DLL 文件
│   ├── SolidWorks.Interop.sldworks.dll
│   ├── SolidWorks.Interop.swconst.dll
│   └── ...                       # 其他 API DLL
│
├── packages/                     # NuGet 包
├── CSharpAndSolidWorks.sln       # 解决方案文件
├── README.md                     # 项目说明（中文）
├── README.en.md                  # 项目说明（英文）
└── Solidworks API Help CN.xlsx   # API 帮助文档（Excel）
```

### B. 开发资源

**官方文档：**
- SolidWorks API Help：安装目录下的 `api\SolidWorksApi.chm`
- 在线帮助：https://help.solidworks.com/

**示例代码：**
- SolidWorks 安装目录：`api\SolidWorksApi\examples`

**社区资源：**
- SolidWorks 论坛：https://forum.solidworks.com/
- CAD 自动化博客

### C. API 版本对照表

| SolidWorks 版本 | API 版本 | .NET Framework |
|----------------|----------|----------------|
| 2018           | 26.x     | 4.5+           |
| 2019           | 27.x     | 4.6+           |
| 2020           | 28.x     | 4.6.1+         |
| 2021           | 29.x     | 4.7+           |
| 2022           | 30.x     | 4.7.2+         |
| 2023           | 31.x     | 4.8+           |
| 2024           | 32.x     | 4.8+           |

### D. 快捷键参考

**Visual Studio：**
- `F5`：启动调试
- `Ctrl+F5`：运行（不调试）
- `Ctrl+Shift+B`：生成解决方案
- `F9`：设置/取消断点
- `F10`：单步跳过
- `F11`：单步调试

**SolidWorks：**
- `Ctrl+N`：新建文档
- `Ctrl+O`：打开文档
- `Ctrl+S`：保存
- `Ctrl+B`：重建模型
- `Ctrl+Q`：强制重建

### E. 技术支持

**问题反馈：**
- Gitee Issues：https://gitee.com/painezeng/CSharpAndSolidWorks/issues

**学习资源：**
- 项目文档：查看 `README.md`
- API 帮助：查看 `Solidworks API Help CN.xlsx`
- 示例视频：查看 `如何测试.mp4`

---

## 📝 更新日志

### 版本历史
- **最新版本**：包含完整的 API 示例和工具集
- 支持 SolidWorks 2018 及以上版本
- 提供中文注释和文档

---

## 📄 许可证

本项目遵循开源许可证，详见 `LICENSE` 文件。

---

## 🙏 致谢

感谢 SolidWorks 提供的强大 API 接口，以及开源社区的贡献。

---

**祝您使用愉快！如有问题，欢迎反馈交流。** 🚀




###SolidWorks API



![界面预览](https://img-blog.csdnimg.cn/20201214143017572.png)

​	

```
#API
	连接Solidworks
	打开和创建新零件
	读取零件属性
	修改零件
	遍历零件特征
	遍历装配体
	遍历工程图视图与球标
	装配新零件
	导出x_t/dwg
	插入库特征
	选择过滤
	清空草图的几何关系
	选择命名的实体/面
	遍历草图中的对象
	给文件增加第三方属性
	显示提示信息
	高级选择
	包围框生成(包括装配体方案)
	测量与获取结果
	获取质量属性
	Add-in的建立
	自动注册插件与更新方案
	Pane创建页面(预览BOM)
	给零件加上材质
	给选定面增加颜色
	打断连接关系
	替换零件 
	PMP新特征
	显示拖拽
	MacroFeature的生成
	等待用户选择后继续操作
	打包文件Pack and Go
	插入块 和 属性块
	工程图中获取模型
	给已有特征增加几何关系
#实例
	创建草图中心点
	自增Note插入
	球标位置自动优化
	批量导出实体，生成新装配体。
	随机上色
	屏幕1：1显示实物
```
