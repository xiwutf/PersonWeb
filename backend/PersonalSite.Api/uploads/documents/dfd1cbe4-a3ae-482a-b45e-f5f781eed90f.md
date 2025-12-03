# API 使用指南

## 一、概述

### 1.1 什么是 API

API（Application Programming Interface，应用程序编程接口）是一组定义了软件组件之间如何交互的规范和协议。通过 API，不同的应用程序可以相互通信和共享数据。

### 1.2 API 的作用

- **数据交换**：允许不同系统之间安全地交换数据
- **功能集成**：将第三方服务集成到自己的应用中
- **提高效率**：避免重复开发，直接使用已有的功能
- **标准化**：提供统一的接口规范，便于协作

### 1.3 本指南内容

本指南将详细介绍如何使用我们的 API 服务，包括：
- API 认证方式
- 请求和响应格式
- 常用接口说明
- 错误处理
- 最佳实践

---

## 二、快速开始

### 2.1 获取 API Key

在使用 API 之前，您需要先获取 API Key：

1. 登录到管理后台
2. 进入"API 管理"页面
3. 点击"创建新的 API Key"
4. 复制生成的 Key 并妥善保管

**重要提示**：API Key 具有完全访问权限，请勿泄露给他人。

### 2.2 基础配置

#### 请求地址

```
生产环境：https://api.example.com/v1
测试环境：https://api-test.example.com/v1
```

#### 请求头设置

所有 API 请求都需要包含以下请求头：

```
Authorization: Bearer YOUR_API_KEY
Content-Type: application/json
Accept: application/json
```

### 2.3 第一个 API 调用

以下是一个简单的 API 调用示例（使用 cURL）：

```bash
curl -X GET "https://api.example.com/v1/users/me" \
  -H "Authorization: Bearer YOUR_API_KEY" \
  -H "Content-Type: application/json"
```

**响应示例**：

```json
{
  "success": true,
  "data": {
    "id": "12345",
    "username": "testuser",
    "email": "test@example.com",
    "created_at": "2024-01-01T00:00:00Z"
  }
}
```

---

## 三、认证与授权

### 3.1 API Key 认证

API Key 是访问 API 的主要认证方式。将 API Key 放在请求头的 `Authorization` 字段中：

```
Authorization: Bearer YOUR_API_KEY
```

### 3.2 Token 认证（可选）

对于需要更高安全性的场景，可以使用 Token 认证：

1. 使用 API Key 获取临时 Token
2. 使用 Token 进行后续 API 调用
3. Token 有效期通常为 24 小时

**获取 Token 的接口**：

```
POST /auth/token
```

**请求体**：

```json
{
  "api_key": "YOUR_API_KEY",
  "expires_in": 86400
}
```

### 3.3 权限说明

不同的 API Key 可能具有不同的权限：

- **只读权限**：只能调用查询类接口
- **读写权限**：可以调用所有接口
- **管理员权限**：拥有完全访问权限

---

## 四、核心接口

### 4.1 用户管理

#### 获取用户信息

**接口地址**：`GET /users/{user_id}`

**请求参数**：

| 参数名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| user_id | string | 是 | 用户 ID |

**响应示例**：

```json
{
  "success": true,
  "data": {
    "id": "12345",
    "username": "testuser",
    "email": "test@example.com",
    "status": "active",
    "created_at": "2024-01-01T00:00:00Z"
  }
}
```

#### 创建用户

**接口地址**：`POST /users`

**请求体**：

```json
{
  "username": "newuser",
  "email": "newuser@example.com",
  "password": "secure_password"
}
```

**响应示例**：

```json
{
  "success": true,
  "data": {
    "id": "67890",
    "username": "newuser",
    "email": "newuser@example.com",
    "created_at": "2024-01-02T00:00:00Z"
  }
}
```

### 4.2 数据查询

#### 查询数据列表

**接口地址**：`GET /data`

**请求参数**：

