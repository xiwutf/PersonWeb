---
title: 智能物联网控制系统
tech: [STM32, C/C++, 嵌入式开发, 传感器, 物联网]
description: 基于STM32的智能物联网控制系统，支持多种传感器数据采集、远程监控、自动控制等功能，应用于智能家居和工业自动化场景
demo_link: https://github.com/Lijing327
source_link: https://github.com/Lijing327
cover: /images/projects/embedded-iot.png
slug: embedded-iot-system
date: 2023-11-20
status: 已完成
category: 嵌入式系统
---

# 智能物联网控制系统

## 项目简介

智能物联网控制系统是一个基于STM32微控制器的嵌入式系统项目，集成了多种传感器模块，实现了数据采集、远程监控、自动控制等功能。该系统可广泛应用于智能家居、工业自动化、环境监测等场景。

## 核心功能

### 📡 传感器数据采集
- **温湿度监测**：DHT22传感器实时采集环境温湿度
- **光照强度检测**：光敏电阻检测环境光照
- **空气质量监测**：MQ系列传感器检测空气质量
- **运动检测**：PIR红外传感器检测人体活动
- **距离测量**：超声波传感器测量距离

### 🌐 物联网通信
- **WiFi连接**：ESP8266模块实现WiFi通信
- **MQTT协议**：使用MQTT协议进行数据传输
- **云端平台**：对接阿里云IoT平台
- **远程控制**：支持手机APP远程控制设备

### ⚙️ 自动控制
- **智能照明**：根据光照强度自动调节LED亮度
- **温控系统**：温度超过阈值自动启动风扇
- **安防监控**：检测到异常自动报警
- **定时任务**：支持定时开关和场景模式

## 技术架构

### 硬件平台

#### 主控芯片
- **STM32F103C8T6**：ARM Cortex-M3内核，72MHz主频
- **Flash**：64KB
- **RAM**：20KB
- **GPIO**：37个通用IO口

#### 外设模块
- **ESP8266 WiFi模块**：实现网络通信
- **DHT22温湿度传感器**：环境监测
- **HC-SR04超声波传感器**：距离测量
- **MQ-2气体传感器**：空气质量检测
- **光敏电阻**：光照检测
- **PIR红外传感器**：运动检测
- **OLED显示屏**：信息显示
- **继电器模块**：设备控制

### 软件架构

```
┌─────────────────────────────────┐
│     应用层 (Application)        │
│  - 控制逻辑                      │
│  - 数据处理                      │
│  - 用户交互                      │
└──────────────┬──────────────────┘
               │
┌──────────────▼──────────────────┐
│     通信层 (Communication)       │
│  - WiFi连接管理                  │
│  - MQTT协议处理                 │
│  - 数据封装/解析                 │
└──────────────┬──────────────────┘
               │
┌──────────────▼──────────────────┐
│     驱动层 (Driver)              │
│  - 传感器驱动                    │
│  - 外设驱动                      │
│  - 中断处理                      │
└──────────────┬──────────────────┘
               │
┌──────────────▼──────────────────┐
│     HAL层 (Hardware Abstraction) │
│  - STM32 HAL库                   │
│  - 寄存器操作                    │
└─────────────────────────────────┘
```

### 开发环境

- **IDE**：Keil MDK-ARM / STM32CubeIDE
- **编程语言**：C/C++
- **固件库**：STM32 HAL库
- **调试工具**：ST-Link / J-Link
- **串口工具**：串口助手、PuTTY

## 核心代码实现

### 1. 传感器数据采集

```c
// DHT22温湿度传感器读取
typedef struct {
    float temperature;
    float humidity;
} DHT22_Data_t;

DHT22_Data_t DHT22_Read(void) {
    DHT22_Data_t data = {0};
    
    // 启动信号
    HAL_GPIO_WritePin(DHT22_GPIO_Port, DHT22_Pin, GPIO_PIN_RESET);
    HAL_Delay(20);
    HAL_GPIO_WritePin(DHT22_GPIO_Port, DHT22_Pin, GPIO_PIN_SET);
    HAL_Delay(30);
    
    // 读取数据...
    // 解析温湿度值
    
    return data;
}
```

### 2. WiFi连接管理

