# 后端生产环境配置说明

## 问题现象

部署后服务反复重启，日志报错：

```text
System.InvalidOperationException: 数据库连接字符串未配置
   at Program.<Main>$(String[] args) in .../Program.cs:line 19
```

## 原因

- 生产连接字符串等敏感配置写在 **`appsettings.Production.json`**，该文件在 `.gitignore` 中，不会随代码部署。
- 部署使用 `rsync --delete` 同步 `publish/` 到服务器，会**删除**服务器上不在 `publish/` 里的文件，因此会把服务器上原有的 `appsettings.Production.json` 删掉。
- 删除后应用只读到空的 `ConnectionStrings:Default`，启动时抛错并退出，systemd 不断重启，形成循环。

## 立即恢复步骤（在 ECS 上执行）

### 1. 先停止服务，避免无谓重启

```bash
sudo systemctl stop personal_site_api
```

### 2. 在部署目录创建生产配置

```bash
sudo nano /var/www/personal_site_api/appsettings.Production.json
```

内容示例（按你的实际数据库和 JWT 填写）：

```json
{
  "ConnectionStrings": {
    "Default": "Server=你的MySQL地址;Database=数据库名;User=用户名;Password=密码;SslMode=None;"
  },
  "Jwt": {
    "Key": "生产环境JWT密钥至少32字符",
    "Issuer": "PersonalSite.Api",
    "Audience": "PersonalSite.Web"
  }
}
```

保存后设置权限（避免被其他用户读取）：

```bash
sudo chmod 600 /var/www/personal_site_api/appsettings.Production.json
```

### 3. 启动服务并检查

```bash
sudo systemctl start personal_site_api
sudo systemctl status personal_site_api
curl -s http://127.0.0.1:5234/api/health
```

若返回 `Healthy` 即恢复成功。

## 避免再次被部署删掉

CI 流程已更新：每次部署会先在服务器上把 `appsettings.Production.json` 备份到 `/tmp`，rsync 完成后再写回部署目录，因此之后推送不会删掉该文件。

**注意**：当前这次恢复仍需你在服务器上**手动创建一次** `appsettings.Production.json`（因为之前已被删掉，没有备份）。创建并启动成功后，以后再部署就会自动保留该文件。
