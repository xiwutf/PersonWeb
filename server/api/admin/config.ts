if (!fs.existsSync(configFile)) {
  return {
    siteName: 'My Personal Site',
    description: 'A personal website built with Nuxt 3',
    author: 'Admin',
    email: '',
    github: '',
    footerText: '© 2024 My Personal Site'
  }
}

const data = fs.readFileSync(configFile, 'utf-8')
return JSON.parse(data)
  }

// 处理 POST：写入配置
if (method === 'POST') {
  const body = await readBody(event)

  // 确保目录存在
  const dir = path.dirname(configFile)
  if (!fs.existsSync(dir)) {
    fs.mkdirSync(dir, { recursive: true })
  }

  // 读旧配置（如果存在）
  let currentConfig: any = {}
  if (fs.existsSync(configFile)) {
    currentConfig = JSON.parse(fs.readFileSync(configFile, 'utf-8'))
  }

  // 合并并写入新配置
  const newConfig = { ...currentConfig, ...body }
  fs.writeFileSync(configFile, JSON.stringify(newConfig, null, 2))

  return { success: true }
}

// 其他 HTTP 方法不支持
throw createError({
  statusCode: 405,
  statusMessage: 'Method Not Allowed'
})
})