```c
// ESP8266 WiFi连接
void ESP8266_ConnectWiFi(const char* ssid, const char* password) {
    // 发送AT指令
    ESP8266_SendCommand("AT+CWMODE=1\r\n");
    HAL_Delay(100);
    
    // 连接WiFi
    char cmd[128];
    sprintf(cmd, "AT+CWJAP=\"%s\",\"%s\"\r\n", ssid, password);
    ESP8266_SendCommand(cmd);
    HAL_Delay(2000);
    
    // 获取IP地址
    ESP8266_SendCommand("AT+CIFSR\r\n");
}
```

### 3. MQTT通信

```c
// MQTT消息发布
void MQTT_Publish(const char* topic, const char* message) {
    char cmd[256];
    sprintf(cmd, "AT+MQTTPUB=\"%s\",\"%s\",1,0\r\n", topic, message);
    ESP8266_SendCommand(cmd);
}

// MQTT消息订阅
void MQTT_Subscribe(const char* topic) {
    char cmd[128];
    sprintf(cmd, "AT+MQTTSUB=\"%s\",1\r\n", topic);
    ESP8266_SendCommand(cmd);
}
```

### 4. 自动控制逻辑

```c
// 智能照明控制
void SmartLighting_Control(void) {
    uint16_t light_value = LightSensor_Read();
    
    if (light_value < LIGHT_THRESHOLD) {
        // 光照不足，开启LED
        LED_SetBrightness(255 - (light_value * 255 / 4095));
    } else {
        // 光照充足，关闭LED
        LED_SetBrightness(0);
    }
}

// 温度控制
void Temperature_Control(void) {
    DHT22_Data_t data = DHT22_Read();
    
    if (data.temperature > TEMP_THRESHOLD) {
        // 温度过高，启动风扇
        Fan_Start();
    } else {
        Fan_Stop();
    }
}
```

## 项目亮点

### 🎯 模块化设计
- 传感器驱动模块化，易于扩展
- 通信协议独立封装
- 控制逻辑清晰分离

### ⚡ 实时性保障
- 中断驱动的传感器读取
- 定时器精确控制
- 低延迟响应

### 🔋 低功耗优化
- 睡眠模式管理
- 传感器按需唤醒
- 动态频率调节

### 🛡️ 稳定性设计
- 看门狗定时器保护
- 异常处理机制
- 数据校验和重传

## 应用场景

### 智能家居
- 环境监测和自动调节
- 智能照明系统
- 安防监控
- 远程控制

### 工业自动化
- 生产线监控
- 设备状态监测
- 数据采集和分析
- 故障预警

### 农业物联网
- 大棚环境监测
- 自动灌溉系统
- 土壤湿度检测
- 作物生长监控

## 开发挑战与解决方案

### 挑战1：传感器数据稳定性
**问题**：传感器数据存在波动和干扰  
**解决**：
- 实现数据滤波算法（移动平均、中值滤波）
- 多次采样取平均值
- 添加数据校验机制

### 挑战2：WiFi连接稳定性
**问题**：网络断开后无法自动重连  
**解决**：
- 实现心跳检测机制
- 自动重连逻辑
- 连接状态监控

### 挑战3：实时性要求
**问题**：多个任务需要实时响应  
**解决**：
- 使用FreeRTOS实时操作系统
- 任务优先级调度
- 中断嵌套处理

## 性能指标

- **响应时间**：< 100ms
- **数据采集频率**：1Hz - 10Hz可调
- **通信距离**：WiFi覆盖范围内
- **功耗**：待机 < 50mA，工作 < 200mA
- **工作温度**：-10°C ~ 60°C

## 未来规划

- [ ] 支持更多传感器类型
- [ ] 增加蓝牙通信功能
- [ ] 实现边缘计算能力
- [ ] 开发移动端APP
- [ ] 支持语音控制
- [ ] 机器学习算法集成

## 技术栈详情

### 硬件
- STM32F103C8T6 开发板
- ESP8266 WiFi模块
- 多种传感器模块
- OLED显示屏
- 继电器模块

### 软件
- STM32 HAL库
- FreeRTOS（可选）
- MQTT协议栈
- 传感器驱动库

## 学习资源

- [STM32官方文档](https://www.st.com/en/microcontrollers-microprocessors/stm32-32-bit-arm-cortex-mcus.html)
- [ESP8266 AT指令集](https://www.espressif.com/en/products/socs/esp8266)
- [MQTT协议规范](https://mqtt.org/)

---

**项目状态**：已完成  
**开发周期**：3个月  
**技术难点**：多传感器协调、实时通信、低功耗设计

