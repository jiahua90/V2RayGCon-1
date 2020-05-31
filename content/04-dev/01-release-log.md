---
title: "发布记录"
date: 2020-02-01T22:16:17+08:00
draft: false
weight: 1
---

##### `v1.4.0.0`  
Luna界面少量调整  
添加luasocket、luasec模块  
添加Sys:CreateHttpServer()、Sys:Volume*()  
Misc:Choice()支持数字键选取及双击确定  
托盘菜单可以停止脚本  
少量其他bug修复  

##### `v1.3.9.0`
界面微调  

##### `v1.3.8.0`
Luna插件少量更新  

##### `v1.3.7.2`  
Luna添加是否允许加载CLR库选项  
`Sys`库添加热键功能(详见手册Sys库最底)  
原`Json`库移进`Misc`并添加`modules.json`模块  
`Signal`添加`ScreenLocked()`查询屏幕是否处于锁定状态  
`Misc:ShowData()`改为返回`Json`字符串  

##### `v1.3.6.2`
此版本因存在界面冻结bug取消发布  

##### `v1.3.5.0`
快速切换功能的迟延要求改为可选  
添加允许自签证书选项  
Luna支持加载CLR库  
Luna添加ILuaSys接口及其他少量调整  

##### `v1.3.4.0`
托盘图标服务器菜单添加快速切换功能  
服务器面板添加备注标签  
程序退出后保留上次测速结果  
优化unicode支持  
用户界面多处微调  

##### `v1.3.3.0`
Luna插件添加Misc:ShowData等几个函数  
批量测速时改用随机顺序  
订阅界面少量调整  

##### `v1.3.2.0`
优化测速算法  
支持v2ray-core v4.23.1的多文件配置功能  
修复ProxySetter插件的菜单不更新问题  
升级失败时无需重启软件  

##### `v1.3.1.0`
修复导入链接解码失败时，其他链接丢失的问题  
服务器很多时，相关菜单自动切换为多层嵌套模式  
优化批量测速的CPU占用  
优化界面刷新逻辑  

##### `v1.3.0.0`
Luna插件的Input控件可设定初值  
修复一个导致菜单冻住的bug  
修复中文tag乱码问题  

##### `v1.2.9.3`
luna插件添加Input，Choices等几个输入控件  

##### `v1.2.9.0`
界面少量调整  
修改lua/libs/utils.lua  

##### `v1.2.8.7`
`v://...`分享链接支持http协议  

##### `v1.2.8.4`
重新设计服务器面板  

##### `v1.2.8.2`
添加停止测速功能  
修复主窗口的失焦问题  

##### `v1.2.8.0`
更新各Nuget包  
修复lua/templates文件夹为空的问题  
修复订阅选项页有时会崩溃的问题  
修复主窗口聚焦问题  
默认启用ProxySetter插件  

##### `v1.2.7.7`
vee链接支持v2ray/discussion [#513](https://github.com/v2ray/discussion/issues/513) 中提出的socks outbound（测试中）

##### `v1.2.7`  
少量更新  