| 参数名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| page | integer | 否 | 页码，默认 1 |
| page_size | integer | 否 | 每页数量，默认 20 |
| keyword | string | 否 | 搜索关键词 |
| status | string | 否 | 状态筛选 |

**响应示例**：

```json
{
  "success": true,
  "data": {
    "list": [
      {
        "id": "1",
        "title": "示例数据",
        "status": "active"
      }
    ],
    "total": 100,
    "page": 1,
    "page_size": 20
  }
}
```

### 4.3 文件上传

#### 上传文件

**接口地址**：`POST /files/upload`

**请求格式**：`multipart/form-data`

**请求参数**：

| 参数名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| file | file | 是 | 要上传的文件 |
| category | string | 否 | 文件分类 |

**响应示例**：

```json
{
  "success": true,
  "data": {
    "file_id": "file_12345",
    "filename": "example.pdf",
    "size": 1024000,
    "url": "https://cdn.example.com/files/file_12345.pdf"
  }
}
```

---

## 五、错误处理

### 5.1 错误码说明

API 使用标准的 HTTP 状态码，常见错误码如下：

| 状态码 | 说明 | 处理建议 |
|--------|------|----------|
| 200 | 请求成功 | - |
| 400 | 请求参数错误 | 检查请求参数格式和必填项 |
| 401 | 未授权 | 检查 API Key 是否正确 |
| 403 | 权限不足 | 检查 API Key 权限 |
| 404 | 资源不存在 | 检查请求的资源 ID |
| 429 | 请求频率过高 | 降低请求频率 |
| 500 | 服务器错误 | 稍后重试或联系技术支持 |

### 5.2 错误响应格式

所有错误响应都遵循以下格式：

```json
{
  "success": false,
  "error_code": "INVALID_PARAMETER",
  "message": "参数 user_id 不能为空",
  "data": null
}
```

### 5.3 常见错误处理

#### 认证失败

**错误信息**：`401 Unauthorized`

**可能原因**：
- API Key 未提供
- API Key 格式错误
- API Key 已过期或被撤销

**解决方法**：
1. 检查请求头中的 Authorization 字段
2. 确认 API Key 格式为 `Bearer YOUR_API_KEY`
3. 重新生成 API Key

#### 参数错误

**错误信息**：`400 Bad Request`

**可能原因**：
- 缺少必填参数
- 参数类型错误
- 参数值不符合要求

**解决方法**：
1. 查看错误信息中的具体提示
2. 检查请求参数是否符合接口文档要求
3. 验证参数的数据类型和格式

---

## 六、最佳实践

### 6.1 请求频率控制

为了避免对服务器造成过大压力，建议：

- **普通接口**：每秒不超过 10 次请求
- **数据查询接口**：每秒不超过 5 次请求
- **文件上传接口**：每秒不超过 2 次请求

如果遇到 429 错误（请求频率过高），请：
1. 降低请求频率
2. 使用批量接口替代多次单独请求
3. 实现请求重试机制，使用指数退避策略

### 6.2 数据缓存

对于不经常变化的数据，建议实现缓存机制：

- **用户信息**：缓存 5 分钟
- **配置信息**：缓存 30 分钟
- **统计数据**：缓存 1 小时

### 6.3 错误重试

对于网络错误或临时服务器错误（5xx），建议实现重试机制：

```python
import time
import requests

def api_call_with_retry(url, headers, max_retries=3):
    for attempt in range(max_retries):
        try:
            response = requests.get(url, headers=headers)
            if response.status_code == 200:
                return response.json()
            elif response.status_code >= 500:
                # 服务器错误，等待后重试
                time.sleep(2 ** attempt)  # 指数退避
                continue
            else:
                # 客户端错误，不重试
                raise Exception(f"请求失败: {response.status_code}")
        except requests.exceptions.RequestException as e:
            if attempt == max_retries - 1:
                raise
            time.sleep(2 ** attempt)
    return None
```

### 6.4 安全性建议

1. **保护 API Key**：
   - 不要在代码中硬编码 API Key
   - 使用环境变量或配置文件存储
   - 不要在公开的代码仓库中提交 API Key

