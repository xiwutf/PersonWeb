import mysql from 'mysql2/promise'

// Create a connection pool
const pool = mysql.createPool({
    host: process.env.DB_HOST,
    user: process.env.DB_USER,
    password: process.env.DB_PASSWORD,
    database: process.env.DB_NAME,
    waitForConnections: true,
    connectionLimit: 10,
    queueLimit: 0
})

// 数据库连接池
// 注意：数据库表结构需要手动维护，不会自动创建
export const db = pool
