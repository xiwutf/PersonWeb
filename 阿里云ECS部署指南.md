# 阿里云ECS Linux部署指南

## 🎯 部署方案选择

### 方案一：Nginx + 静态文件部署（推荐）
- ⚡ 性能最优，访问速度快
- 💰 资源占用少，成本低
- 🔒 安全稳定
- 📱 支持CDN加速

### 方案二：Node.js + PM2部署
- 🔧 支持服务端渲染
- 📊 可扩展后端API
- 🎛️ 更灵活的配置

---

## 🚀 方案一：Nginx + 静态文件部署（推荐）

### 第一步：连接到ECS服务器

```bash
# 使用SSH连接到您的ECS服务器
ssh root@您的服务器IP地址
# 或者使用阿里云控制台的远程连接功能
```

### 第二步：安装Nginx

#### Ubuntu/Debian系统：
```bash
# 更新包列表
sudo apt update

# 安装Nginx
sudo apt install nginx -y

# 启动Nginx并设置开机自启
sudo systemctl start nginx
sudo systemctl enable nginx

# 检查状态
sudo systemctl status nginx
```

#### CentOS/RHEL系统：
```bash
# 安装Nginx
sudo yum install epel-release -y
sudo yum install nginx -y

# 启动Nginx并设置开机自启
sudo systemctl start nginx
sudo systemctl enable nginx

# 检查状态
sudo systemctl status nginx
```

### 第三步：配置防火墙

```bash
# Ubuntu/Debian
sudo ufw allow 'Nginx Full'
sudo ufw allow OpenSSH
sudo ufw enable

# CentOS/RHEL
sudo firewall-cmd --permanent --add-service=http
sudo firewall-cmd --permanent --add-service=https
sudo firewall-cmd --reload
```

### 第四步：上传网站文件

#### 方法1：使用SCP命令上传
```bash
# 在本地电脑的PowerShell中运行（在PersonWeb目录下）
scp -r .output/public/* root@您的服务器IP:/var/www/html/

# 或者先压缩再上传
tar -czf website.tar.gz -C .output/public .
scp website.tar.gz root@您的服务器IP:/tmp/
```

#### 方法2：使用FTP工具
- 推荐使用：FileZilla、WinSCP、Xftp等
- 将 `.output/public` 目录中的所有文件上传到 `/var/www/html/`

#### 方法3：在服务器上解压
```bash
# 如果使用了压缩包上传
cd /tmp
tar -xzf website.tar.gz -C /var/www/html/

# 设置正确的权限
sudo chown -R www-data:www-data /var/www/html/
sudo chmod -R 755 /var/www/html/
```

### 第五步：配置Nginx

```bash
# 创建网站配置文件
sudo nano /etc/nginx/sites-available/personal-website

# 或者使用vi编辑器
sudo vi /etc/nginx/sites-available/personal-website
```

#### Nginx配置文件内容：
```nginx
server {
    listen 80;
    listen [::]:80;
    
    # 替换为您的域名，如果没有域名可以使用服务器IP
    server_name 您的域名.com www.您的域名.com;
    
    root /var/www/html;
    index index.html;
    
    # 支持Vue Router的history模式
    location / {
        try_files $uri $uri/ /index.html;
    }
    
    # 静态资源缓存
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
    
    # Gzip压缩
    gzip on;
    gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript;
    
    # 安全头
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header X-Content-Type-Options "nosniff" always;
}
```

### 第六步：启用网站配置

```bash
# 创建软链接启用网站
sudo ln -s /etc/nginx/sites-available/personal-website /etc/nginx/sites-enabled/

# 测试Nginx配置
sudo nginx -t

# 如果测试通过，重载Nginx
sudo systemctl reload nginx
```

### 第七步：配置域名（可选）

1. **在域名服务商处配置DNS**：
   - A记录：`@` 指向您的服务器IP
   - A记录：`www` 指向您的服务器IP

2. **等待DNS生效**（通常5-30分钟）

### 第八步：配置SSL证书（推荐）

#### 使用Let's Encrypt免费证书：
```bash
# 安装Certbot
sudo apt install certbot python3-certbot-nginx -y  # Ubuntu/Debian
sudo yum install certbot python3-certbot-nginx -y  # CentOS

# 申请SSL证书
sudo certbot --nginx -d 您的域名.com -d www.您的域名.com

# 设置自动续期
sudo crontab -e
# 添加以下行：
0 12 * * * /usr/bin/certbot renew --quiet
```

---

## 🛠️ 方案二：Node.js + PM2部署

### 第一步：安装Node.js

```bash
# 使用NodeSource仓库安装最新LTS版本
curl -fsSL https://deb.nodesource.com/setup_lts.x | sudo -E bash -  # Ubuntu/Debian
sudo yum install -y nodejs  # CentOS

# 验证安装
node --version
npm --version
```

### 第二步：安装PM2

```bash
sudo npm install -g pm2
```

### 第三步：上传项目文件

```bash
# 上传整个.output目录
scp -r .output root@您的服务器IP:/var/www/

# 设置权限
sudo chown -R $USER:$USER /var/www/.output
```

### 第四步：启动应用

```bash
cd /var/www
pm2 start .output/server/index.mjs --name "personal-website"

# 设置开机自启
pm2 startup
pm2 save
```

### 第五步：配置Nginx反向代理

```nginx
server {
    listen 80;
    server_name 您的域名.com;
    
    location / {
        proxy_pass http://localhost:3000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }
}
```

---

## 📋 部署清单

### 准备工作：
- [ ] 确保ECS安全组开放80和443端口
- [ ] 准备域名（可选）
- [ ] 备份现有网站文件（如果有）

### 部署步骤：
- [ ] 连接到ECS服务器
- [ ] 安装Nginx
- [ ] 配置防火墙
- [ ] 上传网站文件
- [ ] 配置Nginx
- [ ] 测试网站访问
- [ ] 配置域名解析
- [ ] 申请SSL证书

---

## 🔧 常见问题排查

### 1. 网站无法访问
```bash
# 检查Nginx状态
sudo systemctl status nginx

# 检查端口占用
sudo netstat -tlnp | grep :80

# 查看Nginx错误日志
sudo tail -f /var/log/nginx/error.log
```

### 2. 权限问题
```bash
# 设置正确的文件权限
sudo chown -R www-data:www-data /var/www/html/
sudo chmod -R 755 /var/www/html/
```

### 3. 防火墙问题
```bash
# 检查防火墙状态
sudo ufw status  # Ubuntu
sudo firewall-cmd --list-all  # CentOS
```

---

## 📞 技术支持

如果在部署过程中遇到问题，可以：
1. 查看本文档的常见问题部分
2. 检查阿里云ECS控制台的安全组设置
3. 查看服务器日志文件

---

## 🎉 部署完成

部署完成后，您的个人网站将具备：
- ✅ 响应式设计，支持所有设备
- ✅ 快速的页面加载速度
- ✅ SEO优化
- ✅ 安全的HTTPS访问
- ✅ 博客、项目、工具展示功能
- ✅ 全站搜索功能
- ✅ 管理后台

访问 `http://您的域名` 或 `http://您的服务器IP` 即可查看网站！ 