2. **使用 HTTPS**：
   - 所有 API 请求都应使用 HTTPS 协议
   - 确保传输过程中的数据安全

3. **验证响应**：
   - 始终验证 API 响应的完整性
   - 检查响应中的 success 字段
   - 处理可能的错误情况

---

## 七、示例代码

### 7.1 Python 示例

```python
import requests

class APIClient:
    def __init__(self, api_key, base_url="https://api.example.com/v1"):
        self.api_key = api_key
        self.base_url = base_url
        self.headers = {
            "Authorization": f"Bearer {api_key}",
            "Content-Type": "application/json"
        }
    
    def get_user(self, user_id):
        """获取用户信息"""
        url = f"{self.base_url}/users/{user_id}"
        response = requests.get(url, headers=self.headers)
        response.raise_for_status()
        return response.json()
    
    def create_user(self, username, email, password):
        """创建用户"""
        url = f"{self.base_url}/users"
        data = {
            "username": username,
            "email": email,
            "password": password
        }
        response = requests.post(url, json=data, headers=self.headers)
        response.raise_for_status()
        return response.json()

# 使用示例
client = APIClient("YOUR_API_KEY")
user = client.get_user("12345")
print(user)
```

### 7.2 JavaScript 示例

```javascript
class APIClient {
  constructor(apiKey, baseUrl = 'https://api.example.com/v1') {
    this.apiKey = apiKey;
    this.baseUrl = baseUrl;
    this.headers = {
      'Authorization': `Bearer ${apiKey}`,
      'Content-Type': 'application/json'
    };
  }

  async getUser(userId) {
    const response = await fetch(`${this.baseUrl}/users/${userId}`, {
      method: 'GET',
      headers: this.headers
    });
    
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    
    return await response.json();
  }

  async createUser(username, email, password) {
    const response = await fetch(`${this.baseUrl}/users`, {
      method: 'POST',
      headers: this.headers,
      body: JSON.stringify({
        username,
        email,
        password
      })
    });
    
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    
    return await response.json();
  }
}

// 使用示例
const client = new APIClient('YOUR_API_KEY');
const user = await client.getUser('12345');
console.log(user);
```

---

## 八、常见问题

### Q1: API Key 在哪里获取？

A: 登录管理后台，进入"API 管理"页面，点击"创建新的 API Key"即可获取。

### Q2: API 调用有频率限制吗？

A: 是的，不同接口有不同的频率限制。普通接口建议每秒不超过 10 次请求。如果遇到 429 错误，请降低请求频率。

### Q3: 如何判断 API 调用是否成功？

A: 检查响应中的 `success` 字段。如果为 `true`，表示请求成功；如果为 `false`，查看 `error_code` 和 `message` 字段了解错误原因。

### Q4: 支持哪些数据格式？

A: 目前支持 JSON 格式。请求和响应都使用 JSON 格式。

### Q5: 如何处理文件上传？

A: 使用 `multipart/form-data` 格式上传文件。具体示例请参考"文件上传"章节。

### Q6: API Key 泄露了怎么办？

A: 立即在管理后台撤销该 API Key，并生成新的 Key。同时检查是否有异常 API 调用记录。

---

## 九、技术支持

如果您在使用 API 过程中遇到问题，可以通过以下方式获取帮助：

- **文档中心**：https://docs.example.com
- **技术支持邮箱**：support@example.com
- **开发者社区**：https://community.example.com
- **GitHub Issues**：https://github.com/example/api-issues

---

## 十、更新日志

### v1.2.0 (2024-01-15)
- 新增批量查询接口
- 优化错误提示信息
- 修复文件上传大小限制问题

### v1.1.0 (2024-01-01)
- 新增 Token 认证方式
- 优化响应格式
- 增加请求频率限制

### v1.0.0 (2023-12-01)
- 初始版本发布
- 支持基础的用户管理和数据查询功能

---

**文档版本**：v1.2.0  
**最后更新**：2024-01-15  
**维护团队**：API 开发团队

