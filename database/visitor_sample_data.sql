-- 访客数据示例数据
-- 用于测试访客分析功能
-- 
-- 使用说明：
-- 1. 执行此脚本前，请确保 visit_logs 和 visitor_analytics 表已创建
-- 2. 此脚本会插入一些测试数据，包括不同时间、不同设备、不同地区的访客记录
-- 3. 执行后，访客分析页面应该能显示统计数据

-- ============================================
-- 1. 插入 visit_logs 表数据（基础访问日志）
-- ============================================

-- 今日访问记录（多个访客，多个页面）
INSERT INTO `visit_logs` (`id`, `visitor_id`, `ip`, `user_agent`, `path`, `timestamp`) VALUES
-- 访客1：今日访问
(UUID(), 'visitor-001', '192.168.1.100', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36', '/', NOW() - INTERVAL 2 HOUR),
(UUID(), 'visitor-001', '192.168.1.100', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36', '/blog', NOW() - INTERVAL 1 HOUR),
(UUID(), 'visitor-001', '192.168.1.100', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36', '/blog/article-1', NOW() - INTERVAL 30 MINUTE),

-- 访客2：今日访问
(UUID(), 'visitor-002', '203.208.60.1', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36', '/', NOW() - INTERVAL 3 HOUR),
(UUID(), 'visitor-002', '203.208.60.1', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36', '/projects', NOW() - INTERVAL 2 HOUR),
(UUID(), 'visitor-002', '203.208.60.1', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36', '/tools', NOW() - INTERVAL 1 HOUR),

-- 访客3：移动设备访问
(UUID(), 'visitor-003', '114.114.114.114', 'Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.0 Mobile/15E148 Safari/604.1', '/', NOW() - INTERVAL 4 HOUR),
(UUID(), 'visitor-003', '114.114.114.114', 'Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.0 Mobile/15E148 Safari/604.1', '/blog', NOW() - INTERVAL 3 HOUR),

-- 访客4：今日访问
(UUID(), 'visitor-004', '8.8.8.8', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:121.0) Gecko/20100101 Firefox/121.0', '/', NOW() - INTERVAL 5 HOUR),
(UUID(), 'visitor-004', '8.8.8.8', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:121.0) Gecko/20100101 Firefox/121.0', '/blog/article-2', NOW() - INTERVAL 4 HOUR),

-- 访客5：今日访问
(UUID(), 'visitor-005', '180.76.76.76', 'Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36', '/', NOW() - INTERVAL 1 HOUR),
(UUID(), 'visitor-005', '180.76.76.76', 'Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36', '/projects', NOW() - INTERVAL 30 MINUTE),

-- 昨日访问记录
(UUID(), 'visitor-006', '61.135.169.125', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36', '/', DATE_SUB(NOW(), INTERVAL 1 DAY) + INTERVAL 10 HOUR),
(UUID(), 'visitor-007', '220.181.38.148', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36', '/blog', DATE_SUB(NOW(), INTERVAL 1 DAY) + INTERVAL 14 HOUR),
(UUID(), 'visitor-008', '123.125.114.144', 'Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X) AppleWebKit/605.1.15', '/', DATE_SUB(NOW(), INTERVAL 1 DAY) + INTERVAL 16 HOUR);

-- ============================================
-- 2. 插入 visitor_analytics 表数据（详细分析数据）
-- ============================================

-- 访客1：Windows + Chrome + 中国
INSERT INTO `visitor_analytics` (
  `visitor_id`, `ip`, `country`, `region`, `city`, 
  `device_type`, `browser`, `os`, `path`, 
  `session_start`, `page_views`, `is_online`, `created_at`, `updated_at`
) VALUES (
  'visitor-001', 
  '192.168.1.100', 
  'China', 'Guangdong', 'Shenzhen',
  'desktop', 'Chrome', 'Windows',
  '/blog/article-1',
  NOW() - INTERVAL 2 HOUR,
  3,
  1,
  NOW() - INTERVAL 2 HOUR,
  NOW() - INTERVAL 30 MINUTE
);

-- 访客2：macOS + Chrome + 美国
INSERT INTO `visitor_analytics` (
  `visitor_id`, `ip`, `country`, `region`, `city`, 
  `device_type`, `browser`, `os`, `path`, 
  `session_start`, `page_views`, `is_online`, `created_at`, `updated_at`
) VALUES (
  'visitor-002', 
  '203.208.60.1', 
  'United States', 'California', 'San Francisco',
  'desktop', 'Chrome', 'macOS',
  '/tools',
  NOW() - INTERVAL 3 HOUR,
  3,
  1,
  NOW() - INTERVAL 3 HOUR,
  NOW() - INTERVAL 1 HOUR
);

-- 访客3：iOS + Safari + 中国（移动设备）
INSERT INTO `visitor_analytics` (
  `visitor_id`, `ip`, `country`, `region`, `city`, 
  `device_type`, `browser`, `os`, `path`, 
  `session_start`, `page_views`, `is_online`, `created_at`, `updated_at`
) VALUES (
  'visitor-003', 
  '114.114.114.114', 
  'China', 'Beijing', 'Beijing',
  'mobile', 'Safari', 'iOS',
  '/blog',
  NOW() - INTERVAL 4 HOUR,
  2,
  0,
  NOW() - INTERVAL 4 HOUR,
  NOW() - INTERVAL 3 HOUR
);

-- 访客4：Windows + Firefox + 美国
INSERT INTO `visitor_analytics` (
  `visitor_id`, `ip`, `country`, `region`, `city`, 
  `device_type`, `browser`, `os`, `path`, 
  `session_start`, `page_views`, `is_online`, `created_at`, `updated_at`
) VALUES (
  'visitor-004', 
  '8.8.8.8', 
  'United States', 'California', 'Mountain View',
  'desktop', 'Firefox', 'Windows',
  '/blog/article-2',
  NOW() - INTERVAL 5 HOUR,
  2,
  0,
  NOW() - INTERVAL 5 HOUR,
  NOW() - INTERVAL 4 HOUR
);

-- 访客5：Linux + Chrome + 中国
INSERT INTO `visitor_analytics` (
  `visitor_id`, `ip`, `country`, `region`, `city`, 
  `device_type`, `browser`, `os`, `path`, 
  `session_start`, `page_views`, `is_online`, `created_at`, `updated_at`
) VALUES (
  'visitor-005', 
  '180.76.76.76', 
  'China', 'Beijing', 'Beijing',
  'desktop', 'Chrome', 'Linux',
  '/projects',
  NOW() - INTERVAL 1 HOUR,
  2,
  1,
  NOW() - INTERVAL 1 HOUR,
  NOW() - INTERVAL 30 MINUTE
);

-- 访客6：Windows + Chrome + 中国（昨日）
INSERT INTO `visitor_analytics` (
  `visitor_id`, `ip`, `country`, `region`, `city`, 
  `device_type`, `browser`, `os`, `path`, 
  `session_start`, `page_views`, `is_online`, `created_at`, `updated_at`
) VALUES (
  'visitor-006', 
  '61.135.169.125', 
  'China', 'Beijing', 'Beijing',
  'desktop', 'Chrome', 'Windows',
  '/',
  DATE_SUB(NOW(), INTERVAL 1 DAY) + INTERVAL 10 HOUR,
  1,
  0,
  DATE_SUB(NOW(), INTERVAL 1 DAY) + INTERVAL 10 HOUR,
  DATE_SUB(NOW(), INTERVAL 1 DAY) + INTERVAL 10 HOUR
);

-- 访客7：macOS + Chrome + 美国（昨日）
INSERT INTO `visitor_analytics` (
  `visitor_id`, `ip`, `country`, `region`, `city`, 
  `device_type`, `browser`, `os`, `path`, 
  `session_start`, `page_views`, `is_online`, `created_at`, `updated_at`
) VALUES (
  'visitor-007', 
  '220.181.38.148', 
  'United States', 'New York', 'New York',
  'desktop', 'Chrome', 'macOS',
  '/blog',
  DATE_SUB(NOW(), INTERVAL 1 DAY) + INTERVAL 14 HOUR,
  1,
  0,
  DATE_SUB(NOW(), INTERVAL 1 DAY) + INTERVAL 14 HOUR,
  DATE_SUB(NOW(), INTERVAL 1 DAY) + INTERVAL 14 HOUR
);

-- 访客8：iOS + Safari + 中国（昨日，移动设备）
INSERT INTO `visitor_analytics` (
  `visitor_id`, `ip`, `country`, `region`, `city`, 
  `device_type`, `browser`, `os`, `path`, 
  `session_start`, `page_views`, `is_online`, `created_at`, `updated_at`
) VALUES (
  'visitor-008', 
  '123.125.114.144', 
  'China', 'Shanghai', 'Shanghai',
  'mobile', 'Safari', 'iOS',
  '/',
  DATE_SUB(NOW(), INTERVAL 1 DAY) + INTERVAL 16 HOUR,
  1,
  0,
  DATE_SUB(NOW(), INTERVAL 1 DAY) + INTERVAL 16 HOUR,
  DATE_SUB(NOW(), INTERVAL 1 DAY) + INTERVAL 16 HOUR
);

-- 访客9：Android + Chrome + 中国（移动设备）
INSERT INTO `visitor_analytics` (
  `visitor_id`, `ip`, `country`, `region`, `city`, 
  `device_type`, `browser`, `os`, `path`, 
  `session_start`, `page_views`, `is_online`, `created_at`, `updated_at`
) VALUES (
  'visitor-009', 
  '223.5.5.5', 
  'China', 'Guangdong', 'Guangzhou',
  'mobile', 'Chrome', 'Android',
  '/tools',
  NOW() - INTERVAL 30 MINUTE,
  1,
  1,
  NOW() - INTERVAL 30 MINUTE,
  NOW() - INTERVAL 30 MINUTE
);

-- 访客10：iPad + Safari + 中国（平板设备）
INSERT INTO `visitor_analytics` (
  `visitor_id`, `ip`, `country`, `region`, `city`, 
  `device_type`, `browser`, `os`, `path`, 
  `session_start`, `page_views`, `is_online`, `created_at`, `updated_at`
) VALUES (
  'visitor-010', 
  '119.29.29.29', 
  'China', 'Sichuan', 'Chengdu',
  'tablet', 'Safari', 'iOS',
  '/projects',
  NOW() - INTERVAL 15 MINUTE,
  1,
  1,
  NOW() - INTERVAL 15 MINUTE,
  NOW() - INTERVAL 15 MINUTE
);

-- 添加一些带搜索关键词的访问记录
INSERT INTO `visitor_analytics` (
  `visitor_id`, `ip`, `country`, `region`, `city`, 
  `device_type`, `browser`, `os`, `path`, `search_keyword`,
  `session_start`, `page_views`, `is_online`, `created_at`, `updated_at`
) VALUES 
('visitor-011', '14.215.177.39', 'China', 'Guangdong', 'Shenzhen',
  'desktop', 'Chrome', 'Windows', '/blog?q=vue3', 'vue3',
  NOW() - INTERVAL 1 HOUR, 1, 0, NOW() - INTERVAL 1 HOUR, NOW() - INTERVAL 1 HOUR),
('visitor-012', '180.97.33.107', 'China', 'Jiangsu', 'Nanjing',
  'desktop', 'Edge', 'Windows', '/blog?q=nuxt', 'nuxt',
  NOW() - INTERVAL 2 HOUR, 1, 0, NOW() - INTERVAL 2 HOUR, NOW() - INTERVAL 2 HOUR),
('visitor-013', '61.135.169.125', 'China', 'Beijing', 'Beijing',
  'desktop', 'Chrome', 'Windows', '/blog?keyword=typescript', 'typescript',
  NOW() - INTERVAL 3 HOUR, 1, 0, NOW() - INTERVAL 3 HOUR, NOW() - INTERVAL 3 HOUR);

-- 添加一些热门文章访问记录
INSERT INTO `visit_logs` (`id`, `visitor_id`, `ip`, `user_agent`, `path`, `timestamp`) VALUES
(UUID(), 'visitor-014', '123.125.114.144', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', '/blog/vue3-tutorial', NOW() - INTERVAL 1 HOUR),
(UUID(), 'visitor-015', '220.181.38.148', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36', '/blog/vue3-tutorial', NOW() - INTERVAL 2 HOUR),
(UUID(), 'visitor-016', '61.135.169.125', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', '/blog/vue3-tutorial', NOW() - INTERVAL 3 HOUR),
(UUID(), 'visitor-017', '180.76.76.76', 'Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36', '/blog/nuxt-guide', NOW() - INTERVAL 1 HOUR),
(UUID(), 'visitor-018', '114.114.114.114', 'Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X) AppleWebKit/605.1.15', '/blog/nuxt-guide', NOW() - INTERVAL 2 HOUR);

-- 更新 visitor_analytics 中的热门文章访问
INSERT INTO `visitor_analytics` (
  `visitor_id`, `ip`, `country`, `region`, `city`, 
  `device_type`, `browser`, `os`, `path`, 
  `session_start`, `page_views`, `is_online`, `created_at`, `updated_at`
) VALUES 
('visitor-014', '123.125.114.144', 'China', 'Beijing', 'Beijing',
  'desktop', 'Chrome', 'Windows', '/blog/vue3-tutorial',
  NOW() - INTERVAL 1 HOUR, 1, 0, NOW() - INTERVAL 1 HOUR, NOW() - INTERVAL 1 HOUR),
('visitor-015', '220.181.38.148', 'China', 'Shanghai', 'Shanghai',
  'desktop', 'Chrome', 'macOS', '/blog/vue3-tutorial',
  NOW() - INTERVAL 2 HOUR, 1, 0, NOW() - INTERVAL 2 HOUR, NOW() - INTERVAL 2 HOUR),
('visitor-016', '61.135.169.125', 'China', 'Guangdong', 'Shenzhen',
  'desktop', 'Chrome', 'Windows', '/blog/vue3-tutorial',
  NOW() - INTERVAL 3 HOUR, 1, 0, NOW() - INTERVAL 3 HOUR, NOW() - INTERVAL 3 HOUR),
('visitor-017', '180.76.76.76', 'China', 'Zhejiang', 'Hangzhou',
  'desktop', 'Chrome', 'Linux', '/blog/nuxt-guide',
  NOW() - INTERVAL 1 HOUR, 1, 0, NOW() - INTERVAL 1 HOUR, NOW() - INTERVAL 1 HOUR),
('visitor-018', '114.114.114.114', 'China', 'Sichuan', 'Chengdu',
  'mobile', 'Safari', 'iOS', '/blog/nuxt-guide',
  NOW() - INTERVAL 2 HOUR, 1, 0, NOW() - INTERVAL 2 HOUR, NOW() - INTERVAL 2 HOUR);

