# YuanShen-Action-System

基于 Unity 的类《原神》3D 角色动作控制系统。

## 技术栈
- Unity 2022.3.28f1c1
- C# 有限状态机（FSM）
- Unity Input System
- Cinemachine 虚拟相机
- Rigidbody 物理运动

## 核心功能
- 13 个角色状态（待机/行走/跑步/冲刺/翻滚/跳跃/坠落等），按接地/移动/停止/落地分层管理
- 代码状态机与 Animator 联动，动画事件帧驱动状态退出
- ScriptableObject 数据驱动参数配置

## 项目规模
- 52 个 C# 脚本，接入 13 段第三方角色动画并完成事件帧标记

## 如何运行
1. 使用 Unity Hub 打开项目根目录（包含 Assets、Packages、ProjectSettings 的文件夹）
2. 在左侧 Project 窗口找到 `Scenes` 文件夹
3. 双击场景文件加载游戏场景
4. 点击 Unity 编辑器顶部的 Play 按钮运行

> 注意：项目需使用 Unity 2022.3.28f1c1 或兼容版本打开。

## 演示视频
https://pan.baidu.com/s/1-G3EcneA000HQCRL7tgy5A?pwd=5s54 提取码：5s54
